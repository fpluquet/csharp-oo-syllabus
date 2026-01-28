# Interfaces Modernes (C# 8+)

## Introduction

Imaginez un **contrat de travail standard** utilisé par des centaines d'entreprises. Ce contrat définit les obligations de base (salaire, horaires, congés). Mais que se passe-t-il si le gouvernement impose une nouvelle clause obligatoire, comme "droit au télétravail" ?

**Avant C# 8** : Chaque entreprise devait modifier individuellement son contrat — un cauchemar administratif !

**Avec C# 8** : Le contrat standard inclut directement une clause par défaut : *"Le télétravail est possible selon les modalités de l'entreprise"*. Les entreprises qui le souhaitent peuvent personnaliser cette clause, les autres utilisent simplement la version par défaut.

```
┌──────────────────────────────────────────────────────────────┐
│              CONTRAT (Interface) v1.0                        │
├──────────────────────────────────────────────────────────────┤
│  ✓ Salaire()           → À définir par l'entreprise         │
│  ✓ Horaires()          → À définir par l'entreprise         │
│  ✓ Congés()            → À définir par l'entreprise         │
└──────────────────────────────────────────────────────────────┘
                           ↓
                    Évolution du contrat
                           ↓
┌──────────────────────────────────────────────────────────────┐
│              CONTRAT (Interface) v2.0                        │
├──────────────────────────────────────────────────────────────┤
│  ✓ Salaire()           → À définir par l'entreprise         │
│  ✓ Horaires()          → À définir par l'entreprise         │
│  ✓ Congés()            → À définir par l'entreprise         │
│  ✓ Télétravail()       → DÉFAUT : "2 jours par semaine"     │
│                          (personnalisable si souhaité)       │
└──────────────────────────────────────────────────────────────┘
```

:::tip 💡 L'idée clé
Les **implémentations par défaut** permettent de faire évoluer une interface sans casser les classes qui l'implémentent déjà. C'est une révolution pour la maintenance du code !
:::

## Rappel : qu'est-ce qu'une interface ?

Une **interface** définit un **contrat** que les classes doivent respecter. Traditionnellement, une interface ne contient que des signatures de méthodes, sans implémentation.

```csharp
// Interface traditionnelle
interface IVehicule
{
    void Demarrer();
    void Arreter();
    int Vitesse { get; }
}

class Voiture : IVehicule
{
    public int Vitesse { get; private set; }
    
    public void Demarrer()
    {
        Console.WriteLine("Vroum!");
    }
    
    public void Arreter()
    {
        Vitesse = 0;
        Console.WriteLine("Arrêt.");
    }
}
```

## Implémentations par défaut (C# 8+)

Depuis C# 8, les interfaces peuvent contenir du **code** : des méthodes avec une implémentation par défaut.

```csharp
interface ILogger
{
    void Log(string message);
    
    // Méthode avec implémentation par défaut
    void LogInfo(string message) => Log($"[INFO] {message}");
    void LogWarning(string message) => Log($"[WARN] {message}");
    void LogError(string message) => Log($"[ERROR] {message}");
}

class ConsoleLogger : ILogger
{
    // Seule Log() est obligatoire
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
    
    // LogInfo, LogWarning, LogError sont fournis par l'interface
}
```

```csharp
ILogger logger = new ConsoleLogger();
logger.Log("Message simple");        // [Console] Message simple
logger.LogInfo("Application démarrée");  // [INFO] Application démarrée
logger.LogError("Une erreur!");      // [ERROR] Une erreur!
```

## Pourquoi des implémentations par défaut ?

### Le problème de l'évolution

Imaginez une interface utilisée par des centaines de classes :

```csharp
interface IRepository<T>
{
    T GetById(int id);
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
}

// 100+ classes implémentent cette interface...
class ClientRepository : IRepository<Client> { ... }
class ProduitRepository : IRepository<Produit> { ... }
class CommandeRepository : IRepository<Commande> { ... }
// etc.
```

Si vous voulez ajouter une nouvelle méthode :

