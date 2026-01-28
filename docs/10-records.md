# Records (C# 9+)

## Introduction aux Records

Les **records** sont un type de référence introduit en C# 9, conçu spécialement pour les objets qui servent principalement à **transporter des données**. Ils offrent une syntaxe concise et des comportements par défaut adaptés aux objets immuables.

::: tip 🎯 Ce que vous allez apprendre
- Créer des records pour simplifier vos classes de données
- Comprendre l'égalité par valeur vs par référence
- Utiliser l'expression `with` pour créer des copies modifiées
- Savoir quand choisir record vs class vs struct
:::

### 📦 Analogie : le colis scellé

Pensez à un record comme un **colis scellé** :
- Son contenu est défini à la création et ne peut plus être modifié
- Deux colis sont identiques si leur contenu est identique (pas besoin d'être le même colis physique)
- Pour modifier quelque chose, vous devez créer un nouveau colis

```
┌─────────────────────────────────────────────────────────────────────┐
│                    RECORD vs CLASSE                                 │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│    Classe (comparaison par référence)                               │
│    ┌───────────┐    ┌───────────┐                                   │
│    │ Nom=Alice │    │ Nom=Alice │                                   │
│    │ Age=25    │    │ Age=25    │                                   │
│    └───────────┘    └───────────┘                                   │
│         p1              p2                                          │
│                                                                     │
│    p1 == p2  →  ❌ FALSE (objets différents)                        │
│                                                                     │
│    Record (comparaison par valeur)                                  │
│    ┌───────────┐    ┌───────────┐                                   │
│    │ Nom=Alice │    │ Nom=Alice │                                   │
│    │ Age=25    │    │ Age=25    │                                   │
│    └───────────┘    └───────────┘                                   │
│         r1              r2                                          │
│                                                                     │
│    r1 == r2  →  ✅ TRUE (mêmes valeurs)                             │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### Problème avec les classes classiques

Avec une classe traditionnelle, créer un objet de données simple nécessite beaucoup de code :

```csharp
// Classe traditionnelle : beaucoup de code boilerplate
class PersonneClasse
{
    public string Nom { get; }
    public string Prenom { get; }
    public int Age { get; }
    
    public PersonneClasse(string nom, string prenom, int age)
    {
        Nom = nom;
        Prenom = prenom;
        Age = age;
    }
    
    // Pour l'égalité par valeur
    public override bool Equals(object obj)
    {
        return obj is PersonneClasse p && 
               Nom == p.Nom && 
               Prenom == p.Prenom && 
               Age == p.Age;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Nom, Prenom, Age);
    }
    
    public override string ToString()
    {
        return $"PersonneClasse {{ Nom = {Nom}, Prenom = {Prenom}, Age = {Age} }}";
    }
}
```

### Solution avec les Records

```csharp
// Record : une seule ligne !
record Personne(string Nom, string Prenom, int Age);
```

Cette simple ligne génère automatiquement :
- Les propriétés `Nom`, `Prenom`, `Age`
- Un constructeur avec ces paramètres
- L'égalité par valeur (`Equals`, `GetHashCode`, `==`, `!=`)
- Une méthode `ToString()` lisible
- Une méthode `Deconstruct` pour la déconstruction

## Syntaxe des Records

### Record positionnel (syntaxe courte)

```csharp
record Livre(string Titre, string Auteur, int Annee);

// Utilisation
Livre livre = new Livre("1984", "George Orwell", 1949);
Console.WriteLine(livre.Titre);   // 1984
Console.WriteLine(livre);         // Livre { Titre = 1984, Auteur = George Orwell, Annee = 1949 }
```

### Record avec corps (syntaxe longue)

```csharp
record Produit(string Reference, string Nom, decimal Prix)
{
    // Propriétés supplémentaires
    public decimal PrixTTC => Prix * 1.21m;
    
    // Méthodes supplémentaires
    public string Afficher() => $"{Nom} ({Reference}) - {Prix:C}";
    
    // Propriété calculée
    public bool EstCher => Prix > 100;
}
```

### Record avec propriétés explicites

```csharp
record Client
{
    public required string Nom { get; init; }
    public required string Email { get; init; }
    public DateTime DateInscription { get; init; } = DateTime.Now;
    public bool EstActif { get; set; } = true;  // Mutable
}

// Utilisation avec initialiseur
Client c = new Client 
{ 
    Nom = "Alice", 
    Email = "alice@email.com" 
};
```

## Immuabilité

Par défaut, les propriétés positionnelles des records sont **init-only** : elles peuvent être définies à la création mais pas modifiées ensuite.

```csharp
record Point(int X, int Y);

