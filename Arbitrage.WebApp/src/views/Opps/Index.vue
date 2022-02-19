<template>
  <v-row>
    <v-overlay :value="loading">
      <v-progress-circular indeterminate size="64"></v-progress-circular>
    </v-overlay>
    <v-col cols="12" class="mx-0 px-0" v-if="!loading">
      <v-card>
        <v-card-title class="d-flex">
          <span v-if="mode == 'FAVS'">Favoriler</span>

          <span v-if="mode.startsWith('B2T')"
            >Binance <v-icon class="mx-0">{{ icons.mdiArrowRightBoldCircle }}</v-icon> BtcTürk</span
          >
          <span v-if="mode.startsWith('B2P')"
            >Binance <v-icon class="mx-0">{{ icons.mdiArrowRightBoldCircle }}</v-icon> Paribu</span
          >
          <span v-if="mode.startsWith('T2B')"
            >BtcTürk <v-icon class="mx-0">{{ icons.mdiArrowRightBoldCircle }}</v-icon> Binance</span
          >
          <span v-if="mode.startsWith('T2P')"
            >BtcTürk <v-icon class="mx-0">{{ icons.mdiArrowRightBoldCircle }}</v-icon> Paribu</span
          >
          <span v-if="mode.startsWith('P2B')"
            >Paribu <v-icon class="mx-0">{{ icons.mdiArrowRightBoldCircle }}</v-icon> Binance</span
          >
          <span v-if="mode.startsWith('P2T')"
            >Paribu <v-icon class="mx-0">{{ icons.mdiArrowRightBoldCircle }}</v-icon> BtcTürk</span
          >
          <span v-if="mode.endsWith('C')" class="ml-2">(Klasik)</span>
          <span v-if="mode.endsWith('X')" class="ml-2">(Çapraz)</span>

          <span v-show="false">Epoch: {{ epoch }}</span>
          <v-spacer />
          <span class="d-flex">
            <v-btn @click="editFavsDialog = true" icon v-if="mode != 'FAVS'">
              <v-icon>{{ icons.mdiHeartHalfFull }}</v-icon>
            </v-btn>
            <v-btn @click="setForMobile" icon>
              <v-icon>{{ icons.mdiCellphone }}</v-icon>
            </v-btn>
            <v-spacer />
            <v-btn @click="setForDesktop" icon>
              <v-icon>{{ icons.mdiMonitor }}</v-icon>
            </v-btn>
            <v-spacer />
            <v-btn @click="filterDialog = true" icon>
              <v-icon>{{ icons.mdiFilter }}</v-icon>
            </v-btn>
            <v-btn @click="showInformation = !showInformation" icon>
              <v-icon>{{ icons.mdiInformation }}</v-icon>
            </v-btn>
          </span>
        </v-card-title>
        <v-divider v-if="showInformation" />
        <v-card-text class="caption" v-if="showInformation">
          Klasik Arbitraj'da borsalar arası aynı paritelerde işlem yapılır. Örneğin Paribu'dan btc-tl paritesinde alım yapılıp Binance'ta BTC-TRY paritesinde satış yapılır.
          <br />
          Çapraz Arbitrajda ise TRY ve USDT çapraz pariteleri kullanılır. Örneğin BtcTürk'te TRXTRY paritesinde alım yapılır, Binance'ta TRXUSDT paritesinden satılır. En baştaki TRY, sonradan USDT olmuştur.
        </v-card-text>

        <!-- filter() yapmazsam nedense güncellemiyor. -->
        <v-data-table dense fixed-header :headers="selectedHeaders" :items="oppsData.filter((x) => x.Symbol)" :items-per-page="-1" class="elevation-0 nowrap" mobile-breakpoint="0" style="width: 100%; min-height: 255px" :footer-props="{ 'items-per-page-options': [5, 10, 20, 25, 50, 100, -1] }">
          <template v-slot:body="{ items }">
            <tbody>
              <tr v-for="(item, i) in items" :key="i">
                <td v-if="headersFilter[0]" align="start" class="pr-0 pl-2 nowrap">{{ item.Symbol }}</td>
                <td v-if="headersFilter[1]" align="start" class="pr-0 pl-2 nowrap">{{ translateMode(item.Mode) }}</td>

                <td v-if="headersFilter[2]" align="center" class="px-0 mono nowrap">{{ translateExchange(item.Sender) }}</td>
                <td v-if="headersFilter[3]" align="center" class="px-0 mono nowrap">{{ translateExchange(item.Recipient) }}</td>

                <td v-if="headersFilter[4]" align="center" class="px-0 mono nowrap">{{ item.BuyMarketSymbol }}</td>
                <td v-if="headersFilter[5]" align="end" class="px-0 mono">{{ item.BuyPriceInSender }}</td>
                <td v-if="headersFilter[6]" align="end" class="px-0 mono">{{ item.BuyAmountInSender }}</td>

                <td v-if="headersFilter[7]" align="center" class="px-0 mono nowrap">{{ item.SellMarketSymbol }}</td>
                <td v-if="headersFilter[8]" align="end" class="px-0 mono">{{ item.Mode == 1 ? item.SellPriceInRecipient : item.CrossSellPriceInRecipient }}</td>
                <td v-if="headersFilter[9]" align="end" class="px-0 mono">{{ item.Mode == 1 ? item.SellAmountInRecipient : item.CrossSellAmountInRecipient }}</td>

                <td v-if="headersFilter[10]" align="end" class="px-0 mono">{{ item.TransferAmount }}</td>
                <td v-if="headersFilter[11]" align="end" class="px-0 mono nowrap">{{ item.TransferAsset }}</td>

                <td v-if="headersFilter[12]" align="end" class="px-0 mono">{{ item.Expense.toFixed(2) }}</td>
                <td v-if="headersFilter[13]" align="end" class="px-0 mono">{{ item.Revenue.toFixed(2) }}</td>

                <td v-if="headersFilter[14]" align="end" class="pl-0 p2-2 mono">{{ item.ProfitPercent }}</td>
                <td v-if="headersFilter[15]" align="end" class="pl-0 p2-2 mono">{{ item.ProfitAmount }}</td>
                <td v-if="headersFilter[16]" align="end" class="pl-0 p2-2 mono nowrap">{{ item.ProfitAsset }}</td>

                <td v-if="headersFilter[17]" align="end" class="pl-0 p2-2 mono nowrap">{{ item.ProfitAmountTRY }}</td>
                <td v-if="headersFilter[18]" align="end" class="pl-0 p2-2 mono nowrap">{{ item.ProfitAmountUSDT }}</td>
              </tr>
            </tbody>
          </template>
        </v-data-table>
      </v-card>
    </v-col>

    <v-dialog v-model="filterDialog" max-width="600" scrollable>
      <v-card>
        <v-card-title class="headline d-flex">
          <span>Sütunlar</span>
          <v-spacer />
          <v-btn @click="filterDialog = false" icon>
            <v-icon>{{ icons.mdiClose }}</v-icon>
          </v-btn>
        </v-card-title>
        <v-card-text class="mt-4">
          <v-row>
          <v-col cols="12" md="6" v-for="(item, i) in headers" :key="i" class="ma-0 py-0">
            <v-checkbox v-model="headersFilter[i]" :label="item.text" hide-details class="mt-2"></v-checkbox>
          </v-col>
          </v-row>
        </v-card-text>
        <v-card-actions> </v-card-actions>
      </v-card>
    </v-dialog>
    <v-dialog v-model="editFavsDialog" max-width="600" scrollable>
      <v-card :loading="editFavsLoading">
        <v-card-title class="headline d-flex">
          <span>Favorileri Düzenle</span>
          <v-spacer />
          <v-btn @click="editFavsDialog = false" icon>
            <v-icon>{{ icons.mdiClose }}</v-icon>
          </v-btn>
        </v-card-title>
        <v-divider></v-divider>
        <v-card-text class="mt-4">
          <v-row>
            <v-col cols="12" md="4" v-for="(item, i) in relatedMarkets" :key="i" class="ma-0 py-0">
              <v-checkbox v-model="editFavsRequest[item.SYMBOL]" :label="item.SYMBOL" hide-details class="mt-2"></v-checkbox>
            </v-col>
          </v-row>
        </v-card-text>
        <v-divider></v-divider>
        <v-card-actions>
          <v-btn block color="primary" @click="submitFavs" :loading="editFavsLoading" :disabled="editFavsLoading">Değişiklikleri Kaydet</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbars :objects.sync="snackMessages" bottom right>
      <template v-slot:action="{ close }">
        <v-btn text @click="close()">Kapat</v-btn>
      </template>
    </v-snackbars>
  </v-row>
