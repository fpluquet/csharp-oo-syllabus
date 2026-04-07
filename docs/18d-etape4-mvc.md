# Étape 4 : Architecture MVC

Nos pièces sont bien organisées grâce à l'héritage et au polymorphisme. Mais notre classe `Plateau` fait encore trop de choses : elle stocke les données, gère les règles du jeu ET affiche le plateau.

Dans cette dernière étape, nous allons séparer ces responsabilités avec le pattern **MVC** (Modèle-Vue-Contrôleur).

::: tip 🎯 Objectifs de cette étape
- Comprendre le pattern MVC et ses bénéfices
- Séparer le modèle (logique) de la vue (affichage)
- Créer une interface pour rendre la vue interchangeable
- Utiliser les fonctionnalités modernes de C# (primary constructors, file-scoped namespaces, etc.)
:::

::: info 📦 Télécharger le code source
Le code complet de cette étape (jeu jouable) est disponible sur GitHub : [**Voir sur GitHub**](https://github.com/fpluquet/csharp-oo-syllabus/tree/main/docs/public/codes/etape4-mvc)

Pour l'exécuter : `dotnet run` dans le dossier téléchargé.
:::

## Pourquoi séparer les responsabilités ?

Imaginons ces scénarios :

**Scénario 1 : Créer une version graphique**
> "Je veux remplacer l'affichage console par une interface graphique WPF."

Avec le code actuel, il faudrait modifier `Plateau.cs`. Mais `Plateau` contient aussi la logique du jeu... Risqué !

**Scénario 2 : Écrire des tests automatisés**
> "Je veux tester que le cavalier se déplace correctement."

Avec le code actuel, les tests afficheraient du texte dans la console. Pas pratique !

**Scénario 3 : Créer une IA**
> "Je veux qu'un ordinateur joue contre un humain."

L'IA a besoin d'accéder à la logique, pas à l'affichage. Difficile de séparer les deux actuellement.

## Le pattern MVC

Le pattern **Modèle-Vue-Contrôleur** sépare le code en trois parties :

```
┌─────────────────────────────────────────────────────────────────────┐
│                        ARCHITECTURE MVC                             │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│                        ┌───────────────┐                            │
│                        │  CONTRÔLEUR   │                            │
│           Entrées      │   (Arbitre)   │      Actions               │
│         utilisateur ──→│               │──→  sur le modèle          │
│                        └───────┬───────┘                            │
│                                │                                    │
│              Lit l'état ↙      │      ↘ Demande l'affichage         │
│                       ↙        │        ↘                           │
│        ┌───────────────┐       │       ┌───────────────┐            │
│        │    MODÈLE     │       │       │      VUE      │            │
│        │  (Données +   │←──────┴──────→│  (Affichage)  │            │
│        │   Logique)    │   Fournit     │               │            │
│        └───────────────┘   les données └───────────────┘            │
│                                                                     │
│        • Plateau                       • Console                    │
│        • Pièces                        • Graphique                  │
│        • Règles du jeu                 • Web                        │
│        • État de la partie             • Mobile                     │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

| Composant | Responsabilité | Dans notre projet |
|-----------|----------------|-------------------|
| **Modèle** | Données et logique métier | Pièces, Plateau, PartieEchecs |
| **Vue** | Affichage et entrées | ConsoleVue (ou GuiVue, WebVue...) |
| **Contrôleur** | Coordination | JeuController |

## Structure du projet

Voici comment nous allons organiser nos fichiers :

```
Echecs/
├── Models/                     ← Le MODÈLE
│   ├── Enums/
│   │   ├── Couleur.cs
│   │   └── ResultatDeplacement.cs
│   ├── Pieces/
│   │   ├── Piece.cs
│   │   ├── Roi.cs
│   │   ├── Dame.cs
│   │   ├── Tour.cs
│   │   ├── Fou.cs
│   │   ├── Cavalier.cs
│   │   └── Pion.cs
│   ├── Plateau.cs
│   └── PartieEchecs.cs
│
├── Views/                      ← La VUE
│   ├── IEchecsVue.cs          (interface)
│   └── ConsoleVue.cs          (implémentation console)
│
├── Controllers/                ← Le CONTRÔLEUR
│   └── JeuController.cs
│
└── Program.cs                  (point d'entrée)
```

## Étape par étape : la refactorisation

### 1. Nettoyer le Modèle

Le modèle ne doit contenir **AUCUN** `Console.WriteLine`. Il se contente de gérer les données et les règles.

**Supprimons la méthode `Afficher()` de `Plateau.cs`** — elle ira dans la vue.

Ajoutons une énumération pour les résultats de déplacement :

```csharp
// Fichier: Models/Enums/ResultatDeplacement.cs
namespace Echecs.Models.Enums;

public enum ResultatDeplacement
{
    Succes,
    CaseDeDepart_Vide,
    MauvaiseCouleur,
    DeplacementInvalide,
    CaseOccupeeParAllie,
    CheminBloque
}
```

Cette énumération permet au modèle de communiquer **pourquoi** un coup a échoué, sans afficher quoi que ce soit.

### 2. Créer la classe PartieEchecs

Cette classe gère l'état d'une partie complète :

```csharp
// Fichier: Models/PartieEchecs.cs
namespace Echecs.Models;

using Echecs.Models.Enums;
using Echecs.Models.Pieces;

public class PartieEchecs
{
    public Plateau Plateau { get; }
    public Couleur JoueurActif { get; private set; } = Couleur.Blanc;
    public bool PartieTerminee { get; private set; }
    public int NombreDeCoups { get; private set; }

    public PartieEchecs()
    {
        Plateau = new Plateau();
    }

    public ResultatDeplacement TenterDeplacement(
        int deLigne, int deColonne, 
        int versLigne, int versColonne)
    {
        // Vérifications...
        Piece? piece = Plateau[deLigne, deColonne];
        
        if (piece == null)
            return ResultatDeplacement.CaseDeDepart_Vide;
        
        if (piece.Couleur != JoueurActif)
            return ResultatDeplacement.MauvaiseCouleur;
        
        if (!piece.PeutSeDeplacer(versLigne, versColonne))
            return ResultatDeplacement.DeplacementInvalide;
        
        // Vérifier la case de destination
        Piece? cible = Plateau[versLigne, versColonne];
        if (cible != null && cible.Couleur == piece.Couleur)
            return ResultatDeplacement.CaseOccupeeParAllie;
        
        // Effectuer le déplacement
        Plateau.EffectuerDeplacement(deLigne, deColonne, versLigne, versColonne);
        NombreDeCoups++;
        
        // Changer de joueur
        JoueurActif = JoueurActif == Couleur.Blanc ? Couleur.Noir : Couleur.Blanc;
        
        return ResultatDeplacement.Succes;
    }

    public void NouvellePartie()
    {
        Plateau.Initialiser();
        JoueurActif = Couleur.Blanc;
        PartieTerminee = false;
        NombreDeCoups = 0;
    }
}
```

Remarquez : **aucun `Console.WriteLine`** ! Le modèle retourne des données, c'est tout.

### 3. Définir l'interface de la Vue

L'interface définit **ce qu'une vue doit savoir faire**, sans préciser comment :

```csharp
// Fichier: Views/IEchecsVue.cs
namespace Echecs.Views;

using Echecs.Models;
using Echecs.Models.Enums;

public interface IEchecsVue
{
    // Affichage
    void AfficherPlateau(Plateau plateau);
    void AfficherMessage(string message);
    void AfficherErreur(string message);
    void AfficherTour(Couleur joueur);
    void AfficherResultatDeplacement(ResultatDeplacement resultat);
    
    // Entrées utilisateur
    (int deLigne, int deColonne, int versLigne, int versColonne)? DemanderCoup();
    bool DemanderConfirmationQuitter();
    
    // Gestion de l'écran
    void AfficherAccueil();
    void EffacerEcran();
}
```

::: info 💡 Pourquoi une interface ?
L'interface permet de créer plusieurs implémentations :
- `ConsoleVue` pour la console
- `WpfVue` pour une application Windows
- `TestVue` pour les tests unitaires (qui simule les entrées)

Le contrôleur travaille avec `IEchecsVue`, il ne sait pas quelle implémentation est utilisée !
:::

### 4. Implémenter la Vue Console

La vue console gère tout l'affichage et les entrées :

```csharp
// Fichier: Views/ConsoleVue.cs
namespace Echecs.Views;

using Echecs.Models;
using Echecs.Models.Enums;
using Echecs.Models.Pieces;

public class ConsoleVue : IEchecsVue
{
    public void AfficherPlateau(Plateau plateau)
    {
        Console.WriteLine();
        Console.WriteLine("    a   b   c   d   e   f   g   h");
        Console.WriteLine("  ┌───┬───┬───┬───┬───┬───┬───┬───┐");
        
        for (int ligne = 7; ligne >= 0; ligne--)
        {
            Console.Write($"{ligne + 1} │");
            
            for (int col = 0; col < 8; col++)
            {
                Piece? piece = plateau[ligne, col];
                
                if (piece != null)
                {
                    Console.Write($" {piece.Symbole} ");
                }
                else
                {
                    Console.Write("   ");
                }
                Console.Write("│");
            }
            
            Console.WriteLine($" {ligne + 1}");
            
            if (ligne > 0)
            {
                Console.WriteLine("  ├───┼───┼───┼───┼───┼───┼───┼───┤");
            }
        }
        
        Console.WriteLine("  └───┴───┴───┴───┴───┴───┴───┴───┘");
        Console.WriteLine("    a   b   c   d   e   f   g   h");
        Console.WriteLine();
    }

    public void AfficherMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"ℹ️  {message}");
        Console.ResetColor();
    }

    public void AfficherErreur(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"❌ {message}");
        Console.ResetColor();
    }

    public void AfficherTour(Couleur joueur)
    {
        string symbole = joueur == Couleur.Blanc ? "⚪" : "⚫";
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n{symbole} C'est au tour des {joueur}s de jouer");
        Console.ResetColor();
    }

    public void AfficherResultatDeplacement(ResultatDeplacement resultat)
    {
        string message = resultat switch
        {
            ResultatDeplacement.Succes => "✓ Déplacement effectué !",
            ResultatDeplacement.CaseDeDepart_Vide => "Aucune pièce à cet emplacement.",
            ResultatDeplacement.MauvaiseCouleur => "Ce n'est pas votre pièce !",
            ResultatDeplacement.DeplacementInvalide => "Cette pièce ne peut pas se déplacer ainsi.",
            ResultatDeplacement.CaseOccupeeParAllie => "Une de vos pièces occupe déjà cette case.",
            ResultatDeplacement.CheminBloque => "Le chemin est bloqué.",
            _ => "Erreur inconnue."
        };

        if (resultat == ResultatDeplacement.Succes)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }
        else
        {
            AfficherErreur(message);
        }
        Console.ResetColor();
    }

    public (int deLigne, int deColonne, int versLigne, int versColonne)? DemanderCoup()
    {
        Console.Write("Entrez votre coup (ex: e2 e4) ou 'q' pour quitter : ");
        
        string? input = Console.ReadLine()?.Trim().ToLower();
        
        if (string.IsNullOrEmpty(input) || input == "q")
            return null;
        
        // Parser l'entrée (ex: "e2 e4")
        if (TryParserCoup(input, out var coup))
            return coup;
        
        AfficherErreur("Format invalide. Utilisez: [colonne][ligne] [colonne][ligne]");
        return DemanderCoup();  // Redemander
    }

    private bool TryParserCoup(string input, 
        out (int deLigne, int deColonne, int versLigne, int versColonne) coup)
    {
        coup = default;
        
        var parties = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parties.Length != 2) return false;
        
        if (!TryParserCase(parties[0], out int deLigne, out int deColonne)) return false;
        if (!TryParserCase(parties[1], out int versLigne, out int versColonne)) return false;
        
        coup = (deLigne, deColonne, versLigne, versColonne);
        return true;
    }

    private bool TryParserCase(string notation, out int ligne, out int colonne)
    {
        ligne = 0;
        colonne = 0;
        
        if (notation.Length != 2) return false;
        
        char colChar = notation[0];
        char ligneChar = notation[1];
        
        if (colChar < 'a' || colChar > 'h') return false;
        if (ligneChar < '1' || ligneChar > '8') return false;
        
        colonne = colChar - 'a';
        ligne = ligneChar - '1';
        
        return true;
    }

    public bool DemanderConfirmationQuitter()
    {
        Console.Write("Voulez-vous vraiment quitter ? (o/n) : ");
        string? reponse = Console.ReadLine()?.Trim().ToLower();
        return reponse == "o" || reponse == "oui";
    }

    public void AfficherAccueil()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║     ♔ ♕ ♖ ♗ ♘ ♙   JEU D'ÉCHECS   ♟ ♞ ♝ ♜ ♛ ♚               ║
