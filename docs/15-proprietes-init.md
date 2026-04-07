# Propriétés Init-Only (C# 9+)

## Introduction

Imaginez que vous remplissez un formulaire d'inscription officiel. Une fois que vous avez soumis le formulaire et qu'il a été validé, certaines informations (comme votre numéro d'inscription ou votre date de naissance) **ne peuvent plus être modifiées** — elles sont "gravées dans le marbre". Mais d'autres informations (comme votre adresse ou votre email) **restent modifiables** par la suite.

```
┌─────────────────────────────────────────────────────────┐
│              FORMULAIRE D'INSCRIPTION                   │
├─────────────────────────────────────────────────────────┤
│  Numéro d'inscription: INS-2025-001  🔒 (immuable)      │
│  Date de naissance: 15/03/1998       🔒 (immuable)      │
│  ─────────────────────────────────────────────────      │
│  Adresse: 123 Rue des Lilas          ✏️ (modifiable)    │
│  Email: jean@email.com               ✏️ (modifiable)    │
└─────────────────────────────────────────────────────────┘
         ↓ Après validation du formulaire ↓
┌─────────────────────────────────────────────────────────┐
│  🔒 Les champs verrouillés ne peuvent PLUS changer      │
│  ✏️ Les champs modifiables restent accessibles          │
└─────────────────────────────────────────────────────────┘
```

Les propriétés **init-only** permettent de créer des objets dont certaines propriétés peuvent être définies lors de l'instanciation, mais deviennent **immuables immédiatement après**.

:::tip 💡 Pourquoi utiliser init ?
- **Sécurité** : Empêcher la modification accidentelle de données d'identité (ID, numéro de série...)
- **Flexibilité** : Contrairement à `get` seul, on peut utiliser les initialiseurs d'objet
- **Clarté** : Le code exprime clairement quelles propriétés sont fixes
:::

## Le problème avec `set`

Avec un setter classique, les propriétés peuvent être modifiées à tout moment :

```csharp
class Livre
{
    public string Isbn { get; set; }
    public string Titre { get; set; }
}

Livre livre = new Livre { Isbn = "978-2-1234", Titre = "Guide C#" };

// Plus tard dans le code... oups !
livre.Isbn = "MODIFIE";  // L'ISBN ne devrait pas changer !
```

## La solution avec `init`

Le mot-clé `init` remplace `set` pour créer des propriétés initialisables une seule fois :

```csharp
class Livre
{
    public string Isbn { get; init; }      // Init-only
    public string Titre { get; init; }     // Init-only
    public int Pages { get; set; }         // Mutable
}

Livre livre = new Livre 
{ 
    Isbn = "978-2-1234", 
    Titre = "Guide C#",
    Pages = 350
};

livre.Pages = 400;        // ✓ OK : set normal
livre.Isbn = "AUTRE";     // ❌ ERREUR : init-only
```

## Syntaxe et comportement

### Déclaration

```csharp
class Produit
{
    // Propriété auto-implémentée init-only
    public string Reference { get; init; }
    
    // Avec valeur par défaut
    public string Categorie { get; init; } = "Non classé";
    
    // Init-only avec backing field (pour validation)
    private decimal _prix;
    public decimal Prix
    {
        get => _prix;
        init
        {
            if (value < 0)
                throw new ArgumentException("Le prix ne peut être négatif");
            _prix = value;
        }
    }
}
```

### Initialisation possible

Les propriétés init-only peuvent être définies :

1. **Dans un initialiseur d'objet**
```csharp
var p = new Produit { Reference = "PRD-001", Prix = 49.99m };
```

2. **Dans le constructeur de la classe**
```csharp
class Produit
{
    public string Reference { get; init; }
    
    public Produit(string reference)
    {
        Reference = reference;  // OK dans le constructeur
    }
}
```

3. **Dans le constructeur d'une classe dérivée** (avec `protected init`)
```csharp
class ProduitBase
{
    public string Id { get; protected init; }
}

class ProduitSpecial : ProduitBase
{
    public ProduitSpecial()
    {
        Id = Guid.NewGuid().ToString();  // OK
    }
}
```

## Comparaison des accesseurs

| Accesseur | Lecture | Écriture constructeur | Écriture initialiseur | Écriture après |
|-----------|---------|----------------------|----------------------|----------------|
| `get` seul | ✓ | ✓ | ✗ | ✗ |
| `get; set;` | ✓ | ✓ | ✓ | ✓ |
| `get; init;` | ✓ | ✓ | ✓ | ✗ |
| `get; private set;` | ✓ | ✓ | ✗ | ✓ (interne) |

### Illustration

```csharp
class Demonstration
{
    // Lecture seule (doit être initialisé dans le constructeur)
    public int A { get; }
    
    // Lecture/écriture classique
    public int B { get; set; }
    
    // Init-only
    public int C { get; init; }
    
    // Privé en écriture
    public int D { get; private set; }
    
    public Demonstration()
    {
        A = 1;  // OK
        B = 2;  // OK
        C = 3;  // OK
        D = 4;  // OK
    }
    
    public void Modifier()
    {
        // A = 10;  // ❌ Interdit
        B = 20;     // ✓ OK
        // C = 30;  // ❌ Interdit
        D = 40;     // ✓ OK (car private set)
    }
}

// À l'utilisation
var demo = new Demonstration();
// demo.A = 100;  // ❌
demo.B = 200;     // ✓
// demo.C = 300;  // ❌
// demo.D = 400;  // ❌ (private)

// Avec initialiseur
var demo2 = new Demonstration
{
    // A = 1,  // ❌ pas de set/init
    B = 2,    // ✓
    C = 3,    // ✓
    // D = 4  // ❌ private set
};
```

## Cas d'utilisation

### 1. Entités avec identifiants immuables

```csharp
class Client
{
    public int Id { get; init; }           // Ne change jamais
    public string NumeroClient { get; init; }  // Ne change jamais
    public string Nom { get; set; }        // Peut changer
    public string Email { get; set; }      // Peut changer
    public DateTime DateInscription { get; init; } = DateTime.Now;
}

// L'identité est fixée, les données peuvent évoluer
var client = new Client
{
    Id = 1,
    NumeroClient = "CLI-2025-001",
    Nom = "Alice Dupont",
    Email = "alice@email.com"
};

client.Nom = "Alice Martin";  // ✓ Après mariage
client.Id = 999;              // ❌ L'ID ne peut pas changer
```

### 2. Configuration d'objets

```csharp
class OptionsHttpClient
{
    public string BaseUrl { get; init; }
    public int TimeoutSeconds { get; init; } = 30;
    public int MaxRetries { get; init; } = 3;
    public bool UseCompression { get; init; } = true;
    public Dictionary<string, string> Headers { get; init; } = new();
}

// Configuration une fois, puis immuable
var options = new OptionsHttpClient
{
    BaseUrl = "https://api.example.com",
    TimeoutSeconds = 60,
    Headers = new() { ["Authorization"] = "Bearer token123" }
};

// options.BaseUrl = "autre";  // ❌ Interdit
```

### 3. DTOs (Data Transfer Objects)

```csharp
class CommandeDTO
{
    public int Id { get; init; }
    public DateTime Date { get; init; }
    public string ClientId { get; init; }
    public List<LigneDTO> Lignes { get; init; } = new();
    public decimal Total { get; init; }
}

class LigneDTO
{
    public string ProduitId { get; init; }
    public string NomProduit { get; init; }
    public int Quantite { get; init; }
    public decimal PrixUnitaire { get; init; }
    public decimal SousTotal => Quantite * PrixUnitaire;
}
```

### 4. Builders avec résultat immuable

```csharp
class Email
{
    public string Destinataire { get; init; }
    public string Sujet { get; init; }
    public string Corps { get; init; }
    public List<string> Cc { get; init; } = new();
    public bool Prioritaire { get; init; }
}

class EmailBuilder
{
    private string _destinataire;
    private string _sujet;
    private string _corps;
    private List<string> _cc = new();
    private bool _prioritaire;
    
    public EmailBuilder Pour(string email)
    {
        _destinataire = email;
        return this;
    }
    
    public EmailBuilder Sujet(string sujet)
    {
        _sujet = sujet;
        return this;
    }
    
    public EmailBuilder Corps(string corps)
    {
        _corps = corps;
        return this;
    }
    
    public EmailBuilder Cc(params string[] emails)
    {
        _cc.AddRange(emails);
        return this;
    }
    
    public EmailBuilder Prioritaire(bool prioritaire = true)
    {
        _prioritaire = prioritaire;
        return this;
    }
    
    public Email Build() => new Email
    {
        Destinataire = _destinataire,
        Sujet = _sujet,
        Corps = _corps,
        Cc = _cc,
        Prioritaire = _prioritaire
    };
}

// Utilisation fluide
var email = new EmailBuilder()
    .Pour("alice@email.com")
    .Sujet("Réunion demain")
    .Corps("N'oubliez pas la réunion à 10h.")
    .Cc("bob@email.com", "charlie@email.com")
    .Prioritaire()
    .Build();

// L'email est maintenant immuable
```

## Combinaison avec `required` (C# 11+)

```csharp
class Utilisateur
{
    public required string Nom { get; init; }     // Obligatoire ET immuable
    public required string Email { get; init; }   // Obligatoire ET immuable
    public string? Bio { get; init; }             // Optionnel ET immuable
    public DateTime DateCreation { get; init; } = DateTime.Now;
}

// Le compilateur force l'initialisation de Nom et Email
var user = new Utilisateur
{
    Nom = "Alice",
    Email = "alice@email.com"
    // Bio est optionnel
};
```

## Init vs Record

Les records utilisent `init` par défaut pour leurs propriétés positionnelles :

```csharp
record Personne(string Nom, int Age);

// Équivalent approximatif :
class PersonneEquivalent
{
    public string Nom { get; init; }
    public int Age { get; init; }
    
    public PersonneEquivalent(string nom, int age)
    {
        Nom = nom;
        Age = age;
    }
}
```

Mais les records offrent aussi `with` pour créer des copies :

```csharp
var p1 = new Personne("Alice", 25);
var p2 = p1 with { Age = 26 };  // Copie avec modification
```

## Exercices

### Exercice 1 : Système de billets

Créez une classe `Billet` avec :
- `NumeroSerie` (init-only, auto-généré)
- `Evenement` (init-only)
- `DateAchat` (init-only, défaut = maintenant)
- `EstUtilise` (mutable, pour le pointage)

:::details 💡 Solution Exercice 1

```csharp
class Billet
{
    private static int _compteur = 1;
    
    // Numéro auto-généré, ne peut jamais changer
    public string NumeroSerie { get; init; } = $"BIL-{_compteur++:D6}";
    
    // L'événement est fixé à la création
    public required string Evenement { get; init; }
    
    // Date d'achat automatique
    public DateTime DateAchat { get; init; } = DateTime.Now;
    
    // Seul EstUtilise peut être modifié (pour le contrôle d'entrée)
    public bool EstUtilise { get; set; } = false;
    
    public override string ToString()
        => $"[{NumeroSerie}] {Evenement} - Acheté le {DateAchat:dd/MM/yyyy} - {(EstUtilise ? "✓ Utilisé" : "En attente")}";
}

// Utilisation
var billet1 = new Billet { Evenement = "Concert Rock" };
var billet2 = new Billet { Evenement = "Spectacle Théâtre" };

Console.WriteLine(billet1);  // [BIL-000001] Concert Rock - Acheté le 15/01/2025 - En attente
Console.WriteLine(billet2);  // [BIL-000002] Spectacle Théâtre - Acheté le 15/01/2025 - En attente

// À l'entrée de l'événement
billet1.EstUtilise = true;   // ✓ OK
Console.WriteLine(billet1);  // [BIL-000001] Concert Rock - Acheté le 15/01/2025 - ✓ Utilisé

// billet1.NumeroSerie = "FAKE";  // ❌ Erreur : init-only
// billet1.Evenement = "Autre";   // ❌ Erreur : init-only
```
:::

### Exercice 2 : Configuration de jeu

Créez une classe `ConfigurationJeu` avec :
- `NomJoueur` (required, init-only)
- `Difficulte` (init-only, défaut = "Normal")
- `VolumeMusique` (mutable)
- `VolumeSons` (mutable)
- `ModeFullscreen` (init-only)

:::details 💡 Solution Exercice 2

```csharp
class ConfigurationJeu
{
    // Fixé au lancement du jeu, obligatoire
    public required string NomJoueur { get; init; }
    
    // Fixé au lancement (nécessite de redémarrer pour changer)
    public string Difficulte { get; init; } = "Normal";
    public bool ModeFullscreen { get; init; } = true;
    
    // Modifiable à tout moment dans les options
    private int _volumeMusique = 50;
    public int VolumeMusique 
    { 
        get => _volumeMusique;
        set => _volumeMusique = Math.Clamp(value, 0, 100);
    }
    
    private int _volumeSons = 70;
    public int VolumeSons 
    { 
        get => _volumeSons;
        set => _volumeSons = Math.Clamp(value, 0, 100);
    }
    
    public void AfficherConfiguration()
    {
        Console.WriteLine($"""
            ═══════════════════════════════════════
            Configuration de {NomJoueur}
            ═══════════════════════════════════════
            🎮 Difficulté     : {Difficulte} (fixé)
            🖥️ Plein écran    : {(ModeFullscreen ? "Oui" : "Non")} (fixé)
            🎵 Musique        : {VolumeMusique}%
            🔊 Sons           : {VolumeSons}%
            ═══════════════════════════════════════
            """);
    }
}

// Création de la configuration au lancement
var config = new ConfigurationJeu
{
    NomJoueur = "DragonSlayer42",
    Difficulte = "Difficile",
    ModeFullscreen = true,
    VolumeMusique = 30
};

config.AfficherConfiguration();

// En cours de jeu, on peut ajuster le volume
config.VolumeMusique = 0;     // ✓ Couper la musique
config.VolumeSons = 100;      // ✓ Sons au maximum

// Mais on ne peut pas changer ces paramètres
// config.Difficulte = "Facile";       // ❌ Tricheur !
// config.NomJoueur = "HackerX";       // ❌ Interdit
// config.ModeFullscreen = false;      // ❌ Nécessite un redémarrage
```
:::

### Exercice 3 : Système de facture

Créez un système avec :
- `Facture` : numéro (init, auto-généré), date (init, auto), client (required init), lignes (init), total calculé
- `LigneFacture` : produit (init), quantité (init), prix unitaire (init), sous-total calculé

:::details 💡 Solution Exercice 3

```csharp
class LigneFacture
{
    public required string Produit { get; init; }
    public int Quantite { get; init; } = 1;
    public decimal PrixUnitaire { get; init; }
    
    // Calculé à partir des propriétés immuables
    public decimal SousTotal => Quantite * PrixUnitaire;
    
    public override string ToString()
        => $"  {Produit,-20} {Quantite,3} x {PrixUnitaire,8:C} = {SousTotal,10:C}";
}

class Facture
{
    private static int _numeroSuivant = 1;
    
    // Identité de la facture - immuable
    public string Numero { get; init; } = $"FAC-{DateTime.Now:yyyyMM}-{_numeroSuivant++:D4}";
    public DateTime Date { get; init; } = DateTime.Now;
    public required string Client { get; init; }
    
    // Lignes définies à la création
    public List<LigneFacture> Lignes { get; init; } = new();
    
    // Calculs basés sur les données immuables
    public decimal TotalHT => Lignes.Sum(l => l.SousTotal);
    public decimal TVA => TotalHT * 0.21m;
    public decimal TotalTTC => TotalHT + TVA;
    
    public void Afficher()
    {
        Console.WriteLine($"""
            ╔══════════════════════════════════════════════════════╗
            ║                    FACTURE                           ║
            ╠══════════════════════════════════════════════════════╣
            ║ Numéro : {Numero,-20} Date : {Date:dd/MM/yyyy}     ║
            ║ Client : {Client,-44} ║
            ╠══════════════════════════════════════════════════════╣
            """);
        
        foreach (var ligne in Lignes)
        {
            Console.WriteLine($"║{ligne,-54}║");
        }
        
        Console.WriteLine($"""
            ╠══════════════════════════════════════════════════════╣
            ║                           Total HT  : {TotalHT,12:C} ║
            ║                           TVA (21%) : {TVA,12:C} ║
            ║                           Total TTC : {TotalTTC,12:C} ║
            ╚══════════════════════════════════════════════════════╝
            """);
    }
}

// Création d'une facture avec ses lignes
var facture = new Facture
{
    Client = "Entreprise ABC",
    Lignes = new List<LigneFacture>
    {
        new() { Produit = "Clavier mécanique", Quantite = 2, PrixUnitaire = 89.99m },
        new() { Produit = "Souris gaming", Quantite = 2, PrixUnitaire = 49.99m },
        new() { Produit = "Écran 27 pouces", Quantite = 1, PrixUnitaire = 299.99m },
        new() { Produit = "Câble HDMI", Quantite = 3, PrixUnitaire = 12.99m }
    }
};

facture.Afficher();

// La facture est maintenant figée
// facture.Client = "Autre";  // ❌ Interdit
// facture.Numero = "FAKE";   // ❌ Interdit
// facture.Lignes[0].Quantite = 100;  // ❌ Interdit (init-only)

// Même les lignes sont protégées !
// Ceci échouerait : facture.Lignes[0] = new LigneFacture { ... };
```
:::

## Résumé

| Aspect | `set` | `init` |
|--------|-------|--------|
| Constructeur | ✓ | ✓ |
| Initialiseur d'objet | ✓ | ✓ |
| Après création | ✓ | ✗ |
| Usage typique | Données mutables | Données d'identité |

| Concept | Description |
|---------|-------------|
| **`init`** | Propriété initialisable une fois puis lecture seule |
| **Avantage** | Immutabilité partielle, flexibilité d'initialisation |
| **Avec `required`** | Force l'initialisation ET garantit l'immutabilité |
| **vs Records** | Les records utilisent `init` par défaut |
