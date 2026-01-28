namespace Echecs;

public class Dame : Piece
{
    public Dame(Couleur couleur, int ligne, int colonne) 
        : base(couleur, ligne, colonne) { }

    public override char Symbole => Couleur == Couleur.Blanc ? '♕' : '♛';
    public override string Nom => "Dame";
    public override int Valeur => 9;

    public override bool PeutSeDeplacer(int versLigne, int versColonne)
    {
        if (!EstDansLimites(versLigne, versColonne) || EstMemeCase(versLigne, versColonne))
            return false;

        int deltaLigne = Math.Abs(versLigne - Ligne);
        int deltaColonne = Math.Abs(versColonne - Colonne);

        bool commeTour = Ligne == versLigne || Colonne == versColonne;
        bool commeFou = deltaLigne == deltaColonne;

        return commeTour || commeFou;
    }
}