║                                                               ║
║                    Version Console C#                         ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝
");
        Console.ResetColor();
        Console.WriteLine("Commandes :");
        Console.WriteLine("  • Entrez un coup au format : e2 e4");
        Console.WriteLine("  • Tapez 'q' pour quitter\n");
        Console.WriteLine("Appuyez sur Entrée pour commencer...");
        Console.ReadLine();
    }

    public void EffacerEcran() => Console.Clear();
}
```

### 5. Le Contrôleur : le chef d'orchestre

Le contrôleur coordonne le modèle et la vue. Il utilise le **primary constructor** de C# 12 :

```csharp
// Fichier: Controllers/JeuController.cs
namespace Echecs.Controllers;

using Echecs.Models;
using Echecs.Models.Enums;
using Echecs.Views;

public class JeuController(IEchecsVue vue)
{
    private readonly PartieEchecs _partie = new();
    private readonly IEchecsVue _vue = vue;
    private bool _quitter;

    public void Demarrer()
    {
        _vue.AfficherAccueil();
        BouclePrincipale();
    }

    private void BouclePrincipale()
    {
        while (!_quitter && !_partie.PartieTerminee)
        {
            _vue.EffacerEcran();
            _vue.AfficherPlateau(_partie.Plateau);
            _vue.AfficherTour(_partie.JoueurActif);
            
            // Demander et traiter le coup
            var coup = _vue.DemanderCoup();
            
            if (coup == null)
            {
                TraiterDemandeQuitter();
                continue;
            }
            
            var (deLigne, deColonne, versLigne, versColonne) = coup.Value;
            var resultat = _partie.TenterDeplacement(
                deLigne, deColonne, 
                versLigne, versColonne
            );
            
            _vue.AfficherResultatDeplacement(resultat);
            
            if (resultat != ResultatDeplacement.Succes)
            {
                Console.WriteLine("\nAppuyez sur Entrée pour continuer...");
                Console.ReadLine();
            }
        }
        
        _vue.AfficherMessage("Merci d'avoir joué ! À bientôt ♔");
    }

