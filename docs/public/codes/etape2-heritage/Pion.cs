namespace Echecs;

public class Pion : Piece
{
    public Pion(Couleur couleur, int ligne, int colonne) 
        : base(couleur, ligne, colonne) { }

    public override char Symbole => Couleur == Couleur.Blanc ? '♙' : '♟';
    public override string Nom => "Pion";

    public override bool PeutSeDeplacer(int versLigne, int versColonne)
    {
        if (!EstDansLimites(versLigne, versColonne) || EstMemeCase(versLigne, versColonne))
            return false;

        int deltaColonne = Math.Abs(versColonne - Colonne);
        if (deltaColonne != 0) return false;

        int direction = Couleur == Couleur.Blanc ? 1 : -1;
        int ligneDepart = Couleur == Couleur.Blanc ? 1 : 6;

        // Avancer d'une case
        if (versLigne - Ligne == direction)
            return true;

        // Avancer de deux cases depuis la position de départ
        if (Ligne == ligneDepart && versLigne - Ligne == 2 * direction)
            return true;

        return false;
    }
}
