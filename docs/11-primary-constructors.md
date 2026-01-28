# Primary Constructors (C# 12+)

## Introduction

Les **Primary Constructors** (constructeurs primaires), introduits en C# 12, permettent de déclarer les paramètres du constructeur directement dans la déclaration de la classe. Cela réduit considérablement le code boilerplate.

::: tip 🎯 Ce que vous allez apprendre
- Utiliser la syntaxe des primary constructors pour simplifier vos classes
- Comprendre la différence avec les records
- Combiner primary constructors avec d'autres constructeurs
- Appliquer la validation dans les primary constructors
:::

### ✂️ Analogie : la recette simplifiée

Avant, pour cuisiner un gâteau, vous deviez écrire :
1. Liste des ingrédients
2. Stocker chaque ingrédient dans un bol séparé
3. Documenter où se trouve chaque ingrédient

Avec les primary constructors, vous dites simplement : "Gâteau(farine, sucre, œufs)" et tout est prêt à l'emploi !

```
┌─────────────────────────────────────────────────────────────────────┐
│                AVANT vs APRÈS C# 12                                 │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│    AVANT (traditionnel)              APRÈS (primary constructor)   │
│    ─────────────────────             ────────────────────────────   │
│                                                                     │
│    class Client                      class Client(int id, string nom)│
│    {                                 {                              │
│        private int _id;                  public int Id => id;       │
│        private string _nom;              public string Nom => nom;  │
│                                      }                              │
│        public Client(int id,                                        │
│                       string nom)    // 10+ lignes → 4 lignes !     │
│        {                                                            │
│            _id = id;                                                │
│            _nom = nom;                                              │
│        }                                                            │
│                                                                     │
│        public int Id => _id;                                        │
│        public string Nom => _nom;                                   │
│    }                                                                │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

## Le problème avec les classes traditionnelles

Avant C# 12, initialiser une classe simple nécessitait beaucoup de code répétitif :

```csharp
// Avant C# 12 : beaucoup de code
class Client
{
    private readonly int _id;
    private readonly string _nom;
    private readonly string _email;
    
    public Client(int id, string nom, string email)
    {
        _id = id;
        _nom = nom;
        _email = email;
    }
    
    public int Id => _id;
    public string Nom => _nom;
    public string Email => _email;
}
```

## Syntaxe du Primary Constructor

```csharp
// C# 12 : syntaxe concise
class Client(int id, string nom, string email)
{
    public int Id => id;
    public string Nom => nom;
    public string Email => email;
}

// Utilisation
Client c = new Client(1, "Alice", "alice@email.com");
Console.WriteLine(c.Nom);  // Alice
```

Les paramètres `id`, `nom`, `email` sont directement accessibles dans toute la classe.

## Différence avec les Records

::: warning Attention
Contrairement aux records, les paramètres d'un primary constructor **ne créent PAS automatiquement des propriétés**.
:::

```csharp
// Record : les paramètres SONT des propriétés
record ClientRecord(int Id, string Nom, string Email);

ClientRecord cr = new ClientRecord(1, "Bob", "bob@email.com");
Console.WriteLine(cr.Id);  // ✓ OK : Id est une propriété

// Classe avec primary constructor : les paramètres NE SONT PAS des propriétés
class ClientClasse(int id, string nom, string email);

ClientClasse cc = new ClientClasse(1, "Bob", "bob@email.com");
// Console.WriteLine(cc.id);  // ❌ ERREUR : 'id' n'est pas accessible
```

Pour exposer les valeurs, vous devez créer des propriétés manuellement :

```csharp
class ClientClasse(int id, string nom, string email)
{
    public int Id { get; } = id;
    public string Nom { get; } = nom;
    public string Email { get; } = email;
}
```

## Utilisation des paramètres

Les paramètres du primary constructor peuvent être utilisés :

### 1. Dans les initialiseurs de propriétés

```csharp
class Produit(string reference, string nom, decimal prix)
{
    public string Reference { get; } = reference;
    public string Nom { get; set; } = nom;
    public decimal Prix { get; set; } = prix;
    public decimal PrixTTC { get; } = prix * 1.21m;
}
```

### 2. Dans les initialiseurs de champs

```csharp
class Service(string nom)
{
    private readonly string _nomMajuscules = nom.ToUpper();
    private readonly DateTime _dateCreation = DateTime.Now;
    