    private void TraiterDemandeQuitter()
    {
        if (_vue.DemanderConfirmationQuitter())
        {
            _quitter = true;
        }
    }
}
```

::: info 💡 Primary Constructor
La syntaxe `public class JeuController(IEchecsVue vue)` est un **primary constructor** C# 12. Le paramètre `vue` est disponible dans toute la classe. C'est équivalent à un constructeur traditionnel avec un champ privé.
:::

### 6. Le point d'entrée

Enfin, `Program.cs` assemble les pièces du puzzle :

```csharp
// Fichier: Program.cs
using Echecs.Controllers;
using Echecs.Views;

// 1. Créer la vue (ici, console)
IEchecsVue vue = new ConsoleVue();

// 2. Créer le contrôleur avec la vue
JeuController jeu = new(vue);

// 3. Démarrer le jeu
jeu.Demarrer();
```

C'est tout ! Pour changer d'interface, il suffit de remplacer `new ConsoleVue()` par une autre implémentation.

## Mise à jour du Plateau pour le MVC

Le plateau doit exposer les pièces sans méthode d'affichage :

```csharp
// Fichier: Models/Plateau.cs
namespace Echecs.Models;

using Echecs.Models.Pieces;

public class Plateau
{
    private readonly Piece?[,] _cases = new Piece?[8, 8];

