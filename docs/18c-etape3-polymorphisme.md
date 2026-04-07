# Étape 3 : Le Polymorphisme en Action

À l'étape précédente, nous avons créé une hiérarchie de classes avec `Piece` comme classe abstraite et des classes spécialisées pour chaque type. Mais un mystère persiste : comment C# sait-il quelle méthode `PeutSeDeplacer` appeler quand la variable est de type `Piece` ?

La réponse : le **polymorphisme**.

::: tip 🎯 Objectifs de cette étape
- Comprendre la différence entre **type déclaré** et **type réel**
- Maîtriser les mots-clés `virtual` et `override`
- Voir la **liaison dynamique** en action
- Enrichir notre modèle avec des méthodes polymorphes
:::

::: info 📦 Télécharger le code source
Le code complet de cette étape est disponible sur GitHub : [**Voir sur GitHub**](https://github.com/fpluquet/csharp-oo-syllabus/tree/main/docs/public/codes/etape3-polymorphisme)

Pour l'exécuter : `dotnet run` dans le dossier téléchargé.
:::

## Le mystère résolu

Observons ce code du plateau :

```csharp
Piece? piece = GetPiece(ligne, colonne);
piece.PeutSeDeplacer(versLigne, versColonne);
```

La variable `piece` est déclarée comme `Piece`, mais elle peut contenir un `Tour`, un `Cavalier`, un `Pion`... Comment C# fait-il pour appeler la bonne méthode ?

### Type déclaré vs Type réel

En C#, chaque variable a deux types :

1. **Type déclaré** : celui qu'on écrit dans le code (à gauche du `=`)
2. **Type réel** : celui de l'objet effectivement créé (à droite du `=`)

```csharp
Piece piece = new Cavalier(Couleur.Blanc, 0, 1);
//    ↑                    ↑
//    Type déclaré         Type réel
//    (Piece)              (Cavalier)
```

Le **polymorphisme** permet d'utiliser le type déclaré (`Piece`) tout en exécutant le comportement du type réel (`Cavalier`).

```
┌─────────────────────────────────────────────────────────────────────┐
│                    TYPE DÉCLARÉ vs TYPE RÉEL                        │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│    Piece piece = new Cavalier(Couleur.Blanc, 0, 1);                 │
│    ─────              ────────                                      │
│      │                    │                                         │
│      ▼                    ▼                                         │
│  Type déclaré         Type réel                                     │
│  "Je suis une Piece"  "Mais en vrai, je suis un Cavalier"           │
│                                                                     │
│    piece.PeutSeDeplacer(2, 2);                                      │
│           │                                                         │
│           ▼                                                         │
│    → Appelle Cavalier.PeutSeDeplacer() ! (liaison dynamique)        │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### La liaison dynamique

Le choix de la méthode à exécuter se fait **à l'exécution** (runtime), pas à la compilation. C'est la **liaison dynamique** (ou *late binding*).

Démonstration :

```csharp
using Echecs;
using Echecs.Pieces;

// Créons une liste de pièces de types différents
List<Piece> pieces = new()
{
    new Tour(Couleur.Blanc, 0, 0),
    new Cavalier(Couleur.Blanc, 0, 1),
    new Fou(Couleur.Blanc, 0, 2),
    new Dame(Couleur.Blanc, 0, 3),
    new Roi(Couleur.Blanc, 0, 4)
};

// Testons un déplacement vers (2, 2)
Console.WriteLine("Test de déplacement de (position initiale) vers (2, 2) :\n");

foreach (Piece piece in pieces)
{
    // La variable 'piece' est de type Piece
    // Mais le comportement dépend du TYPE RÉEL de l'objet
    
    bool peutBouger = piece.PeutSeDeplacer(2, 2);
    string typeReel = piece.GetType().Name;  // Retourne le type réel !
    
    Console.WriteLine($"  {piece.Symbole} {typeReel,-10} : {(peutBouger ? "✓ OUI" : "✗ NON")}");
}
```

**Résultat :**

```
Test de déplacement de (position initiale) vers (2, 2) :

  ♖ Tour       : ✗ NON   ← (0,0) vers (2,2) n'est pas en ligne droite
  ♘ Cavalier   : ✓ OUI   ← (0,1) vers (2,2) = L (2 lignes, 1 colonne)
  ♗ Fou        : ✓ OUI   ← (0,2) vers (2,2) = diagonale parfaite
  ♕ Dame       : ✗ NON   ← (0,3) vers (2,2) n'est ni droit ni diagonal
  ♔ Roi        : ✗ NON   ← (0,4) vers (2,2) = trop loin (plus d'1 case)
```

Chaque pièce applique **ses propres règles**, bien que la boucle ne connaisse que le type `Piece` !

## Les mécanismes du polymorphisme

### Le mot-clé `abstract`

Nous l'avons déjà utilisé. Une méthode `abstract` :
- N'a **pas d'implémentation** dans la classe de base
- **Doit** être implémentée dans les classes dérivées
- Utilise automatiquement la liaison dynamique

```csharp
public abstract class Piece
{
    // Pas de corps, juste la signature
    public abstract bool PeutSeDeplacer(int versLigne, int versColonne);
}
```

### Le mot-clé `virtual`

Une méthode `virtual` a une implémentation **par défaut**, mais peut être **redéfinie** :

```csharp
public abstract class Piece
{
    // Implémentation par défaut
    public virtual string Nom => GetType().Name;
    
    // Peut être redéfini dans les classes dérivées
}
```

### Le mot-clé `override`

Pour redéfinir une méthode `virtual` ou `abstract`, on utilise `override` :

```csharp
public class Cavalier : Piece
{
    // Redéfinition de la propriété Nom
    public override string Nom => "Cavalier";  // Au lieu de "Cavalier" (via GetType)
    
    // Implémentation obligatoire (abstract)
    public override bool PeutSeDeplacer(...) { ... }
}
```

### Récapitulatif

| Mot-clé | Classe de base | Classe dérivée | Liaison |
|---------|----------------|----------------|---------|
| `abstract` | Pas d'implémentation | `override` obligatoire | Dynamique |
| `virtual` | Implémentation par défaut | `override` optionnel | Dynamique |
| (rien) | Implémentation | Ne peut pas redéfinir | Statique |

## Enrichir notre modèle avec le polymorphisme

Profitons du polymorphisme pour ajouter des fonctionnalités utiles à nos pièces.

### La valeur des pièces

Aux échecs, chaque pièce a une "valeur" relative pour évaluer les positions :

| Pièce | Valeur |
|-------|--------|
| Pion | 1 |
| Cavalier | 3 |
| Fou | 3 |
| Tour | 5 |
| Dame | 9 |
| Roi | ∞ (100) |

Ajoutons cela à notre modèle :

```csharp
// Dans Piece.cs
public abstract class Piece
{
    // ... propriétés existantes ...
    
    // Propriété abstraite : chaque pièce définit sa valeur
    public abstract int Valeur { get; }
}
```

Et dans chaque classe dérivée :

```csharp
// Dans Tour.cs
public override int Valeur => 5;

// Dans Cavalier.cs
public override int Valeur => 3;

// Dans Fou.cs
public override int Valeur => 3;

// Dans Dame.cs
public override int Valeur => 9;

// Dans Roi.cs
public override int Valeur => 100;

// Dans Pion.cs
public override int Valeur => 1;
```

### Calcul du score

Le plateau peut maintenant calculer le score de chaque joueur :

```csharp
// Dans Plateau.cs
public int CalculerScore(Couleur couleur)
{
    int score = 0;
    
    for (int l = 0; l < 8; l++)
    {
        for (int c = 0; c < 8; c++)
        {
            Piece? piece = _cases[l, c];
            
            // Le polymorphisme fait tout le travail !
            if (piece?.Couleur == couleur)
            {
                score += piece.Valeur;  // ← Appelle la bonne version
            }
        }
    }
    
    return score;
}
```

**Test :**

```csharp
Plateau plateau = new();

int scoreBlanc = plateau.CalculerScore(Couleur.Blanc);
int scoreNoir = plateau.CalculerScore(Couleur.Noir);

Console.WriteLine($"Score des Blancs : {scoreBlanc}");  // 39
Console.WriteLine($"Score des Noirs : {scoreNoir}");    // 39
```

Sans polymorphisme, il aurait fallu un switch ou des if pour déterminer la valeur de chaque pièce !

### Représentation textuelle

Redéfinissons `ToString()` pour un affichage informatif :

```csharp
// Dans Piece.cs
public override string ToString()
{
    char colLettre = (char)('a' + Colonne);
    return $"{Symbole} {GetType().Name} {Couleur} en {colLettre}{Ligne + 1}";
}
```

**Utilisation :**

```csharp
Piece cavalier = new Cavalier(Couleur.Blanc, 0, 1);
Console.WriteLine(cavalier);  // ♘ Cavalier Blanc en b1
```

### Lister les mouvements possibles

Ajoutons une méthode pour obtenir tous les mouvements possibles d'une pièce :

```csharp
// Dans Piece.cs
public virtual List<(int ligne, int colonne)> GetMouvementsPossibles()
{
    var mouvements = new List<(int, int)>();
    
    // Tester toutes les cases du plateau
    for (int l = 0; l < 8; l++)
    {
        for (int c = 0; c < 8; c++)
        {
            if (PeutSeDeplacer(l, c))
            {
                mouvements.Add((l, c));
            }
        }
    }
    
    return mouvements;
}
```

Cette implémentation par défaut fonctionne pour toutes les pièces grâce au polymorphisme !

**Test :**

```csharp
Piece cavalier = new Cavalier(Couleur.Blanc, 4, 4);  // Centre du plateau

Console.WriteLine($"Mouvements possibles pour {cavalier} :");
foreach (var (l, c) in cavalier.GetMouvementsPossibles())
{
    char col = (char)('a' + c);
    Console.WriteLine($"  → {col}{l + 1}");
}
```

**Résultat :**

```
Mouvements possibles pour ♘ Cavalier Blanc en e5 :
  → c4
  → c6
  → d3
  → d7
  → f3
  → f7
  → g4
  → g6
```

Le cavalier au centre a 8 mouvements possibles, formant un cercle de "L" autour de lui.

## Démonstration visuelle du polymorphisme

Créons un programme qui illustre clairement le polymorphisme :

```csharp
using Echecs;
using Echecs.Pieces;

Console.WriteLine("╔═══════════════════════════════════════════════════╗");
Console.WriteLine("║         DÉMONSTRATION DU POLYMORPHISME            ║");
Console.WriteLine("╚═══════════════════════════════════════════════════╝\n");

// Toutes ces variables sont de type Piece (type déclaré)
// Mais contiennent des objets de types différents (type réel)
Piece[] pieces = 
{
    new Tour(Couleur.Blanc, 3, 3),
    new Fou(Couleur.Noir, 3, 3),
    new Cavalier(Couleur.Blanc, 3, 3),
    new Dame(Couleur.Noir, 3, 3),
    new Roi(Couleur.Blanc, 3, 3),
    new Pion(Couleur.Noir, 3, 3)
};

Console.WriteLine("Toutes les pièces sont en position d4 (ligne 3, colonne 3).\n");
Console.WriteLine("Nombre de cases atteignables par chaque pièce :\n");

foreach (Piece piece in pieces)
{
    int nbMouvements = piece.GetMouvementsPossibles().Count;
    
    // Affichage avec barre de progression visuelle
    string barre = new string('█', nbMouvements / 2);
    string espaces = new string('░', 14 - nbMouvements / 2);
    
    Console.WriteLine($"  {piece.Symbole} {piece.GetType().Name,-10} │{barre}{espaces}│ {nbMouvements} cases");
}

Console.WriteLine();
Console.WriteLine("Le même appel (GetMouvementsPossibles) produit des");
Console.WriteLine("résultats différents selon le type réel de la pièce !");
```

**Résultat :**

```
╔═══════════════════════════════════════════════════╗
║         DÉMONSTRATION DU POLYMORPHISME            ║
╚═══════════════════════════════════════════════════╝

Toutes les pièces sont en position d4 (ligne 3, colonne 3).

Nombre de cases atteignables par chaque pièce :

  ♖ Tour       │██████░░░░░░░░│ 14 cases
  ♝ Fou        │██████░░░░░░░░│ 13 cases
  ♘ Cavalier   │████░░░░░░░░░░│ 8 cases
  ♛ Dame       │█████████████░│ 27 cases
  ♔ Roi        │████░░░░░░░░░░│ 8 cases
  ♟ Pion       │░░░░░░░░░░░░░░│ 1 cases

Le même appel (GetMouvementsPossibles) produit des
résultats différents selon le type réel de la pièce !
```

## ✅ Bilan de l'étape 3

### Ce que nous maîtrisons maintenant

| Concept | Description |
|---------|-------------|
| **Type déclaré vs réel** | Une variable `Piece` peut contenir un `Cavalier` |
| **Liaison dynamique** | La méthode appelée dépend du type réel |
| **abstract** | Force l'implémentation dans les classes dérivées |
| **virtual/override** | Permet de redéfinir un comportement par défaut |

### Les avantages concrets

```csharp
// Le plateau peut manipuler TOUTES les pièces de manière uniforme
foreach (Piece piece in plateau.ToutesPieces)
{
    // Fonctionne pour Tour, Cavalier, Dame, etc.
    Console.WriteLine($"{piece} peut atteindre {piece.GetMouvementsPossibles().Count} cases");
    Console.WriteLine($"  Valeur : {piece.Valeur}");
}
```

Le code appelant n'a pas besoin de connaître les types spécifiques !

## ❌ Le problème restant : le mélange des responsabilités

Notre code fonctionne bien, mais regardons la classe `Plateau` :

```csharp
public class Plateau
{
    // DONNÉES
    private readonly Piece?[,] _cases;
    
    // LOGIQUE MÉTIER
    public bool TenterDeplacement(...) { ... }
    public int CalculerScore(...) { ... }
    
    // AFFICHAGE ← Problème !
    public void Afficher()
    {
        Console.WriteLine(...);  // Couplage fort avec Console
    }
}
```

Cette classe fait **trop de choses** ! Elle mélange :
1. Le stockage des données
2. La logique du jeu
3. L'affichage

::: danger 🚨 Pourquoi c'est un problème ?

**Impossible de réutiliser le modèle**
Si on veut créer une version graphique (WPF, Unity), on doit modifier `Plateau`.

**Impossible de tester proprement**
Les tests unitaires ne devraient pas écrire dans la console.

**Difficile à maintenir**
Un bug d'affichage force à toucher la même classe qu'un bug de règles.

**Violation du principe de responsabilité unique**
Une classe devrait avoir UNE seule raison de changer.
:::

```
┌─────────────────────────────────────────────────────────────────────┐
│                    LE PROBLÈME DU MÉLANGE                           │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│         ┌──────────────────────────────────────────┐                │
│         │              PLATEAU                     │                │
│         │  ┌────────┐ ┌────────┐ ┌────────────┐   │                │
│         │  │Données │ │Logique │ │ Affichage  │   │                │
│         │  │        │ │ métier │ │ Console    │   │                │
│         │  └────────┘ └────────┘ └────────────┘   │                │
│         │         TOUT EST MÉLANGÉ !               │                │
│         └──────────────────────────────────────────┘                │
│                                                                     │
│  Et si je veux une interface graphique ?                            │
│  → Je dois MODIFIER Plateau... 😱                                   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

## 🎯 Vers l'étape 4

La solution : séparer notre code en **couches distinctes** selon le pattern **MVC** (Modèle-Vue-Contrôleur) :

- **Modèle** : les pièces, le plateau, les règles du jeu
- **Vue** : l'affichage (console, graphique, web...)
- **Contrôleur** : la coordination entre utilisateur et modèle

Rendez-vous à l'[Étape 4 : Architecture MVC](./18d-etape4-mvc.md) !

## 📝 Code complet de l'étape 3

::: details Cliquez pour voir les ajouts de cette étape

**Pieces/Piece.cs (version enrichie)**
```csharp
namespace Echecs.Pieces;

public abstract class Piece
{
    public Couleur Couleur { get; }
    public int Ligne { get; private set; }
    public int Colonne { get; private set; }

    public abstract char Symbole { get; }
    public abstract int Valeur { get; }

    protected Piece(Couleur couleur, int ligne, int colonne)
    {
        Couleur = couleur;
        Ligne = ligne;
        Colonne = colonne;
    }

    public void Deplacer(int nouvelleLigne, int nouvelleColonne)
    {
        Ligne = nouvelleLigne;
        Colonne = nouvelleColonne;
    }

    public abstract bool PeutSeDeplacer(int versLigne, int versColonne);

    protected int DeltaLigne(int versLigne) => Math.Abs(versLigne - Ligne);
    protected int DeltaColonne(int versColonne) => Math.Abs(versColonne - Colonne);

    public virtual List<(int ligne, int colonne)> GetMouvementsPossibles()
    {
        var mouvements = new List<(int, int)>();
        
        for (int l = 0; l < 8; l++)
        {
            for (int c = 0; c < 8; c++)
            {
                if (PeutSeDeplacer(l, c))
                {
                    mouvements.Add((l, c));
                }
            }
        }
        
        return mouvements;
    }

    public override string ToString()
    {
        char colLettre = (char)('a' + Colonne);
        return $"{Symbole} {GetType().Name} {Couleur} en {colLettre}{Ligne + 1}";
    }
}
```

**Ajouts dans chaque pièce :**
```csharp
// Tour.cs
public override int Valeur => 5;

// Fou.cs
public override int Valeur => 3;

// Cavalier.cs
public override int Valeur => 3;

// Dame.cs
public override int Valeur => 9;

// Roi.cs
public override int Valeur => 100;

// Pion.cs
public override int Valeur => 1;
```

**Plateau.cs (ajout de CalculerScore)**
```csharp
public int CalculerScore(Couleur couleur)
{
    int score = 0;
    
    for (int l = 0; l < 8; l++)
    {
        for (int c = 0; c < 8; c++)
        {
            Piece? piece = _cases[l, c];
            if (piece?.Couleur == couleur)
            {
                score += piece.Valeur;
            }
        }
    }
    
    return score;
}
```

**Programme de démonstration :**
```csharp
using Echecs;
using Echecs.Pieces;

Console.WriteLine("=== Démonstration du Polymorphisme ===\n");

List<Piece> pieces = new()
{
    new Tour(Couleur.Blanc, 0, 0),
    new Cavalier(Couleur.Blanc, 0, 1),
    new Fou(Couleur.Blanc, 0, 2),
    new Dame(Couleur.Blanc, 0, 3),
    new Roi(Couleur.Blanc, 0, 4)
};

Console.WriteLine("Test de déplacement vers (2, 2):\n");

foreach (Piece piece in pieces)
{
    bool peutBouger = piece.PeutSeDeplacer(2, 2);
    string typePiece = piece.GetType().Name;
    string resultat = peutBouger ? "✓ OUI" : "✗ NON";
    
    Console.WriteLine($"  {piece.Symbole} {typePiece,-10} : {resultat}");
}

Console.WriteLine("\n--- Calcul des scores ---");
Plateau plateau = new();
Console.WriteLine($"Score Blancs : {plateau.CalculerScore(Couleur.Blanc)}");
Console.WriteLine($"Score Noirs : {plateau.CalculerScore(Couleur.Noir)}");
```
:::
