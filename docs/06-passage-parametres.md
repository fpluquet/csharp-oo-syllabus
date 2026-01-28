# Passage de Paramètres

Quand vous appelez une méthode avec des paramètres, vous vous êtes peut-être demandé : *"Pourquoi ma variable n'a-t-elle pas changé après l'appel ?"*. Ce chapitre vous explique comment C# transmet les données aux méthodes et comment contrôler ce comportement.

::: tip 🎯 Ce que vous allez apprendre
- Comprendre la différence entre passage par valeur et par référence
- Utiliser `ref` pour modifier une variable existante
- Utiliser `out` pour retourner plusieurs valeurs
- Maîtriser les paramètres optionnels et nommés
- Utiliser `params` pour des arguments variables
:::

## Rappel : passage par valeur (défaut)

En C#, le **passage par valeur** est le comportement par défaut. Lors d'un appel de méthode, une **copie** de la valeur est transmise. La variable originale n'est pas modifiée.

### 📬 Analogie : la photocopie

Imaginez que vous avez un document important. Quand vous le donnez à quelqu'un pour qu'il travaille dessus :

| Situation | Passage par valeur | Passage par référence |
|-----------|-------------------|----------------------|
| **Ce que vous donnez** | Une **photocopie** du document | Le **document original** |
| **Si la personne écrit dessus** | Votre original reste intact | Votre original est modifié |
| **Usage typique** | Partager une information | Permettre la modification |

```
┌─────────────────────────────────────────────────────────────────────┐
│                     PASSAGE PAR VALEUR (défaut)                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│    Appelant                           Méthode                       │
│    ┌───────────┐                      ┌───────────┐                 │
│    │  x = 10   │ ──── copie ────────► │ nombre=10 │                 │
│    │           │                      │ nombre=20 │  (modifié)      │
│    │  x = 10   │ ◄─── (rien) ───────  │           │                 │
│    └───────────┘                      └───────────┘                 │
│        ▲                                                            │
│        │                                                            │
│    INCHANGÉ !                                                       │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

```csharp
void Doubler(int nombre)
{
    nombre = nombre * 2;
    Console.WriteLine($"Dans la méthode: {nombre}");  // 20
}

int x = 10;
Doubler(x);
Console.WriteLine($"Après l'appel: {x}");  // 10 (inchangé)
```

::: warning ⚠️ Piège fréquent
Beaucoup de débutants s'attendent à ce que `x` soit modifié à 20. Ce n'est pas le cas car `nombre` est une **copie indépendante** de `x`. Modifier `nombre` n'affecte pas `x` !
:::

::: tip Rappel du cours précédent
Ce comportement a été vu dans le chapitre sur les fonctions du syllabus d'introduction. Ici, nous approfondissons les mécanismes de passage par référence.
:::

## Passage par référence avec `ref`

Le mot-clé `ref` permet de passer une variable **par référence**. La méthode travaille directement sur la variable originale.

### 📬 Analogie : donner l'original

Avec `ref`, vous ne donnez plus une photocopie, vous donnez **accès au document original**. Toute modification affecte directement votre document.

```
┌─────────────────────────────────────────────────────────────────────┐
│                     PASSAGE PAR RÉFÉRENCE (ref)                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│    Appelant                           Méthode                       │
│    ┌───────────┐                      ┌───────────┐                 │
│    │  x = 10   │ ═══ référence ═════► │ ref nombre│                 │
│    │     ↑     │                      │   │       │                 │
│    │     └─────────── même case ──────────┘       │                 │
│    │  x = 20   │ ◄═══════════════════ │ nombre=20 │                 │
│    └───────────┘                      └───────────┘                 │
│        ▲                                                            │
│        │                                                            │
│     MODIFIÉ !                                                       │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### Syntaxe

Le mot-clé `ref` doit apparaître **deux fois** : dans la déclaration ET à l'appel.

```csharp
void Doubler(ref int nombre)  // 1️⃣ ref dans la déclaration
{
    nombre = nombre * 2;
}

int x = 10;
Doubler(ref x);  // 2️⃣ ref à l'appel aussi !
Console.WriteLine(x);  // 20 (modifié !)
```

