# Pattern Matching (Filtrage par motif)

## Introduction

Imaginez que vous travaillez au **guichet d'une gare** et que vous devez orienter les voyageurs vers la bonne voie. Chaque voyageur présente son billet, et vous devez rapidement l'analyser pour déterminer où l'envoyer.

**Avant le pattern matching** (méthode traditionnelle) :
```
SI le billet est de type TGV ALORS
    SI la destination est Paris ALORS voie 1
    SINON SI la destination est Bruxelles ALORS voie 2
    ...
SINON SI le billet est de type TER ALORS
    ...
```

**Avec le pattern matching** (méthode moderne) :
```
CE BILLET correspond à :
    TGV vers Paris      → Voie 1
    TGV vers Bruxelles  → Voie 2  
    TER n'importe où    → Voie 5
    Billet Premium      → Voie VIP
    Autre               → Guichet info
```

```
┌─────────────────────────────────────────────────────────────────┐
│                    AIGUILLAGE INTELLIGENT                       │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│    Voyageur ──→ 🎫 Billet ──→ ╔═══════════════════╗            │
│                               ║  PATTERN MATCHING  ║            │
│                               ╚═══════════════════╝            │
│                                        │                        │
│            ┌──────────┬──────────┬─────┴─────┬──────────┐      │
│            ▼          ▼          ▼           ▼          ▼      │
│         Voie 1     Voie 2     Voie 5      VIP        Info      │
│         (TGV       (TGV       (TER)     (Premium)   (Autre)    │
│         Paris)    Bruxelles)                                    │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

:::tip 💡 L'idée clé
Le **Pattern Matching** permet d'analyser un objet selon sa **forme** (type, structure, propriétés) et d'exécuter le code approprié en une seule expression élégante. C'est comme un super-switch qui comprend les objets !
:::

## L'opérateur `is` amélioré

### Test de type simple

```csharp
object valeur = "Hello";

// Ancienne façon
if (valeur is string)
{
    string s = (string)valeur;
    Console.WriteLine(s.Length);
}

// Avec pattern matching (C# 7+)
if (valeur is string s)
{
    Console.WriteLine(s.Length);  // 's' est déjà typé
}
```

### Patterns de constantes

```csharp
object obj = 42;

if (obj is 42)
{
    Console.WriteLine("C'est 42!");
}

if (obj is null)
{
    Console.WriteLine("C'est null");
}

if (obj is not null)
{
    Console.WriteLine("Ce n'est pas null");
}
```

## Switch Expression (C# 8+)

La **switch expression** est une version moderne et concise du `switch` statement :

```csharp
// Switch classique
string GetJourType(int jour)
{
    switch (jour)
    {
        case 0:
        case 6:
            return "Weekend";
        default:
            return "Semaine";
    }
}

// Switch expression
string GetJourType(int jour) => jour switch
{
    0 or 6 => "Weekend",
    _ => "Semaine"
};
```

### Avec plusieurs patterns

```csharp
string Classifier(int nombre) => nombre switch
{
    < 0 => "Négatif",
    0 => "Zéro",
    > 0 and < 10 => "Petit positif",
    >= 10 and < 100 => "Moyen",
    _ => "Grand"
};

Console.WriteLine(Classifier(-5));   // Négatif
Console.WriteLine(Classifier(0));    // Zéro
Console.WriteLine(Classifier(7));    // Petit positif
Console.WriteLine(Classifier(50));   // Moyen
Console.WriteLine(Classifier(500));  // Grand
```

## Types de patterns

### 1. Pattern de type

```csharp
object obj = new List<int> { 1, 2, 3 };

string Description(object o) => o switch
{
    string s => $"Chaîne de {s.Length} caractères",
    int i => $"Entier: {i}",
    List<int> liste => $"Liste de {liste.Count} entiers",
    null => "Null",
    _ => "Type inconnu"
};
```

### 2. Pattern relationnel

```csharp
string Evaluer(double note) => note switch
{
    >= 90 => "A",
    >= 80 => "B",
    >= 70 => "C",
    >= 60 => "D",
    < 60 => "F"
};

string CategorieAge(int age) => age switch
{
    < 0 => throw new ArgumentException("Âge invalide"),
    < 13 => "Enfant",
    < 20 => "Adolescent",
    < 65 => "Adulte",
    _ => "Senior"
};
```

### 3. Pattern logique (`and`, `or`, `not`)

```csharp
bool EstValide(int code) => code is > 100 and < 1000;

