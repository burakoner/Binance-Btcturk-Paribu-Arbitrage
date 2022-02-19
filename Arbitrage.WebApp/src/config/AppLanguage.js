import Vue from 'vue';
import VueI18n from 'vue-i18n';
import { en } from '../i18n/en';
import { tr } from '../i18n/tr';
import * as AppConfig from '@/config/AppConfig';
import { AppLanguages } from '@/config/AppLanguages';

Vue.use(VueI18n);

let defaultLanguage = AppConfig.DEFAULT_LANGUAGE;
const lsLang = localStorage.getItem('lang');
if (lsLang) {
  AppLanguages.forEach((el) => {
    if (lsLang.toLowerCase() == el.value) {
      defaultLanguage = el.value;
    }
  });
}

export default new VueI18n({
  locale: defaultLanguage,
  fallbackLocale: defaultLanguage,
  messages: {
    en,
    tr,
  },
});
