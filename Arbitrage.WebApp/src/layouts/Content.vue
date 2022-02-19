<template>
  <v-app>
    <vertical-nav-menu :is-drawer-open.sync="isDrawerOpen" @closed="onDrawerClosed"></vertical-nav-menu>

    <div style="position: fixed; bottom: 10px; left: 10px; z-index: 2">
      <v-btn @click="isDrawerOpen = true" icon elevation="5" x-large :style="$vuetify.theme.dark ? 'background-color:rgba(53, 59, 90, 0.85);' : 'background-color:rgba(250, 250, 250, 0.85);'">
        <v-icon>{{ icons.mdiTransferRight }}</v-icon>
      </v-btn>
    </div>

    <v-app-bar app flat absolute color="transparent">
      <div class="w-full">
        <div class="d-flex align-center mx-6">
          <!-- Left Content -->
          <v-app-bar-nav-icon class="d-block d-lg-none me-2" @click="isDrawerOpen = !isDrawerOpen"></v-app-bar-nav-icon>
          <span class="v-card__title px-0 py-0" style="font-size: 1.4rem">Arbitraj Paneli</span>
          <v-spacer></v-spacer>

          <!-- Right Content -->
          <theme-switcher></theme-switcher>
          <app-bar-user-menu></app-bar-user-menu>
        </div>
      </div>
    </v-app-bar>

    <v-main>
      <div class="app-content-container pa-6">
        <slot></slot>
      </div>
    </v-main>
  </v-app>
</template>

<script>
import { mdiChevronRight, mdiTransferRight } from '@mdi/js';
import VerticalNavMenu from './components/vertical-nav-menu/VerticalNavMenu.vue';
import ThemeSwitcher from './components/ThemeSwitcher.vue';
import AppBarUserMenu from './components/AppBarUserMenu.vue';
import { mapState } from 'vuex';
import * as ApiConstants from '@/modules/ApiConstants';

export default {
  components: {
    VerticalNavMenu,
    ThemeSwitcher,
    AppBarUserMenu,
  },

  data: () => ({
    icons: {
      mdiChevronRight,
      mdiTransferRight,
    },
    isDrawerOpen: false,
    wsunique: 0,
  }),

  methods: {
    // on socket init fail
    onDrawerClosed(value) {
      this.isDrawerOpen = false;
    },

    setupOppsHandlers() {
      this.$api.on(ApiConstants.WSS_SOCKET_ONFAIL, this.onSockFail);
      this.$api.on(ApiConstants.WSS_OPPS_ONINIT, this.onTickerInit);
      this.$api.on(ApiConstants.WSS_OPPS_ONFAIL, this.onTickerFail);
      this.$api.on(ApiConstants.WSS_OPPS_ONERROR, this.onTickerError);
      this.$api.on(ApiConstants.WSS_OPPS_ONCLOSE, this.onTickerClose);
      this.$api.on(ApiConstants.WSS_OPPS_ONOPEN, this.onTickerOpen);
      this.$api.on(ApiConstants.WSS_OPPS_ONDATA, this.onTickerData);
      // this.$api.on(ApiConstants.WSS_OPPS_ONTICK, this.onTickerPrices);
      this.$api.startOpportunitiesStream(this.wsunique, true);
    },

    // on socket init fail
    onSockFail(error) {},

    // on socket conenction attempt
    onTickerInit(time) {
      this.tickerStart = time;
    },

    // on socket failure to start
    onTickerFail(error) {},

    // when socket connection ends
    onTickerError(e) {},

    // when socket connection ends
    onTickerClose(e) {},

    // when socket connection opens
    onTickerOpen(e) {
      this.tickerStart = Date.now();
    },

    // when socket connection has data
    onTickerData(data) {},

    onTickerPrices(prices) {
      // console.log(prices);
      let secs = (Date.now() - this.tickerStart) / 1000;
      this.tickerTime = this.$utils.elapsed(secs, '', true);

      prices.forEach((item, index) => {
        // console.log(item);
        this.$store.commit('UPDATE_SPOT_TICKER', item);
      });
    },
  },

  computed: {
    width() {
      return window.innerWidth;
    },
  },

  beforeCreate() {
    //console.log("beforeCreate");
  },

  created() {
    // console.log("created");
    this.isDrawerOpen = window.innerWidth > 1250;
    this.wsunique = Date.now();
    this.setupOppsHandlers();
  },

  beforeMount() {
    //console.log("beforeMount");
  },

  mounted() {
    //console.log("mounted");
  },

  beforeUpdate() {
    //console.log("beforeUpdate");
  },

  updated() {
    //console.log("updated");
  },

  beforeDestroy() {
    //console.log("beforeDestroy");
  },

  destroyed() {
    //console.log("destroyed");
    this.$api.stopOpportunitiesStream(this.wsunique);
    // clearInterval(this.epochInterval);
  },
};
</script>

<style lang="scss" scoped>
.v-app-bar ::v-deep {
  .v-toolbar__content {
    padding: 0;

    .app-bar-search {
      .v-input__slot {
        padding-left: 18px;
      }
    }
  }
}

.boxed-container {
  max-width: 1440px;
  margin-left: auto;
  margin-right: auto;
}
</style>
