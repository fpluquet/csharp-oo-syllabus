namespace Echecs.Views;

using Echecs.Models;
using Echecs.Models.Enums;
using Echecs.Models.Pieces;

public class ConsoleVue : IEchecsVue
{
    public void AfficherPlateau(Plateau plateau)
    {
        Console.WriteLine();
        Console.WriteLine("    a   b   c   d   e   f   g   h");
        Console.WriteLine("  ┌───┬───┬───┬───┬───┬───┬───┬───┐");
        
        for (int ligne = 7; ligne >= 0; ligne--)
        {
            Console.Write($"{ligne + 1} │");
            
            for (int col = 0; col < 8; col++)
            {
                Piece? piece = plateau[ligne, col];
                Console.Write(piece != null ? $" {piece.Symbole} " : "   ");
                Console.Write("│");
            }
            
            Console.WriteLine($" {ligne + 1}");
            
            if (ligne > 0)
                Console.WriteLine("  ├───┼───┼───┼───┼───┼───┼───┼───┤");
        }
        
        Console.WriteLine("  └───┴───┴───┴───┴───┴───┴───┴───┘");
        Console.WriteLine("    a   b   c   d   e   f   g   h\n");
    }

    public void AfficherMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"ℹ️  {message}");
        Console.ResetColor();
    }

    public void AfficherErreur(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"❌ {message}");
        Console.ResetColor();
    }

    public void AfficherTour(Couleur joueur)
    {
        string symbole = joueur == Couleur.Blanc ? "⚪" : "⚫";
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n{symbole} C'est au tour des {joueur}s de jouer");
        Console.ResetColor();
    }

    public void AfficherResultatDeplacement(ResultatDeplacement resultat)
    {
        string message = resultat switch
        {
            ResultatDeplacement.Succes => "✓ Déplacement effectué !",
            ResultatDeplacement.CaseDeDepart_Vide => "Aucune pièce à cet emplacement.",
            ResultatDeplacement.MauvaiseCouleur => "Ce n'est pas votre pièce !",
            ResultatDeplacement.DeplacementInvalide => "Mouvement invalide pour cette pièce.",
            ResultatDeplacement.CaseOccupeeParAllie => "Votre pièce occupe déjà cette case.",
            ResultatDeplacement.CheminBloque => "Le chemin est bloqué.",
            _ => "Erreur inconnue."
        };

        if (resultat == ResultatDeplacement.Succes)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        else
        {
            AfficherErreur(message);
        }
    }

    public (int deLigne, int deColonne, int versLigne, int versColonne)? DemanderCoup()
    {
        Console.Write("Entrez votre coup (ex: e2 e4) ou 'q' pour quitter : ");
        
        string? input = Console.ReadLine()?.Trim().ToLower();
        
        if (string.IsNullOrEmpty(input) || input == "q")
            return null;
        
        if (TryParserCoup(input, out var coup))
            return coup;
        
        AfficherErreur("Format invalide. Utilisez: e2 e4");
        return DemanderCoup();
    }

    private bool TryParserCoup(string input, 
        out (int deLigne, int deColonne, int versLigne, int versColonne) coup)
    {
        coup = default;
        
        var parties = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parties.Length != 2) return false;
        
        if (!TryParserCase(parties[0], out int deLigne, out int deColonne)) return false;
        if (!TryParserCase(parties[1], out int versLigne, out int versColonne)) return false;
        
        coup = (deLigne, deColonne, versLigne, versColonne);
        return true;
    }

    private bool TryParserCase(string notation, out int ligne, out int colonne)
    {
        ligne = colonne = 0;
        if (notation.Length != 2) return false;
        
        if (notation[0] < 'a' || notation[0] > 'h') return false;
        if (notation[1] < '1' || notation[1] > '8') return false;
        
        colonne = notation[0] - 'a';
        ligne = notation[1] - '1';
        return true;
    }

    public bool DemanderConfirmationQuitter()
    {
        Console.Write("Voulez-vous vraiment quitter ? (o/n) : ");
        string? reponse = Console.ReadLine()?.Trim().ToLower();
        return reponse == "o" || reponse == "oui";
    }

    public void AfficherAccueil()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║     ♔ ♕ ♖ ♗ ♘ ♙   JEU D'ÉCHECS   ♟ ♞ ♝ ♜ ♛ ♚               ║
║                                                               ║
║                    Version Console C#                         ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝
");
        Console.ResetColor();
        Console.WriteLine("Commandes :");
        Console.WriteLine("  • Entrez un coup au format : e2 e4");
        Console.WriteLine("  • Tapez 'q' pour quitter\n");
        Console.WriteLine("Appuyez sur Entrée pour commencer...");
        Console.ReadLine();
    }

    public void EffacerEcran() => Console.Clear();
}