Point p = new Point(10, 20);
Console.WriteLine(p.X);  // 10

// ❌ ERREUR : les propriétés sont en lecture seule
// p.X = 30;
```

::: tip Pourquoi l'immuabilité ?
L'immuabilité rend le code plus prévisible et plus sûr :
- Pas de modifications surprises d'objets partagés
- Thread-safe par conception
- Facilite le débogage (état constant)
:::

## Expression `with` (copie avec modification)

Puisque les records sont immuables, comment créer une version modifiée ? Avec l'expression `with` :

```csharp
record Etudiant(string Nom, int Age, double Moyenne);

Etudiant alice = new Etudiant("Alice", 20, 15.5);

// Créer une copie avec une modification
Etudiant aliceAgee = alice with { Age = 21 };

Console.WriteLine(alice);      // Etudiant { Nom = Alice, Age = 20, Moyenne = 15.5 }
Console.WriteLine(aliceAgee);  // Etudiant { Nom = Alice, Age = 21, Moyenne = 15.5 }

// L'original n'est pas modifié
Console.WriteLine(alice.Age);  // 20
```

On peut modifier plusieurs propriétés :

```csharp
Etudiant aliceModifiee = alice with { Age = 21, Moyenne = 16.0 };
```

## Égalité par valeur

La grande différence entre les classes et les records : les records comparent par **valeur**, pas par **référence**.

```csharp
record Coordonnee(int X, int Y);

Coordonnee c1 = new Coordonnee(10, 20);
Coordonnee c2 = new Coordonnee(10, 20);
Coordonnee c3 = new Coordonnee(30, 40);

// Comparaison par valeur
Console.WriteLine(c1 == c2);  // True (mêmes valeurs)
Console.WriteLine(c1 == c3);  // False

Console.WriteLine(c1.Equals(c2));  // True
```

Avec une classe, la même comparaison :

```csharp
class PointClasse
{
    public int X { get; set; }
    public int Y { get; set; }
}

PointClasse p1 = new PointClasse { X = 10, Y = 20 };
PointClasse p2 = new PointClasse { X = 10, Y = 20 };

Console.WriteLine(p1 == p2);  // False ! (références différentes)
```

## Déconstruction

Les records supportent automatiquement la déconstruction :

```csharp
record Adresse(string Rue, string Ville, string CodePostal);

Adresse adr = new Adresse("Rue de la Paix", "Bruxelles", "1000");

// Déconstruction
var (rue, ville, cp) = adr;
Console.WriteLine($"{rue}, {cp} {ville}");  // Rue de la Paix, 1000 Bruxelles

// Ignorer certaines valeurs
var (_, villeSeule, _) = adr;
Console.WriteLine(villeSeule);  // Bruxelles
```

## Record struct (C# 10+)

C# 10 introduit les **record struct** : des records qui sont des types valeur au lieu de types référence.

```csharp
// Record struct (type valeur)
record struct Point3D(double X, double Y, double Z);

// Utilisation
Point3D p = new Point3D(1.0, 2.0, 3.0);
```

Différences avec `record` (class) :

| Aspect | `record` (class) | `record struct` |
|--------|------------------|-----------------|
| Type | Référence | Valeur |
| Allocation | Heap | Stack |
| Défaut | null | Valeurs par défaut |
| Performance | Allocation GC | Pas d'allocation |

### readonly record struct

Pour garantir l'immuabilité :

```csharp
readonly record struct Temperature(double Celsius)
{
    public double Fahrenheit => Celsius * 9 / 5 + 32;
    public double Kelvin => Celsius + 273.15;
}
```

## Héritage de records

Les records peuvent hériter d'autres records :

```csharp
record Vehicule(string Marque, string Modele);
record Voiture(string Marque, string Modele, int NombrePortes) : Vehicule(Marque, Modele);
record VoitureElectrique(string Marque, string Modele, int NombrePortes, int Autonomie) 
    : Voiture(Marque, Modele, NombrePortes);

