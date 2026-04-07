# Concepts Avancés

Ce chapitre présente des concepts avancés de la programmation orientée objet en C# : la surcharge d'opérateurs, les types anonymes et les itérateurs.

::: tip 🎯 Ce que vous allez apprendre
- Surcharger les opérateurs pour créer des classes intuitives
- Utiliser les types anonymes pour regrouper des données temporaires
- Créer des itérateurs avec `yield return` pour des collections personnalisées
- Comprendre l'évaluation paresseuse (lazy evaluation)
:::

## Surcharge d'opérateurs

La **surcharge d'opérateurs** permet de redéfinir le comportement des opérateurs (`+`, `-`, `*`, `==`, etc.) pour vos propres classes.

### 🔧 Analogie : définir ses propres règles

Imaginez que vous créez un nouveau jeu de cartes. Vous devez définir :
- Comment **additionner** deux cartes (peut-être combiner leurs valeurs ?)
- Comment **comparer** deux cartes (laquelle est plus forte ?)
- Comment **égaler** deux cartes (mêmes propriétés ?)

La surcharge d'opérateurs vous permet de définir ces "règles du jeu" pour vos classes !

### Pourquoi surcharger les opérateurs ?

Considérez une classe `Vecteur2D`. Sans surcharge, l'addition serait :

```csharp
Vecteur2D v1 = new Vecteur2D(3, 4);
Vecteur2D v2 = new Vecteur2D(1, 2);
Vecteur2D v3 = v1.Additionner(v2);  // Peu intuitif
```

Avec la surcharge de l'opérateur `+` :

```csharp
Vecteur2D v3 = v1 + v2;  // Naturel et lisible
```

### Syntaxe de base

```csharp
public static TypeRetour operator +(TypeGauche a, TypeDroit b)
{
    // Logique de l'opération
    return resultat;
}
```

### Exemple complet : Vecteur2D

```csharp
class Vecteur2D
{
    public double X { get; }
    public double Y { get; }
    
    public Vecteur2D(double x, double y)
    {
        X = x;
        Y = y;
    }
    
    // Surcharge de +
    public static Vecteur2D operator +(Vecteur2D a, Vecteur2D b)
    {
        return new Vecteur2D(a.X + b.X, a.Y + b.Y);
    }
    
    // Surcharge de -
    public static Vecteur2D operator -(Vecteur2D a, Vecteur2D b)
    {
        return new Vecteur2D(a.X - b.X, a.Y - b.Y);
    }
    
    // Surcharge de * (multiplication par un scalaire)
    public static Vecteur2D operator *(Vecteur2D v, double scalaire)
    {
        return new Vecteur2D(v.X * scalaire, v.Y * scalaire);
    }
    
    // Version inversée (scalaire * vecteur)
    public static Vecteur2D operator *(double scalaire, Vecteur2D v)
    {
        return v * scalaire;  // Réutilise l'autre surcharge
    }
    
    // Surcharge de - unaire (négation)
    public static Vecteur2D operator -(Vecteur2D v)
    {
        return new Vecteur2D(-v.X, -v.Y);
    }
    
    // Propriété calculée : magnitude (longueur)
    public double Magnitude => Math.Sqrt(X * X + Y * Y);
    
    public override string ToString() => $"({X}, {Y})";
}
```

```csharp
// Utilisation naturelle
Vecteur2D v1 = new Vecteur2D(3, 4);
Vecteur2D v2 = new Vecteur2D(1, 2);

Vecteur2D somme = v1 + v2;          // (4, 6)
Vecteur2D diff = v1 - v2;           // (2, 2)
Vecteur2D double_ = v1 * 2;         // (6, 8)
Vecteur2D inverse = -v1;            // (-3, -4)

Console.WriteLine($"v1 + v2 = {somme}");
Console.WriteLine($"v1 * 2 = {double_}");
Console.WriteLine($"Magnitude de v1 = {v1.Magnitude}");  // 5
```

### Opérateurs de comparaison

Les opérateurs de comparaison doivent être surchargés **par paires** :

