import '@/plugins/vue-composition-api'
import '@/styles/styles.scss'
import Vue from 'vue'
import App from './App.vue'
import vuetify from './plugins/vuetify'
import router from './router'
import store from './store'

// Config
import i18n from './config/AppLanguage';
import * as AppConfig from '@/config/AppConfig';
import { AppLanguages } from '@/config/AppLanguages';

// Modules
import ApiClient from '@/modules/ApiClient';
import { Utils } from '@/modules/Utils';

// API Client
const _api = new ApiClient(AppConfig, router, store);
if (store?.state?.session?.loggedIn) {
  _api._loggedIn = store.state.session.loggedIn;
  _api._accountId = store.state.session.accountId;
  _api._accessToken = store.state.session.accessToken;
}

// Custom Global Vue Properties
let _languages = AppLanguages;
Object.defineProperties(Vue.prototype, {
  $api: { get() { return _api; }, },
  $config: { get() { return AppConfig; }, },
  $languages: { get() { return _languages; }, },
  $utils: { get() { return Utils; }, },
});

// v-snackbars
import VSnackbars from "v-snackbars";
Vue.component('v-snackbars', VSnackbars);

// Vue Configuration
Vue.config.productionTip = false

// Vue Instance
new Vue({
  router, store, i18n, vuetify,
  render: h => h(App),
}).$mount('#app')