::: info 💡 Pourquoi `ref` à l'appel aussi ?
C'est une sécurité. En voyant `Doubler(ref x)`, le programmeur sait immédiatement que `x` pourrait être modifié. Sans ce `ref` visible, on pourrait être surpris par une modification inattendue.
:::

### Règles importantes

::: danger ⛔ Règle obligatoire
Avec `ref`, la variable **doit être initialisée** avant l'appel. La méthode s'attend à pouvoir lire une valeur existante.
:::

```csharp
void Modifier(ref int valeur)
{
    valeur = 100;
}

int a;
// Modifier(ref a);  // ❌ ERREUR : 'a' n'est pas initialisé

int b = 5;           // ✅ b est initialisé
Modifier(ref b);     // ✅ OK
Console.WriteLine(b);  // 100
```

| Règle | Explication |
|-------|-------------|
| Variable initialisée | La méthode peut lire la valeur avant de la modifier |
| `ref` partout | À la déclaration ET à chaque appel |
| Types identiques | Le type doit correspondre exactement (pas de conversion) |

### Cas d'usage typique : échanger deux valeurs

L'échange de deux valeurs est l'exemple classique où `ref` est indispensable. Sans `ref`, il est **impossible** de réaliser cette opération dans une méthode.

```csharp
void Echanger(ref int a, ref int b)
{
    int temp = a;  // 1. Sauvegarder a
    a = b;         // 2. a prend la valeur de b
    b = temp;      // 3. b prend l'ancienne valeur de a
}

int x = 5, y = 10;
Console.WriteLine($"Avant: x={x}, y={y}");  // x=5, y=10

Echanger(ref x, ref y);

Console.WriteLine($"Après: x={x}, y={y}");  // x=10, y=5
```

```
┌─────────────────────────────────────────────────────────────────────┐
│              Déroulement de Echanger(ref x, ref y)                  │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│   Avant:    x = 5      y = 10                                       │
│                                                                     │
│   Étape 1:  temp = a (= 5)                                          │
│   Étape 2:  a = b      →   x = 10                                   │
│   Étape 3:  b = temp   →   y = 5                                    │
│                                                                     │
│   Après:    x = 10     y = 5                                        │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

## Passage par référence avec `out`

Le mot-clé `out` est similaire à `ref`, mais avec une différence importante : la variable **n'a pas besoin d'être initialisée** avant l'appel, mais **doit être assignée** dans la méthode.

### 📦 Analogie : formulaire à remplir

Pensez à `out` comme un formulaire vide que vous donnez à quelqu'un :
- Vous donnez un **formulaire vide** (variable non initialisée)
- La personne **doit le remplir** (assignation obligatoire)
- Vous récupérez le formulaire **complété** (nouvelle valeur)

```
┌─────────────────────────────────────────────────────────────────────┐
│                        PARAMÈTRE OUT                                │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│    Appelant                              Méthode                    │
│    ┌───────────┐                        ┌────────────────────┐      │
│    │ q = ???   │ ─── (pas de valeur)──► │ out quotient       │      │
│    │ r = ???   │ ─── (pas de valeur)──► │ out reste          │      │
│    │           │                        │                    │      │
│    │           │                        │ quotient = 3  ✅   │      │
│    │           │                        │ reste = 2     ✅   │      │
│    │ q = 3     │ ◄═════════════════════ │                    │      │
│    │ r = 2     │ ◄═════════════════════ │                    │      │
│    └───────────┘                        └────────────────────┘      │
│                                                                     │
│    La méthode DOIT assigner chaque paramètre out !                  │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### Syntaxe

```csharp
void Diviser(int dividende, int diviseur, out int quotient, out int reste)
{
    quotient = dividende / diviseur;  // ✅ Obligatoire !
    reste = dividende % diviseur;     // ✅ Obligatoire !
}

int q, r;  // Pas besoin d'initialiser avec out
Diviser(17, 5, out q, out r);
Console.WriteLine($"17 ÷ 5 = {q} reste {r}");  // 17 ÷ 5 = 3 reste 2
```

