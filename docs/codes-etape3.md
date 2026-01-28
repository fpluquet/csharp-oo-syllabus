# Code source - Étape 3 : Polymorphisme

Le code source complet de cette étape est disponible ci-dessous.

## 📥 Télécharger

Vous pouvez télécharger tous les fichiers du projet en tant que dossier compressé ou les copier individuellement.

## 📂 Structure des fichiers

```
etape3-polymorphisme/
├── Echecs.csproj
├── Couleur.cs
├── Piece.cs (abstract avec Valeur)
├── Roi.cs
├── Dame.cs
├── Tour.cs
├── Fou.cs
├── Cavalier.cs
├── Pion.cs
├── Plateau.cs (avec CalculerScore polymorphe)
└── Program.cs
```

## 🚀 Comment utiliser ?

1. **Créer un dossier** pour votre projet
2. **Copier les fichiers** dans ce dossier
3. **Ouvrir un terminal** dans le dossier
4. **Exécuter** : `dotnet run`

## 💾 Fichiers sources

Les fichiers sont situés dans le dossier statique `/public/codes/etape3-polymorphisme/`

### Fichiers à copier :

- `Echecs.csproj` - Configuration du projet
- `Couleur.cs` - Énumération
- `Piece.cs` - Classe abstraite avec `Valeur { get; }`
- `Roi.cs`, `Dame.cs`, `Tour.cs`, `Fou.cs`, `Cavalier.cs`, `Pion.cs` - Avec `override int Valeur`
- `Plateau.cs` - Avec méthode `CalculerScore(Couleur)` polymorphe
- `Program.cs` - Démonstration du polymorphisme

## ℹ️ À propos de cette étape

Cette étape couvre :
- Les propriétés abstraites
- Le polymorphisme avec `abstract` et `override`
- La liaison dynamique (dynamic binding)
- Type déclaré vs type réel

Retour à la [Partie III - Projet Fil Rouge](/16-projet-echecs)
