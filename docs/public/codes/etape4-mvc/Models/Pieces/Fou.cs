namespace Echecs.Models.Pieces;

using Echecs.Models.Enums;

public class Fou : Piece
{
    public Fou(Couleur couleur, int ligne, int colonne) 
        : base(couleur, ligne, colonne) { }

    public override char Symbole => Couleur == Couleur.Blanc ? '♗' : '♝';
    public override string Nom => "Fou";
    public override int Valeur => 3;

    public override bool PeutSeDeplacer(int versLigne, int versColonne)
    {
        if (!EstDansLimites(versLigne, versColonne) || EstMemeCase(versLigne, versColonne))
            return false;

        int deltaLigne = Math.Abs(versLigne - Ligne);
        int deltaColonne = Math.Abs(versColonne - Colonne);

        return deltaLigne == deltaColonne;
    }
}