::: danger ⛔ Règle obligatoire avec `out`
La méthode **doit assigner** tous les paramètres `out` avant de terminer. Si vous oubliez, le compilateur génère une erreur.
:::

```csharp
// ❌ ERREUR : quotient n'est pas assigné dans tous les chemins
void DiviserMauvais(int a, int b, out int quotient)
{
    if (b != 0)
    {
        quotient = a / b;
    }
    // Que se passe-t-il si b == 0 ? quotient non assigné !
}

// ✅ CORRECT : tous les chemins assignent quotient
void DiviserCorrect(int a, int b, out int quotient)
{
    if (b != 0)
    {
        quotient = a / b;
    }
    else
    {
        quotient = 0;  // Valeur par défaut
    }
}
```

### Déclaration inline (C# 7+)

Depuis C# 7, on peut déclarer la variable **directement lors de l'appel**. C'est plus compact et très pratique !

```csharp
// Avant C# 7 : déclaration séparée
int quotient, reste;
Diviser(17, 5, out quotient, out reste);

// ✅ C# 7+ : déclaration inline avec type
Diviser(17, 5, out int q, out int r);
Console.WriteLine($"Quotient: {q}, Reste: {r}");

// ✅ C# 7+ : encore plus compact avec var
Diviser(17, 5, out var q2, out var r2);
```

::: tip 💡 Ignorer une valeur avec `_`
Si vous n'avez pas besoin d'une valeur retournée, utilisez `_` (discard) :

```csharp
// On veut seulement le quotient, pas le reste
Diviser(17, 5, out int quotient, out _);  // Le reste est ignoré
```
:::

### Cas d'usage typique : le pattern TryParse

Le pattern `TryParse` est **omniprésent** en C#. Il permet de retourner à la fois :
- Un **booléen** : succès ou échec de l'opération
- Une **valeur** : le résultat de la conversion (via `out`)

::: info 🔍 Vous l'avez déjà utilisé !
`int.TryParse`, `double.TryParse`, `DateTime.TryParse`... tous ces méthodes utilisent le pattern `TryParse` avec `out`.
:::

```csharp
string input = "42";

// Le pattern TryParse classique
if (int.TryParse(input, out int resultat))
{
    Console.WriteLine($"Conversion réussie: {resultat}");  // 42
}
else
{
    Console.WriteLine("Échec de la conversion");
}
```

### Créer votre propre méthode TryParse

Vous pouvez appliquer ce pattern à vos propres conversions :

```csharp
bool TryParseCoordonnees(string texte, out double x, out double y)
{
    // Valeurs par défaut (obligatoire avec out)
    x = 0;
    y = 0;
    
    // Vérification du format
    string[] parties = texte.Split(',');
    if (parties.Length != 2)
        return false;  // Format invalide
    
    // Tentative de conversion des deux parties
    return double.TryParse(parties[0], out x) && 
           double.TryParse(parties[1], out y);
}

// Utilisation
if (TryParseCoordonnees("3.5,7.2", out var coordX, out var coordY))
{
    Console.WriteLine($"Point: ({coordX}, {coordY})");  // Point: (3.5, 7.2)
}
else
{
    Console.WriteLine("Coordonnées invalides");
}
```

::: tip 💡 Avantage du pattern TryParse
Ce pattern évite les exceptions ! Au lieu de faire un `try/catch` coûteux, on vérifie simplement le booléen retourné.
:::

## Comparaison `ref` vs `out`

Voici un tableau récapitulatif des différences essentielles :

| Aspect | `ref` | `out` |
|--------|-------|-------|
| **Initialisation avant appel** | ✅ **Requise** | ❌ Non requise |
| **Assignation dans la méthode** | ⚪ Optionnelle | ✅ **Requise** |
| **Usage typique** | Modifier une valeur existante | Retourner plusieurs valeurs |
| **Peut lire la valeur initiale** | ✅ Oui | ❌ Non (pas de sens) |
| **Analogie** | Donner l'original à modifier | Formulaire vide à remplir |