</template>

<script>
import { mapState } from 'vuex';
import { mdiArrowRightBoldCircle, mdiFilter, mdiClose, mdiCellphone, mdiMonitor, mdiHeartHalfFull, mdiInformation } from '@mdi/js';

export default {
  components: {},
  data: () => ({
    epoch: null,
    epochInterval: null,
    loading: true,
    filterDialog: false,
    editFavsDialog: false,
    editFavsRequest: {},
    editFavsLoading: false,
    showInformation: false,
    icons: {
      mdiArrowRightBoldCircle,
      mdiFilter,
      mdiClose,
      mdiCellphone,
      mdiMonitor,
      mdiHeartHalfFull,
      mdiInformation,
    },

    headers: [
      { text: 'Parite', align: 'start', value: 'Symbol' },
      { text: 'Mod', align: 'start', value: 'Mode' },

      { text: 'Nereden', align: 'start', value: 'Sender' },
      { text: 'Nereye', align: 'start', value: 'Recipient' },

      { text: 'Alış Paritesi', align: 'center', value: 'BuyMarketSymbol' },
      { text: 'Alış', align: 'end', value: 'BuyPriceInSender' },
      { text: 'Alış Miktarı', align: 'end', value: 'BuyAmountInSender' },

      { text: 'Satış Paritesi', align: 'center', value: 'SellMarketSymbol' },
      { text: 'Satış', align: 'end', value: 'SellPriceInRecipient' },
      { text: 'Satış Miktarı', align: 'end', value: 'SellAmountInRecipient' },

      { text: 'İşlem Miktarı', align: 'end', value: 'TransferAmount' },
      { text: 'İşlem Varlığı', align: 'end', value: 'TransferAsset' },

      { text: 'Maliyet', align: 'end', value: 'Expense' },
      { text: 'Gelir', align: 'end', value: 'Revenue' },

      { text: 'Kar (%)', align: 'end', value: 'ProfitPercent' },
      { text: 'Kazanç', align: 'end', value: 'ProfitAmount' },
      { text: 'Kazanç Varlığı', align: 'end', value: 'ProfitAsset' },

      { text: 'TRY Kazanç', align: 'end', value: 'ProfitAmountTRY' },
      { text: 'USDT Kazanç', align: 'end', value: 'ProfitAmountUSDT' },
    ],

    headersFilter: [true, false, false, false, false, true, false, false, true, false, true, false, true, true, true, true, true, true, true],

    snackMessages: [],
  }),

  methods: {
    updateEpoch() {
      this.epoch = Date.now();
    },
    setForMobile() {
      this.headersFilter = [true, false, false, false, false, true, false, false, true, false, false, false, false, false, true, false, false, false, false];
    },
    setForDesktop() {
      this.headersFilter = [true, false, false, false, false, true, false, false, true, false, true, false, true, true, true, true, true, true, true];
    },

    getMarkets() {
      this.$api
        .oppsMarkets()
        .then((response) => {
          if (response.data.success) {
            this.getUserFavs();
          }
        })
        .catch((error) => {})
        .finally(() => {});
    },

    getUserFavs() {
      this.$api
        .accountFavs()
        .then((response) => {
          if (response.data.success) {
            this.loading = false;
          }
        })
        .catch((error) => {})
        .finally(() => {
          this.setEditFavsRequest();
          this.loading = false;
        });
    },

    setEditFavsRequest() {
      if (this.userFavs && this.oppsMarkets) {
        for (let i = 0; i < this.userFavs.length; i++) {
          let market = this.oppsMarkets.find((x) => x.ID == this.userFavs[i].MARKET_ID);
          if (market) {
            let m = this.mode.endsWith('C') ? 1 : 2;
            if (this.mode.startsWith('B2T') && this.userFavs[i].EXC_SENDER == 1 && this.userFavs[i].EXC_RECIPIENT == 3 && this.userFavs[i].MODE == m) {
              this.editFavsRequest[market.SYMBOL] = true;
            }
            if (this.mode.startsWith('B2P') && this.userFavs[i].EXC_SENDER == 1 && this.userFavs[i].EXC_RECIPIENT == 2 && this.userFavs[i].MODE == m) {
              this.editFavsRequest[market.SYMBOL] = true;
            }
            if (this.mode.startsWith('T2B') && this.userFavs[i].EXC_SENDER == 3 && this.userFavs[i].EXC_RECIPIENT == 1 && this.userFavs[i].MODE == m) {
              this.editFavsRequest[market.SYMBOL] = true;
            }
            if (this.mode.startsWith('T2P') && this.userFavs[i].EXC_SENDER == 3 && this.userFavs[i].EXC_RECIPIENT == 2 && this.userFavs[i].MODE == m) {
              this.editFavsRequest[market.SYMBOL] = true;
            }
            if (this.mode.startsWith('P2B') && this.userFavs[i].EXC_SENDER == 2 && this.userFavs[i].EXC_RECIPIENT == 1 && this.userFavs[i].MODE == m) {
              this.editFavsRequest[market.SYMBOL] = true;
            }
            if (this.mode.startsWith('P2T') && this.userFavs[i].EXC_SENDER == 2 && this.userFavs[i].EXC_RECIPIENT == 3 && this.userFavs[i].MODE == m) {
              this.editFavsRequest[market.SYMBOL] = true;
            }
          }
        }
      }
    },

    translateExchange(num) {
      if (num == 1) return 'Binance';
      if (num == 2) return 'Paribu';
      if (num == 3) return 'BtcTürk';
      return '';
    },

    translateMode(num) {
      if (num == 1) return 'Klasik';
      if (num == 2) return 'Çapraz';
      return '';
    },

    submitFavs: function (e) {
      this.editFavsLoading = true;

      let s = 0;
      let r = 0;
      let m = this.mode.endsWith('C') ? 1 : 2;
      if (this.mode.startsWith('B2T')) {
        s = 1;
        r = 3;
      }
      if (this.mode.startsWith('B2P')) {
        s = 1;
        r = 2;
      }
      if (this.mode.startsWith('T2B')) {
        s = 3;
        r = 1;
      }
      if (this.mode.startsWith('T2P')) {
        s = 3;
        r = 2;
      }
      if (this.mode.startsWith('P2B')) {
        s = 2;
        r = 1;
      }
      if (this.mode.startsWith('P2T')) {
        s = 2;
        r = 3;
      }

      this.$api
        .setAccountFavs(s, r, m, this.editFavsRequest)
        .then((response) => {
          if (response.data.success) {
            this.snackMessages.push({ message: 'Değişiklikler kaydedildi', color: 'green', timeout: 15000 });
            this.editFavsDialog=false;
          } else {
            let finalMessage = 'Bilinmeyen Hata';
            let errcode = response?.data?.error?.code;
            let errmessage = response?.data?.error?.message;
            if (errcode) finalMessage = errcode + ': ';
            if (errmessage) finalMessage += errmessage;
            this.snackMessages.push({ message: finalMessage, color: 'red', timeout: 15000 });
          }
        })
        .catch((error) => {
          let finalMessage = 'Bilinmeyen Hata';
          let errcode = error?.response?.data?.error?.code;
          let errmessage = error?.response?.data?.error?.message;
          if (errcode) finalMessage = errcode + ': ';
          if (errmessage) finalMessage += errmessage;
          this.snackMessages.push({ message: finalMessage, color: 'red', timeout: 15000 });
        })
        .finally(() => {
          this.editFavsLoading = false;
        });
    },
  },

  computed: {
    ...mapState(['session', 'userData', 'userFavs', 'oppsMarkets', 'oppsFavs', 'oppsB2TC', 'oppsB2PC', 'oppsT2BC', 'oppsT2PC', 'oppsP2BC', 'oppsP2TC', 'oppsB2TX', 'oppsB2PX', 'oppsT2BX', 'oppsT2PX', 'oppsP2BX', 'oppsP2TX']),

    mode: function () {
      return this.$route.params.mode === undefined ? 'FAVS' : this.$route.params.mode;
    },

    relatedMarkets: function () {
      if (this.mode == 'B2TC') return this.oppsMarkets.filter((x) => x.BINANCE_TO_BTCTURK_CLASSIC);
      if (this.mode == 'B2PC') return this.oppsMarkets.filter((x) => x.BINANCE_TO_PARIBU_CLASSIC);
      if (this.mode == 'T2BC') return this.oppsMarkets.filter((x) => x.BTCTURK_TO_BINANCE_CLASSIC);
      if (this.mode == 'T2PC') return this.oppsMarkets.filter((x) => x.BTCTURK_TO_PARIBU_CLASSIC);
      if (this.mode == 'P2BC') return this.oppsMarkets.filter((x) => x.PARIBU_TO_BINANCE_CLASSIC);
      if (this.mode == 'P2TC') return this.oppsMarkets.filter((x) => x.PARIBU_TO_BTCTURK_CLASSIC);

      if (this.mode == 'B2TX') return this.oppsMarkets.filter((x) => x.BINANCE_TO_BTCTURK_CROSS);
      if (this.mode == 'B2PX') return this.oppsMarkets.filter((x) => x.BINANCE_TO_PARIBU_CROSS);
      if (this.mode == 'T2BX') return this.oppsMarkets.filter((x) => x.BTCTURK_TO_BINANCE_CROSS);
      if (this.mode == 'T2PX') return this.oppsMarkets.filter((x) => x.BTCTURK_TO_PARIBU_CROSS);
      if (this.mode == 'P2BX') return this.oppsMarkets.filter((x) => x.PARIBU_TO_BINANCE_CROSS);
      if (this.mode == 'P2TX') return this.oppsMarkets.filter((x) => x.PARIBU_TO_BTCTURK_CROSS);

      return [];
    },

    oppsData: function () {
      if (this.mode == 'FAVS') return this.oppsFavs;

      if (this.mode == 'B2TC') return this.oppsB2TC;
      if (this.mode == 'B2PC') return this.oppsB2PC;
      if (this.mode == 'T2BC') return this.oppsT2BC;
      if (this.mode == 'T2PC') return this.oppsT2PC;
      if (this.mode == 'P2BC') return this.oppsP2BC;
      if (this.mode == 'P2TC') return this.oppsP2TC;

      if (this.mode == 'B2TX') return this.oppsB2TX;
      if (this.mode == 'B2PX') return this.oppsB2PX;
      if (this.mode == 'T2BX') return this.oppsT2BX;
      if (this.mode == 'T2PX') return this.oppsT2PX;
      if (this.mode == 'P2BX') return this.oppsP2BX;
      if (this.mode == 'P2TX') return this.oppsP2TX;

      return [];
    },

    selectedHeaders: function () {
      let data = [];

      for (let i = 0; i < this.headersFilter.length; i++) {
        if (this.headersFilter[i]) {
          data.push(this.headers[i]);
        }
      }

      return data;
    },
  },

  watch: {
    headersFilter(val) {
      localStorage.setItem('columns', JSON.stringify(val));
    },
  },

  created: function () {
    // console.log('created');
    if (!this.userFavs || this.userFavs.length <= 0) {
      this.getMarkets();
    } else {
      this.setEditFavsRequest();
      this.loading = false;
    }
  },

  mounted: function () {
    // console.log('mounted');
    this.epochInterval = setInterval(this.updateEpoch, 500);

    let columns = localStorage.getItem('columns');
    if (columns) this.headersFilter = JSON.parse(columns);
  },

  destroyed: function () {
    // console.log('destroyed');
    clearInterval(this.epochInterval);
  },
};
</script>
<style scoped>
.mono {
  font-family: sans-serif !important;
}
.nowrap {
  white-space: nowrap;
}
</style>