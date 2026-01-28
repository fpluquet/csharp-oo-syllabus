namespace Echecs.Models.Pieces;

using Echecs.Models.Enums;

public class Roi : Piece
{
    public Roi(Couleur couleur, int ligne, int colonne) 
        : base(couleur, ligne, colonne) { }

    public override char Symbole => Couleur == Couleur.Blanc ? '♔' : '♚';
    public override string Nom => "Roi";
    public override int Valeur => 0;

    public override bool PeutSeDeplacer(int versLigne, int versColonne)
    {
        if (!EstDansLimites(versLigne, versColonne) || EstMemeCase(versLigne, versColonne))
            return false;

        int deltaLigne = Math.Abs(versLigne - Ligne);
        int deltaColonne = Math.Abs(versColonne - Colonne);

        return deltaLigne <= 1 && deltaColonne <= 1;
    }
}
