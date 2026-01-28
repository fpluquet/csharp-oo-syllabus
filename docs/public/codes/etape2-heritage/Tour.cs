namespace Echecs;

public class Tour : Piece
{
    public Tour(Couleur couleur, int ligne, int colonne) 
        : base(couleur, ligne, colonne) { }

    public override char Symbole => Couleur == Couleur.Blanc ? '♖' : '♜';
    public override string Nom => "Tour";

    public override bool PeutSeDeplacer(int versLigne, int versColonne)
    {
        if (!EstDansLimites(versLigne, versColonne) || EstMemeCase(versLigne, versColonne))
            return false;

        // Horizontale ou verticale uniquement
        return Ligne == versLigne || Colonne == versColonne;
    }
}