    public Plateau()
    {
        Initialiser();
    }

    // Indexeur pour accéder aux pièces
    public Piece? this[int ligne, int colonne]
    {
        get => EstDansLimites(ligne, colonne) ? _cases[ligne, colonne] : null;
    }

    public static bool EstDansLimites(int ligne, int colonne) 
        => ligne >= 0 && ligne < 8 && colonne >= 0 && colonne < 8;

    public void Initialiser()
    {
        Array.Clear(_cases);
        
        PlacerRangeeArriere(0, Couleur.Blanc);
        PlacerPions(1, Couleur.Blanc);
        PlacerPions(6, Couleur.Noir);
        PlacerRangeeArriere(7, Couleur.Noir);
    }

    public void EffectuerDeplacement(int deLigne, int deColonne, 
                                      int versLigne, int versColonne)
    {
        Piece? piece = _cases[deLigne, deColonne];
        if (piece == null) return;

        _cases[versLigne, versColonne] = piece;
        _cases[deLigne, deColonne] = null;
        piece.Deplacer(versLigne, versColonne);
    }

    private void PlacerRangeeArriere(int ligne, Couleur couleur)
    {
        _cases[ligne, 0] = new Tour(couleur, ligne, 0);
        _cases[ligne, 1] = new Cavalier(couleur, ligne, 1);
        _cases[ligne, 2] = new Fou(couleur, ligne, 2);
        _cases[ligne, 3] = new Dame(couleur, ligne, 3);
        _cases[ligne, 4] = new Roi(couleur, ligne, 4);
        _cases[ligne, 5] = new Fou(couleur, ligne, 5);
        _cases[ligne, 6] = new Cavalier(couleur, ligne, 6);
        _cases[ligne, 7] = new Tour(couleur, ligne, 7);
    }

