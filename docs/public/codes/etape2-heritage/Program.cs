using Echecs;

Console.WriteLine("=== Jeu d'Échecs - Étape 2 : Héritage ===\n");

// Créer et afficher le plateau
Plateau plateau = new();
plateau.Afficher();

// Démonstration de l'héritage
Console.WriteLine("--- Démonstration de l'héritage ---\n");

// Créer différentes pièces
Piece roi = new Roi(Couleur.Blanc, 0, 4);
Piece dame = new Dame(Couleur.Noir, 7, 3);
Piece cavalier = new Cavalier(Couleur.Blanc, 0, 1);

// Toutes sont des Piece, mais chacune a son comportement
Console.WriteLine($"{roi.Nom} ({roi.Symbole}) - Peut aller en (1,4) ? {roi.PeutSeDeplacer(1, 4)}");
Console.WriteLine($"{dame.Nom} ({dame.Symbole}) - Peut aller en (0,3) ? {dame.PeutSeDeplacer(0, 3)}");
Console.WriteLine($"{cavalier.Nom} ({cavalier.Symbole}) - Peut aller en (2,2) ? {cavalier.PeutSeDeplacer(2, 2)}");

Console.WriteLine("\n--- Collection hétérogène ---\n");

// On peut stocker différentes pièces dans une même liste
List<Piece> pieces = [roi, dame, cavalier];

foreach (Piece p in pieces)
{
    Console.WriteLine($"  {p}");
}
