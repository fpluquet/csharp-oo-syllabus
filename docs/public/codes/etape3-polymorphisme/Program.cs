using Echecs;

Console.WriteLine("=== Jeu d'Échecs - Étape 3 : Polymorphisme ===\n");

// Créer et afficher le plateau
Plateau plateau = new();
plateau.Afficher();

// Démonstration du polymorphisme avec les valeurs
Console.WriteLine("--- Valeurs des pièces (polymorphisme) ---\n");

List<Piece> pieces =
[
    new Roi(Couleur.Blanc, 0, 4),
    new Dame(Couleur.Blanc, 0, 3),
    new Tour(Couleur.Blanc, 0, 0),
    new Fou(Couleur.Blanc, 0, 2),
    new Cavalier(Couleur.Blanc, 0, 1),
    new Pion(Couleur.Blanc, 1, 0)
];

foreach (Piece p in pieces)
{
    // Appel polymorphe : p.Valeur utilise le type réel
    Console.WriteLine($"  {p.Symbole} {p.Nom,-10} : {p.Valeur} points");
}

// Calcul du score polymorphe
Console.WriteLine("\n--- Calcul des scores ---\n");

int scoreBlancs = plateau.CalculerScore(Couleur.Blanc);
int scoreNoirs = plateau.CalculerScore(Couleur.Noir);

Console.WriteLine($"  Score des Blancs : {scoreBlancs} points");
Console.WriteLine($"  Score des Noirs  : {scoreNoirs} points");

Console.WriteLine("\n--- Type déclaré vs Type réel ---\n");

Piece maPiece = new Dame(Couleur.Noir, 7, 3);  // Déclaré: Piece, Réel: Dame

Console.WriteLine($"  Type déclaré : Piece");
Console.WriteLine($"  Type réel    : {maPiece.GetType().Name}");
Console.WriteLine($"  Nom          : {maPiece.Nom}");      // "Dame" (polymorphe)
Console.WriteLine($"  Valeur       : {maPiece.Valeur}");   // 9 (polymorphe)
