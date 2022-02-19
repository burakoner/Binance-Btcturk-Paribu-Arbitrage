<template>
  <v-card>
    <v-list rounded>
      <v-subheader>FIRSATLAR</v-subheader>
      <v-list-item-group color="primary">
        <v-list-item :to="{ name: 'opps', params: { mode: 'FAVS' } }">
          <v-list-item-icon><v-icon>{{ icons.mdiHeart }}</v-icon></v-list-item-icon>
          <v-list-item-content><v-list-item-title>Favoriler</v-list-item-title></v-list-item-content>
        </v-list-item>
      </v-list-item-group>
      <v-divider />

      <v-subheader>KLASİK</v-subheader>
      <v-list-item-group color="primary">
        <v-list-item :to="{ name: 'opps', params: { mode: 'B2TC' } }">
          <v-list-item-icon><v-icon>{{ icons.mdiSwapHorizontalCircle }}</v-icon></v-list-item-icon>
          <v-list-item-content><v-list-item-title>Binance > BtcTürk</v-list-item-title></v-list-item-content>
        </v-list-item>

        <v-list-item :to="{ name: 'opps', params: { mode: 'B2PC' } }">
          <v-list-item-icon><v-icon>{{ icons.mdiSwapHorizontalCircle }}</v-icon></v-list-item-icon>
          <v-list-item-content><v-list-item-title>Binance > Paribu</v-list-item-title></v-list-item-content>
        </v-list-item>

        <v-list-item :to="{ name: 'opps', params: { mode: 'T2BC' } }">
          <v-list-item-icon><v-icon>{{ icons.mdiSwapHorizontalCircle }}</v-icon></v-list-item-icon>
          <v-list-item-content><v-list-item-title>BtcTürk > Binance</v-list-item-title></v-list-item-content>
        </v-list-item>

        <v-list-item :to="{ name: 'opps', params: { mode: 'T2PC' } }">
          <v-list-item-icon><v-icon>{{ icons.mdiSwapHorizontalCircle }}</v-icon></v-list-item-icon>
          <v-list-item-content><v-list-item-title>BtcTürk > Paribu</v-list-item-title></v-list-item-content>
        </v-list-item>

        <v-list-item :to="{ name: 'opps', params: { mode: 'P2BC' } }">
          <v-list-item-icon><v-icon>{{ icons.mdiSwapHorizontalCircle }}</v-icon></v-list-item-icon>
          <v-list-item-content><v-list-item-title>Paribu > Binance</v-list-item-title></v-list-item-content>
        </v-list-item>
        
        <v-list-item :to="{ name: 'opps', params: { mode: 'P2TC' } }">
          <v-list-item-icon><v-icon>{{ icons.mdiSwapHorizontalCircle }}</v-icon></v-list-item-icon>
          <v-list-item-content><v-list-item-title>Paribu > BtcTürk</v-list-item-title></v-list-item-content>
        </v-list-item>
      </v-list-item-group>

      <v-divider />
      
      <v-subheader>ÇAPRAZ</v-subheader>
      <v-list-item-group color="primary">
        <v-list-item :to="{ name: 'opps', params: { mode: 'B2TX' } }">
          <v-list-item-icon><v-icon>{{ icons.mdiSwapHorizontalCircle }}</v-icon></v-list-item-icon>
          <v-list-item-content><v-list-item-title>Binance > BtcTürk</v-list-item-title></v-list-item-content>
        </v-list-item>

        <v-list-item :to="{ name: 'opps', params: { mode: 'B2PX' } }">
          <v-list-item-icon><v-icon>{{ icons.mdiSwapHorizontalCircle }}</v-icon></v-list-item-icon>
          <v-list-item-content><v-list-item-title>Binance > Paribu</v-list-item-title></v-list-item-content>
        </v-list-item>

        <v-list-item :to="{ name: 'opps', params: { mode: 'T2BX' } }">
          <v-list-item-icon><v-icon>{{ icons.mdiSwapHorizontalCircle }}</v-icon></v-list-item-icon>
          <v-list-item-content><v-list-item-title>BtcTürk > Binance</v-list-item-title></v-list-item-content>
        </v-list-item>

        <v-list-item :to="{ name: 'opps', params: { mode: 'T2PX' } }">
          <v-list-item-icon><v-icon>{{ icons.mdiSwapHorizontalCircle }}</v-icon></v-list-item-icon>
          <v-list-item-content><v-list-item-title>BtcTürk > Paribu</v-list-item-title></v-list-item-content>
        </v-list-item>

        <v-list-item :to="{ name: 'opps', params: { mode: 'P2BX' } }">
          <v-list-item-icon><v-icon>{{ icons.mdiSwapHorizontalCircle }}</v-icon></v-list-item-icon>
          <v-list-item-content><v-list-item-title>Paribu > Binance</v-list-item-title></v-list-item-content>
        </v-list-item>
        
        <v-list-item :to="{ name: 'opps', params: { mode: 'P2TX' } }">
          <v-list-item-icon><v-icon>{{ icons.mdiSwapHorizontalCircle }}</v-icon></v-list-item-icon>
          <v-list-item-content><v-list-item-title>Paribu > BtcTürk</v-list-item-title></v-list-item-content>
        </v-list-item>
      </v-list-item-group>

      <v-divider />

      <v-subheader>HESABIM</v-subheader>
      <v-list-item-group color="primary">
        <v-list-item :to="{ name: 'account' }">
          <v-list-item-icon>
            <v-icon> {{ icons.mdiAccount }}</v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>Hesap Ayarları</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item :to="{ name: 'auth-logout' }">
          <v-list-item-icon>
            <v-icon> {{ icons.mdiLogout }}</v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>Çıkış</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list-item-group>
    </v-list>
  </v-card>
</template>

<script>
import { mdiDotsGrid, mdiHeart, mdiSwapHorizontalBold, mdiAlphaBCircleOutline, mdiAlphaPCircleOutline, mdiAccount, mdiLogout, mdiSwapHorizontalCircle } from '@mdi/js';

export default {
  data: () => ({
    icons: {
      mdiDotsGrid,
      mdiHeart,
      mdiSwapHorizontalBold,
      mdiAlphaBCircleOutline,
      mdiAlphaPCircleOutline,
      mdiAccount,
      mdiLogout,
      mdiSwapHorizontalCircle,
    },
  }),
};
</script>