    public void Afficher()
    {
        Console.WriteLine($"{_nomMajuscules} créé le {_dateCreation}");
    }
}
```

### 3. Dans les méthodes

```csharp
class Calculatrice(int precision)
{
    public double Arrondir(double valeur)
    {
        return Math.Round(valeur, precision);
    }
    
    public string Formater(double valeur)
    {
        return valeur.ToString($"F{precision}");
    }
}

Calculatrice calc = new Calculatrice(2);
Console.WriteLine(calc.Arrondir(3.14159));  // 3.14
Console.WriteLine(calc.Formater(3.14159)); // 3.14
```

### 4. Dans les propriétés calculées

```csharp
class Rectangle(double largeur, double hauteur)
{
    public double Aire => largeur * hauteur;
    public double Perimetre => 2 * (largeur + hauteur);
    public bool EstCarre => largeur == hauteur;
}
```

## Combinaison avec d'autres constructeurs

Vous pouvez avoir des constructeurs secondaires qui appellent le primary constructor :

```csharp
class Personne(string nom, string prenom, DateTime dateNaissance)
{
    public string Nom { get; } = nom;
    public string Prenom { get; } = prenom;
    public DateTime DateNaissance { get; } = dateNaissance;
    
    // Constructeur secondaire
    public Personne(string nom, string prenom) 
        : this(nom, prenom, DateTime.MinValue)
    {
    }
    
    // Autre constructeur secondaire
    public Personne(string nomComplet, DateTime dateNaissance)
        : this(
            nomComplet.Split(' ').Last(),
            nomComplet.Split(' ').First(),
            dateNaissance)
    {
    }
    
    public int Age => DateTime.Now.Year - DateNaissance.Year;
}
```

## Validation dans le Primary Constructor

La validation peut se faire via des propriétés avec logique :

```csharp
class CompteBancaire(string titulaire, decimal soldeInitial)
{
    public string Titulaire { get; } = string.IsNullOrWhiteSpace(titulaire) 
        ? throw new ArgumentException("Le titulaire est requis")
        : titulaire;
        
    public decimal Solde { get; private set; } = soldeInitial >= 0 
        ? soldeInitial 
        : throw new ArgumentException("Le solde initial ne peut pas être négatif");
    
    public void Deposer(decimal montant)
    {
        if (montant <= 0)
            throw new ArgumentException("Le montant doit être positif");
        Solde += montant;
    }
}
```

Ou via un bloc d'initialisation :

```csharp
class Email(string adresse)
{
    public string Adresse { get; } = adresse;
    public string Domaine { get; } = adresse.Split('@').Last();
    
    // Bloc d'initialisation pour validation complexe
    private readonly bool _ = ValidEmail(adresse) 
        ? true 
        : throw new ArgumentException("Email invalide");
    
    private static bool ValidEmail(string email) 
        => email.Contains("@") && email.Contains(".");
}
```

## Primary Constructors et DI (Dependency Injection)

Les primary constructors sont particulièrement utiles pour l'injection de dépendances :

```csharp
// Avant C# 12
class ClientService
{
    private readonly IClientRepository _repository;
    private readonly ILogger _logger;
    
    public ClientService(IClientRepository repository, ILogger logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public Client ObtenirClient(int id)
    {
        _logger.Log($"Recherche du client {id}");
        return _repository.GetById(id);
    }
}

// Avec C# 12
class ClientService(IClientRepository repository, ILogger logger)
{
    public Client ObtenirClient(int id)
    {
        logger.Log($"Recherche du client {id}");
        return repository.GetById(id);
    }
}
```

## Struct avec Primary Constructor

Les primary constructors fonctionnent aussi avec les structs :

```csharp
struct Point(double x, double y)
{
    public double X { get; } = x;
    public double Y { get; } = y;
    
    public double Distance => Math.Sqrt(X * X + Y * Y);
    
    public Point Translater(double dx, double dy) 
        => new Point(X + dx, Y + dy);
}
```

## Comparaison des syntaxes

### Classe simple

```csharp
// Traditionnel (10+ lignes)
class Etudiant
{
    private readonly string _nom;
    private readonly int _age;
    
    public Etudiant(string nom, int age)
    {
        _nom = nom;
        _age = age;
    }
    