### Quand utiliser lequel ?

```csharp
// ✅ ref : modifier une valeur existante
void Incrementer(ref int compteur)
{
    compteur++;  // On LIT puis MODIFIE la valeur existante
}

// ✅ out : retourner une ou plusieurs nouvelles valeurs
void ObtenirDimensions(out int largeur, out int hauteur)
{
    largeur = 1920;   // On CRÉE de nouvelles valeurs
    hauteur = 1080;
}
```

::: warning ❓ Comment choisir ?
- Besoin de **lire ET modifier** la valeur ? → `ref`
- Besoin de **retourner plusieurs résultats** ? → `out`
- Besoin de **vérifier si une opération réussit** (TryParse) ? → `out`
:::

## Paramètre `in` (C# 7.2+)

Le mot-clé `in` passe une référence **en lecture seule**. C'est utile pour les grandes structures afin d'éviter la copie coûteuse tout en empêchant la modification.

### 🔒 Analogie : document en vitrine

Pensez à un document précieux exposé dans une vitrine de musée :
- Vous pouvez le **lire** à travers la vitre
- Vous ne pouvez **pas le modifier**
- Il n'y a **pas de copie** à manipuler

```csharp
struct GrandeStructure
{
    public double[] Donnees;  // Potentiellement des milliers de valeurs
}

void Analyser(in GrandeStructure data)
{
    // ✅ Lecture autorisée
    Console.WriteLine(data.Donnees.Length);
    
    // ❌ ERREUR de compilation : ne peut pas modifier
    // data.Donnees = new double[10];
}

GrandeStructure s = new GrandeStructure { Donnees = new double[1000] };
Analyser(in s);  // Pas de copie de 1000 doubles, mais modification impossible
```

