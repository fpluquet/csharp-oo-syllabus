# Étape 1 : Encapsulation et Objets de Base

Dans cette première étape, nous allons créer les fondations de notre jeu d'échecs. Nous commencerons par une approche simple et directe, qui fonctionnera... mais qui révélera rapidement ses limites.

::: tip 🎯 Objectifs de cette étape
- Créer une classe `Piece` avec ses propriétés encapsulées
- Utiliser des énumérations pour représenter les couleurs et types de pièces
- Créer une classe `Plateau` pour gérer l'échiquier
- Identifier les **problèmes** de cette approche simple
:::

::: info 📦 Télécharger le code source
Le code complet de cette étape est disponible sur GitHub : [**Voir sur GitHub**](https://github.com/fpluquet/csharp-oo-syllabus/tree/main/docs/public/codes/etape1-encapsulation)

Pour l'exécuter : `dotnet run` dans le dossier téléchargé.
:::

## Réflexion préliminaire : que doit-on modéliser ?

Avant d'écrire la moindre ligne de code, posons-nous la question : **quelles sont les entités de notre jeu ?**

En observant un échiquier, on identifie :

1. **Les pièces** - chaque pièce a :
   - Un **type** (roi, dame, tour, fou, cavalier, pion)
   - Une **couleur** (blanc ou noir)
   - Une **position** sur le plateau (ligne et colonne)

2. **Le plateau** - il contient :
   - 64 cases organisées en 8×8
   - Les pièces placées sur ces cases

Commençons par modéliser ces éléments.

## Les énumérations : limiter les valeurs possibles

### Pourquoi des énumérations ?

Imaginons qu'on représente la couleur d'une pièce par une chaîne de caractères :

```csharp
string couleur = "blanc";
```

Quels problèmes cela pose-t-il ?

- On pourrait écrire `"Blanc"`, `"BLANC"`, `"white"`, ou même `"bleu"` par erreur
- Le compilateur ne peut pas nous aider à détecter ces erreurs
- Les comparaisons sont peu fiables (`"blanc" == "Blanc"` → `false` !)

Les **énumérations** résolvent ce problème en définissant un ensemble **fermé** de valeurs possibles.

### L'énumération Couleur

Créons notre première énumération :

```csharp
// Fichier: Couleur.cs
namespace Echecs;

public enum Couleur
{
    Blanc,
    Noir
}
```

C'est tout ! Désormais, une variable de type `Couleur` ne peut contenir que `Couleur.Blanc` ou `Couleur.Noir`. Toute autre valeur provoquera une erreur de compilation.

```csharp
Couleur maCouleur = Couleur.Blanc;  // ✅ OK
Couleur erreur = "blanc";           // ❌ Erreur de compilation !
```

### L'énumération TypePiece

De la même manière, créons une énumération pour les types de pièces :

```csharp
// Fichier: TypePiece.cs
namespace Echecs;

public enum TypePiece
{
    Roi,
    Dame,
    Tour,
    Fou,
    Cavalier,
    Pion
}
```

::: info 💡 Avantages des énumérations
1. **Sécurité de type** : impossible d'assigner une valeur invalide
2. **Autocomplétion** : l'IDE vous propose les valeurs possibles
3. **Lisibilité** : le code est plus expressif (`TypePiece.Cavalier` vs `"cavalier"`)
4. **Refactoring** : renommer une valeur se fait automatiquement partout
:::

## La classe Piece : notre premier objet

### Quelles propriétés pour une pièce ?

Une pièce d'échecs doit mémoriser :
- Son **type** (roi, dame, etc.) → ne change jamais
- Sa **couleur** → ne change jamais  
- Sa **position** (ligne et colonne) → change quand la pièce se déplace

Traduisons cela en propriétés C# :

```csharp
public class Piece
{
    public TypePiece Type { get; }        // Lecture seule
    public Couleur Couleur { get; }       // Lecture seule
    public int Ligne { get; private set; }    // Modifiable en interne
    public int Colonne { get; private set; }  // Modifiable en interne
}
```

Remarquez les niveaux d'accès :
- `Type` et `Couleur` : **lecture seule** (`get` sans `set`) car ils ne changent jamais
- `Ligne` et `Colonne` : **setter privé** car seule la pièce elle-même peut modifier sa position

### Le constructeur : initialiser correctement

Une pièce doit être créée avec toutes ses caractéristiques. Le constructeur garantit qu'une pièce est toujours dans un état valide :

```csharp
public Piece(TypePiece type, Couleur couleur, int ligne, int colonne)
{
    Type = type;
    Couleur = couleur;
    Ligne = ligne;
    Colonne = colonne;
}
```

Utilisation :

```csharp
// Créer un cavalier blanc en position b1 (ligne 0, colonne 1)
Piece cavalier = new Piece(TypePiece.Cavalier, Couleur.Blanc, 0, 1);
```

### La méthode Deplacer : changer de position

Quand une pièce se déplace, elle doit mettre à jour sa position :

```csharp
public void Deplacer(int nouvelleLigne, int nouvelleColonne)
{
    Ligne = nouvelleLigne;
    Colonne = nouvelleColonne;
}
```

Simple et efficace. La pièce elle-même gère sa position.

### Afficher la pièce : les symboles Unicode

Pour afficher le plateau, nous avons besoin d'un symbole pour chaque pièce. Unicode propose des caractères d'échecs :

| Pièce | Blanc | Noir |
|-------|-------|------|
| Roi | ♔ | ♚ |
| Dame | ♕ | ♛ |
| Tour | ♖ | ♜ |
| Fou | ♗ | ♝ |
| Cavalier | ♘ | ♞ |
| Pion | ♙ | ♟ |

Créons une méthode qui retourne le bon symbole :

```csharp
public char GetSymbole()
{
    return (Type, Couleur) switch
    {
        (TypePiece.Roi, Couleur.Blanc) => '♔',
        (TypePiece.Dame, Couleur.Blanc) => '♕',
        (TypePiece.Tour, Couleur.Blanc) => '♖',
        (TypePiece.Fou, Couleur.Blanc) => '♗',
        (TypePiece.Cavalier, Couleur.Blanc) => '♘',
        (TypePiece.Pion, Couleur.Blanc) => '♙',
        (TypePiece.Roi, Couleur.Noir) => '♚',
        (TypePiece.Dame, Couleur.Noir) => '♛',
        (TypePiece.Tour, Couleur.Noir) => '♜',
        (TypePiece.Fou, Couleur.Noir) => '♝',
        (TypePiece.Cavalier, Couleur.Noir) => '♞',
        (TypePiece.Pion, Couleur.Noir) => '♟',
        _ => '?'
    };
}
```

::: info 💡 Pattern matching avec tuples
L'expression `(Type, Couleur) switch { ... }` est du **pattern matching** C#. Elle permet de tester plusieurs valeurs simultanément de manière élégante. Nous y reviendrons au chapitre Pattern Matching.
:::

### La validation des déplacements : le cœur du problème

Voici la partie la plus importante : **comment savoir si un déplacement est valide ?**

Chaque type de pièce a ses propres règles :
- **Tour** : lignes droites (horizontales ou verticales)
- **Fou** : diagonales uniquement
- **Dame** : tour + fou combinés
- **Roi** : une case dans toutes les directions
- **Cavalier** : mouvement en "L"
- **Pion** : avance d'une case (ou deux au premier coup)

Dans notre approche simple avec une seule classe `Piece`, nous devons utiliser un `switch` pour distinguer les types :

```csharp
public bool PeutSeDeplacer(int versLigne, int versColonne)
{
    // Calcul des différences de position
    int deltaLigne = Math.Abs(versLigne - Ligne);
    int deltaColonne = Math.Abs(versColonne - Colonne);

    switch (Type)
    {
        case TypePiece.Tour:
            // La tour se déplace en ligne droite uniquement
            return deltaLigne == 0 || deltaColonne == 0;

        case TypePiece.Fou:
            // Le fou se déplace en diagonale uniquement
            return deltaLigne == deltaColonne;

        case TypePiece.Dame:
            // La dame combine tour et fou
            return deltaLigne == 0 || deltaColonne == 0 
                   || deltaLigne == deltaColonne;

        case TypePiece.Roi:
            // Le roi se déplace d'une case maximum
            return deltaLigne <= 1 && deltaColonne <= 1;

        case TypePiece.Cavalier:
            // Le cavalier fait un "L" : 2+1 ou 1+2
            return (deltaLigne == 2 && deltaColonne == 1) 
                   || (deltaLigne == 1 && deltaColonne == 2);

        case TypePiece.Pion:
            // Simplifié : avance d'une case
            return deltaColonne == 0 && deltaLigne == 1;

        default:
            return false;
    }
}
```

Ce code **fonctionne**, mais... regardez sa taille ! Et nous n'avons même pas géré tous les cas (direction du pion, premier mouvement, etc.).

## La classe Plateau : gérer l'échiquier

### Structure de données

Un plateau d'échecs est une grille 8×8. En C#, nous utilisons un tableau bidimensionnel :

```csharp
public class Plateau
{
    private readonly Piece?[,] _cases = new Piece?[8, 8];
}
```

Quelques points importants :
- `Piece?` : le `?` indique qu'une case peut être **vide** (`null`)
- `[8, 8]` : 8 lignes × 8 colonnes
- `readonly` : le tableau lui-même ne sera jamais remplacé
- `private` : l'accès aux cases est contrôlé

### Initialisation des pièces

Au début d'une partie, les pièces sont placées en position standard. Créons des méthodes pour cela :

```csharp
public Plateau()
{
    InitialiserPieces();
}

private void InitialiserPieces()
{
    // Pièces blanches (lignes 0 et 1)
    PlacerPiecesLourdes(0, Couleur.Blanc);
    PlacerPions(1, Couleur.Blanc);

    // Pièces noires (lignes 6 et 7)
    PlacerPions(6, Couleur.Noir);
    PlacerPiecesLourdes(7, Couleur.Noir);
}
```

La méthode pour placer les pièces "lourdes" (tout sauf les pions) :

```csharp
private void PlacerPiecesLourdes(int ligne, Couleur couleur)
{
    _cases[ligne, 0] = new Piece(TypePiece.Tour, couleur, ligne, 0);
    _cases[ligne, 1] = new Piece(TypePiece.Cavalier, couleur, ligne, 1);
    _cases[ligne, 2] = new Piece(TypePiece.Fou, couleur, ligne, 2);
    _cases[ligne, 3] = new Piece(TypePiece.Dame, couleur, ligne, 3);
    _cases[ligne, 4] = new Piece(TypePiece.Roi, couleur, ligne, 4);
    _cases[ligne, 5] = new Piece(TypePiece.Fou, couleur, ligne, 5);
    _cases[ligne, 6] = new Piece(TypePiece.Cavalier, couleur, ligne, 6);
    _cases[ligne, 7] = new Piece(TypePiece.Tour, couleur, ligne, 7);
}
```

Et pour les pions :

```csharp
private void PlacerPions(int ligne, Couleur couleur)
{
    for (int col = 0; col < 8; col++)
    {
        _cases[ligne, col] = new Piece(TypePiece.Pion, couleur, ligne, col);
    }
}
```

### Récupérer une pièce

Pour savoir quelle pièce occupe une case :

```csharp
public Piece? GetPiece(int ligne, int colonne)
{
    // Vérifier que la position est valide
    if (ligne < 0 || ligne > 7 || colonne < 0 || colonne > 7)
        return null;
    
    return _cases[ligne, colonne];
}
```

### Effectuer un déplacement

La méthode qui tente de déplacer une pièce :

```csharp
public bool TenterDeplacement(int deLigne, int deColonne, 
                               int versLigne, int versColonne)
{
    // 1. Récupérer la pièce à déplacer
    Piece? piece = GetPiece(deLigne, deColonne);
    
    if (piece == null)
        return false;  // Pas de pièce à cette position

    // 2. Vérifier si le déplacement est valide
    if (!piece.PeutSeDeplacer(versLigne, versColonne))
        return false;  // Mouvement invalide pour cette pièce

    // 3. Effectuer le déplacement
    _cases[versLigne, versColonne] = piece;
    _cases[deLigne, deColonne] = null;
    piece.Deplacer(versLigne, versColonne);
    
    return true;
}
```

### Afficher le plateau

Pour visualiser l'état du jeu :

```csharp
public void Afficher()
{
    Console.WriteLine("  a b c d e f g h");
    Console.WriteLine("  ─────────────────");
    
    for (int ligne = 7; ligne >= 0; ligne--)
    {
        Console.Write($"{ligne + 1}│");
        
        for (int col = 0; col < 8; col++)
        {
            Piece? piece = _cases[ligne, col];
            if (piece != null)
            {
                Console.Write($"{piece.GetSymbole()} ");
            }
            else
            {
                // Cases vides avec alternance de couleurs
                Console.Write((ligne + col) % 2 == 0 ? "□ " : "■ ");
            }
        }
        
        Console.WriteLine($"│{ligne + 1}");
    }
    
    Console.WriteLine("  ─────────────────");
    Console.WriteLine("  a b c d e f g h");
}
```

## Test de notre code

Créons un programme de test :

```csharp
// Fichier: Program.cs
using Echecs;

Plateau plateau = new();
plateau.Afficher();

Console.WriteLine("\n--- Test : déplacer le cavalier b1 vers c3 ---");
bool succes = plateau.TenterDeplacement(0, 1, 2, 2);
Console.WriteLine(succes ? "✓ Déplacement réussi !" : "✗ Déplacement impossible");

plateau.Afficher();
```

**Résultat dans la console :**

```
  a b c d e f g h
  ─────────────────
8│♜ ♞ ♝ ♛ ♚ ♝ ♞ ♜ │8
7│♟ ♟ ♟ ♟ ♟ ♟ ♟ ♟ │7
6│□ ■ □ ■ □ ■ □ ■ │6
5│■ □ ■ □ ■ □ ■ □ │5
4│□ ■ □ ■ □ ■ □ ■ │4
3│■ □ ■ □ ■ □ ■ □ │3
2│♙ ♙ ♙ ♙ ♙ ♙ ♙ ♙ │2
1│♖ ♘ ♗ ♕ ♔ ♗ ♘ ♖ │1
  ─────────────────
  a b c d e f g h

--- Test : déplacer le cavalier b1 vers c3 ---
✓ Déplacement réussi !

  a b c d e f g h
  ─────────────────
8│♜ ♞ ♝ ♛ ♚ ♝ ♞ ♜ │8
7│♟ ♟ ♟ ♟ ♟ ♟ ♟ ♟ │7
6│□ ■ □ ■ □ ■ □ ■ │6
5│■ □ ■ □ ■ □ ■ □ │5
4│□ ■ □ ■ □ ■ □ ■ │4
3│■ □ ♘ □ ■ □ ■ □ │3
2│♙ ♙ ♙ ♙ ♙ ♙ ♙ ♙ │2
1│♖ □ ♗ ♕ ♔ ♗ ♘ ♖ │1
  ─────────────────
  a b c d e f g h
```

Ça fonctionne ! Le cavalier s'est bien déplacé de b1 à c3. 🎉

## ✅ Bilan de l'étape 1

Nous avons créé un jeu d'échecs fonctionnel avec :
- Des **énumérations** pour les types et couleurs
- Une classe **Piece** avec propriétés encapsulées
- Une classe **Plateau** qui gère les cases
- Une validation basique des déplacements

## ❌ Le problème : le switch géant

Mais regardons à nouveau notre méthode `PeutSeDeplacer` :

```csharp
public bool PeutSeDeplacer(int versLigne, int versColonne)
{
    switch (Type)
    {
        case TypePiece.Tour:     // ... 2 lignes
        case TypePiece.Fou:      // ... 2 lignes
        case TypePiece.Dame:     // ... 3 lignes
        case TypePiece.Roi:      // ... 2 lignes
        case TypePiece.Cavalier: // ... 3 lignes
        case TypePiece.Pion:     // ... 2 lignes (et encore, simplifié !)
    }
}
```

::: danger 🚨 Problèmes de cette approche

**1. Violation du principe ouvert/fermé**
Pour ajouter une nouvelle pièce (imaginons une "Princesse" dans une variante), il faut **modifier** la classe `Piece` existante.

**2. Classe trop volumineuse**
Toute la logique de TOUTES les pièces est concentrée dans une seule classe. Cette classe va grossir démesurément.

**3. Difficile à maintenir**
Si un bug affecte le déplacement du cavalier, il faut chercher dans ce switch géant au lieu d'aller directement dans "le fichier du cavalier".

**4. Difficile à tester**
Pour tester le cavalier seul, on doit quand même instancier la classe `Piece` qui contient la logique de toutes les pièces.

**5. Pas de spécialisation**
Un pion et une dame sont traités comme le "même type d'objet" alors qu'ils sont fondamentalement différents.
:::

```
┌─────────────────────────────────────────────────────────────────────┐
│                    LE PROBLÈME DU SWITCH GÉANT                      │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  public bool PeutSeDeplacer(...)                                    │
│  {                                                                  │
│      switch (Type)           ← Chaque nouveau type = modification   │
│      {                                                              │
│          case Tour: ...      ← Logique Tour ici                     │
│          case Fou: ...       ← Logique Fou ici                      │
│          case Dame: ...      ← Logique Dame ici                     │
│          case Roi: ...       ← Et si j'ajoute "Princesse" ?         │
│          case Cavalier: ...  ← Je dois modifier CETTE classe !      │
│          case Pion: ...                                             │
│      }                                                              │
│  }                                                                  │
│                                                                     │
│  💡 Solution : Chaque pièce devrait connaître SES propres règles !  │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

## 🎯 Vers l'étape 2

La solution à ce problème s'appelle l'**héritage** et l'**abstraction**.

L'idée est simple : au lieu d'avoir UNE classe `Piece` qui gère tous les cas, nous aurons :
- Une classe **abstraite** `Piece` qui définit ce que toute pièce doit pouvoir faire
- Des classes **spécialisées** (`Tour`, `Cavalier`, `Fou`, etc.) qui implémentent leur propre logique

Rendez-vous à l'[Étape 2 : Héritage et Abstraction](./16b-etape2-heritage.md) !

## 📝 Code complet de l'étape 1

::: details Cliquez pour voir le code complet

**Couleur.cs**
```csharp
namespace Echecs;

public enum Couleur
{
    Blanc,
    Noir
}
```

**TypePiece.cs**
```csharp
namespace Echecs;

public enum TypePiece
{
    Roi,
    Dame,
    Tour,
    Fou,
    Cavalier,
    Pion
}
```

**Piece.cs**
```csharp
namespace Echecs;

public class Piece
{
    public TypePiece Type { get; }
    public Couleur Couleur { get; }
    public int Ligne { get; private set; }
    public int Colonne { get; private set; }

    public Piece(TypePiece type, Couleur couleur, int ligne, int colonne)
    {
        Type = type;
        Couleur = couleur;
        Ligne = ligne;
        Colonne = colonne;
    }

    public void Deplacer(int nouvelleLigne, int nouvelleColonne)
    {
        Ligne = nouvelleLigne;
        Colonne = nouvelleColonne;
    }

    public bool PeutSeDeplacer(int versLigne, int versColonne)
    {
        int deltaLigne = Math.Abs(versLigne - Ligne);
        int deltaColonne = Math.Abs(versColonne - Colonne);

        switch (Type)
        {
            case TypePiece.Tour:
                return deltaLigne == 0 || deltaColonne == 0;

            case TypePiece.Fou:
                return deltaLigne == deltaColonne;

            case TypePiece.Dame:
                return deltaLigne == 0 || deltaColonne == 0 
                       || deltaLigne == deltaColonne;

            case TypePiece.Roi:
                return deltaLigne <= 1 && deltaColonne <= 1;

            case TypePiece.Cavalier:
                return (deltaLigne == 2 && deltaColonne == 1) 
                       || (deltaLigne == 1 && deltaColonne == 2);

            case TypePiece.Pion:
                return deltaColonne == 0 && deltaLigne == 1;

            default:
                return false;
        }
    }

    public char GetSymbole()
    {
        return (Type, Couleur) switch
        {
            (TypePiece.Roi, Couleur.Blanc) => '♔',
            (TypePiece.Dame, Couleur.Blanc) => '♕',
            (TypePiece.Tour, Couleur.Blanc) => '♖',
            (TypePiece.Fou, Couleur.Blanc) => '♗',
            (TypePiece.Cavalier, Couleur.Blanc) => '♘',
            (TypePiece.Pion, Couleur.Blanc) => '♙',
            (TypePiece.Roi, Couleur.Noir) => '♚',
            (TypePiece.Dame, Couleur.Noir) => '♛',
            (TypePiece.Tour, Couleur.Noir) => '♜',
            (TypePiece.Fou, Couleur.Noir) => '♝',
            (TypePiece.Cavalier, Couleur.Noir) => '♞',
            (TypePiece.Pion, Couleur.Noir) => '♟',
            _ => '?'
        };
    }
}
```

**Plateau.cs**
```csharp
namespace Echecs;

public class Plateau
{
    private readonly Piece?[,] _cases = new Piece?[8, 8];

    public Plateau()
    {
        InitialiserPieces();
    }

    private void InitialiserPieces()
    {
        PlacerPiecesLourdes(0, Couleur.Blanc);
        PlacerPions(1, Couleur.Blanc);
        PlacerPions(6, Couleur.Noir);
        PlacerPiecesLourdes(7, Couleur.Noir);
    }

    private void PlacerPiecesLourdes(int ligne, Couleur couleur)
    {
        _cases[ligne, 0] = new Piece(TypePiece.Tour, couleur, ligne, 0);
        _cases[ligne, 1] = new Piece(TypePiece.Cavalier, couleur, ligne, 1);
        _cases[ligne, 2] = new Piece(TypePiece.Fou, couleur, ligne, 2);
        _cases[ligne, 3] = new Piece(TypePiece.Dame, couleur, ligne, 3);
        _cases[ligne, 4] = new Piece(TypePiece.Roi, couleur, ligne, 4);
        _cases[ligne, 5] = new Piece(TypePiece.Fou, couleur, ligne, 5);
        _cases[ligne, 6] = new Piece(TypePiece.Cavalier, couleur, ligne, 6);
        _cases[ligne, 7] = new Piece(TypePiece.Tour, couleur, ligne, 7);
    }

    private void PlacerPions(int ligne, Couleur couleur)
    {
        for (int col = 0; col < 8; col++)
        {
            _cases[ligne, col] = new Piece(TypePiece.Pion, couleur, ligne, col);
        }
    }

    public Piece? GetPiece(int ligne, int colonne)
    {
        if (ligne < 0 || ligne > 7 || colonne < 0 || colonne > 7)
            return null;
        return _cases[ligne, colonne];
    }

    public bool TenterDeplacement(int deLigne, int deColonne, 
                                   int versLigne, int versColonne)
    {
        Piece? piece = GetPiece(deLigne, deColonne);
        
        if (piece == null)
            return false;

        if (!piece.PeutSeDeplacer(versLigne, versColonne))
            return false;

        _cases[versLigne, versColonne] = piece;
        _cases[deLigne, deColonne] = null;
        piece.Deplacer(versLigne, versColonne);
        
        return true;
    }

    public void Afficher()
    {
        Console.WriteLine("  a b c d e f g h");
        Console.WriteLine("  ─────────────────");
        
        for (int ligne = 7; ligne >= 0; ligne--)
        {
            Console.Write($"{ligne + 1}│");
            for (int col = 0; col < 8; col++)
            {
                Piece? piece = _cases[ligne, col];
                if (piece != null)
                {
                    Console.Write($"{piece.GetSymbole()} ");
                }
                else
                {
                    Console.Write((ligne + col) % 2 == 0 ? "□ " : "■ ");
                }
            }
            Console.WriteLine($"│{ligne + 1}");
        }
        
        Console.WriteLine("  ─────────────────");
        Console.WriteLine("  a b c d e f g h");
    }
}
```

**Program.cs**
```csharp
using Echecs;

Plateau plateau = new();
plateau.Afficher();

Console.WriteLine("\n--- Test : déplacer le cavalier b1 vers c3 ---");
bool succes = plateau.TenterDeplacement(0, 1, 2, 2);
Console.WriteLine(succes ? "✓ Déplacement réussi !" : "✗ Déplacement impossible");

plateau.Afficher();
```
:::