```csharp
interface IRepository<T>
{
    // Méthodes existantes...
    
    bool Exists(int id);  // ❌ Nouvelle méthode = 100+ classes à modifier !
}
```

### La solution avec implémentation par défaut

```csharp
interface IRepository<T>
{
    T GetById(int id);
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
    
    // Nouvelle méthode avec implémentation par défaut
    bool Exists(int id) => GetById(id) != null;
}

// Les 100+ classes continuent de fonctionner !
// Celles qui le souhaitent peuvent surcharger Exists() pour optimiser
```

## Syntaxe des membres d'interface

### Méthodes par défaut

```csharp
interface ICalculable
{
    double Valeur { get; }
    
    // Méthode abstraite (pas d'implémentation)
    double Calculer();
    
    // Méthode par défaut
    double CalculerAvecMarge(double marge) => Calculer() * (1 + marge);
    
    // Méthode statique
    static double Arrondir(double valeur) => Math.Round(valeur, 2);
}
```

### Propriétés avec accesseurs par défaut

```csharp
interface IIdentifiable
{
    int Id { get; }
    string Code { get; }
    
    // Propriété calculée par défaut
    string IdentifiantComplet => $"{Code}-{Id:D5}";
}

class Produit : IIdentifiable
{
    public int Id { get; init; }
    public string Code { get; init; }
    // IdentifiantComplet est fourni par l'interface
}

IIdentifiable p = new Produit { Id = 42, Code = "PRD" };
Console.WriteLine(p.IdentifiantComplet);  // PRD-00042
```

### Méthodes statiques abstraites (C# 11+)

```csharp
interface IParseable<T> where T : IParseable<T>
{
    static abstract T Parse(string s);
    static abstract bool TryParse(string s, out T result);
}

class Temperature : IParseable<Temperature>
{
    public double Celsius { get; init; }
    
    public static Temperature Parse(string s)
    {
        return new Temperature { Celsius = double.Parse(s) };
    }
    
    public static bool TryParse(string s, out Temperature result)
    {
        if (double.TryParse(s, out double c))
        {
            result = new Temperature { Celsius = c };
            return true;
        }
        result = default;
        return false;
    }
}
```

## Modificateurs d'accès dans les interfaces

C# 8+ permet d'utiliser des modificateurs d'accès :

```csharp
interface IService
{
    // Membres publics par défaut
    void MethodePublique();
    
    // Membre privé (pour l'implémentation interne)
    private void MethodePrivee()
    {
        Console.WriteLine("Méthode helper privée");
    }
    
    // Méthode par défaut qui utilise le membre privé
    void MethodeAvecHelper()
    {
        MethodePrivee();
        MethodePublique();
    }
    
    // Membre protected (accessible aux interfaces dérivées)
    protected void MethodeProtegee() { }
}
```

## Surcharge des méthodes par défaut

Une classe peut surcharger l'implémentation par défaut :

```csharp
interface IFormattable
{
    string Nom { get; }
    
    string Formater() => Nom.ToUpper();
}

class Article : IFormattable
{
    public string Nom { get; set; }
    public string Reference { get; set; }
    
    // Surcharge de la méthode par défaut
    public string Formater() => $"[{Reference}] {Nom}";
}

IFormattable a = new Article { Nom = "Clavier", Reference = "CLV-001" };
Console.WriteLine(a.Formater());  // [CLV-001] Clavier
```

::: warning Attention à l'accès
Les méthodes par défaut ne sont accessibles que via le **type interface**, pas via le type concret (sauf si surchargées) :

```csharp
Article article = new Article { Nom = "Test", Reference = "TST" };

// Si Formater() est surchargée, les deux fonctionnent :
article.Formater();              // OK
((IFormattable)article).Formater();  // OK

// Si Formater() N'EST PAS surchargée :
// article.Formater();           // ❌ ERREUR
((IFormattable)article).Formater();  // OK
```
:::

## Exemple complet : système de notifications

