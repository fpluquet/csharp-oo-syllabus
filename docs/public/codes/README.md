# 🎮 Jeu d'Échecs en C# - Codes sources

Ce dossier contient les codes sources complets pour chaque étape du projet d'échecs.

## 📁 Structure

```
codes/
├── etape1-encapsulation/   ← Classes de base, enum, switch
├── etape2-heritage/        ← Classes abstraites, héritage
├── etape3-polymorphisme/   ← Virtual/Override, valeur des pièces
└── etape4-mvc/             ← Architecture MVC complète
```

## 🚀 Comment exécuter ?

### Prérequis
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou supérieur

### Exécution

1. **Télécharger** le dossier de l'étape souhaitée

2. **Ouvrir un terminal** dans ce dossier

3. **Exécuter** :
   ```bash
   dotnet run
   ```

### Exemple pour l'étape 4 (version jouable) :

```bash
cd etape4-mvc
dotnet run
```

## 📋 Contenu de chaque étape

### Étape 1 : Encapsulation
- `Couleur.cs` - Énumération Blanc/Noir
- `TypePiece.cs` - Énumération des types de pièces
- `Piece.cs` - Classe avec switch géant
- `Plateau.cs` - Plateau avec affichage
- `Program.cs` - Tests de déplacement

### Étape 2 : Héritage
- `Piece.cs` - Classe abstraite
- `Roi.cs`, `Dame.cs`, `Tour.cs`, `Fou.cs`, `Cavalier.cs`, `Pion.cs` - Classes dérivées
- `Plateau.cs` - Plateau utilisant l'héritage
- `Program.cs` - Démonstration de l'héritage

### Étape 3 : Polymorphisme
- Mêmes fichiers que l'étape 2
- Ajout de la propriété `Valeur` à chaque pièce
- `CalculerScore()` polymorphe dans `Plateau`
- `Program.cs` - Démonstration du polymorphisme

### Étape 4 : Architecture MVC
```
Models/
├── Enums/
│   ├── Couleur.cs
│   └── ResultatDeplacement.cs
├── Pieces/
│   ├── Piece.cs (abstract)
│   ├── Roi.cs, Dame.cs, Tour.cs, Fou.cs, Cavalier.cs, Pion.cs
├── Plateau.cs
└── PartieEchecs.cs

Views/
├── IEchecsVue.cs (interface)
└── ConsoleVue.cs

Controllers/
└── JeuController.cs

Program.cs
```

## 🎯 Commandes du jeu (Étape 4)

| Commande | Description |
|----------|-------------|
| `e2 e4` | Déplacer la pièce de e2 vers e4 |
| `g1 f3` | Déplacer le cavalier de g1 vers f3 |
| `q` | Quitter le jeu |

## ⚠️ Notes

- Ces projets utilisent **.NET 8** et les fonctionnalités modernes de C# 12
- L'affichage utilise des caractères Unicode (♔ ♕ ♖ etc.)
- Sur Windows, utilisez Windows Terminal pour un meilleur rendu

## 📚 Documentation

Consultez le syllabus complet sur :
[https://fpluquet.github.io/csharp-oo-syllabus/](https://fpluquet.github.io/csharp-oo-syllabus/)
