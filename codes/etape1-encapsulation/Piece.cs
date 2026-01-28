namespace Echecs;

public class Piece
{
    public TypePiece Type { get; }
    public Couleur Couleur { get; }
    public int Ligne { get; private set; }
    public int Colonne { get; private set; }

    public Piece(TypePiece type, Couleur couleur, int ligne, int colonne)
    {
        Type = type;
        Couleur = couleur;
        Ligne = ligne;
        Colonne = colonne;
    }

    public char Symbole => (Type, Couleur) switch
    {
        (TypePiece.Roi, Couleur.Blanc) => '♔',
        (TypePiece.Dame, Couleur.Blanc) => '♕',
        (TypePiece.Tour, Couleur.Blanc) => '♖',
        (TypePiece.Fou, Couleur.Blanc) => '♗',
        (TypePiece.Cavalier, Couleur.Blanc) => '♘',
        (TypePiece.Pion, Couleur.Blanc) => '♙',
        (TypePiece.Roi, Couleur.Noir) => '♚',
        (TypePiece.Dame, Couleur.Noir) => '♛',
        (TypePiece.Tour, Couleur.Noir) => '♜',
        (TypePiece.Fou, Couleur.Noir) => '♝',
        (TypePiece.Cavalier, Couleur.Noir) => '♞',
        (TypePiece.Pion, Couleur.Noir) => '♟',
        _ => '?'
    };

    public bool PeutSeDeplacer(int versLigne, int versColonne)
    {
        if (versLigne < 0 || versLigne > 7 || versColonne < 0 || versColonne > 7)
            return false;

        if (versLigne == Ligne && versColonne == Colonne)
            return false;

        int deltaLigne = Math.Abs(versLigne - Ligne);
        int deltaColonne = Math.Abs(versColonne - Colonne);

        return Type switch
        {
            TypePiece.Roi => deltaLigne <= 1 && deltaColonne <= 1,
            
            TypePiece.Dame => Ligne == versLigne || 
                              Colonne == versColonne || 
                              deltaLigne == deltaColonne,
            
            TypePiece.Tour => Ligne == versLigne || Colonne == versColonne,
            
            TypePiece.Fou => deltaLigne == deltaColonne,
            
            TypePiece.Cavalier => (deltaLigne == 2 && deltaColonne == 1) ||
                                   (deltaLigne == 1 && deltaColonne == 2),
            
            TypePiece.Pion => PeutDeplacerPion(versLigne, versColonne, deltaLigne, deltaColonne),
            
            _ => false
        };
    }

    private bool PeutDeplacerPion(int versLigne, int versColonne, int deltaLigne, int deltaColonne)
    {
        if (deltaColonne != 0) return false;

        int direction = Couleur == Couleur.Blanc ? 1 : -1;
        int ligneDepart = Couleur == Couleur.Blanc ? 1 : 6;

        if (versLigne - Ligne == direction)
            return true;

        if (Ligne == ligneDepart && versLigne - Ligne == 2 * direction)
            return true;

        return false;
    }

    public void Deplacer(int nouvelleLigne, int nouvelleColonne)
    {
        Ligne = nouvelleLigne;
        Colonne = nouvelleColonne;
    }

    public override string ToString() => $"{Couleur} {Type} en ({Ligne}, {Colonne})";
}