```csharp
interface INotification
{
    string Destinataire { get; }
    string Message { get; }
    
    // Méthode abstraite - chaque type doit implémenter
    void Envoyer();
    
    // Méthodes par défaut
    void EnvoyerAvecLog()
    {
        Console.WriteLine($"Envoi à {Destinataire}...");
        Envoyer();
        Console.WriteLine("Envoyé!");
    }
    
    string Formater() => $"À: {Destinataire}\n{Message}";
    
    bool Valider() => !string.IsNullOrEmpty(Destinataire) && 
                      !string.IsNullOrEmpty(Message);
}

class EmailNotification : INotification
{
    public string Destinataire { get; init; }
    public string Message { get; init; }
    public string Sujet { get; init; }
    
    public void Envoyer()
    {
        Console.WriteLine($"📧 Email envoyé à {Destinataire}: {Sujet}");
    }
    
    // Surcharge de Formater
    public string Formater() => $"À: {Destinataire}\nSujet: {Sujet}\n{Message}";
}

class SmsNotification : INotification
{
    public string Destinataire { get; init; }
    public string Message { get; init; }
    
    public void Envoyer()
    {
        Console.WriteLine($"📱 SMS envoyé à {Destinataire}");
    }
    
    // Utilise Formater() et Valider() par défaut
}

class PushNotification : INotification
{
    public string Destinataire { get; init; }
    public string Message { get; init; }
    public string AppId { get; init; }
    
    public void Envoyer()
    {
        Console.WriteLine($"🔔 Push envoyé via {AppId} à {Destinataire}");
    }
    
    // Surcharge de Valider
    public bool Valider() => !string.IsNullOrEmpty(AppId) &&
                             !string.IsNullOrEmpty(Destinataire);
}
```

```csharp
// Utilisation polymorphique
List<INotification> notifications = new()
{
    new EmailNotification 
    { 
        Destinataire = "alice@email.com", 
        Sujet = "Bienvenue", 
        Message = "Bonjour!" 
    },
    new SmsNotification 
    { 
        Destinataire = "+32123456789", 
        Message = "Votre code: 1234" 
    },
    new PushNotification 
    { 
        Destinataire = "user123", 
        AppId = "myapp", 
        Message = "Nouvelle mise à jour!" 
    }
};

foreach (var notif in notifications)
{
    if (notif.Valider())
    {
        notif.EnvoyerAvecLog();
    }
}
```

## Interfaces et héritage multiple

C# ne supporte pas l'héritage multiple de classes, mais une classe peut implémenter plusieurs interfaces :

```csharp
interface IStockable
{
    int Stock { get; set; }
    void AjouterStock(int quantite);
    void RetirerStock(int quantite) => Stock -= quantite;
}

interface IPromotionable
{
    decimal Prix { get; }
    decimal PrixPromo => Prix * 0.9m;  // 10% de réduction par défaut
    
    decimal AppliquerPromo(decimal pourcentage) => Prix * (1 - pourcentage / 100);
}

interface IExportable
{
    string ExporterJson();
    string ExporterCsv() => $"\"{GetType().Name}\"";
}

// Une classe peut implémenter plusieurs interfaces
class Produit : IStockable, IPromotionable, IExportable
{
    public string Nom { get; set; }
    public decimal Prix { get; set; }
    public int Stock { get; set; }
    
    public void AjouterStock(int quantite) => Stock += quantite;
    
    public string ExporterJson() => 
        $"{{\"nom\":\"{Nom}\",\"prix\":{Prix},\"stock\":{Stock}}}";
}
```

## Bonnes pratiques

| Pratique | Explication |
|----------|-------------|
| **Méthodes par défaut = évolution** | Utilisez-les pour faire évoluer les interfaces existantes |
| **Garder les contrats simples** | Les méthodes par défaut ne remplacent pas les classes de base |
| **Éviter la logique métier complexe** | Les interfaces définissent des comportements, pas la logique métier |
| **Documentation** | Documentez les méthodes par défaut comme les abstraites |

## Exercices

### Exercice 1 : Interface IValidatable

Créez une interface `IValidatable` avec :
- `bool EstValide()` (abstraite)
- `List<string> ObtenirErreurs()` (abstraite)
- `string Resume()` qui retourne un résumé (par défaut)

