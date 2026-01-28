namespace Echecs.Controllers;

using Echecs.Models;
using Echecs.Models.Enums;
using Echecs.Views;

public class JeuController(IEchecsVue vue)
{
    private readonly PartieEchecs _partie = new();
    private readonly IEchecsVue _vue = vue;
    private bool _quitter;

    public void Demarrer()
    {
        _vue.AfficherAccueil();
        BouclePrincipale();
    }

    private void BouclePrincipale()
    {
        while (!_quitter && !_partie.PartieTerminee)
        {
            _vue.EffacerEcran();
            _vue.AfficherPlateau(_partie.Plateau);
            AfficherScores();
            _vue.AfficherTour(_partie.JoueurActif);
            
            var coup = _vue.DemanderCoup();
            
            if (coup == null)
            {
                if (_vue.DemanderConfirmationQuitter())
                    _quitter = true;
                continue;
            }
            
            var (deLigne, deColonne, versLigne, versColonne) = coup.Value;
            var resultat = _partie.TenterDeplacement(
                deLigne, deColonne, versLigne, versColonne);
            
            _vue.AfficherResultatDeplacement(resultat);
            
            if (resultat != ResultatDeplacement.Succes)
            {
                Console.WriteLine("\nAppuyez sur Entrée pour continuer...");
                Console.ReadLine();
            }
        }
        
        _vue.AfficherMessage("Merci d'avoir joué ! ♔");
    }

    private void AfficherScores()
    {
        int scoreBlancs = _partie.Plateau.CalculerScore(Couleur.Blanc);
        int scoreNoirs = _partie.Plateau.CalculerScore(Couleur.Noir);
        
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Score: ⚪ Blancs = {scoreBlancs} pts | ⚫ Noirs = {scoreNoirs} pts");
        Console.WriteLine($"Coup n°{_partie.NombreDeCoups + 1}");
        Console.ResetColor();
    }
}
