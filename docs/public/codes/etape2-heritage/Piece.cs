namespace Echecs;

public abstract class Piece
{
    public Couleur Couleur { get; }
    public int Ligne { get; private set; }
    public int Colonne { get; private set; }

    protected Piece(Couleur couleur, int ligne, int colonne)
    {
        Couleur = couleur;
        Ligne = ligne;
        Colonne = colonne;
    }

    public abstract char Symbole { get; }
    public abstract string Nom { get; }

    public abstract bool PeutSeDeplacer(int versLigne, int versColonne);

    protected bool EstDansLimites(int ligne, int colonne)
        => ligne >= 0 && ligne < 8 && colonne >= 0 && colonne < 8;

    protected bool EstMemeCase(int versLigne, int versColonne)
        => versLigne == Ligne && versColonne == Colonne;

    public void Deplacer(int nouvelleLigne, int nouvelleColonne)
    {
        Ligne = nouvelleLigne;
        Colonne = nouvelleColonne;
    }

    public override string ToString() => $"{Couleur} {Nom} en ({Ligne}, {Colonne})";
}