Implémentez pour `Client` et `Commande`.

:::details 💡 Solution Exercice 1

```csharp
interface IValidatable
{
    // Méthodes abstraites - chaque classe DOIT implémenter
    bool EstValide();
    List<string> ObtenirErreurs();
    
    // Méthode par défaut - implémentation fournie
    string Resume()
    {
        if (EstValide())
            return "✓ Objet valide";
        
        var erreurs = ObtenirErreurs();
        return $"✗ {erreurs.Count} erreur(s) :\n  - " + string.Join("\n  - ", erreurs);
    }
    
    // Méthode par défaut supplémentaire
    void AfficherValidation()
    {
        Console.WriteLine(Resume());
    }
}

class Client : IValidatable
{
    public string Nom { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    
    public bool EstValide()
    {
        return ObtenirErreurs().Count == 0;
    }
    
    public List<string> ObtenirErreurs()
    {
        var erreurs = new List<string>();
        
        if (string.IsNullOrWhiteSpace(Nom))
            erreurs.Add("Le nom est obligatoire");
        else if (Nom.Length < 2)
            erreurs.Add("Le nom doit contenir au moins 2 caractères");
            
        if (string.IsNullOrWhiteSpace(Email))
            erreurs.Add("L'email est obligatoire");
        else if (!Email.Contains("@"))
            erreurs.Add("L'email doit contenir un @");
            
        if (Age < 0 || Age > 150)
            erreurs.Add("L'âge doit être entre 0 et 150");
            
        return erreurs;
    }
    
    // Utilise Resume() et AfficherValidation() par défaut de l'interface
}

class Commande : IValidatable
{
    public int Id { get; set; }
    public string ClientId { get; set; }
    public List<string> Produits { get; set; } = new();
    public decimal Total { get; set; }
    
    public bool EstValide() => ObtenirErreurs().Count == 0;
    
    public List<string> ObtenirErreurs()
    {
        var erreurs = new List<string>();
        
        if (string.IsNullOrWhiteSpace(ClientId))
            erreurs.Add("Le client est obligatoire");
            
        if (Produits.Count == 0)
            erreurs.Add("La commande doit contenir au moins un produit");
            
        if (Total <= 0)
            erreurs.Add("Le total doit être positif");
            
        return erreurs;
    }
    
    // Surcharge de Resume() pour un format personnalisé
    public string Resume()
    {
        if (EstValide())
            return $"✓ Commande #{Id} valide ({Produits.Count} produits, {Total:C})";
        
        return $"✗ Commande #{Id} invalide : {string.Join(", ", ObtenirErreurs())}";
    }
}

// Utilisation
var client1 = new Client { Nom = "Alice", Email = "alice@email.com", Age = 25 };
var client2 = new Client { Nom = "B", Email = "invalide", Age = -5 };

IValidatable v1 = client1;
IValidatable v2 = client2;

v1.AfficherValidation();  // ✓ Objet valide
v2.AfficherValidation();  // ✗ 3 erreur(s) : - Le nom doit contenir au moins 2 caractères...

var commande = new Commande
{
    Id = 1,
    ClientId = "CLI-001",
    Produits = new List<string> { "Clavier", "Souris" },
    Total = 149.99m
};

Console.WriteLine(((IValidatable)commande).Resume());
// ✓ Commande #1 valide (2 produits, 149,99 €)
```
:::

### Exercice 2 : Interface avec versioning

Créez une interface `IDocument` version 1 avec `Save()` et `Load()`.
Ajoutez ensuite `SaveAsync()` et `LoadAsync()` avec des implémentations par défaut qui appellent les méthodes synchrones.

:::details 💡 Solution Exercice 2

