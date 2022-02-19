<template>
  <v-navigation-drawer :value="isDrawerOpen" app floating width="260" class="app-navigation-menu" :right="$vuetify.rtl" @input="(val) => $emit('update:is-drawer-open', val)">
    <!-- Navigation Items -->
    <v-list expand shaped class="vertical-nav-menu-items pr-5">
      <!--
      <nav-menu-link title="Dashboard" :to="{ name: 'dashboard' }" :icon="icons.mdiDotsGrid"></nav-menu-link>
      -->

      <nav-menu-link title="Favoriler" :to="{ name: 'redir', params: { routeName: 'opps', routeParams: { mode: 'FAVS' } } }" :icon="icons.mdiHeart"></nav-menu-link>
      
      <nav-menu-section-title title="KLASİK"></nav-menu-section-title>
      <nav-menu-link title="Binance > BtcTürk" :to="{ name: 'redir', params: { routeName: 'opps', routeParams: { mode: 'B2TC' } } }" :icon="icons.mdiSwapHorizontalCircle"></nav-menu-link>
      <nav-menu-link title="Binance > Paribu " :to="{ name: 'redir', params: { routeName: 'opps', routeParams: { mode: 'B2PC' } } }" :icon="icons.mdiSwapHorizontalCircle"></nav-menu-link>
      <nav-menu-link title="BtcTürk > Binance" :to="{ name: 'redir', params: { routeName: 'opps', routeParams: { mode: 'T2BC' } } }" :icon="icons.mdiSwapHorizontalCircle"></nav-menu-link>
      <nav-menu-link title="BtcTürk > Paribu " :to="{ name: 'redir', params: { routeName: 'opps', routeParams: { mode: 'T2PC' } } }" :icon="icons.mdiSwapHorizontalCircle"></nav-menu-link>
      <nav-menu-link title="Paribu > Binance " :to="{ name: 'redir', params: { routeName: 'opps', routeParams: { mode: 'P2BC' } } }" :icon="icons.mdiSwapHorizontalCircle"></nav-menu-link>
      <nav-menu-link title="Paribu > BtcTürk " :to="{ name: 'redir', params: { routeName: 'opps', routeParams: { mode: 'P2TC' } } }" :icon="icons.mdiSwapHorizontalCircle"></nav-menu-link>

      <nav-menu-section-title title="ÇAPRAZ"></nav-menu-section-title>
      <nav-menu-link title="Binance > BtcTürk" :to="{ name: 'redir', params: { routeName: 'opps', routeParams: { mode: 'B2TX' } } }" :icon="icons.mdiSwapHorizontalCircle"></nav-menu-link>
      <nav-menu-link title="Binance > Paribu " :to="{ name: 'redir', params: { routeName: 'opps', routeParams: { mode: 'B2PX' } } }" :icon="icons.mdiSwapHorizontalCircle"></nav-menu-link>
      <nav-menu-link title="BtcTürk > Binance" :to="{ name: 'redir', params: { routeName: 'opps', routeParams: { mode: 'T2BX' } } }" :icon="icons.mdiSwapHorizontalCircle"></nav-menu-link>
      <nav-menu-link title="BtcTürk > Paribu " :to="{ name: 'redir', params: { routeName: 'opps', routeParams: { mode: 'T2PX' } } }" :icon="icons.mdiSwapHorizontalCircle"></nav-menu-link>
      <nav-menu-link title="Paribu > Binance " :to="{ name: 'redir', params: { routeName: 'opps', routeParams: { mode: 'P2BX' } } }" :icon="icons.mdiSwapHorizontalCircle"></nav-menu-link>
      <nav-menu-link title="Paribu > BtcTürk " :to="{ name: 'redir', params: { routeName: 'opps', routeParams: { mode: 'P2TX' } } }" :icon="icons.mdiSwapHorizontalCircle"></nav-menu-link>

      <nav-menu-section-title title="HESABIM"></nav-menu-section-title>
      <nav-menu-link title="Hesap Ayarları" :to="{ name: 'account' }" :icon="icons.mdiAccount"></nav-menu-link>
      <nav-menu-link title="Çıkış" :to="{ name: 'auth-logout' }" :icon="icons.mdiLogout"></nav-menu-link>
    </v-list>
    <template v-slot:append>
      <div class="pa-2 pb-4">
        <!--<v-btn block @click="isDrawerOpen = false" elevation="0">-->
        <v-btn block @click="$emit('closed', 'someValue')" elevation="0">
          <v-icon>{{icons.mdiTransferLeft}}</v-icon>
        </v-btn>
      </div>
    </template>
  </v-navigation-drawer>
</template>

<script>
import { mdiDotsGrid, mdiHeart, mdiAlphaPCircleOutline, mdiAlphaBCircleOutline, mdiAccount, mdiLogout, mdiTransferLeft, mdiSwapHorizontalCircle } from '@mdi/js';
import NavMenuSectionTitle from './components/NavMenuSectionTitle.vue';
import NavMenuGroup from './components/NavMenuGroup.vue';
import NavMenuLink from './components/NavMenuLink.vue';

export default {
  components: {
    NavMenuSectionTitle,
    NavMenuGroup,
    NavMenuLink,
  },
  props: {
    isDrawerOpen: {
      type: Boolean,
      default: null,
    },
  },
  setup() {
    return {
      icons: {
        mdiDotsGrid,
        mdiHeart,
        mdiAlphaPCircleOutline,
        mdiAlphaBCircleOutline,
        mdiAccount,
        mdiLogout,
        mdiTransferLeft,
        mdiSwapHorizontalCircle,
      },
    };
  },
};
</script>

<style lang="scss" scoped>
.app-title {
  font-size: 1.45rem;
  font-weight: 700;
  font-stretch: normal;
  font-style: normal;
  line-height: normal;
  letter-spacing: 0.3px;
}

// ? Adjust this `translateX` value to keep logo in center when vertical nav menu is collapsed (Value depends on your logo)
.app-logo {
  transition: all 0.18s ease-in-out;
  .v-navigation-drawer--mini-variant & {
    transform: translateX(-4px);
  }
}

@include theme(app-navigation-menu) using ($material) {
  background-color: map-deep-get($material, 'background');
}

.app-navigation-menu {
  .v-list-item {
    &.vertical-nav-menu-link {
      ::v-deep .v-list-item__icon {
        .v-icon {
          transition: none !important;
        }
      }
      ::v-deep .v-list-item__title {
        font-size: 0.85rem;
      }
    }
  }
}

// You can remove below style
// Upgrade Banner
.app-navigation-menu {
  .upgrade-banner {
    position: absolute;
    bottom: 13px;
    left: 50%;
    transform: translateX(-50%);
  }
}
</style>