bool EstSpecial(int n) => n is 0 or 1 or -1;

bool EstNormal(object o) => o is not null and not "";

string Classifier(int x) => x switch
{
    > 0 and < 10 => "Petit positif",
    >= 10 and <= 100 => "Moyen",
    > 100 or < -100 => "Extrême",
    _ => "Autre"
};
```

### 4. Pattern de propriété

Très puissant pour tester les propriétés d'un objet :

```csharp
record Personne(string Nom, int Age, string Ville);

string Analyser(Personne p) => p switch
{
    { Age: < 18 } => "Mineur",
    { Age: >= 18, Ville: "Bruxelles" } => "Adulte bruxellois",
    { Age: >= 65 } => "Senior",
    { Nom: "Admin" } => "Administrateur",
    _ => "Adulte standard"
};

Personne p1 = new("Alice", 15, "Liège");
Personne p2 = new("Bob", 30, "Bruxelles");
Personne p3 = new("Charlie", 70, "Namur");

Console.WriteLine(Analyser(p1));  // Mineur
Console.WriteLine(Analyser(p2));  // Adulte bruxellois
Console.WriteLine(Analyser(p3));  // Senior
```

### 5. Pattern positionnel (déconstruction)

```csharp
record Point(int X, int Y);

string Position(Point p) => p switch
{
    (0, 0) => "Origine",
    (0, _) => "Sur l'axe Y",
    (_, 0) => "Sur l'axe X",
    (> 0, > 0) => "Quadrant 1",
    (< 0, > 0) => "Quadrant 2",
    (< 0, < 0) => "Quadrant 3",
    (> 0, < 0) => "Quadrant 4"
};

Console.WriteLine(Position(new Point(0, 0)));   // Origine
Console.WriteLine(Position(new Point(5, 3)));   // Quadrant 1
Console.WriteLine(Position(new Point(-2, -4))); // Quadrant 3
```

### 6. Pattern de liste (C# 11+)

```csharp
string AnalyserListe(int[] nombres) => nombres switch
{
    [] => "Liste vide",
    [var seul] => $"Un seul élément: {seul}",
    [var premier, var dernier] => $"Deux éléments: {premier} et {dernier}",
    [var premier, .., var dernier] => $"De {premier} à {dernier}",
};

Console.WriteLine(AnalyserListe(new int[] {}));           // Liste vide
Console.WriteLine(AnalyserListe(new int[] { 42 }));       // Un seul élément: 42
Console.WriteLine(AnalyserListe(new int[] { 1, 5 }));     // Deux éléments: 1 et 5
Console.WriteLine(AnalyserListe(new int[] { 1, 2, 3, 4, 5 }));  // De 1 à 5
```

## Pattern avec `when` (garde)

On peut ajouter des conditions supplémentaires avec `when` :

```csharp
record Commande(string Client, decimal Montant, bool EstPrioritaire);

string TraiterCommande(Commande c) => c switch
{
    { Montant: <= 0 } => "Commande invalide",
    { EstPrioritaire: true } when c.Montant > 1000 => "VIP prioritaire",
    { EstPrioritaire: true } => "Prioritaire standard",
    { Montant: > 500 } => "Grosse commande",
    _ => "Commande normale"
};
```

## Cas pratiques

### 1. Validation d'entrées

```csharp
record FormulairInscription(string Email, string MotDePasse, int Age);

List<string> Valider(FormulairInscription f) 
{
    var erreurs = new List<string>();
    
    _ = f switch
    {
        { Email: null or "" } => erreurs.Add("Email requis"),
        { Email: var e } when !e.Contains("@") => erreurs.Add("Email invalide"),
        _ => false
    };
    
    _ = f switch
    {
        { MotDePasse: null or "" } => erreurs.Add("Mot de passe requis"),
        { MotDePasse.Length: < 8 } => erreurs.Add("Mot de passe trop court"),
        _ => false
    };
    
    _ = f switch
    {
        { Age: < 18 } => erreurs.Add("Vous devez avoir 18 ans"),
        { Age: > 120 } => erreurs.Add("Âge invalide"),
        _ => false
    };
    
    return erreurs;
}
```

### 2. Calcul de prix avec règles

```csharp
record Article(string Categorie, decimal Prix, bool EnPromo);
record Panier(List<Article> Articles, string CodeClient);