    public string Nom => _nom;
    public int Age => _age;
}

// Primary Constructor (4 lignes)
class Etudiant(string nom, int age)
{
    public string Nom { get; } = nom;
    public int Age { get; } = age;
}

// Record (1 ligne)
record Etudiant(string Nom, int Age);
```

### Quand utiliser quoi ?

| Besoin | Recommandation |
|--------|----------------|
| Données immuables simples | **Record** |
| Classe avec logique métier | **Primary Constructor** |
| Compatibilité < C# 12 | **Constructeur traditionnel** |
| Propriétés mutables | **Primary Constructor** ou classe |
| DI / Services | **Primary Constructor** |

## Exemple complet : Gestionnaire de tâches

```csharp
interface INotificationService
{
    void Notifier(string message);
}

class TaskManager(INotificationService notifications, int maxTaches = 100)
{
    private readonly List<Tache> _taches = new();
    
    public int NombreTaches => _taches.Count;
    public int Capacite => maxTaches;
    public bool EstPlein => _taches.Count >= maxTaches;
    
    public bool AjouterTache(string titre, DateTime echeance)
    {
        if (EstPlein)
        {
            notifications.Notifier("Liste de tâches pleine!");
            return false;
        }
        
        var tache = new Tache(titre, echeance);
        _taches.Add(tache);
        notifications.Notifier($"Tâche '{titre}' ajoutée");
        return true;
    }
    
    public IEnumerable<Tache> TachesEnRetard => 
        _taches.Where(t => t.Echeance < DateTime.Now && !t.EstTerminee);
    
    record Tache(string Titre, DateTime Echeance)
    {
        public bool EstTerminee { get; set; }
    }
}
```

## Exercices

### Exercice 1 : Convertisseur de température

Créez une classe `Temperature(double celsius)` avec :
- Propriétés `Celsius`, `Fahrenheit`, `Kelvin` (calculées)
- Méthodes statiques factory : `FromFahrenheit(double f)`, `FromKelvin(double k)`

::: details 💡 Solution Exercice 1

```csharp
class Temperature(double celsius)
{
    public double Celsius { get; } = celsius;
    public double Fahrenheit => Celsius * 9 / 5 + 32;
    public double Kelvin => Celsius + 273.15;
    
    // Factory methods
    public static Temperature FromCelsius(double c) => new Temperature(c);
    
    public static Temperature FromFahrenheit(double f) 
        => new Temperature((f - 32) * 5 / 9);
    
    public static Temperature FromKelvin(double k) 
        => new Temperature(k - 273.15);
    
    // Validation
    private static readonly double ZeroAbsolu = -273.15;
    private readonly bool _ = celsius >= ZeroAbsolu 
        ? true 
        : throw new ArgumentException($"Température impossible (sous le zéro absolu)");
    
    public override string ToString() 
        => $"{Celsius:F1}°C = {Fahrenheit:F1}°F = {Kelvin:F1}K";
}

// Tests
var t1 = new Temperature(25);
Console.WriteLine(t1);  // 25.0°C = 77.0°F = 298.2K

var t2 = Temperature.FromFahrenheit(98.6);
Console.WriteLine($"Température corporelle: {t2.Celsius:F1}°C");  // 37.0°C

var t3 = Temperature.FromKelvin(0);
Console.WriteLine($"Zéro absolu: {t3}");  // -273.2°C = -459.7°F = 0.0K

// try { new Temperature(-300); }  // Exception : sous le zéro absolu
```
:::

### Exercice 2 : Service de validation

Créez une classe `ValidationService(int longueurMinMdp, bool exigerChiffre)` avec :
- Méthode `ValiderMotDePasse(string mdp)` → `(bool valide, string erreur)`
- Méthode `ValiderEmail(string email)` → `(bool valide, string erreur)`

::: details 💡 Solution Exercice 2

```csharp
class ValidationService(int longueurMinMdp, bool exigerChiffre, bool exigerMajuscule = false)
{
    public (bool Valide, string Erreur) ValiderMotDePasse(string mdp)
    {
        if (string.IsNullOrEmpty(mdp))
            return (false, "Le mot de passe ne peut pas être vide");
        
        if (mdp.Length < longueurMinMdp)
            return (false, $"Le mot de passe doit contenir au moins {longueurMinMdp} caractères");
        
        if (exigerChiffre && !mdp.Any(char.IsDigit))
            return (false, "Le mot de passe doit contenir au moins un chiffre");
        
        if (exigerMajuscule && !mdp.Any(char.IsUpper))
            return (false, "Le mot de passe doit contenir au moins une majuscule");
        
        return (true, null);
    }
    
