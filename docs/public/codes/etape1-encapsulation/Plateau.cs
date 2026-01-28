namespace Echecs;

public class Plateau
{
    private readonly Piece?[,] _cases = new Piece?[8, 8];

    public Plateau()
    {
        Initialiser();
    }

    public void Initialiser()
    {
        Array.Clear(_cases);
        
        // Pièces blanches (lignes 0 et 1)
        PlacerRangeeArriere(0, Couleur.Blanc);
        PlacerPions(1, Couleur.Blanc);

        // Pièces noires (lignes 6 et 7)
        PlacerPions(6, Couleur.Noir);
        PlacerRangeeArriere(7, Couleur.Noir);
    }

    private void PlacerRangeeArriere(int ligne, Couleur couleur)
    {
        _cases[ligne, 0] = new Piece(TypePiece.Tour, couleur, ligne, 0);
        _cases[ligne, 1] = new Piece(TypePiece.Cavalier, couleur, ligne, 1);
        _cases[ligne, 2] = new Piece(TypePiece.Fou, couleur, ligne, 2);
        _cases[ligne, 3] = new Piece(TypePiece.Dame, couleur, ligne, 3);
        _cases[ligne, 4] = new Piece(TypePiece.Roi, couleur, ligne, 4);
        _cases[ligne, 5] = new Piece(TypePiece.Fou, couleur, ligne, 5);
        _cases[ligne, 6] = new Piece(TypePiece.Cavalier, couleur, ligne, 6);
        _cases[ligne, 7] = new Piece(TypePiece.Tour, couleur, ligne, 7);
    }

    private void PlacerPions(int ligne, Couleur couleur)
    {
        for (int col = 0; col < 8; col++)
        {
            _cases[ligne, col] = new Piece(TypePiece.Pion, couleur, ligne, col);
        }
    }

    public Piece? GetPiece(int ligne, int colonne)
    {
        if (ligne < 0 || ligne > 7 || colonne < 0 || colonne > 7)
            return null;
        return _cases[ligne, colonne];
    }

    public void Afficher()
    {
        Console.WriteLine();
        Console.WriteLine("    a   b   c   d   e   f   g   h");
        Console.WriteLine("  ┌───┬───┬───┬───┬───┬───┬───┬───┐");

        for (int ligne = 7; ligne >= 0; ligne--)
        {
            Console.Write($"{ligne + 1} │");

            for (int col = 0; col < 8; col++)
            {
                Piece? piece = _cases[ligne, col];
                if (piece != null)
                {
                    Console.Write($" {piece.Symbole} ");
                }
                else
                {
                    Console.Write("   ");
                }
                Console.Write("│");
            }

            Console.WriteLine($" {ligne + 1}");

            if (ligne > 0)
            {
                Console.WriteLine("  ├───┼───┼───┼───┼───┼───┼───┼───┤");
            }
        }

        Console.WriteLine("  └───┴───┴───┴───┴───┴───┴───┴───┘");
        Console.WriteLine("    a   b   c   d   e   f   g   h");
        Console.WriteLine();
    }
}