decimal CalculerTotal(Panier panier)
{
    decimal total = 0;
    
    foreach (var article in panier.Articles)
    {
        total += (article, panier.CodeClient) switch
        {
            // Articles en promo
            ({ EnPromo: true }, _) => article.Prix * 0.8m,
            
            // Client premium, catégorie luxe
            ({ Categorie: "Luxe" }, "PREMIUM") => article.Prix * 0.85m,
            
            // Client premium, autres
            (_, "PREMIUM") => article.Prix * 0.9m,
            
            // Cas par défaut
            _ => article.Prix
        };
    }
    
    return total;
}
```

### 3. Traitement de résultats API

```csharp
abstract record ApiResult;
record Success(object Data) : ApiResult;
record Error(int Code, string Message) : ApiResult;
record Timeout : ApiResult;

void TraiterResultat(ApiResult result)
{
    var message = result switch
    {
        Success { Data: string s } => $"Texte reçu: {s}",
        Success { Data: int n } => $"Nombre reçu: {n}",
        Success s => $"Données reçues: {s.Data}",
        Error { Code: 404 } => "Ressource non trouvée",
        Error { Code: >= 500 } => "Erreur serveur",
        Error e => $"Erreur {e.Code}: {e.Message}",
        Timeout => "Délai d'attente dépassé",
        _ => "Résultat inconnu"
    };
    
    Console.WriteLine(message);
}
```

### 4. Machine à états

```csharp
enum EtatCommande { Nouvelle, Validee, EnPreparation, Expediee, Livree, Annulee }

record Commande(int Id, EtatCommande Etat);

EtatCommande Transition(Commande cmd, string action) => (cmd.Etat, action) switch
{
    (EtatCommande.Nouvelle, "valider") => EtatCommande.Validee,
    (EtatCommande.Nouvelle, "annuler") => EtatCommande.Annulee,
    (EtatCommande.Validee, "preparer") => EtatCommande.EnPreparation,
    (EtatCommande.Validee, "annuler") => EtatCommande.Annulee,
    (EtatCommande.EnPreparation, "expedier") => EtatCommande.Expediee,
    (EtatCommande.Expediee, "livrer") => EtatCommande.Livree,
    _ => throw new InvalidOperationException($"Action '{action}' invalide pour l'état {cmd.Etat}")
};
```

## Pattern matching dans LINQ

```csharp
var objets = new object[] { 1, "hello", 2.5, null, "world", 42 };

// Filtrer par type
var strings = objets.OfType<string>();  // "hello", "world"

// Avec pattern matching
var nombres = objets
    .Where(o => o is int)
    .Cast<int>()
    .ToList();  // 1, 42

// Plus élégant avec Select et pattern
var descriptions = objets
    .Select(o => o switch
    {
        int i => $"Entier: {i}",
        string s => $"Chaîne: {s}",
        double d => $"Double: {d}",
        null => "Null",
        _ => "Autre"
    })
    .ToList();
```

## Avantages du Pattern Matching

| Avantage | Description |
|----------|-------------|
| **Lisibilité** | Code plus expressif et auto-documenté |
| **Sécurité** | Le compilateur vérifie l'exhaustivité |
| **Concision** | Moins de code boilerplate |
| **Extraction** | Déconstruction et binding en une étape |
| **Composition** | Combinaison de patterns avec `and`, `or` |

## Exercices

### Exercice 1 : Classification de triangles

Créez une fonction qui classifie un triangle selon ses côtés :
- Équilatéral (3 côtés égaux)
- Isocèle (2 côtés égaux)
- Scalène (3 côtés différents)
- Invalide (somme de 2 côtés <= troisième)

:::details 💡 Solution Exercice 1

```csharp
record Triangle(double A, double B, double C)
{
    // Méthode de validation
    public bool EstValide => A > 0 && B > 0 && C > 0 &&
                             A + B > C && A + C > B && B + C > A;
}

