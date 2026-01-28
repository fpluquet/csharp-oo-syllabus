# Projet : Jeu d'Échecs en Console

Bienvenue dans ce projet fil rouge ! Nous allons construire ensemble un jeu d'échecs en console, en partant d'un code simple et en l'améliorant progressivement grâce aux concepts de la programmation orientée objet.

::: tip 🎯 Ce que vous allez apprendre
- Appliquer concrètement l'encapsulation, l'héritage et le polymorphisme
- Comprendre **pourquoi** et **quand** utiliser chaque concept OO
- Structurer un projet avec l'architecture MVC
- Utiliser les fonctionnalités modernes de C# dans un contexte réel
:::

## Pourquoi un jeu d'échecs ?

Le jeu d'échecs est un excellent terrain d'apprentissage pour la programmation orientée objet. Regardons pourquoi :

### Des objets naturels

Quand vous regardez un échiquier, vous voyez naturellement des **objets** :
- Des **pièces** (roi, dame, tour, fou, cavalier, pion)
- Un **plateau** avec 64 cases
- Deux **joueurs** qui s'affrontent

Chaque pièce a ses propres **caractéristiques** (couleur, position) et son propre **comportement** (règles de déplacement). C'est exactement ce que l'OO modélise !

### Des comportements similaires mais différents

Toutes les pièces peuvent "se déplacer", mais chacune le fait différemment :
- La tour se déplace en ligne droite
- Le fou se déplace en diagonale  
- Le cavalier fait un "L"

C'est le cas d'école parfait pour comprendre le **polymorphisme** !

### Une complexité croissante

Un jeu d'échecs complet nécessite de gérer :
- L'affichage du plateau
- Les entrées utilisateur
- Les règles du jeu
- L'alternance des tours

Cette complexité nous obligera à **structurer** notre code proprement.

## Notre parcours en 4 étapes

```
┌─────────────────────────────────────────────────────────────────────┐
│                    NOTRE PARCOURS D'APPRENTISSAGE                   │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  Étape 1          Étape 2           Étape 3          Étape 4        │
│  ┌──────┐        ┌──────┐          ┌──────┐         ┌──────┐        │
│  │Simple│   →    │Hérit.│    →     │Poly. │    →    │ MVC  │        │
│  │Classe│        │Abstr.│          │      │         │      │        │
│  └──────┘        └──────┘          └──────┘         └──────┘        │
│                                                                     │
│  Encapsulation   Spécialisation    Flexibilité     Architecture     │
│  Énumérations    Classes dérivées  virtual/override Séparation      │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

Chaque étape résout un problème identifié à l'étape précédente :

| Étape | Objectif | Problème résolu |
|-------|----------|-----------------|
| [Étape 1](./16a-etape1-encapsulation.md) | Créer les objets de base | - |
| [Étape 2](./16b-etape2-heritage.md) | Spécialiser chaque pièce | Switch géant illisible |
| [Étape 3](./16c-etape3-polymorphisme.md) | Manipuler les pièces uniformément | Code dupliqué, rigidité |
| [Étape 4](./16d-etape4-mvc.md) | Séparer les responsabilités | Code mélangé, non testable |

::: warning ⚠️ Approche pédagogique importante
Nous allons **volontairement** commencer par un code imparfait. L'objectif est de **comprendre les problèmes** avant d'apprendre les solutions. 

Ne sautez pas les étapes ! Chaque limitation rencontrée vous fera comprendre pourquoi le concept suivant est utile.
:::

## Ce que nous allons construire

À la fin de ce projet, vous aurez un jeu d'échecs fonctionnel en console :

```
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║     ♔ ♕ ♖ ♗ ♘ ♙   JEU D'ÉCHECS   ♟ ♞ ♝ ♜ ♛ ♚               ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝

    a   b   c   d   e   f   g   h
  ┌───┬───┬───┬───┬───┬───┬───┬───┐
8 │ ♜ │ ♞ │ ♝ │ ♛ │ ♚ │ ♝ │ ♞ │ ♜ │ 8
  ├───┼───┼───┼───┼───┼───┼───┼───┤
7 │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │ 7
  ├───┼───┼───┼───┼───┼───┼───┼───┤
6 │   │   │   │   │   │   │   │   │ 6
  ├───┼───┼───┼───┼───┼───┼───┼───┤
5 │   │   │   │   │   │   │   │   │ 5
  ├───┼───┼───┼───┼───┼───┼───┼───┤
4 │   │   │   │   │   │   │   │   │ 4
  ├───┼───┼───┼───┼───┼───┼───┼───┤
3 │   │   │   │   │   │   │   │   │ 3
  ├───┼───┼───┼───┼───┼───┼───┼───┤
2 │ ♙ │ ♙ │ ♙ │ ♙ │ ♙ │ ♙ │ ♙ │ ♙ │ 2
  ├───┼───┼───┼───┼───┼───┼───┼───┤
1 │ ♖ │ ♘ │ ♗ │ ♕ │ ♔ │ ♗ │ ♘ │ ♖ │ 1
  └───┴───┴───┴───┴───┴───┴───┴───┘
    a   b   c   d   e   f   g   h

⚪ C'est au tour des Blancs de jouer
Entrez votre coup (ex: e2 e4) : _
```

## Prérequis

Avant de commencer, assurez-vous de maîtriser :
- Les bases de C# (variables, conditions, boucles)
- Les notions de classes et d'objets
- Les propriétés et les constructeurs

Si ces concepts ne sont pas clairs, relisez les chapitres correspondants du syllabus.

## C'est parti !

Rendez-vous à l'[Étape 1 : Encapsulation et objets de base](./16a-etape1-encapsulation.md) pour commencer notre aventure !