::: info 📊 Performance
`in` est principalement utilisé pour des **grosses structures** (plusieurs dizaines d'octets). Pour les types simples (`int`, `double`), le passage par valeur est souvent plus efficace.
:::

## Tableau récapitulatif des modificateurs

| Modificateur | Initialisation avant | Modification dans méthode | Usage principal |
|--------------|---------------------|--------------------------|-----------------|
| *(aucun)* | ✅ Oui | ❌ Copie locale | Passer une valeur en lecture |
| `ref` | ✅ **Requise** | ✅ Autorisée | Modifier une variable existante |
| `out` | ❌ Non | ✅ **Requise** | Retourner plusieurs valeurs |
| `in` | ✅ Oui | ❌ Interdite | Performance (grosses structures) |

## Arguments nommés

C# permet de spécifier les arguments par leur nom, dans n'importe quel ordre. C'est particulièrement utile quand une méthode a beaucoup de paramètres.

### 🏷️ Analogie : étiquettes sur des cadeaux

Imaginez que vous envoyez des cadeaux. Vous pouvez soit :
- Les mettre **dans le bon ordre** dans le colis (positionnels)
- Coller une **étiquette** sur chacun indiquant à qui il est destiné (nommés)

```csharp
void CreerUtilisateur(string nom, string email, int age, bool actif)
{
    Console.WriteLine($"{nom}, {email}, {age} ans, actif={actif}");
}

// 1️⃣ Appel classique (ordre requis)
CreerUtilisateur("Alice", "alice@email.com", 25, true);

// 2️⃣ Arguments nommés (ordre libre !)
CreerUtilisateur(
    email: "bob@email.com",
    actif: false,
    nom: "Bob",
    age: 30
);

// 3️⃣ Mixte : positionnels d'abord, puis nommés
CreerUtilisateur("Charlie", "charlie@email.com", age: 22, actif: true);
```

::: tip ✅ Avantages des arguments nommés
| Avantage | Explication |
|----------|-------------|
| **Lisibilité** | Le code est auto-documenté : on sait ce que représente chaque valeur |
| **Flexibilité** | L'ordre n'a plus d'importance |
| **Sécurité** | Évite les erreurs de placement (ex: inverser deux `int`) |
| **Sauter des optionnels** | Permet de fournir seulement certains paramètres optionnels |
:::

## Paramètres optionnels

On peut définir des **valeurs par défaut** pour les paramètres. Si l'appelant ne fournit pas ces paramètres, les valeurs par défaut sont utilisées.

### 🍕 Analogie : commande de pizza

Quand vous commandez une pizza :
- La **taille** est obligatoire (quel format ?)
- Le **supplément fromage** est optionnel (par défaut : non)
- La **livraison** est optionnelle (par défaut : sur place)

```csharp
void AfficherMessage(string message, string prefixe = "INFO", bool majuscules = false)
{
    string resultat = majuscules ? message.ToUpper() : message;
    Console.WriteLine($"[{prefixe}] {resultat}");
}

// Différentes façons d'appeler
AfficherMessage("Démarrage");                           // [INFO] Démarrage
AfficherMessage("Attention!", "WARN");                  // [WARN] Attention!
AfficherMessage("erreur critique", "ERROR", true);      // [ERROR] ERREUR CRITIQUE

// 💡 Avec arguments nommés pour sauter des paramètres
AfficherMessage("test", majuscules: true);              // [INFO] TEST
```

::: info 💡 Combiner optionnels et nommés
Les arguments nommés brillent avec les paramètres optionnels. Ils permettent de fournir **uniquement les paramètres souhaités**, dans n'importe quel ordre.
:::

### Règles des paramètres optionnels

::: danger ⛔ Règles à respecter
1. Les paramètres optionnels doivent être **à la fin** de la liste
2. La valeur par défaut doit être une **constante** (connue à la compilation)
:::

```csharp
// ✅ Correct : optionnels à la fin
void Exemple(int obligatoire, string optionnel1 = "défaut", int optionnel2 = 0)
{
    // ...
}

// ❌ Incorrect : optionnel avant obligatoire
// void Exemple(string optionnel = "défaut", int obligatoire)  // ERREUR !

// ❌ Incorrect : valeur par défaut non constante
// void Exemple(DateTime date = DateTime.Now)  // ERREUR ! Now n'est pas constant
```

### Tableau des valeurs par défaut autorisées

| Type | Valeurs autorisées | Exemple |
|------|-------------------|---------|
| Types numériques | Littéraux | `int x = 0`, `double y = 3.14` |
| `string` | `null` ou littéral | `string s = "defaut"`, `string s = null` |
| `bool` | `true` ou `false` | `bool b = false` |
| Énumérations | Valeurs de l'enum | `Jour j = Jour.Lundi` |
| Références | Uniquement `null` | `object o = null` |

## Le mot-clé `params`

Le mot-clé `params` permet de passer un **nombre variable d'arguments**. C'est très pratique pour créer des méthodes flexibles.

### 🎒 Analogie : un sac sans limite

Imaginez un sac magique qui peut contenir autant d'objets que vous voulez :
- Vous pouvez y mettre **0, 1, 2, 10, ou 100** objets
- La méthode reçoit tout dans un **tableau**
- L'appelant n'a pas besoin de créer le tableau explicitement

```csharp
int Somme(params int[] nombres)
{
    int total = 0;
    foreach (int n in nombres)
    {
        total += n;
    }
    return total;
}

// 💡 On peut appeler avec n'importe quel nombre d'arguments
Console.WriteLine(Somme());                // 0        (aucun argument)
Console.WriteLine(Somme(1));               // 1        (un seul)
Console.WriteLine(Somme(1, 2, 3));         // 6        (trois arguments)
Console.WriteLine(Somme(1, 2, 3, 4, 5));   // 15       (cinq arguments)

// On peut aussi passer un tableau directement
int[] valeurs = { 10, 20, 30 };
Console.WriteLine(Somme(valeurs));         // 60
```

::: info 🔍 Vous l'avez déjà utilisé !
`Console.WriteLine` utilise `params` en interne :
```csharp
Console.WriteLine("{0} + {1} = {2}", 5, 3, 8);  // Nombre variable d'arguments !
```
:::

### Exemple pratique : formatage personnalisé

```csharp
string Formater(string template, params object[] valeurs)
{
    return string.Format(template, valeurs);
}

// Utilisation flexible
Console.WriteLine(Formater("Bonjour {0}!", "Alice"));           // Bonjour Alice!
Console.WriteLine(Formater("{0} + {1} = {2}", 5, 3, 8));        // 5 + 3 = 8
Console.WriteLine(Formater("{0} a {1} ans et habite à {2}", "Bob", 25, "Mons"));
```

### Règles de `params`

::: danger ⛔ Règles à respecter
| Règle | Explication |
|-------|-------------|
| **Dernier paramètre** | `params` doit être le dernier de la liste |
| **Un seul `params`** | Une méthode ne peut avoir qu'un seul paramètre `params` |
| **Tableau uniquement** | Le type doit être un tableau unidimensionnel |
:::

```csharp
// ✅ Correct
void Methode(string titre, params int[] nombres) { }

// ❌ Incorrect : params pas en dernier
// void Methode(params int[] nombres, string titre) { }

// ❌ Incorrect : deux params
// void Methode(params int[] a, params string[] b) { }
```

## Combinaison des modificateurs

On peut combiner différents types de paramètres :

```csharp
class Calculatrice
{
    public static bool TryDiviser(
        int dividende,              // Obligatoire, par valeur
        int diviseur,               // Obligatoire, par valeur
        out int quotient,           // Retour par out
        out int reste,              // Retour par out
        bool arrondir = false)      // Optionnel avec défaut
    {
        quotient = 0;
        reste = 0;
        
        if (diviseur == 0)
            return false;
        
        quotient = dividende / diviseur;
        reste = dividende % diviseur;
        
        return true;
    }
}

// Utilisation
if (Calculatrice.TryDiviser(17, 5, out var q, out var r))
{
    Console.WriteLine($"17 ÷ 5 = {q} reste {r}");
}
```

## Exemple complet : Statistiques

```csharp
class Statistiques
{
    public static void Calculer(
        int[] donnees,
        out double moyenne,
        out double min,
        out double max,
        out double ecartType)
    {
        if (donnees == null || donnees.Length == 0)
        {
            moyenne = min = max = ecartType = 0;
            return;
        }
        
        // Calcul moyenne
        double somme = 0;
        min = double.MaxValue;
        max = double.MinValue;
        
        foreach (int valeur in donnees)
        {
            somme += valeur;
            if (valeur < min) min = valeur;
            if (valeur > max) max = valeur;
        }
        
        moyenne = somme / donnees.Length;
        
        // Calcul écart-type
        double sommeCarres = 0;
        foreach (int valeur in donnees)
        {
            double diff = valeur - moyenne;
            sommeCarres += diff * diff;
        }
        ecartType = Math.Sqrt(sommeCarres / donnees.Length);
    }
    
    public static void Afficher(
        string titre,
        params int[] valeurs)
    {
        Calculer(valeurs, 
            out double moy, 
            out double min, 
            out double max, 
            out double ecart);
        
        Console.WriteLine($"=== {titre} ===");
        Console.WriteLine($"  Valeurs: [{string.Join(", ", valeurs)}]");
        Console.WriteLine($"  Moyenne: {moy:F2}");
        Console.WriteLine($"  Min: {min}, Max: {max}");
        Console.WriteLine($"  Écart-type: {ecart:F2}");
    }
}
```

```csharp
// Utilisation
Statistiques.Afficher("Notes de l'examen", 12, 15, 8, 17, 14, 11);
Statistiques.Afficher("Températures", 22, 25, 19, 28, 31, 24, 20);
```

## Exercices

### Exercice 1 : Fonction ModifierTableau

Créez une méthode qui prend un tableau en `ref` et le modifie :
- `void TrierEtFiltrer(ref int[] tableau, int seuilMin)` 
- Trie le tableau et ne garde que les valeurs >= seuilMin

::: details 💡 Solution Exercice 1

```csharp
void TrierEtFiltrer(ref int[] tableau, int seuilMin)
{
    // 1. Trier le tableau
    Array.Sort(tableau);
    
    // 2. Compter les éléments à garder
    int count = 0;
    foreach (int val in tableau)
    {
        if (val >= seuilMin)
            count++;
    }
    
    // 3. Créer un nouveau tableau avec les éléments filtrés
    int[] resultat = new int[count];
    int index = 0;
    foreach (int val in tableau)
    {
        if (val >= seuilMin)
        {
            resultat[index] = val;
            index++;
        }
    }
    
    // 4. Remplacer le tableau original
    tableau = resultat;
}

// Test
int[] nombres = { 5, 2, 8, 1, 9, 3, 7 };
Console.WriteLine($"Avant: [{string.Join(", ", nombres)}]");  // [5, 2, 8, 1, 9, 3, 7]

TrierEtFiltrer(ref nombres, 5);
Console.WriteLine($"Après: [{string.Join(", ", nombres)}]");  // [5, 7, 8, 9]
```

**Explication** : On utilise `ref` car on veut remplacer **le tableau lui-même** par un nouveau tableau. Sans `ref`, le tableau original ne serait pas modifié.
:::

### Exercice 2 : Parser de configuration

Créez une méthode `TryParseConfig` qui :
- Prend une ligne de configuration "clé=valeur"
- Retourne `true` si le format est valide
- Retourne la clé et la valeur via des paramètres `out`

::: details 💡 Solution Exercice 2

```csharp
bool TryParseConfig(string ligne, out string cle, out string valeur)
{
    // Valeurs par défaut (obligatoire avec out)
    cle = "";
    valeur = "";
    
    // Vérification : la ligne ne doit pas être vide
    if (string.IsNullOrWhiteSpace(ligne))
        return false;
    
    // Chercher le séparateur '='
    int indexEgal = ligne.IndexOf('=');
    
    // Vérification : le '=' doit exister et ne pas être au début
    if (indexEgal <= 0)
        return false;
    
    // Extraire la clé et la valeur
    cle = ligne.Substring(0, indexEgal).Trim();
    valeur = ligne.Substring(indexEgal + 1).Trim();
    
    // Vérification : la clé ne doit pas être vide
    return !string.IsNullOrWhiteSpace(cle);
}

// Tests
string[] lignes = {
    "serveur=localhost",
    "port=8080",
    "debug=true",
    "invalide",          // Pas de '='
    "=sansClé",          // Clé vide
    "",                  // Ligne vide
    "  base_de_donnees = ma_bdd  "  // Avec espaces
};

foreach (string ligne in lignes)
{
    if (TryParseConfig(ligne, out var cle, out var valeur))
    {
        Console.WriteLine($"✅ Clé: '{cle}' → Valeur: '{valeur}'");
    }
    else
    {
        Console.WriteLine($"❌ Ligne invalide: '{ligne}'");
    }
}
```

**Sortie attendue** :
```
✅ Clé: 'serveur' → Valeur: 'localhost'
✅ Clé: 'port' → Valeur: '8080'
✅ Clé: 'debug' → Valeur: 'true'
❌ Ligne invalide: 'invalide'
❌ Ligne invalide: '=sansClé'
❌ Ligne invalide: ''
✅ Clé: 'base_de_donnees' → Valeur: 'ma_bdd'
```
:::

### Exercice 3 : Calculer min, max et moyenne

Créez une méthode qui calcule les statistiques d'un ensemble de nombres :

```csharp
void CalculerStats(out double min, out double max, out double moyenne, params double[] nombres)
```

::: details 💡 Solution Exercice 3

```csharp
void CalculerStats(out double min, out double max, out double moyenne, params double[] nombres)
{
    // Cas particulier : aucun nombre
    if (nombres == null || nombres.Length == 0)
    {
        min = 0;
        max = 0;
        moyenne = 0;
        return;
    }
    
    // Initialisation avec la première valeur
    min = nombres[0];
    max = nombres[0];
    double somme = 0;
    
    // Parcours des nombres
    foreach (double n in nombres)
    {
        if (n < min) min = n;
        if (n > max) max = n;
        somme += n;
    }
    
    moyenne = somme / nombres.Length;
}

// Test
CalculerStats(out var minimum, out var maximum, out var moy, 10, 5, 23, 7, 15);

Console.WriteLine($"Min: {minimum}");      // Min: 5
Console.WriteLine($"Max: {maximum}");      // Max: 23
Console.WriteLine($"Moyenne: {moy:F2}");   // Moyenne: 12.00
```
:::

### Exercice 4 : Formateur de texte avancé

Créez une méthode de formatage avec des options :

```csharp
string FormaterTexte(
    string texte,
    bool majuscules = false,
    string prefixe = "",
    string suffixe = "",
    int repeter = 1)
```

::: details 💡 Solution Exercice 4

```csharp
string FormaterTexte(
    string texte,
    bool majuscules = false,
    string prefixe = "",
    string suffixe = "",
    int repeter = 1)
{
    // Appliquer les transformations
    string resultat = texte;
    
    if (majuscules)
        resultat = resultat.ToUpper();
    
    resultat = prefixe + resultat + suffixe;
    
    // Répéter si demandé
    if (repeter > 1)
    {
        string original = resultat;
        for (int i = 1; i < repeter; i++)
        {
            resultat += " " + original;
        }
    }
    
    return resultat;
}

// Tests avec différentes combinaisons
Console.WriteLine(FormaterTexte("hello"));
// hello

Console.WriteLine(FormaterTexte("hello", majuscules: true));
// HELLO

Console.WriteLine(FormaterTexte("erreur", prefixe: "[ERREUR] ", majuscules: true));
// [ERREUR] ERREUR

Console.WriteLine(FormaterTexte("tic", suffixe: " tac", repeter: 3));
// tic tac tic tac tic tac

Console.WriteLine(FormaterTexte("test", prefixe: "<<", suffixe: ">>", majuscules: true));
// <<TEST>>
```

**Remarque** : Grâce aux arguments nommés, on peut fournir uniquement les paramètres souhaités, dans n'importe quel ordre !
:::

## Résumé

### Modificateurs de paramètres

| Modificateur | Initialisation avant | Assignation dans méthode | Usage |
|--------------|---------------------|--------------------------|-------|
| *(aucun)* | ✅ Oui | ❌ Non (copie locale) | Lecture seule |
| `ref` | ✅ **Requise** | ⚪ Optionnelle | Modifier une valeur existante |
| `out` | ❌ Non | ✅ **Requise** | Retourner plusieurs valeurs |
| `in` | ✅ Oui | ❌ Interdite | Grosse structure en lecture |

### Fonctionnalités avancées

| Fonctionnalité | Syntaxe | Description |
|----------------|---------|-------------|
| **Arguments nommés** | `methode(param: valeur)` | Ordre libre, code auto-documenté |
| **Paramètres optionnels** | `void M(int x = 0)` | Valeur par défaut si non fourni |
| **`params`** | `void M(params int[] x)` | Nombre variable d'arguments |

::: tip 🎯 Points clés à retenir
1. **Par défaut** = passage par valeur (copie)
2. **`ref`** = modifier une variable existante (doit être initialisée)
3. **`out`** = retourner plusieurs valeurs (doit être assignée)
4. **Arguments nommés** = lisibilité et flexibilité
5. **`params`** = nombre variable d'arguments
:::

### Erreurs courantes à éviter

| Erreur | Problème | Solution |
|--------|----------|----------|
| Oublier `ref` à l'appel | La variable n'est pas modifiée | Toujours écrire `ref` partout |
| Variable non initialisée avec `ref` | Erreur de compilation | Initialiser avant l'appel |
| Oublier d'assigner avec `out` | Erreur de compilation | Assigner dans tous les chemins |
| Optionnel avant obligatoire | Erreur de compilation | Optionnels toujours à la fin |
| `params` pas en dernier | Erreur de compilation | `params` toujours dernier |