```csharp
// Version 2 de l'interface - compatible avec la version 1
interface IDocument
{
    // === Version 1 : méthodes synchrones (obligatoires) ===
    void Save(string path);
    string Load(string path);
    
    // === Version 2 : méthodes async avec implémentations par défaut ===
    // Les classes existantes continuent de fonctionner !
    
    Task SaveAsync(string path)
    {
        return Task.Run(() => Save(path));
    }
    
    Task<string> LoadAsync(string path)
    {
        return Task.Run(() => Load(path));
    }
    
    // === Version 3 : fonctionnalités supplémentaires ===
    
    // Sauvegarde avec backup automatique
    void SaveWithBackup(string path)
    {
        if (File.Exists(path))
        {
            File.Copy(path, path + ".bak", overwrite: true);
        }
        Save(path);
    }
    
    // Vérification de l'existence
    bool Exists(string path) => File.Exists(path);
}

// Classe existante (version 1) - fonctionne toujours !
class TextDocument : IDocument
{
    public string Content { get; set; } = "";
    
    public void Save(string path)
    {
        File.WriteAllText(path, Content);
        Console.WriteLine($"📄 Sauvegardé : {path}");
    }
    
    public string Load(string path)
    {
        Content = File.ReadAllText(path);
        Console.WriteLine($"📄 Chargé : {path}");
        return Content;
    }
    
    // Utilise SaveAsync, LoadAsync, SaveWithBackup, Exists par défaut
}

// Nouvelle classe qui optimise les méthodes async
class JsonDocument : IDocument
{
    public Dictionary<string, object> Data { get; set; } = new();
    
    public void Save(string path)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(Data);
        File.WriteAllText(path, json);
    }
    
    public string Load(string path)
    {
        var json = File.ReadAllText(path);
        Data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        return json;
    }
    
    // Surcharge pour une vraie implémentation async
    public async Task SaveAsync(string path)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(Data);
        await File.WriteAllTextAsync(path, json);
        Console.WriteLine($"📋 JSON sauvegardé (async) : {path}");
    }
    
    public async Task<string> LoadAsync(string path)
    {
        var json = await File.ReadAllTextAsync(path);
        Data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        return json;
    }
}

// Utilisation
IDocument textDoc = new TextDocument { Content = "Bonjour le monde !" };
textDoc.Save("test.txt");              // Synchrone
await textDoc.SaveAsync("test2.txt");  // Utilise l'implémentation par défaut
textDoc.SaveWithBackup("test.txt");    // Crée test.txt.bak puis sauvegarde

IDocument jsonDoc = new JsonDocument { Data = { ["nom"] = "Alice", ["age"] = 25 } };
await jsonDoc.SaveAsync("data.json");  // Utilise la vraie implémentation async
```
:::

### Exercice 3 : Système de plugins extensible

Créez une interface `IPlugin` pour un système extensible avec :
- `string Nom` (abstraite)
- `string Version` (abstraite)
- `void Executer()` (abstraite)
- `void Initialiser()` (par défaut, ne fait rien)
- `void Terminer()` (par défaut, ne fait rien)
- `string ObtenirInfo()` (par défaut, retourne Nom + Version)

:::details 💡 Solution Exercice 3

