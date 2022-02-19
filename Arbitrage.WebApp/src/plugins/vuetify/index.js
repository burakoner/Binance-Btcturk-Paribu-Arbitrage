import Vue from 'vue'
import Vuetify from 'vuetify/lib/framework'
import preset from './default-preset/preset'

import tr from 'vuetify/lib/locale/tr';
import en from 'vuetify/lib/locale/en';

Vue.use(Vuetify)

export default new Vuetify({
  preset,
  
  lang: {
    locales: { tr, en },
    current: 'tr',
  },
  icons: {
    iconfont: 'mdiSvg',
  },
  theme: {
    options: {
      customProperties: true,
      variations: false,
    },
  },
})