    private void PlacerPions(int ligne, Couleur couleur)
    {
        for (int col = 0; col < 8; col++)
        {
            _cases[ligne, col] = new Pion(couleur, ligne, col);
        }
    }
}
```

Notez l'utilisation de l'**indexeur** `this[int ligne, int colonne]` qui permet d'écrire `plateau[2, 3]` au lieu de `plateau.GetPiece(2, 3)`.

## Test du jeu complet

Exécutons notre jeu :

```
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║     ♔ ♕ ♖ ♗ ♘ ♙   JEU D'ÉCHECS   ♟ ♞ ♝ ♜ ♛ ♚               ║
║                                                               ║
║                    Version Console C#                         ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝

Commandes :
  • Entrez un coup au format : e2 e4
  • Tapez 'q' pour quitter

Appuyez sur Entrée pour commencer...
```

Après avoir appuyé sur Entrée :

```
    a   b   c   d   e   f   g   h
  ┌───┬───┬───┬───┬───┬───┬───┬───┐
8 │ ♜ │ ♞ │ ♝ │ ♛ │ ♚ │ ♝ │ ♞ │ ♜ │ 8
  ├───┼───┼───┼───┼───┼───┼───┼───┤
7 │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │ 7
  ├───┼───┼───┼───┼───┼───┼───┼───┤
6 │   │   │   │   │   │   │   │   │ 6
  ├───┼───┼───┼───┼───┼───┼───┼───┤
5 │   │   │   │   │   │   │   │   │ 5
  ├───┼───┼───┼───┼───┼───┼───┼───┤
4 │   │   │   │   │   │   │   │   │ 4
  ├───┼───┼───┼───┼───┼───┼───┼───┤
3 │   │   │   │   │   │   │   │   │ 3
  ├───┼───┼───┼───┼───┼───┼───┼───┤
2 │ ♙ │ ♙ │ ♙ │ ♙ │ ♙ │ ♙ │ ♙ │ ♙ │ 2
  ├───┼───┼───┼───┼───┼───┼───┼───┤
1 │ ♖ │ ♘ │ ♗ │ ♕ │ ♔ │ ♗ │ ♘ │ ♖ │ 1
  └───┴───┴───┴───┴───┴───┴───┴───┘
    a   b   c   d   e   f   g   h

⚪ C'est au tour des Blancs de jouer
Entrez votre coup (ex: e2 e4) ou 'q' pour quitter : e2 e4
✓ Déplacement effectué !
```

## ✅ Bilan final

### Ce que nous avons construit

```
┌─────────────────────────────────────────────────────────────────────┐
│                    ARCHITECTURE FINALE                              │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │                        MODÈLE                                │   │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────────┐   │   │
│  │  │   Plateau    │  │PartieEchecs │  │     Pieces/      │   │   │
│  │  │  (données)   │  │  (logique)  │  │Tour,Cavalier,... │   │   │
│  │  └──────────────┘  └──────────────┘  └──────────────────┘   │   │
│  └─────────────────────────────────────────────────────────────┘   │
│                              ↑                                      │
│                              │ Lit/Modifie                          │
│                              ↓                                      │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │                      CONTRÔLEUR                              │   │
│  │  ┌──────────────────────────────────────────────────────┐   │   │
│  │  │              JeuController                            │   │   │
│  │  │         (coordination, boucle de jeu)                 │   │   │
│  │  └──────────────────────────────────────────────────────┘   │   │
│  └─────────────────────────────────────────────────────────────┘   │
│                              ↑                                      │
│                              │ Utilise (via interface)              │
│                              ↓                                      │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │                          VUE                                 │   │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐       │   │
│  │  │ IEchecsVue   │  │ ConsoleVue   │  │  (GuiVue?)   │       │   │
│  │  │ (interface)  │  │(implémentation)│ │  (future)   │       │   │
│  │  └──────────────┘  └──────────────┘  └──────────────┘       │   │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### Récapitulatif des 4 étapes

| Étape | Concept clé | Problème résolu |
|-------|-------------|-----------------|
| 1 | Encapsulation, énumérations | Structure de base |
| 2 | Héritage, abstraction | Switch géant → classes spécialisées |
| 3 | Polymorphisme | Manipulation uniforme des pièces |
| 4 | Architecture MVC | Séparation des responsabilités |