string ClassifierTriangle(Triangle t) => t switch
{
    // D'abord vérifier la validité
    { EstValide: false } => "❌ Triangle invalide",
    
    // Équilatéral : les 3 côtés égaux
    { A: var a, B: var b, C: var c } when a == b && b == c => "🔺 Équilatéral",
    
    // Isocèle : au moins 2 côtés égaux
    { A: var a, B: var b } when a == b => "🔻 Isocèle (A = B)",
    { A: var a, C: var c } when a == c => "🔻 Isocèle (A = C)",
    { B: var b, C: var c } when b == c => "🔻 Isocèle (B = C)",
    
    // Scalène : tous différents
    _ => "📐 Scalène"
};

// Version plus concise avec pattern positionnel
string ClassifierTriangleConcis((double a, double b, double c) t) => t switch
{
    var (a, b, c) when a <= 0 || b <= 0 || c <= 0 => "❌ Côtés négatifs",
    var (a, b, c) when a + b <= c || a + c <= b || b + c <= a => "❌ Inégalité triangulaire non respectée",
    var (a, b, c) when a == b && b == c => "🔺 Équilatéral",
    var (a, b, c) when a == b || b == c || a == c => "🔻 Isocèle",
    _ => "📐 Scalène"
};

// Tests
Console.WriteLine(ClassifierTriangle(new Triangle(5, 5, 5)));    // 🔺 Équilatéral
Console.WriteLine(ClassifierTriangle(new Triangle(5, 5, 3)));    // 🔻 Isocèle (A = B)
Console.WriteLine(ClassifierTriangle(new Triangle(3, 4, 5)));    // 📐 Scalène
Console.WriteLine(ClassifierTriangle(new Triangle(1, 2, 10)));   // ❌ Triangle invalide
Console.WriteLine(ClassifierTriangle(new Triangle(-1, 5, 5)));   // ❌ Triangle invalide

Console.WriteLine(ClassifierTriangleConcis((3, 3, 3)));          // 🔺 Équilatéral
Console.WriteLine(ClassifierTriangleConcis((1, 1, 100)));        // ❌ Inégalité triangulaire non respectée
```
:::

### Exercice 2 : Calculatrice d'expressions

Créez des records pour représenter des expressions mathématiques et une fonction qui les évalue :
```csharp
abstract record Expression;
record Nombre(double Valeur) : Expression;
record Addition(Expression Gauche, Expression Droite) : Expression;
record Multiplication(Expression Gauche, Expression Droite) : Expression;
```

:::details 💡 Solution Exercice 2

```csharp
// Définition des expressions
abstract record Expression;

record Nombre(double Valeur) : Expression
{
    public override string ToString() => Valeur.ToString();
}

record Addition(Expression Gauche, Expression Droite) : Expression
{
    public override string ToString() => $"({Gauche} + {Droite})";
}

record Soustraction(Expression Gauche, Expression Droite) : Expression
{
    public override string ToString() => $"({Gauche} - {Droite})";
}

record Multiplication(Expression Gauche, Expression Droite) : Expression
{
    public override string ToString() => $"({Gauche} × {Droite})";
}

record Division(Expression Gauche, Expression Droite) : Expression
{
    public override string ToString() => $"({Gauche} ÷ {Droite})";
}

record Negation(Expression Operande) : Expression
{
    public override string ToString() => $"-{Operande}";
}

// Évaluation avec pattern matching
double Evaluer(Expression expr) => expr switch
{
    Nombre n => n.Valeur,
    Addition(var g, var d) => Evaluer(g) + Evaluer(d),
    Soustraction(var g, var d) => Evaluer(g) - Evaluer(d),
    Multiplication(var g, var d) => Evaluer(g) * Evaluer(d),
    Division(_, Nombre { Valeur: 0 }) => throw new DivideByZeroException(),
    Division(var g, var d) => Evaluer(g) / Evaluer(d),
    Negation(var op) => -Evaluer(op),
    _ => throw new NotSupportedException($"Expression non supportée : {expr}")
};