```csharp
interface IPlugin
{
    // Propriétés abstraites
    string Nom { get; }
    string Version { get; }
    
    // Méthode principale abstraite
    void Executer();
    
    // Lifecycle hooks avec implémentations par défaut
    void Initialiser()
    {
        Console.WriteLine($"🔌 Plugin {Nom} initialisé");
    }
    
    void Terminer()
    {
        Console.WriteLine($"🔌 Plugin {Nom} terminé");
    }
    
    // Méthodes utilitaires par défaut
    string ObtenirInfo() => $"{Nom} v{Version}";
    
    bool EstCompatible(string versionMinimale)
    {
        var actuelle = new Version(Version);
        var minimale = new Version(versionMinimale);
        return actuelle >= minimale;
    }
}

// Plugin simple utilisant les implémentations par défaut
class LoggerPlugin : IPlugin
{
    public string Nom => "Logger";
    public string Version => "1.0.0";
    
    public void Executer()
    {
        Console.WriteLine("📝 Logging activé...");
    }
}

// Plugin avec initialisation personnalisée
class DatabasePlugin : IPlugin
{
    public string Nom => "Database Connector";
    public string Version => "2.1.0";
    
    private bool _connected = false;
    
    public void Initialiser()
    {
        Console.WriteLine("🔌 Connexion à la base de données...");
        _connected = true;
        Console.WriteLine("✓ Connecté !");
    }
    
    public void Executer()
    {
        if (_connected)
            Console.WriteLine("📊 Exécution des requêtes...");
        else
            Console.WriteLine("❌ Non connecté !");
    }
    
    public void Terminer()
    {
        Console.WriteLine("🔌 Fermeture de la connexion...");
        _connected = false;
    }
}

// Plugin avec info personnalisée
class AnalyticsPlugin : IPlugin
{
    public string Nom => "Analytics";
    public string Version => "3.0.0";
    public string ApiKey { get; init; }
    
    public void Executer()
    {
        Console.WriteLine($"📈 Envoi des analytics (API: {ApiKey})");
    }
    
    // Surcharge de ObtenirInfo
    public string ObtenirInfo() => $"{Nom} v{Version} (API: {ApiKey[..8]}...)";
}

// Gestionnaire de plugins
class PluginManager
{
    private List<IPlugin> _plugins = new();
    
    public void Enregistrer(IPlugin plugin)
    {
        _plugins.Add(plugin);
        Console.WriteLine($"✓ Plugin enregistré : {plugin.ObtenirInfo()}");
    }
    
    public void Demarrer()
    {
        Console.WriteLine("\n=== Initialisation des plugins ===");
        foreach (var plugin in _plugins)
        {
            plugin.Initialiser();
        }
        
        Console.WriteLine("\n=== Exécution des plugins ===");
        foreach (var plugin in _plugins)
        {
            plugin.Executer();
        }
    }
    
    public void Arreter()
    {
        Console.WriteLine("\n=== Arrêt des plugins ===");
        foreach (var plugin in _plugins.AsEnumerable().Reverse())
        {
            plugin.Terminer();
        }
    }
    
    public void AfficherPlugins()
    {
        Console.WriteLine("\n=== Plugins installés ===");
        foreach (var plugin in _plugins)
        {
            var compat = plugin.EstCompatible("1.5.0") ? "✓" : "⚠️ Ancien";
            Console.WriteLine($"  {compat} {plugin.ObtenirInfo()}");
        }
    }
}

// Utilisation
var manager = new PluginManager();
manager.Enregistrer(new LoggerPlugin());
manager.Enregistrer(new DatabasePlugin());
manager.Enregistrer(new AnalyticsPlugin { ApiKey = "abc123def456ghi789" });

manager.AfficherPlugins();
manager.Demarrer();
manager.Arreter();
```

Sortie :
```
✓ Plugin enregistré : Logger v1.0.0
✓ Plugin enregistré : Database Connector v2.1.0
✓ Plugin enregistré : Analytics v3.0.0 (API: abc123de...)

=== Plugins installés ===
  ⚠️ Ancien Logger v1.0.0
  ✓ Database Connector v2.1.0
  ✓ Analytics v3.0.0 (API: abc123de...)

=== Initialisation des plugins ===
🔌 Plugin Logger initialisé
🔌 Connexion à la base de données...
✓ Connecté !
🔌 Plugin Analytics initialisé

=== Exécution des plugins ===
📝 Logging activé...
📊 Exécution des requêtes...
📈 Envoi des analytics (API: abc123def456ghi789)

=== Arrêt des plugins ===
🔌 Plugin Analytics terminé
🔌 Fermeture de la connexion...
🔌 Plugin Logger terminé
```
:::

## Résumé

| Fonctionnalité | Description |
|----------------|-------------|
| **Implémentation par défaut** | Méthodes avec corps dans l'interface |
| **Évolution d'interface** | Ajouter des méthodes sans casser le code existant |
| **Membres statiques** | `static` et `static abstract` (C# 11) |
| **Modificateurs d'accès** | `private`, `protected` dans les interfaces |
| **Surcharge** | Les classes peuvent remplacer l'implémentation par défaut |