```csharp
class Fraction
{
    public int Numerateur { get; }
    public int Denominateur { get; }
    
    public Fraction(int num, int denom)
    {
        if (denom == 0) throw new DivideByZeroException();
        Numerateur = num;
        Denominateur = denom;
    }
    
    public double Valeur => (double)Numerateur / Denominateur;
    
    // == et != doivent être ensemble
    public static bool operator ==(Fraction a, Fraction b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
        return a.Numerateur * b.Denominateur == b.Numerateur * a.Denominateur;
    }
    
    public static bool operator !=(Fraction a, Fraction b)
    {
        return !(a == b);
    }
    
    // < et > doivent être ensemble
    public static bool operator <(Fraction a, Fraction b)
    {
        return a.Valeur < b.Valeur;
    }
    
    public static bool operator >(Fraction a, Fraction b)
    {
        return a.Valeur > b.Valeur;
    }
    
    // <= et >= doivent être ensemble
    public static bool operator <=(Fraction a, Fraction b)
    {
        return a.Valeur <= b.Valeur;
    }
    
    public static bool operator >=(Fraction a, Fraction b)
    {
        return a.Valeur >= b.Valeur;
    }
    
    // Recommandé : surcharger Equals et GetHashCode aussi
    public override bool Equals(object obj)
    {
        return obj is Fraction f && this == f;
    }
    
    public override int GetHashCode()
    {
        return Valeur.GetHashCode();
    }
}
```

### Opérateurs surchargeable

| Catégorie | Opérateurs |
|-----------|-----------|
| Arithmétiques | `+`, `-`, `*`, `/`, `%` |
| Unaires | `+`, `-`, `!`, `~`, `++`, `--`, `true`, `false` |
| Comparaison | `==`, `!=`, `<`, `>`, `<=`, `>=` |
| Bit à bit | `&`, `|`, `^`, `<<`, `>>` |

::: warning Non surchargeables
Certains opérateurs ne peuvent **pas** être surchargés : `=`, `.`, `?:`, `??`, `->`, `=>`, `is`, `as`, `typeof`, etc.
:::

## Types Anonymes

Les **types anonymes** permettent de créer des objets sans définir explicitement une classe. Ils sont utiles pour regrouper temporairement des données.

### Syntaxe

```csharp
var personne = new { Nom = "Alice", Age = 25, Ville = "Bruxelles" };

Console.WriteLine(personne.Nom);   // Alice
Console.WriteLine(personne.Age);   // 25
Console.WriteLine(personne.Ville); // Bruxelles
```

### Caractéristiques

1. **Immuables** : les propriétés sont en lecture seule
2. **Inférence de type** : le compilateur génère automatiquement la classe
3. **Égalité par valeur** : deux objets anonymes identiques sont égaux

```csharp
var p1 = new { X = 10, Y = 20 };
var p2 = new { X = 10, Y = 20 };
var p3 = new { X = 30, Y = 40 };

Console.WriteLine(p1 == p2);       // False (comparaison de références)
Console.WriteLine(p1.Equals(p2));  // True (comparaison par valeur)
Console.WriteLine(p1.Equals(p3));  // False
```

### Cas d'usage : projections

Les types anonymes sont souvent utilisés avec LINQ pour projeter des données :

```csharp
var etudiants = new[]
{
    new { Nom = "Alice", Note = 15.5 },
    new { Nom = "Bob", Note = 12.0 },
    new { Nom = "Charlie", Note = 18.0 }
};

// Projection avec type anonyme
var resultats = etudiants
    .Select(e => new { e.Nom, Reussi = e.Note >= 10 })
    .ToList();

foreach (var r in resultats)
{
    Console.WriteLine($"{r.Nom}: {(r.Reussi ? "Réussi" : "Échec")}");
}
```

### Limitations

- Impossible de déclarer comme type de retour d'une méthode (sauf avec `object` ou `dynamic`)
- Portée limitée au bloc où ils sont créés
- Pas de méthodes personnalisées

## Itérateurs

Les **itérateurs** permettent de définir comment une classe peut être parcourue avec `foreach`. Ils utilisent le mot-clé `yield return`.

### L'interface IEnumerable

Pour être parcourable avec `foreach`, une classe doit implémenter `IEnumerable<T>` (présentée dans le chapitre [Interfaces](./09-interfaces.md)). L'interface générique `IEnumerable<T>` est détaillée dans le chapitre [Classes et Interfaces Génériques](./10-generiques.md).

```csharp
using System.Collections;
using System.Collections.Generic;

class Playlist : IEnumerable<string>
{
    private List<string> _chansons = new List<string>();
    
    public void Ajouter(string chanson)
    {
        _chansons.Add(chanson);
    }
    
    // Implémentation de IEnumerable<T>
    public IEnumerator<string> GetEnumerator()
    {
        foreach (string chanson in _chansons)
        {
            yield return chanson;  // Retourne un élément à la fois
        }
    }
    
    // Implémentation de IEnumerable (non générique)
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
```

```csharp
Playlist maPlaylist = new Playlist();
maPlaylist.Ajouter("Bohemian Rhapsody");
maPlaylist.Ajouter("Stairway to Heaven");
maPlaylist.Ajouter("Hotel California");

foreach (string chanson in maPlaylist)
{
    Console.WriteLine(chanson);
}
```

