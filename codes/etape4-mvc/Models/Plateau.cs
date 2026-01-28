namespace Echecs.Models;

using Echecs.Models.Enums;
using Echecs.Models.Pieces;

public class Plateau
{
    private readonly Piece?[,] _cases = new Piece?[8, 8];

    public Plateau()
    {
        Initialiser();
    }

    public Piece? this[int ligne, int colonne]
    {
        get => EstDansLimites(ligne, colonne) ? _cases[ligne, colonne] : null;
    }

    public static bool EstDansLimites(int ligne, int colonne)
        => ligne >= 0 && ligne < 8 && colonne >= 0 && colonne < 8;

    public void Initialiser()
    {
        Array.Clear(_cases);
        PlacerRangeeArriere(0, Couleur.Blanc);
        PlacerPions(1, Couleur.Blanc);
        PlacerPions(6, Couleur.Noir);
        PlacerRangeeArriere(7, Couleur.Noir);
    }

    public void EffectuerDeplacement(int deLigne, int deColonne, 
                                      int versLigne, int versColonne)
    {
        Piece? piece = _cases[deLigne, deColonne];
        if (piece == null) return;

        _cases[versLigne, versColonne] = piece;
        _cases[deLigne, deColonne] = null;
        piece.Deplacer(versLigne, versColonne);
    }

    public int CalculerScore(Couleur couleur)
    {
        int score = 0;
        for (int ligne = 0; ligne < 8; ligne++)
        {
            for (int col = 0; col < 8; col++)
            {
                Piece? piece = _cases[ligne, col];
                if (piece != null && piece.Couleur == couleur)
                {
                    score += piece.Valeur;
                }
            }
        }
        return score;
    }

    private void PlacerRangeeArriere(int ligne, Couleur couleur)
    {
        _cases[ligne, 0] = new Tour(couleur, ligne, 0);
        _cases[ligne, 1] = new Cavalier(couleur, ligne, 1);
        _cases[ligne, 2] = new Fou(couleur, ligne, 2);
        _cases[ligne, 3] = new Dame(couleur, ligne, 3);
        _cases[ligne, 4] = new Roi(couleur, ligne, 4);
        _cases[ligne, 5] = new Fou(couleur, ligne, 5);
        _cases[ligne, 6] = new Cavalier(couleur, ligne, 6);
        _cases[ligne, 7] = new Tour(couleur, ligne, 7);
    }

    private void PlacerPions(int ligne, Couleur couleur)
    {
        for (int col = 0; col < 8; col++)
        {
            _cases[ligne, col] = new Pion(couleur, ligne, col);
        }
    }
}