### Fonctionnalités C# modernes utilisées

| Fonctionnalité | Où ? |
|----------------|------|
| **File-scoped namespaces** | `namespace X;` (tous les fichiers) |
| **Primary constructors** | `class JeuController(IEchecsVue vue)` |
| **Pattern matching** | Switch expressions |
| **Nullable reference types** | `Piece?` |
| **Target-typed new** | `new()` sans répéter le type |
| **Expression-bodied members** | `=> expression;` |
| **Indexeurs** | `plateau[ligne, colonne]` |

## Pour aller plus loin

::: tip 🚀 Défis supplémentaires

**Niveau 1 - Facile**
- Ajouter la vérification du chemin libre (tour, fou, dame)
- Empêcher la capture de ses propres pièces
- Détecter l'échec au roi

**Niveau 2 - Intermédiaire**
- Implémenter le roque
- Gérer la promotion du pion
- Créer une `TestVue` pour les tests unitaires

**Niveau 3 - Avancé**
- Détecter l'échec et mat
- Créer une IA simple (minimax)
- Créer une interface graphique avec Avalonia ou MAUI

:::

## 🎉 Conclusion

Félicitations ! Vous avez construit un jeu d'échecs complet en appliquant progressivement les concepts fondamentaux de la POO :

1. **L'encapsulation** protège vos données et garantit la cohérence
2. **L'héritage** permet de spécialiser le comportement
3. **Le polymorphisme** offre la flexibilité de traiter différents objets uniformément
4. **L'architecture MVC** sépare les responsabilités pour un code maintenable

Ces principes s'appliquent à **tout projet logiciel**, pas seulement aux jeux. Un formulaire web, une API, une application mobile... tous bénéficient de ces concepts.

::: tip 💡 Le mot de la fin
Le code parfait n'existe pas. L'objectif est de comprendre **pourquoi** on structure le code d'une certaine manière. Chaque décision d'architecture est un compromis entre simplicité, flexibilité et performance.

Maintenant, c'est à vous de jouer ! ♔
:::

## 📝 Code complet de l'étape 4

::: details Cliquez pour voir le code complet

**Models/Enums/ResultatDeplacement.cs**
```csharp
namespace Echecs.Models.Enums;

public enum ResultatDeplacement
{
    Succes,
    CaseDeDepart_Vide,
    MauvaiseCouleur,
    DeplacementInvalide,
    CaseOccupeeParAllie,
    CheminBloque
}
```

**Models/Plateau.cs**
```csharp
namespace Echecs.Models;

using Echecs.Models.Pieces;

public class Plateau
{
    private readonly Piece?[,] _cases = new Piece?[8, 8];

    public Plateau()
    {
        Initialiser();
    }

    public Piece? this[int ligne, int colonne]
    {
        get => EstDansLimites(ligne, colonne) ? _cases[ligne, colonne] : null;
    }

    public static bool EstDansLimites(int ligne, int colonne) 
        => ligne >= 0 && ligne < 8 && colonne >= 0 && colonne < 8;

    public void Initialiser()
    {
        Array.Clear(_cases);
        PlacerRangeeArriere(0, Couleur.Blanc);
        PlacerPions(1, Couleur.Blanc);
        PlacerPions(6, Couleur.Noir);
        PlacerRangeeArriere(7, Couleur.Noir);
    }

    public void EffectuerDeplacement(int deLigne, int deColonne, 
                                      int versLigne, int versColonne)
    {
        Piece? piece = _cases[deLigne, deColonne];
        if (piece == null) return;

        _cases[versLigne, versColonne] = piece;
        _cases[deLigne, deColonne] = null;
        piece.Deplacer(versLigne, versColonne);
    }

    private void PlacerRangeeArriere(int ligne, Couleur couleur)
    {
        _cases[ligne, 0] = new Tour(couleur, ligne, 0);
        _cases[ligne, 1] = new Cavalier(couleur, ligne, 1);
        _cases[ligne, 2] = new Fou(couleur, ligne, 2);
        _cases[ligne, 3] = new Dame(couleur, ligne, 3);
        _cases[ligne, 4] = new Roi(couleur, ligne, 4);
        _cases[ligne, 5] = new Fou(couleur, ligne, 5);
        _cases[ligne, 6] = new Cavalier(couleur, ligne, 6);
        _cases[ligne, 7] = new Tour(couleur, ligne, 7);
    }

    private void PlacerPions(int ligne, Couleur couleur)
    {
        for (int col = 0; col < 8; col++)
        {
            _cases[ligne, col] = new Pion(couleur, ligne, col);
        }
    }
}
```

