import { defineConfig } from 'vitepress'
import { withMermaid } from 'vitepress-plugin-mermaid'

export default withMermaid(defineConfig({
  lang: 'fr-FR',
  title: 'Programmation OO en C#',
  description: 'Syllabus du cours de Programmation Orientée Objet en C# pour les étudiants de BA1 - HELHa',
  base: '/csharp-oo-syllabus/',

  head: [
    ['link', { rel: 'icon', href: '/favicon.ico' }]
  ],
  
  // Configuration de Mermaid
  mermaid: {
    theme: 'neutral',
    darkMode: false,
    securityLevel: 'loose',
    logLevel: 'error',
    htmlLabels: true,
    flowchart: {
      htmlLabels: true,
      useMaxWidth: true,
      rankSpacing: 65,
      nodeSpacing: 30,
      padding: 15
    },
    themeVariables: {
      primaryColor: '#5D8AA8',
      primaryTextColor: '#fff',
      primaryBorderColor: '#7C0200',
      lineColor: '#404040',
      secondaryColor: '#006100',
      tertiaryColor: '#fff',
      nodeBorder: '#2b6387',
      clusterBkg: '#e9f3f8',
      clusterBorder: '#2b6387',
      titleColor: '#333333'
    }
  },

  themeConfig: {
    logo: '/logo.png',

    nav: [
      { text: 'Accueil', link: '/' },
      { text: 'Syllabus Q1', link: 'https://fpluquet.github.io/csharp-intro-syllabus/', target: '_blank' },
      { text: 'HELHa', link: 'https://www.helha.be', target: '_blank' },
    ],

    sidebar: [
      {
        text: 'Partie I - Fondamentaux OO',
        collapsed: false,
        items: [
          { text: 'Introduction à l\'OO', link: '/01-introduction-oo' },
          { text: 'Classes et Objets', link: '/02-classes-objets' },
          { text: 'Constructeurs', link: '/03-constructeurs' },
          { text: 'Encapsulation et Propriétés', link: '/04-encapsulation' },
          { text: 'Membres Statiques', link: '/05-membres-statiques' },
          { text: 'Passage de Paramètres', link: '/06-passage-parametres' },
          { text: 'Héritage', link: '/07-heritage' },
          { text: 'Polymorphisme', link: '/08-polymorphisme' },
          { text: 'Concepts Avancés', link: '/09-concepts-avances' },
        ]
      },
      {
        text: 'Partie II - C# Moderne',
        collapsed: false,
        items: [
          { text: 'Records', link: '/10-records' },
          { text: 'Primary Constructors', link: '/11-primary-constructors' },
          { text: 'Nullable Reference Types', link: '/12-nullable-reference-types' },
          { text: 'Propriétés Init-Only', link: '/13-proprietes-init' },
          { text: 'Interfaces Modernes', link: '/14-interfaces-modernes' },
          { text: 'Pattern Matching', link: '/15-pattern-matching' },
        ]
      },
    ],

    footer: {
      message: 'Syllabus de Programmation Orientée Objet en C#',
      copyright: '© 2025 HELHa - Frédéric Pluquet'
    },

    docFooter: {
      prev: 'Page précédente',
      next: 'Page suivante'
    },

    outline: {
      label: 'Sur cette page',
      level: [2, 3]
    },

    lastUpdated: {
      text: 'Dernière mise à jour',
      formatOptions: {
        dateStyle: 'long',
        timeStyle: 'short'
      }
    },

    search: {
      provider: 'local',
      options: {
        translations: {
          button: {
            buttonText: 'Rechercher...',
            buttonAriaLabel: 'Rechercher'
          },
          modal: {
            noResultsText: 'Aucun résultat pour',
            resetButtonTitle: 'Effacer la recherche',
            footer: {
              selectText: 'pour sélectionner',
              navigateText: 'pour naviguer',
              closeText: 'pour fermer'
            }
          }
        }
      }
    }
  }
}))