    public (bool Valide, string Erreur) ValiderEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return (false, "L'email ne peut pas être vide");
        
        if (!email.Contains("@"))
            return (false, "L'email doit contenir @");
        
        var parties = email.Split('@');
        if (parties.Length != 2)
            return (false, "L'email ne peut contenir qu'un seul @");
        
        if (string.IsNullOrEmpty(parties[0]))
            return (false, "La partie avant @ ne peut pas être vide");
        
        if (!parties[1].Contains("."))
            return (false, "Le domaine doit contenir un point");
        
        return (true, null);
    }
    
    public void AfficherResultat(string nom, (bool Valide, string Erreur) resultat)
    {
        if (resultat.Valide)
            Console.WriteLine($"✅ {nom} : Valide");
        else
            Console.WriteLine($"❌ {nom} : {resultat.Erreur}");
    }
}

// Tests
var validator = new ValidationService(
    longueurMinMdp: 8,
    exigerChiffre: true,
    exigerMajuscule: true
);

validator.AfficherResultat("'abc'", validator.ValiderMotDePasse("abc"));
// ❌ 'abc' : Le mot de passe doit contenir au moins 8 caractères

validator.AfficherResultat("'motdepasse'", validator.ValiderMotDePasse("motdepasse"));
// ❌ 'motdepasse' : Le mot de passe doit contenir au moins un chiffre

validator.AfficherResultat("'Secure123'", validator.ValiderMotDePasse("Secure123"));
// ✅ 'Secure123' : Valide

validator.AfficherResultat("'test@email.com'", validator.ValiderEmail("test@email.com"));
// ✅ 'test@email.com' : Valide

validator.AfficherResultat("'invalid'", validator.ValiderEmail("invalid"));
// ❌ 'invalid' : L'email doit contenir @
```
:::

### Exercice 3 : Calculatrice configurable

Créez une classe `Calculatrice(int precision, bool afficherEtapes)` qui :
- Arrondit les résultats selon la précision
- Affiche optionnellement les étapes de calcul

::: details 💡 Solution Exercice 3

```csharp
class Calculatrice(int precision, bool afficherEtapes = false)
{
    public double Additionner(double a, double b)
    {
        if (afficherEtapes)
            Console.WriteLine($"  Étape: {a} + {b}");
        return Arrondir(a + b);
    }
    
    public double Soustraire(double a, double b)
    {
        if (afficherEtapes)
            Console.WriteLine($"  Étape: {a} - {b}");
        return Arrondir(a - b);
    }
    
    public double Multiplier(double a, double b)
    {
        if (afficherEtapes)
            Console.WriteLine($"  Étape: {a} × {b}");
        return Arrondir(a * b);
    }
    
    public double Diviser(double a, double b)
    {
        if (b == 0)
            throw new DivideByZeroException();
        
        if (afficherEtapes)
            Console.WriteLine($"  Étape: {a} ÷ {b}");
        return Arrondir(a / b);
    }
    
    public double CalculerExpression(double a, double b, double c)
    {
        // Calcule (a + b) * c
        if (afficherEtapes)
            Console.WriteLine("Calcul de (a + b) × c :");
        
        double somme = Additionner(a, b);
        double resultat = Multiplier(somme, c);
        
        if (afficherEtapes)
            Console.WriteLine($"  Résultat final: {resultat}");
        
        return resultat;
    }
    
    private double Arrondir(double valeur) => Math.Round(valeur, precision);
}

// Test sans étapes
var calc = new Calculatrice(precision: 2);
Console.WriteLine(calc.Diviser(10, 3));  // 3.33

// Test avec étapes
var calcVerbose = new Calculatrice(precision: 3, afficherEtapes: true);
calcVerbose.CalculerExpression(5.5, 3.3, 2);
// Calcul de (a + b) × c :
//   Étape: 5.5 + 3.3
//   Étape: 8.8 × 2
//   Résultat final: 17.6
```
:::

## Résumé

| Aspect | Description |
|--------|-------------|
| **Syntaxe** | `class Nom(params) { }` |
| **Paramètres** | Accessibles partout dans la classe |
| **Propriétés** | Doivent être créées explicitement |
| **vs Record** | Record crée auto des propriétés, pas le primary constructor |
| **Validation** | Via propriétés ou initialiseurs |
| **DI** | Excellent pour l'injection de dépendances |
