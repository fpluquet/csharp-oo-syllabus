using Echecs;

Console.WriteLine("=== Jeu d'Échecs - Étape 1 : Encapsulation ===\n");

// Créer et afficher le plateau
Plateau plateau = new();
plateau.Afficher();

// Tester quelques déplacements
Console.WriteLine("--- Tests de déplacements ---\n");

Piece? cavalier = plateau.GetPiece(0, 1);
if (cavalier != null)
{
    Console.WriteLine($"Pièce : {cavalier}");
    Console.WriteLine($"  Peut aller en (2,2) ? {cavalier.PeutSeDeplacer(2, 2)}"); // true - mouvement en L
    Console.WriteLine($"  Peut aller en (2,1) ? {cavalier.PeutSeDeplacer(2, 1)}"); // false - pas un L
}

Console.WriteLine();

Piece? pion = plateau.GetPiece(1, 4);
if (pion != null)
{
    Console.WriteLine($"Pièce : {pion}");
    Console.WriteLine($"  Peut aller en (2,4) ? {pion.PeutSeDeplacer(2, 4)}"); // true - 1 case
    Console.WriteLine($"  Peut aller en (3,4) ? {pion.PeutSeDeplacer(3, 4)}"); // true - 2 cases depuis départ
    Console.WriteLine($"  Peut aller en (4,4) ? {pion.PeutSeDeplacer(4, 4)}"); // false - trop loin
}

Console.WriteLine();

Piece? roi = plateau.GetPiece(0, 4);
if (roi != null)
{
    Console.WriteLine($"Pièce : {roi}");
    Console.WriteLine($"  Peut aller en (1,4) ? {roi.PeutSeDeplacer(1, 4)}"); // true
    Console.WriteLine($"  Peut aller en (1,5) ? {roi.PeutSeDeplacer(1, 5)}"); // true - diagonale
    Console.WriteLine($"  Peut aller en (2,4) ? {roi.PeutSeDeplacer(2, 4)}"); // false - 2 cases
}