**Models/PartieEchecs.cs**
```csharp
namespace Echecs.Models;

using Echecs.Models.Enums;
using Echecs.Models.Pieces;

public class PartieEchecs
{
    public Plateau Plateau { get; }
    public Couleur JoueurActif { get; private set; } = Couleur.Blanc;
    public bool PartieTerminee { get; private set; }
    public int NombreDeCoups { get; private set; }

    public PartieEchecs()
    {
        Plateau = new Plateau();
    }

    public ResultatDeplacement TenterDeplacement(
        int deLigne, int deColonne, 
        int versLigne, int versColonne)
    {
        Piece? piece = Plateau[deLigne, deColonne];
        
        if (piece == null)
            return ResultatDeplacement.CaseDeDepart_Vide;
        
        if (piece.Couleur != JoueurActif)
            return ResultatDeplacement.MauvaiseCouleur;
        
        if (!piece.PeutSeDeplacer(versLigne, versColonne))
            return ResultatDeplacement.DeplacementInvalide;
        
        Piece? cible = Plateau[versLigne, versColonne];
        if (cible != null && cible.Couleur == piece.Couleur)
            return ResultatDeplacement.CaseOccupeeParAllie;
        
        Plateau.EffectuerDeplacement(deLigne, deColonne, versLigne, versColonne);
        NombreDeCoups++;
        
        JoueurActif = JoueurActif == Couleur.Blanc ? Couleur.Noir : Couleur.Blanc;
        
        return ResultatDeplacement.Succes;
    }

    public void NouvellePartie()
    {
        Plateau.Initialiser();
        JoueurActif = Couleur.Blanc;
        PartieTerminee = false;
        NombreDeCoups = 0;
    }
}
```

**Views/IEchecsVue.cs**
```csharp
namespace Echecs.Views;

using Echecs.Models;
using Echecs.Models.Enums;

public interface IEchecsVue
{
    void AfficherPlateau(Plateau plateau);
    void AfficherMessage(string message);
    void AfficherErreur(string message);
    void AfficherTour(Couleur joueur);
    void AfficherResultatDeplacement(ResultatDeplacement resultat);
    
    (int deLigne, int deColonne, int versLigne, int versColonne)? DemanderCoup();
    bool DemanderConfirmationQuitter();
    
    void AfficherAccueil();
    void EffacerEcran();
}
```

