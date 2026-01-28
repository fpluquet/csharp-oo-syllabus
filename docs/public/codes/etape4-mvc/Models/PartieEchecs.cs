namespace Echecs.Models;

using Echecs.Models.Enums;
using Echecs.Models.Pieces;

public class PartieEchecs
{
    public Plateau Plateau { get; }
    public Couleur JoueurActif { get; private set; } = Couleur.Blanc;
    public bool PartieTerminee { get; private set; }
    public int NombreDeCoups { get; private set; }

    public PartieEchecs()
    {
        Plateau = new Plateau();
    }

    public ResultatDeplacement TenterDeplacement(
        int deLigne, int deColonne, 
        int versLigne, int versColonne)
    {
        Piece? piece = Plateau[deLigne, deColonne];
        
        if (piece == null)
            return ResultatDeplacement.CaseDeDepart_Vide;
        
        if (piece.Couleur != JoueurActif)
            return ResultatDeplacement.MauvaiseCouleur;
        
        if (!piece.PeutSeDeplacer(versLigne, versColonne))
            return ResultatDeplacement.DeplacementInvalide;
        
        Piece? cible = Plateau[versLigne, versColonne];
        if (cible != null && cible.Couleur == piece.Couleur)
            return ResultatDeplacement.CaseOccupeeParAllie;
        
        Plateau.EffectuerDeplacement(deLigne, deColonne, versLigne, versColonne);
        NombreDeCoups++;
        
        JoueurActif = JoueurActif == Couleur.Blanc ? Couleur.Noir : Couleur.Blanc;
        
        return ResultatDeplacement.Succes;
    }

    public void NouvellePartie()
    {
        Plateau.Initialiser();
        JoueurActif = Couleur.Blanc;
        PartieTerminee = false;
        NombreDeCoups = 0;
    }
}