### Le mot-clé `yield return`

`yield return` est magique : il **suspend** l'exécution de la méthode et retourne une valeur, puis reprend là où il s'était arrêté lors de la prochaine itération.

```csharp
class GenerateurNombres
{
    public static IEnumerable<int> De1A10()
    {
        for (int i = 1; i <= 10; i++)
        {
            Console.WriteLine($"Génération de {i}");
            yield return i;
        }
    }
}

// Les nombres sont générés à la demande (lazy evaluation)
foreach (int n in GenerateurNombres.De1A10())
{
    Console.WriteLine($"Reçu: {n}");
    if (n == 3) break;  // On peut arrêter tôt
}

// Output:
// Génération de 1
// Reçu: 1
// Génération de 2
// Reçu: 2
// Génération de 3
// Reçu: 3
```

### `yield break` : arrêter l'itération

```csharp
IEnumerable<int> NombresPositifs(int[] tableau)
{
    foreach (int n in tableau)
    {
        if (n < 0)
            yield break;  // Arrête complètement l'itération
        yield return n;
    }
}

int[] nombres = { 1, 2, 3, -1, 4, 5 };
foreach (int n in NombresPositifs(nombres))
{
    Console.WriteLine(n);  // 1, 2, 3 (s'arrête à -1)
}
```

### Exemple avancé : générateur de Fibonacci

```csharp
static IEnumerable<long> Fibonacci(int count)
{
    long a = 0, b = 1;
    
    for (int i = 0; i < count; i++)
    {
        yield return a;
        
        long temp = a;
        a = b;
        b = temp + b;
    }
}

// Utilisation
Console.WriteLine("Les 10 premiers nombres de Fibonacci:");
foreach (long fib in Fibonacci(10))
{
    Console.Write($"{fib} ");  // 0 1 1 2 3 5 8 13 21 34
}
```

### Collection personnalisée complète

Cet exemple combine interfaces génériques (voir chapitre [Génériques](./10-generiques.md)) et itérateurs :

```csharp
class ListeTriee<T> : IEnumerable<T> where T : IComparable<T>
{
    private List<T> _elements = new List<T>();
    
    public void Ajouter(T element)
    {
        _elements.Add(element);
        _elements.Sort();  // Maintient le tri
    }
    
    public int Count => _elements.Count;
    
    // Itérateur standard
    public IEnumerator<T> GetEnumerator()
    {
        foreach (T element in _elements)
            yield return element;
    }
    
    // Itérateur inverse
    public IEnumerable<T> Inverse()
    {
        for (int i = _elements.Count - 1; i >= 0; i--)
            yield return _elements[i];
    }
    
    // Itérateur filtré
    public IEnumerable<T> Filtrer(Func<T, bool> condition)
    {
        foreach (T element in _elements)
        {
            if (condition(element))
                yield return element;
        }
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
```

```csharp
ListeTriee<int> nombres = new ListeTriee<int>();
nombres.Ajouter(5);
nombres.Ajouter(2);
nombres.Ajouter(8);
nombres.Ajouter(1);

Console.WriteLine("Ordre croissant:");
foreach (int n in nombres)
    Console.Write($"{n} ");  // 1 2 5 8

Console.WriteLine("\nOrdre décroissant:");
foreach (int n in nombres.Inverse())
    Console.Write($"{n} ");  // 8 5 2 1

Console.WriteLine("\nSupérieurs à 3:");
foreach (int n in nombres.Filtrer(x => x > 3))
    Console.Write($"{n} ");  // 5 8
```

## Avantages de `yield`

| Avantage | Description |
|----------|-------------|
| **Évaluation paresseuse** | Les éléments sont générés à la demande |
| **Mémoire efficace** | Pas besoin de stocker toute la séquence |
| **Code simplifié** | Pas besoin de créer une classe Enumerator |
| **Composabilité** | Facile à chaîner avec LINQ |

## Exercices

### Exercice 1 : Classe Argent

Créez une classe `Argent` avec :
- Propriétés `Montant` et `Devise`
- Surcharge de `+` (même devise uniquement)
- Surcharge de `*` (par un scalaire)
- Surcharge de `==` et `!=`

::: details 💡 Solution Exercice 1

