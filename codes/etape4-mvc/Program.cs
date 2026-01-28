using Echecs.Controllers;
using Echecs.Views;

// 1. Créer la vue (ici, console)
IEchecsVue vue = new ConsoleVue();

// 2. Créer le contrôleur avec la vue
JeuController jeu = new(vue);

// 3. Démarrer le jeu
jeu.Demarrer();
