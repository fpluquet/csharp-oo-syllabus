namespace Echecs;

public class Cavalier : Piece
{
    public Cavalier(Couleur couleur, int ligne, int colonne) 
        : base(couleur, ligne, colonne) { }

    public override char Symbole => Couleur == Couleur.Blanc ? '♘' : '♞';
    public override string Nom => "Cavalier";

    public override bool PeutSeDeplacer(int versLigne, int versColonne)
    {
        if (!EstDansLimites(versLigne, versColonne) || EstMemeCase(versLigne, versColonne))
            return false;

        int deltaLigne = Math.Abs(versLigne - Ligne);
        int deltaColonne = Math.Abs(versColonne - Colonne);

        // Mouvement en L
        return (deltaLigne == 2 && deltaColonne == 1) ||
               (deltaLigne == 1 && deltaColonne == 2);
    }
}