**Views/ConsoleVue.cs**
```csharp
namespace Echecs.Views;

using Echecs.Models;
using Echecs.Models.Enums;
using Echecs.Models.Pieces;

public class ConsoleVue : IEchecsVue
{
    public void AfficherPlateau(Plateau plateau)
    {
        Console.WriteLine();
        Console.WriteLine("    a   b   c   d   e   f   g   h");
        Console.WriteLine("  ┌───┬───┬───┬───┬───┬───┬───┬───┐");
        
        for (int ligne = 7; ligne >= 0; ligne--)
        {
            Console.Write($"{ligne + 1} │");
            
            for (int col = 0; col < 8; col++)
            {
                Piece? piece = plateau[ligne, col];
                Console.Write(piece != null ? $" {piece.Symbole} " : "   ");
                Console.Write("│");
            }
            
            Console.WriteLine($" {ligne + 1}");
            
            if (ligne > 0)
                Console.WriteLine("  ├───┼───┼───┼───┼───┼───┼───┼───┤");
        }
        
        Console.WriteLine("  └───┴───┴───┴───┴───┴───┴───┴───┘");
        Console.WriteLine("    a   b   c   d   e   f   g   h\n");
    }

    public void AfficherMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"ℹ️  {message}");
        Console.ResetColor();
    }

    public void AfficherErreur(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"❌ {message}");
        Console.ResetColor();
    }

    public void AfficherTour(Couleur joueur)
    {
        string symbole = joueur == Couleur.Blanc ? "⚪" : "⚫";
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n{symbole} C'est au tour des {joueur}s de jouer");
        Console.ResetColor();
    }

    public void AfficherResultatDeplacement(ResultatDeplacement resultat)
    {
        string message = resultat switch
        {
            ResultatDeplacement.Succes => "✓ Déplacement effectué !",
            ResultatDeplacement.CaseDeDepart_Vide => "Aucune pièce à cet emplacement.",
            ResultatDeplacement.MauvaiseCouleur => "Ce n'est pas votre pièce !",
            ResultatDeplacement.DeplacementInvalide => "Mouvement invalide pour cette pièce.",
            ResultatDeplacement.CaseOccupeeParAllie => "Votre pièce occupe déjà cette case.",
            ResultatDeplacement.CheminBloque => "Le chemin est bloqué.",
            _ => "Erreur inconnue."
        };

        if (resultat == ResultatDeplacement.Succes)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        else
        {
            AfficherErreur(message);
        }
    }

    public (int deLigne, int deColonne, int versLigne, int versColonne)? DemanderCoup()
    {
        Console.Write("Entrez votre coup (ex: e2 e4) ou 'q' pour quitter : ");
        
        string? input = Console.ReadLine()?.Trim().ToLower();
        
        if (string.IsNullOrEmpty(input) || input == "q")
            return null;
        
        if (TryParserCoup(input, out var coup))
            return coup;
        
        AfficherErreur("Format invalide. Utilisez: e2 e4");
        return DemanderCoup();
    }

    private bool TryParserCoup(string input, 
        out (int deLigne, int deColonne, int versLigne, int versColonne) coup)
    {
        coup = default;
        
        var parties = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parties.Length != 2) return false;
        
        if (!TryParserCase(parties[0], out int deLigne, out int deColonne)) return false;
        if (!TryParserCase(parties[1], out int versLigne, out int versColonne)) return false;
        
        coup = (deLigne, deColonne, versLigne, versColonne);
        return true;
    }

    private bool TryParserCase(string notation, out int ligne, out int colonne)
    {
        ligne = colonne = 0;
        if (notation.Length != 2) return false;
        
        if (notation[0] < 'a' || notation[0] > 'h') return false;
        if (notation[1] < '1' || notation[1] > '8') return false;
        
        colonne = notation[0] - 'a';
        ligne = notation[1] - '1';
        return true;
    }

    public bool DemanderConfirmationQuitter()
    {
        Console.Write("Voulez-vous vraiment quitter ? (o/n) : ");
        string? reponse = Console.ReadLine()?.Trim().ToLower();
        return reponse == "o" || reponse == "oui";
    }

    public void AfficherAccueil()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════════╗
║     ♔ ♕ ♖ ♗ ♘ ♙   JEU D'ÉCHECS   ♟ ♞ ♝ ♜ ♛ ♚               ║
╚═══════════════════════════════════════════════════════════════╝
");
        Console.ResetColor();
        Console.WriteLine("Commandes : e2 e4 (déplacer) | q (quitter)\n");
        Console.WriteLine("Appuyez sur Entrée pour commencer...");
        Console.ReadLine();
    }

    public void EffacerEcran() => Console.Clear();
}
```

**Controllers/JeuController.cs**
```csharp
namespace Echecs.Controllers;

using Echecs.Models;
using Echecs.Models.Enums;
using Echecs.Views;

public class JeuController(IEchecsVue vue)
{
    private readonly PartieEchecs _partie = new();
    private readonly IEchecsVue _vue = vue;
    private bool _quitter;

    public void Demarrer()
    {
        _vue.AfficherAccueil();
        BouclePrincipale();
    }

    private void BouclePrincipale()
    {
        while (!_quitter && !_partie.PartieTerminee)
        {
            _vue.EffacerEcran();
            _vue.AfficherPlateau(_partie.Plateau);
            _vue.AfficherTour(_partie.JoueurActif);
            
            var coup = _vue.DemanderCoup();
            
            if (coup == null)
            {
                if (_vue.DemanderConfirmationQuitter())
                    _quitter = true;
                continue;
            }
            
            var (deLigne, deColonne, versLigne, versColonne) = coup.Value;
            var resultat = _partie.TenterDeplacement(
                deLigne, deColonne, versLigne, versColonne);
            
            _vue.AfficherResultatDeplacement(resultat);
            
            if (resultat != ResultatDeplacement.Succes)
            {
                Console.WriteLine("\nAppuyez sur Entrée...");
                Console.ReadLine();
            }
        }
        
        _vue.AfficherMessage("Merci d'avoir joué ! ♔");
    }
}
```

**Program.cs**
```csharp
using Echecs.Controllers;
using Echecs.Views;

IEchecsVue vue = new ConsoleVue();
JeuController jeu = new(vue);
jeu.Demarrer();
```
:::
