# Code source - Étape 2 : Héritage

Le code source complet de cette étape est disponible ci-dessous.

## 📥 Télécharger

Vous pouvez télécharger tous les fichiers du projet en tant que dossier compressé ou les copier individuellement.

## 📂 Structure des fichiers

```
etape2-heritage/
├── Echecs.csproj
├── Couleur.cs
├── Piece.cs (class abstraite)
├── Roi.cs
├── Dame.cs
├── Tour.cs
├── Fou.cs
├── Cavalier.cs
├── Pion.cs
├── Plateau.cs
└── Program.cs
```

## 🚀 Comment utiliser ?

1. **Créer un dossier** pour votre projet
2. **Copier les fichiers** dans ce dossier
3. **Ouvrir un terminal** dans le dossier
4. **Exécuter** : `dotnet run`

## 💾 Fichiers sources

Les fichiers sont situés dans le dossier statique `/public/codes/etape2-heritage/`

### Fichiers à copier :

- `Echecs.csproj` - Configuration du projet
- `Couleur.cs` - Énumération
- `Piece.cs` - **Classe abstraite** (changement clé!)
- `Roi.cs`, `Dame.cs`, `Tour.cs`, `Fou.cs`, `Cavalier.cs`, `Pion.cs` - Classes dérivées
- `Plateau.cs` - Plateau utilisant l'héritage
- `Program.cs` - Démonstration

## ℹ️ À propos de cette étape

Cette étape couvre :
- Les classes abstraites
- L'héritage (`class X : Piece`)
- La spécialisation des comportements
- La polymorphie (implicite)

Retour à la [Partie III - Projet Fil Rouge](/16-projet-echecs)
