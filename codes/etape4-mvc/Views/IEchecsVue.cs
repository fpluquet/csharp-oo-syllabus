namespace Echecs.Views;

using Echecs.Models;
using Echecs.Models.Enums;

public interface IEchecsVue
{
    void AfficherPlateau(Plateau plateau);
    void AfficherMessage(string message);
    void AfficherErreur(string message);
    void AfficherTour(Couleur joueur);
    void AfficherResultatDeplacement(ResultatDeplacement resultat);
    
    (int deLigne, int deColonne, int versLigne, int versColonne)? DemanderCoup();
    bool DemanderConfirmationQuitter();
    
    void AfficherAccueil();
    void EffacerEcran();
}
