# Code source - Étape 4 : Architecture MVC

Le code source complet de cette étape avec le **jeu d'échecs jouable** est disponible ci-dessous.

## 📥 Télécharger

Vous pouvez télécharger tous les fichiers du projet en tant que dossier compressé ou les copier individuellement.

## 📂 Structure des fichiers

```
etape4-mvc/
├── Echecs.csproj
├── Program.cs
├── Models/
│   ├── Enums/
│   │   ├── Couleur.cs
│   │   └── ResultatDeplacement.cs
│   ├── Pieces/
│   │   ├── Piece.cs (abstract)
│   │   ├── Roi.cs, Dame.cs, Tour.cs, Fou.cs, Cavalier.cs, Pion.cs
│   ├── Plateau.cs
│   └── PartieEchecs.cs
├── Views/
│   ├── IEchecsVue.cs (interface)
│   └── ConsoleVue.cs (implémentation)
└── Controllers/
    └── JeuController.cs
```

## 🚀 Comment utiliser ?

1. **Créer un dossier** pour votre projet
2. **Copier les fichiers** en respectant la structure de dossiers ci-dessus
3. **Ouvrir un terminal** dans le dossier racine
4. **Exécuter** : `dotnet run`

### Exemple de structure :

```
MonProjet/
├── Echecs.csproj
├── Program.cs
├── Models/
│   ├── Enums/
│   │   ├── Couleur.cs
│   │   └── ResultatDeplacement.cs
│   └── ... (autres fichiers)
└── ...
```

## 💾 Fichiers sources

Les fichiers sont situés dans le dossier statique `/public/codes/etape4-mvc/`

### Organisation des fichiers

**Models/** (Modèle)
- `PartieEchecs.cs` - Gère l'état de la partie
- `Plateau.cs` - Le plateau 8x8
- `Pieces/Piece.cs` et classes dérivées
- `Enums/` - Énumérations

**Views/** (Vue)
- `IEchecsVue.cs` - Interface que la vue doit implémenter
- `ConsoleVue.cs` - Implémentation pour la console

**Controllers/** (Contrôleur)
- `JeuController.cs` - Orchestre le jeu

## 🎮 Jouer au jeu

```bash
dotnet run
```

Ensuite, utilisez les commandes :
- `e2 e4` - Déplacer une pièce de e2 vers e4
- `q` - Quitter

## ℹ️ À propos de cette étape

Cette étape couvre :
- Le pattern **MVC** (Modèle-Vue-Contrôleur)
- La **séparation des responsabilités**
- Les **interfaces** pour rendre le code flexible
- Les **primary constructors** (C# 12)
- L'architecture en couches

## 🎯 Fonctionnalités complètes

✅ Affichage du plateau  
✅ Déplacement des pièces  
✅ Validation des mouvements  
✅ Calcul des scores  
✅ Alternance des joueurs  
✅ Gestion des erreurs  

Retour à la [Partie III - Projet Fil Rouge](/18-projet-echecs)