// Utilisation
VoitureElectrique tesla = new VoitureElectrique("Tesla", "Model 3", 4, 500);
Console.WriteLine(tesla.Marque);     // Tesla
Console.WriteLine(tesla.Autonomie);  // 500
Console.WriteLine(tesla);
// VoitureElectrique { Marque = Tesla, Modele = Model 3, NombrePortes = 4, Autonomie = 500 }
```

## Cas d'usage typiques

### 1. Data Transfer Objects (DTO)

```csharp
record ClientDTO(int Id, string Nom, string Email);
record CommandeDTO(int Id, DateTime Date, decimal Total, List<LigneDTO> Lignes);
record LigneDTO(string Produit, int Quantite, decimal Prix);
```

### 2. Messages d'événements

```csharp
record UtilisateurCreé(int Id, string Email, DateTime Date);
record CommandeValidée(int CommandeId, int ClientId, decimal Montant);
record PaiementReçu(string TransactionId, decimal Montant, DateTime Date);
```

### 3. Configuration immuable

```csharp
record ConfigurationApi(
    string BaseUrl,
    int TimeoutSeconds = 30,
    int MaxRetries = 3,
    bool UseHttps = true
);

// Utilisation avec valeurs par défaut
var config = new ConfigurationApi("https://api.example.com");
var configCustom = new ConfigurationApi("https://api.example.com", TimeoutSeconds: 60);
```

### 4. Résultats d'opérations

```csharp
record Resultat<T>(bool Succes, T Valeur, string MessageErreur = null)
{
    public static Resultat<T> Ok(T valeur) => new(true, valeur);
    public static Resultat<T> Erreur(string message) => new(false, default, message);
}

// Utilisation
Resultat<int> ParseInt(string s)
{
    if (int.TryParse(s, out int valeur))
        return Resultat<int>.Ok(valeur);
    return Resultat<int>.Erreur($"'{s}' n'est pas un entier valide");
}
```

## Comparaison : Record vs Class vs Struct

| Caractéristique | Class | Record (class) | Record struct |
|-----------------|-------|----------------|---------------|
| Type | Référence | Référence | Valeur |
| Égalité | Par référence | Par valeur | Par valeur |
| Immuabilité | Non (par défaut) | Oui (par défaut) | Configurable |
| `with` | Non | Oui | Oui |
| Héritage | Oui | Oui (records) | Non |
| ToString | Basique | Détaillé | Détaillé |

## Quand utiliser les Records ?

| Situation | Recommandation |
|-----------|----------------|
| Objet avec beaucoup de logique | **Classe** |
| Données immuables | **Record** |
| Égalité par valeur importante | **Record** |
| Type léger, fréquemment créé | **Record struct** |
| Collections de données | **Record** |
| Héritage complexe | **Classe** |

## Exercices

### Exercice 1 : Système de commandes

Créez les records suivants :
- `Article(string Reference, string Nom, decimal Prix)`
- `LigneCommande(Article Article, int Quantite)` avec une propriété `Total`
- `Commande(int Numero, DateTime Date, List<LigneCommande> Lignes)` avec un `Total`

::: details 💡 Solution Exercice 1

```csharp
record Article(string Reference, string Nom, decimal Prix);

record LigneCommande(Article Article, int Quantite)
{
    public decimal Total => Article.Prix * Quantite;
    
    public override string ToString() => 
        $"  {Quantite}x {Article.Nom} @ {Article.Prix:C} = {Total:C}";
}

record Commande(int Numero, DateTime Date, List<LigneCommande> Lignes)
{
    public decimal Total => Lignes.Sum(l => l.Total);
    
    public void Afficher()
    {
        Console.WriteLine($"=== Commande #{Numero} du {Date:d} ===");
        foreach (var ligne in Lignes)
        {
            Console.WriteLine(ligne);
        }
        Console.WriteLine($"  TOTAL: {Total:C}");
    }
}

// Test
var article1 = new Article("REF001", "Clavier mécanique", 89.99m);
var article2 = new Article("REF002", "Souris gaming", 49.99m);
var article3 = new Article("REF003", "Tapis de souris XL", 24.99m);

var commande = new Commande(
    1001,
    DateTime.Now,
    new List<LigneCommande>
    {
        new LigneCommande(article1, 1),
        new LigneCommande(article2, 2),
        new LigneCommande(article3, 1)
    }
);

commande.Afficher();
```

**Sortie** :
```
=== Commande #1001 du 15/01/2025 ===
  1x Clavier mécanique @ 89,99 € = 89,99 €
  2x Souris gaming @ 49,99 € = 99,98 €
  1x Tapis de souris XL @ 24,99 € = 24,99 €
  TOTAL: 214,96 €
```
:::

### Exercice 2 : Historique de modifications

Créez un système où chaque modification d'un `Document` crée une nouvelle version :
- `Document(int Id, string Titre, string Contenu, int Version)`
- Méthode `Modifier(string nouveauContenu)` qui retourne un nouveau record avec version+1

::: details 💡 Solution Exercice 2

```csharp
record Document(int Id, string Titre, string Contenu, int Version)
{
    public DateTime DateModification { get; init; } = DateTime.Now;
    
