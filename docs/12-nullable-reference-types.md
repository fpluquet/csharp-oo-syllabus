# Nullable Reference Types (C# 8+)

## Le problème du null

La fameuse `NullReferenceException` est l'une des erreurs les plus courantes en programmation. Elle se produit quand on essaie d'utiliser un objet qui vaut `null` :

```csharp
string nom = null;
Console.WriteLine(nom.Length);  // 💥 NullReferenceException!
```

Tony Hoare, l'inventeur du `null`, l'a lui-même qualifié de **"erreur d'un milliard de dollars"**.

::: tip 🎯 Ce que vous allez apprendre
- Comprendre le problème des références nulles
- Utiliser les nullable reference types pour prévenir les erreurs
- Maîtriser les opérateurs `?.`, `??` et `??=`
- Annoter correctement vos types et méthodes
:::

### 📦 Analogie : le colis vide

Imaginez que vous commandez un colis. Parfois :
- Le colis arrive avec le contenu attendu ✅
- Le colis arrive **vide** (c'est le `null`) ❌

Le problème : si vous essayez d'utiliser le contenu d'un colis vide sans vérifier d'abord, vous aurez une mauvaise surprise !

```
┌─────────────────────────────────────────────────────────────────────┐
│                    LE PROBLÈME DU NULL                              │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│    string nom = "Alice";          string nom = null;                │
│    ┌────────────────────┐         ┌────────────────────┐            │
│    │      "Alice"       │         │        ∅           │            │
│    └────────────────────┘         └────────────────────┘            │
│                                                                     │
│    nom.Length  →  5 ✅             nom.Length  →  💥 CRASH !        │
│                                                                     │
│    Avec les Nullable Reference Types, le compilateur vous          │
│    prévient AVANT le crash !                                        │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

## Nullable Reference Types (NRT)

À partir de C# 8, le compilateur peut vous aider à détecter les problèmes de null **avant** l'exécution. Cette fonctionnalité s'appelle **Nullable Reference Types**.

### Activation

Les NRT sont activés par défaut dans les nouveaux projets .NET 6+. On peut aussi les activer manuellement :

```xml
<!-- Dans le fichier .csproj -->
<PropertyGroup>
    <Nullable>enable</Nullable>
</PropertyGroup>
```

Ou par fichier :
```csharp
#nullable enable  // En haut du fichier
```

### Principe de base

Une fois activé, le compilateur considère que :
- `string` → **ne peut jamais être null**
- `string?` → **peut être null**

```csharp
#nullable enable

string nom = "Alice";     // OK : ne peut pas être null
string? surnom = null;    // OK : peut être null

nom = null;               // ⚠️ Warning : assignation de null à un type non-nullable
Console.WriteLine(nom.Length);  // OK : le compilateur sait que nom n'est pas null

Console.WriteLine(surnom.Length);  // ⚠️ Warning : surnom peut être null
```

## Annotation des types nullables

### Types référence

```csharp
class Personne
{
    public string Nom { get; set; }          // Non-nullable (requis)
    public string? Surnom { get; set; }      // Nullable (optionnel)
    public string Email { get; set; }        // Non-nullable (requis)
    public Adresse? AdresseSecondaire { get; set; }  // Nullable
}
```

### Paramètres et retours de méthodes

```csharp
class Service
{
    // Le paramètre ne peut pas être null
    public void Traiter(string donnees)
    {
        Console.WriteLine(donnees.Length);  // Safe
    }
    
    // Le paramètre peut être null
    public void TraiterOptional(string? donnees)
    {
        if (donnees != null)
        {
            Console.WriteLine(donnees.Length);  // Safe après vérification
        }
    }
    
    // La méthode peut retourner null
    public string? TrouverNom(int id)
    {
        if (id == 0) return null;
        return "Nom trouvé";
    }
    
    // La méthode ne retourne jamais null
    public string ObtenirNomParDefaut(int id)
    {
        return TrouverNom(id) ?? "Inconnu";
    }
}
```

## Opérateurs null-safe

### Opérateur null-conditionnel (`?.`)

Accède à un membre seulement si l'objet n'est pas null :

```csharp
string? nom = null;
int? longueur = nom?.Length;  // null (pas d'exception)

string prenom = "Alice";
int? longueurPrenom = prenom?.Length;  // 5

// Chaînage
Personne? p = null;
string? ville = p?.Adresse?.Ville;  // null (sécurisé)
```

### Opérateur null-coalescent (`??`)

Fournit une valeur par défaut si l'expression est null :

```csharp
string? nom = null;
string nomAffiche = nom ?? "Anonyme";  // "Anonyme"

string prenom = "Alice";
string prenomAffiche = prenom ?? "Anonyme";  // "Alice"
```

### Assignation null-coalescent (`??=`)

Assigne une valeur uniquement si la variable est null :

```csharp
string? message = null;
message ??= "Valeur par défaut";  // message = "Valeur par défaut"

message ??= "Autre valeur";  // message reste "Valeur par défaut"
```

### Opérateur null-forgiving (`!`)

Indique au compilateur "je suis sûr que ce n'est pas null" :

```csharp
string? texte = ObtenirTexte();

// ⚠️ Warning : texte peut être null
Console.WriteLine(texte.Length);

// Utilisation de ! pour ignorer le warning (à utiliser avec prudence)
Console.WriteLine(texte!.Length);
```

::: danger Attention
L'opérateur `!` ne fait que supprimer le warning du compilateur. Si la valeur est réellement null, vous aurez quand même une `NullReferenceException` à l'exécution.
:::

## Vérification du null

### Pattern matching avec `is`

```csharp
string? texte = ObtenirTexte();

if (texte is not null)
{
    // Le compilateur sait que texte n'est pas null ici
    Console.WriteLine(texte.Length);
}

if (texte is null)
{
    Console.WriteLine("Pas de texte");
}
```

### Pattern matching avec `is` et extraction

```csharp
string? texte = ObtenirTexte();

if (texte is string t)  // Vérifie non-null ET extrait
{
    Console.WriteLine(t.Length);  // t est garanti non-null
}
```

### Vérification classique

```csharp
string? texte = ObtenirTexte();

if (texte != null)
{
    Console.WriteLine(texte.Length);  // Safe
}

// Ou avec string
if (!string.IsNullOrEmpty(texte))
{
    Console.WriteLine(texte.Length);  // Safe
}
```

## Constructeurs et initialisation

Le compilateur vérifie que les propriétés non-nullable sont initialisées :

```csharp
class Client
{
    public string Nom { get; set; }    // ⚠️ Warning : non initialisé
    public string Email { get; set; }  // ⚠️ Warning : non initialisé
}
```

### Solutions

#### 1. Initialiser dans le constructeur

```csharp
class Client
{
    public string Nom { get; set; }
    public string Email { get; set; }
    
    public Client(string nom, string email)
    {
        Nom = nom;
        Email = email;
    }
}
```

#### 2. Utiliser `required` (C# 11+)

```csharp
class Client
{
    public required string Nom { get; set; }
    public required string Email { get; set; }
}

// Force l'initialisation
Client c = new Client { Nom = "Alice", Email = "alice@email.com" };
```

#### 3. Valeur par défaut

```csharp
class Client
{
    public string Nom { get; set; } = "";
    public string Email { get; set; } = "";
}
```

#### 4. Mot-clé `null!` (pour les cas où on sait que ça sera initialisé)

```csharp
class Client
{
    public string Nom { get; set; } = null!;  // Sera initialisé par le framework
}
```

## Exemples pratiques

### Repository avec résultats nullables

```csharp
interface IClientRepository
{
    Client? TrouverParId(int id);           // Peut ne pas exister
    Client TrouverOuCreer(int id);          // Toujours un résultat
    IEnumerable<Client> TrouverTous();      // Liste (jamais null, peut être vide)
}

class ClientRepository : IClientRepository
{
    private readonly List<Client> _clients = new();
    
    public Client? TrouverParId(int id)
    {
        return _clients.FirstOrDefault(c => c.Id == id);  // Peut être null
    }
    
    public Client TrouverOuCreer(int id)
    {
        return TrouverParId(id) ?? new Client { Id = id, Nom = "Nouveau" };
    }
    
    public IEnumerable<Client> TrouverTous()
    {
        return _clients;  // Jamais null
    }
}
```

### Utilisation sécurisée

```csharp
IClientRepository repo = new ClientRepository();

// Recherche qui peut échouer
Client? client = repo.TrouverParId(123);

if (client is null)
{
    Console.WriteLine("Client non trouvé");
    return;
}

// Ici, client est garanti non-null
Console.WriteLine(client.Nom);

// Ou avec l'opérateur ?.
string? nomClient = repo.TrouverParId(456)?.Nom;
Console.WriteLine(nomClient ?? "Inconnu");
```

### DTO avec propriétés optionnelles

```csharp
class CommandeDTO
{
    public required int Id { get; init; }
    public required string NumeroCommande { get; init; }
    public required DateTime Date { get; init; }
    
    // Optionnels
    public string? Commentaire { get; init; }
    public AdresseDTO? AdresseLivraison { get; init; }
    public string? CodePromo { get; init; }
}

class AdresseDTO
{
    public required string Rue { get; init; }
    public required string Ville { get; init; }
    public required string CodePostal { get; init; }
    public string? Complement { get; init; }  // Optionnel
}
```

## Attributs pour les cas avancés

### `[NotNull]` et `[MaybeNull]`

```csharp
using System.Diagnostics.CodeAnalysis;

class Cache<T> where T : class
{
    private T? _valeur;
    
    // Garantit que la valeur retournée n'est pas null
    [return: NotNull]
    public T ObtenirOuCreer(Func<T> factory)
    {
        _valeur ??= factory();
        return _valeur;
    }
    
    // La valeur peut être null
    [return: MaybeNull]
    public T? Obtenir()
    {
        return _valeur;
    }
}
```

### `[NotNullWhen]` pour les méthodes Try

```csharp
using System.Diagnostics.CodeAnalysis;

class Parser
{
    public bool TryParse(string input, [NotNullWhen(true)] out Resultat? resultat)
    {
        if (string.IsNullOrEmpty(input))
        {
            resultat = null;
            return false;
        }
        
        resultat = new Resultat(input);
        return true;
    }
}

// Utilisation
Parser parser = new Parser();
if (parser.TryParse("test", out var resultat))
{
    // Le compilateur sait que resultat n'est pas null ici
    Console.WriteLine(resultat.Valeur);
}
```

## Bonnes pratiques

| Pratique | Description |
|----------|-------------|
| **Activer NRT** | Toujours activer dans les nouveaux projets |
| **Être explicite** | Marquer clairement ce qui peut être null avec `?` |
| **Éviter `!`** | N'utiliser qu'en dernier recours avec certitude |
| **Vérifier tôt** | Valider les entrées au début des méthodes |
| **Utiliser `required`** | Pour les propriétés obligatoires |
| **Préférer des valeurs** | `string.Empty` plutôt que `null` si possible |

## Exercices

### Exercice 1 : Gestion de profil

Créez une classe `ProfilUtilisateur` avec :
- `Nom` (requis)
- `Email` (requis)
- `Telephone` (optionnel)
- `Bio` (optionnel)
- Une méthode `AfficherComplet()` qui gère les valeurs nulles

::: details 💡 Solution Exercice 1

```csharp
#nullable enable

class ProfilUtilisateur
{
    public required string Nom { get; init; }
    public required string Email { get; init; }
    public string? Telephone { get; set; }
    public string? Bio { get; set; }
    
    public void AfficherComplet()
    {
        Console.WriteLine($"=== Profil de {Nom} ===");
        Console.WriteLine($"Email: {Email}");
        
        // Gestion du téléphone optionnel
        Console.WriteLine($"Téléphone: {Telephone ?? "Non renseigné"}");
        
        // Gestion de la bio optionnelle
        if (Bio is not null)
        {
            Console.WriteLine($"Bio: {Bio}");
        }
        else
        {
            Console.WriteLine("Bio: (aucune bio)");
        }
    }
    
    public string ObtenirResume()
    {
        var telephone = Telephone is not null ? $" - {Telephone}" : "";
        return $"{Nom} ({Email}){telephone}";
    }
}

// Test
var profil1 = new ProfilUtilisateur
{
    Nom = "Alice Dupont",
    Email = "alice@email.com",
    Telephone = "+32 123 456 789",
    Bio = "Développeuse passionnée par C#"
};

var profil2 = new ProfilUtilisateur
{
    Nom = "Bob Martin",
    Email = "bob@email.com"
    // Telephone et Bio restent null
};

profil1.AfficherComplet();
Console.WriteLine();
profil2.AfficherComplet();
```

**Sortie** :
```
=== Profil de Alice Dupont ===
Email: alice@email.com
Téléphone: +32 123 456 789
Bio: Développeuse passionnée par C#

=== Profil de Bob Martin ===
Email: bob@email.com
Téléphone: Non renseigné
Bio: (aucune bio)
```
:::

### Exercice 2 : Recherche sécurisée

Créez une méthode `TrouverProduit(string reference)` qui :
- Retourne `Produit?`
- Recherche dans une liste de produits
- Utilisez cette méthode en gérant proprement le cas null

::: details 💡 Solution Exercice 2

```csharp
#nullable enable

record Produit(string Reference, string Nom, decimal Prix);

class CatalogueProduits
{
    private readonly List<Produit> _produits = new()
    {
        new Produit("REF001", "Clavier mécanique", 89.99m),
        new Produit("REF002", "Souris gaming", 49.99m),
        new Produit("REF003", "Écran 27 pouces", 299.99m)
    };
    
    // Méthode qui peut retourner null
    public Produit? TrouverProduit(string reference)
    {
        return _produits.FirstOrDefault(p => p.Reference == reference);
    }
    
    // Version avec valeur par défaut
    public Produit TrouverOuDefault(string reference, Produit produitParDefaut)
    {
        return TrouverProduit(reference) ?? produitParDefaut;
    }
    
    // Pattern Try
    public bool TryTrouverProduit(string reference, out Produit? produit)
    {
        produit = TrouverProduit(reference);
        return produit is not null;
    }
}

// Utilisation sécurisée
var catalogue = new CatalogueProduits();

// Méthode 1 : vérification avec is
Produit? produit1 = catalogue.TrouverProduit("REF001");
if (produit1 is not null)
{
    Console.WriteLine($"Trouvé: {produit1.Nom} à {produit1.Prix:C}");
}

// Méthode 2 : opérateur ?.
string? nomProduit = catalogue.TrouverProduit("REF999")?.Nom;
Console.WriteLine($"Nom: {nomProduit ?? "Produit non trouvé"}");

// Méthode 3 : pattern Try
if (catalogue.TryTrouverProduit("REF002", out var produit2))
{
    Console.WriteLine($"Trouvé via Try: {produit2!.Nom}");
}

// Méthode 4 : avec valeur par défaut
var produitInconnu = new Produit("???", "Produit inconnu", 0);
var resultat = catalogue.TrouverOuDefault("REF999", produitInconnu);
Console.WriteLine($"Résultat: {resultat.Nom}");
```
:::

### Exercice 3 : Chaîne d'objets nullable

Créez une structure avec `Entreprise` → `Departement` → `Manager` → `Email` où chaque niveau peut être null, et extrayez l'email du manager de manière sécurisée.

::: details 💡 Solution Exercice 3

```csharp
#nullable enable

class Entreprise
{
    public required string Nom { get; init; }
    public Departement? DepartementPrincipal { get; set; }
}

class Departement
{
    public required string Nom { get; init; }
    public Employe? Manager { get; set; }
}

class Employe
{
    public required string Nom { get; init; }
    public string? Email { get; set; }
}

// Méthode pour extraire l'email de manière sécurisée
string ObtenirEmailManager(Entreprise? entreprise)
{
    // ❌ DANGEREUX sans vérification :
    // return entreprise.DepartementPrincipal.Manager.Email;
    
    // ✅ SÉCURISÉ avec opérateur ?.
    string? email = entreprise?.DepartementPrincipal?.Manager?.Email;
    
    return email ?? "Email non disponible";
}

// Tests
var entreprise1 = new Entreprise
{
    Nom = "TechCorp",
    DepartementPrincipal = new Departement
    {
        Nom = "IT",
        Manager = new Employe
        {
            Nom = "Alice",
            Email = "alice@techcorp.com"
        }
    }
};

var entreprise2 = new Entreprise
{
    Nom = "StartupXYZ",
    DepartementPrincipal = new Departement
    {
        Nom = "R&D",
        Manager = null  // Pas encore de manager
    }
};

var entreprise3 = new Entreprise
{
    Nom = "SmallBiz"
    // Pas de département principal
};

Console.WriteLine(ObtenirEmailManager(entreprise1));  // alice@techcorp.com
Console.WriteLine(ObtenirEmailManager(entreprise2));  // Email non disponible
Console.WriteLine(ObtenirEmailManager(entreprise3));  // Email non disponible
Console.WriteLine(ObtenirEmailManager(null));          // Email non disponible
```

**Points clés** :
- L'opérateur `?.` permet de "court-circuiter" la chaîne dès qu'un null est rencontré
- On n'a jamais de `NullReferenceException`
- Le résultat final est `string?` car n'importe quel maillon peut être null
:::

## Résumé

| Syntaxe | Signification |
|---------|---------------|
| `string` | Ne peut jamais être null |
| `string?` | Peut être null |
| `?.` | Accès conditionnel (null-safe) |
| `??` | Valeur par défaut si null |
| `??=` | Assigne si null |
| `!` | "Je suis sûr que ce n'est pas null" |
| `is null` | Test de nullité |
| `is not null` | Test de non-nullité |