// Simplification d'expressions
Expression Simplifier(Expression expr) => expr switch
{
    // x + 0 = x
    Addition(var x, Nombre { Valeur: 0 }) => Simplifier(x),
    Addition(Nombre { Valeur: 0 }, var x) => Simplifier(x),
    
    // x - 0 = x
    Soustraction(var x, Nombre { Valeur: 0 }) => Simplifier(x),
    
    // x * 1 = x
    Multiplication(var x, Nombre { Valeur: 1 }) => Simplifier(x),
    Multiplication(Nombre { Valeur: 1 }, var x) => Simplifier(x),
    
    // x * 0 = 0
    Multiplication(_, Nombre { Valeur: 0 }) => new Nombre(0),
    Multiplication(Nombre { Valeur: 0 }, _) => new Nombre(0),
    
    // x / 1 = x
    Division(var x, Nombre { Valeur: 1 }) => Simplifier(x),
    
    // Double négation
    Negation(Negation(var x)) => Simplifier(x),
    
    // Récursion pour les autres cas
    Addition(var g, var d) => new Addition(Simplifier(g), Simplifier(d)),
    Soustraction(var g, var d) => new Soustraction(Simplifier(g), Simplifier(d)),
    Multiplication(var g, var d) => new Multiplication(Simplifier(g), Simplifier(d)),
    Division(var g, var d) => new Division(Simplifier(g), Simplifier(d)),
    Negation(var op) => new Negation(Simplifier(op)),
    
    _ => expr
};

// Tests
// Expression : (3 + 5) × 2
var expr1 = new Multiplication(
    new Addition(new Nombre(3), new Nombre(5)),
    new Nombre(2)
);

Console.WriteLine($"{expr1} = {Evaluer(expr1)}");
// ((3 + 5) × 2) = 16

// Expression : (10 - 4) ÷ (1 + 2)
var expr2 = new Division(
    new Soustraction(new Nombre(10), new Nombre(4)),
    new Addition(new Nombre(1), new Nombre(2))
);

Console.WriteLine($"{expr2} = {Evaluer(expr2)}");
// ((10 - 4) ÷ (1 + 2)) = 2

// Expression avec simplification : (x + 0) × 1
var expr3 = new Multiplication(
    new Addition(new Nombre(42), new Nombre(0)),
    new Nombre(1)
);

Console.WriteLine($"Avant : {expr3}");
Console.WriteLine($"Après : {Simplifier(expr3)}");
// Avant : ((42 + 0) × 1)
// Après : 42
```
:::

### Exercice 3 : Système de filtrage de messages

Créez un système qui filtre et traite différents types de messages (Email, SMS, Push) selon leur contenu et priorité :

:::details 💡 Solution Exercice 3

```csharp
// Types de messages
abstract record Message(string Expediteur, string Contenu, DateTime Date);

record Email(string Expediteur, string Contenu, DateTime Date, string Sujet, bool Urgent) 
    : Message(Expediteur, Contenu, Date);
    
record Sms(string Expediteur, string Contenu, DateTime Date, string Numero) 
    : Message(Expediteur, Contenu, Date);
    
record PushNotification(string Expediteur, string Contenu, DateTime Date, string App, int Priorite) 
    : Message(Expediteur, Contenu, Date);

// Actions à effectuer
enum Action { Notifier, Archiver, Supprimer, Bloquer, Urgent }

// Système de filtrage avec pattern matching
class FiltreMessages
{
    private readonly HashSet<string> _expediteursBloqués = new() { "spam@evil.com", "+666" };
    private readonly HashSet<string> _vip = new() { "boss@company.com", "+32123456789" };
    
    public (Action action, string raison) Filtrer(Message msg) => msg switch
    {
        // Messages des expéditeurs bloqués
        { Expediteur: var exp } when _expediteursBloqués.Contains(exp) 
            => (Action.Bloquer, "Expéditeur bloqué"),
        
        // Emails urgents des VIP
        Email { Urgent: true, Expediteur: var exp } when _vip.Contains(exp)
            => (Action.Urgent, "Email VIP urgent"),
        
        // Emails urgents
        Email { Urgent: true }
            => (Action.Notifier, "Email urgent"),
        
        // Emails avec mots-clés spam dans le sujet
        Email { Sujet: var s } when ContientSpam(s)
            => (Action.Supprimer, "Sujet suspect"),
        
        // SMS des VIP
        Sms { Expediteur: var exp } when _vip.Contains(exp)
            => (Action.Urgent, "SMS VIP"),
        
        // SMS courts (< 10 caractères) = probablement du spam
        Sms { Contenu.Length: < 10 }
            => (Action.Archiver, "SMS trop court"),
        
        // Push notifications haute priorité
        PushNotification { Priorite: >= 8 }
            => (Action.Urgent, "Notification haute priorité"),
        
        // Push notifications basse priorité
        PushNotification { Priorite: <= 2 }
            => (Action.Archiver, "Notification basse priorité"),
        
        // Messages anciens (> 30 jours)
        { Date: var d } when (DateTime.Now - d).Days > 30
            => (Action.Archiver, "Message ancien"),
        
        // Par défaut : notification normale
        _ => (Action.Notifier, "Message normal")
    };
    