    // Créer une nouvelle version avec le contenu modifié
    public Document Modifier(string nouveauContenu)
    {
        return this with 
        { 
            Contenu = nouveauContenu, 
            Version = Version + 1,
            DateModification = DateTime.Now
        };
    }
    
    // Créer une nouvelle version avec le titre modifié
    public Document RenommerEn(string nouveauTitre)
    {
        return this with 
        { 
            Titre = nouveauTitre, 
            Version = Version + 1,
            DateModification = DateTime.Now
        };
    }
    
    public override string ToString() => 
        $"Document #{Id} v{Version}: \"{Titre}\" ({Contenu.Length} caractères)";
}

// Test
var doc1 = new Document(1, "Mon rapport", "Contenu initial du rapport.", 1);
Console.WriteLine(doc1);  // Document #1 v1: "Mon rapport" (28 caractères)

var doc2 = doc1.Modifier("Contenu modifié avec plus de détails et d'informations.");
Console.WriteLine(doc2);  // Document #1 v2: "Mon rapport" (54 caractères)

var doc3 = doc2.RenommerEn("Rapport final");
Console.WriteLine(doc3);  // Document #1 v3: "Rapport final" (54 caractères)

// Les anciennes versions existent toujours !
Console.WriteLine($"\nHistorique :");
Console.WriteLine($"  v1: {doc1.Titre} - {doc1.DateModification:T}");
Console.WriteLine($"  v2: {doc2.Titre} - {doc2.DateModification:T}");
Console.WriteLine($"  v3: {doc3.Titre} - {doc3.DateModification:T}");
```

**Remarque** : L'expression `with` crée une copie du record avec les propriétés spécifiées modifiées. Les anciennes versions ne sont pas affectées, ce qui permet de conserver un historique complet.
:::

### Exercice 3 : Résultat d'opération générique

Créez un record générique `Resultat<T>` qui encapsule le succès ou l'échec d'une opération :

::: details 💡 Solution Exercice 3

```csharp
record Resultat<T>(bool EstSucces, T Valeur, string Erreur = null)
{
    // Factory methods
    public static Resultat<T> Succes(T valeur) => new(true, valeur);
    public static Resultat<T> Echec(string erreur) => new(false, default, erreur);
    
    // Méthode pour traiter le résultat
    public TResult Match<TResult>(
        Func<T, TResult> siSucces,
        Func<string, TResult> siEchec)
    {
        return EstSucces ? siSucces(Valeur) : siEchec(Erreur);
    }
}

// Exemple d'utilisation
Resultat<int> ParseEntier(string texte)
{
    if (int.TryParse(texte, out int valeur))
        return Resultat<int>.Succes(valeur);
    
    return Resultat<int>.Echec($"'{texte}' n'est pas un entier valide");
}

Resultat<double> Diviser(double a, double b)
{
    if (b == 0)
        return Resultat<double>.Echec("Division par zéro impossible");
    
    return Resultat<double>.Succes(a / b);
}

// Tests
var r1 = ParseEntier("42");
var r2 = ParseEntier("abc");
var r3 = Diviser(10, 3);
var r4 = Diviser(10, 0);

Console.WriteLine(r1.Match(
    v => $"✅ Valeur: {v}",
    e => $"❌ Erreur: {e}"
));  // ✅ Valeur: 42

Console.WriteLine(r2.Match(
    v => $"✅ Valeur: {v}",
    e => $"❌ Erreur: {e}"
));  // ❌ Erreur: 'abc' n'est pas un entier valide

Console.WriteLine(r3.Match(
    v => $"✅ Résultat: {v:F2}",
    e => $"❌ Erreur: {e}"
));  // ✅ Résultat: 3.33

Console.WriteLine(r4.Match(
    v => $"✅ Résultat: {v:F2}",
    e => $"❌ Erreur: {e}"
));  // ❌ Erreur: Division par zéro impossible
```
:::

## Résumé

| Concept | Description |
|---------|-------------|
| **record** | Type pour données immuables avec égalité par valeur |
| **Syntaxe positionnelle** | `record Nom(Type Prop);` - ultra concis |
| **`with`** | Crée une copie avec modifications |
| **Égalité par valeur** | `==` compare les propriétés, pas les références |
| **Déconstruction** | `var (a, b) = monRecord;` |
| **record struct** | Version type valeur (C# 10+) |
