export default {
  base: '/visma-hiring-assignment/',
  title: 'e-conomic SRE assignment',
  description: 'Pavel`s solution to e-conomic assignment for SRE position',
  theme: 'default-prefers-color-scheme',
  themeConfig: {
    nav: [
      {text: 'Home', link: '/'},
      {text: 'Getting started', link: '/getting-started/'},
    ],
    sidebar: {
      '/getting-started/': [
        {
          title: 'Getting started',
          collapsable: false,
          children: [
            ''
          ]
        },
      ],
    },
    searchPlaceholder: 'Search docs...',
    lastUpdated: 'Last updated',
    repo: 'https://github.com/PavelPikat/visma-hiring-assignment',
    repoLabel: 'GitHub'
  },
  smoothScroll: true,
  plugins: ['@vuepress/active-header-links']
}