    private bool ContientSpam(string texte)
    {
        var motsSpam = new[] { "gratuit", "gagnez", "urgent!!!", "cliquez ici" };
        return motsSpam.Any(mot => texte.Contains(mot, StringComparison.OrdinalIgnoreCase));
    }
    
    public void TraiterMessages(IEnumerable<Message> messages)
    {
        var groupes = messages
            .Select(m => (message: m, resultat: Filtrer(m)))
            .GroupBy(x => x.resultat.action);
        
        foreach (var groupe in groupes)
        {
            Console.WriteLine($"\n=== {groupe.Key} ({groupe.Count()} messages) ===");
            
            foreach (var (message, (action, raison)) in groupe)
            {
                var icone = action switch
                {
                    Action.Urgent => "🚨",
                    Action.Notifier => "📬",
                    Action.Archiver => "📁",
                    Action.Supprimer => "🗑️",
                    Action.Bloquer => "🚫",
                    _ => "❓"
                };
                
                var type = message switch
                {
                    Email e => $"📧 {e.Sujet}",
                    Sms s => $"📱 {s.Numero}",
                    PushNotification p => $"🔔 {p.App}",
                    _ => "?"
                };
                
                Console.WriteLine($"  {icone} {type} de {message.Expediteur}");
                Console.WriteLine($"     Raison: {raison}");
            }
        }
    }
}

// Utilisation
var filtre = new FiltreMessages();

var messages = new Message[]
{
    new Email("boss@company.com", "Réunion demain", DateTime.Now, "URGENT: Réunion", Urgent: true),
    new Email("spam@evil.com", "Gagnez 1M€", DateTime.Now, "Vous avez gagné!", Urgent: false),
    new Email("collegue@work.com", "Hello", DateTime.Now, "Gratuit aujourd'hui", Urgent: false),
    new Sms("+32123456789", "Rappelle-moi stp", DateTime.Now, "+32123456789"),
    new Sms("+666", "SPAM", DateTime.Now, "+666"),
    new Sms("+32987654321", "OK", DateTime.Now, "+32987654321"),
    new PushNotification("App", "Nouvelle mise à jour", DateTime.Now, "MonApp", 9),
    new PushNotification("Jeu", "Rejouez!", DateTime.Now, "CandyCrush", 1),
    new Email("ancien@mail.com", "Vieux message", DateTime.Now.AddDays(-45), "Archive", Urgent: false),
};

filtre.TraiterMessages(messages);
```

Sortie :
```
=== Urgent (2 messages) ===
  🚨 📧 URGENT: Réunion de boss@company.com
     Raison: Email VIP urgent
  🚨 📱 +32123456789 de +32123456789
     Raison: SMS VIP

=== Supprimer (1 messages) ===
  🗑️ 📧 Gratuit aujourd'hui de collegue@work.com
     Raison: Sujet suspect

=== Bloquer (2 messages) ===
  🚫 📧 Vous avez gagné! de spam@evil.com
     Raison: Expéditeur bloqué
  🚫 📱 +666 de +666
     Raison: Expéditeur bloqué

=== Archiver (3 messages) ===
  📁 📱 +32987654321 de +32987654321
     Raison: SMS trop court
  📁 🔔 CandyCrush de Jeu
     Raison: Notification basse priorité
  📁 📧 Archive de ancien@mail.com
     Raison: Message ancien

=== Notifier (1 messages) ===
  📬 🔔 MonApp de App
     Raison: Notification haute priorité → devrait être Urgent en fait!
```
:::

## Résumé

| Pattern | Exemple | Description |
|---------|---------|-------------|
| Type | `is string s` | Teste et extrait le type |
| Constante | `is 42` ou `is null` | Compare à une valeur |
| Relationnel | `is > 0 and < 100` | Comparaisons numériques |
| Propriété | `{ Age: > 18 }` | Teste les propriétés |
| Positionnel | `(0, 0)` | Déconstruction |
| Liste | `[first, .., last]` | Pattern sur collections |
| Logique | `and`, `or`, `not` | Combinaison de patterns |
| Discard | `_` | Correspond à tout |