```csharp
class Argent
{
    public decimal Montant { get; }
    public string Devise { get; }
    
    public Argent(decimal montant, string devise)
    {
        Montant = montant;
        Devise = devise.ToUpper();
    }
    
    // Addition (même devise uniquement)
    public static Argent operator +(Argent a, Argent b)
    {
        if (a.Devise != b.Devise)
            throw new InvalidOperationException($"Impossible d'additionner {a.Devise} et {b.Devise}");
        
        return new Argent(a.Montant + b.Montant, a.Devise);
    }
    
    // Multiplication par un scalaire
    public static Argent operator *(Argent a, decimal scalaire)
    {
        return new Argent(a.Montant * scalaire, a.Devise);
    }
    
    public static Argent operator *(decimal scalaire, Argent a)
    {
        return a * scalaire;
    }
    
    // Égalité
    public static bool operator ==(Argent a, Argent b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
        return a.Montant == b.Montant && a.Devise == b.Devise;
    }
    
    public static bool operator !=(Argent a, Argent b)
    {
        return !(a == b);
    }
    
    public override bool Equals(object obj)
    {
        return obj is Argent a && this == a;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Montant, Devise);
    }
    
    public override string ToString() => $"{Montant:F2} {Devise}";
}

// Test
var euros1 = new Argent(100, "EUR");
var euros2 = new Argent(50, "EUR");
var dollars = new Argent(75, "USD");

Console.WriteLine(euros1 + euros2);  // 150.00 EUR
Console.WriteLine(euros1 * 2);       // 200.00 EUR
Console.WriteLine(euros1 == new Argent(100, "EUR"));  // True

// Console.WriteLine(euros1 + dollars);  // Exception !
```
:::

### Exercice 2 : Itérateur de plage

Créez une méthode `Plage(int debut, int fin, int pas)` qui :
- Retourne un `IEnumerable<int>`
- Génère les nombres de début à fin avec le pas spécifié
- Fonctionne dans les deux sens (croissant/décroissant)

::: details 💡 Solution Exercice 2

```csharp
static IEnumerable<int> Plage(int debut, int fin, int pas = 1)
{
    // Validation
    if (pas == 0)
        throw new ArgumentException("Le pas ne peut pas être 0");
    
    // Déterminer le sens automatiquement si pas non spécifié
    if (debut < fin && pas < 0)
        throw new ArgumentException("Pas négatif pour une plage croissante");
    
    if (debut > fin && pas > 0)
        throw new ArgumentException("Pas positif pour une plage décroissante");
    
    // Génération croissante
    if (debut <= fin)
    {
        for (int i = debut; i <= fin; i += pas)
        {
            yield return i;
        }
    }
    // Génération décroissante
    else
    {
        for (int i = debut; i >= fin; i += pas)  // pas est négatif
        {
            yield return i;
        }
    }
}

// Tests
Console.WriteLine("De 1 à 10 :");
foreach (int n in Plage(1, 10))
    Console.Write($"{n} ");  // 1 2 3 4 5 6 7 8 9 10

Console.WriteLine("\n\nDe 1 à 10 par 2 :");
foreach (int n in Plage(1, 10, 2))
    Console.Write($"{n} ");  // 1 3 5 7 9

Console.WriteLine("\n\nDe 10 à 1 :");
foreach (int n in Plage(10, 1, -1))
    Console.Write($"{n} ");  // 10 9 8 7 6 5 4 3 2 1

Console.WriteLine("\n\nDe 100 à 50 par 10 :");
foreach (int n in Plage(100, 50, -10))
    Console.Write($"{n} ");  // 100 90 80 70 60 50
```
:::

### Exercice 3 : Générateur de mots de passe

Créez un itérateur qui génère des mots de passe aléatoires :

```csharp
IEnumerable<string> GenererMotsDePasse(int longueur, int nombre)
```

::: details 💡 Solution Exercice 3

```csharp
static IEnumerable<string> GenererMotsDePasse(int longueur, int nombre)
{
    const string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%";
    Random random = new Random();
    
    for (int i = 0; i < nombre; i++)
    {
        char[] password = new char[longueur];
        
        for (int j = 0; j < longueur; j++)
        {
            password[j] = caracteres[random.Next(caracteres.Length)];
        }
        
        yield return new string(password);
    }
}

// Test
Console.WriteLine("5 mots de passe de 12 caractères :");
foreach (string mdp in GenererMotsDePasse(12, 5))
{
    Console.WriteLine(mdp);
}
```

**Exemple de sortie** :
```
5 mots de passe de 12 caractères :
Kp3@mNx9Fq2%
aB7$Yz1!RtWe
mQ5#Lp8vXcJn
sD2@Gh6kBnMw
rT9!Uy3pEqZa
```
:::

## Résumé

| Concept | Description |
|---------|-------------|
| **Surcharge d'opérateurs** | Redéfinir `+`, `-`, `==`, etc. pour vos classes |
| **Types anonymes** | Objets créés à la volée avec `new { }` |
| **IEnumerable** | Interface pour les collections parcourables |
| **yield return** | Retourne un élément et suspend l'exécution |
| **yield break** | Termine l'itération prématurément |
