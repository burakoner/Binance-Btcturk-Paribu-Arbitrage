<template>
  <div>
    <v-overlay :value="pageLoading">
      <v-progress-circular indeterminate size="64"></v-progress-circular>
    </v-overlay>
    <v-card id="account-setting-card" v-if="!pageLoading" :loading="cardLoading">
      <v-tabs v-model="tab" show-arrows v-if="!cardLoading">
        <v-tab style="letter-spacing: 0px">Hesabım</v-tab>
        <!--<v-tab style="letter-spacing: 0px">API Bilgileri</v-tab>-->
        <v-tab style="letter-spacing: 0px">Telegram</v-tab>
      </v-tabs>
      <v-tabs-items v-model="tab" v-if="!pageLoading">
        <v-tab-item>
          <v-card flat class="mt-5">
            <v-card-text>
              <v-row>
                <v-col md="6" cols="12">
                  <v-text-field v-model="userData.EMAIL" label="E-Mail" dense readonly></v-text-field>
                </v-col>
                <v-col md="6" cols="12"></v-col>

                <v-col md="6" cols="12">
                  <v-text-field v-model="userData.NAME" label="İsim" dense></v-text-field>
                </v-col>
                <v-col md="6" cols="12"></v-col>

                <v-col md="6" cols="12">
                  <v-text-field v-model="userData.SURNAME" label="Soyisim" dense></v-text-field>
                </v-col>
                <v-col md="6" cols="12"></v-col>

                <v-col md="6" cols="12">
                  <v-text-field v-model="passw" :type="isPasswordVisible ? 'text' : 'password'" :append-icon="isPasswordVisible ? icons.mdiEyeOffOutline : icons.mdiEyeOutline" label="Yeni Şifre" dense @click:append="isPasswordVisible = !isPasswordVisible"></v-text-field>
                </v-col>
                <v-col md="6" cols="12"></v-col>
              </v-row>
            </v-card-text>
            <v-divider></v-divider>
            <v-card-actions class="pa-5">
              <v-btn color="primary" @click="submitOne"> Değişiklikleri Kaydet</v-btn>
            </v-card-actions>
          </v-card>
        </v-tab-item>

        <!--
        <v-tab-item>
          <v-card flat class="mt-5">
            <v-card-text>
              <v-row>
                <v-col md="6" cols="12">
                  <v-text-field v-model="userData.PARIBU_TOKEN" label="Paribu Token" dense></v-text-field>
                </v-col>
                <v-col md="6" cols="12"></v-col>

                <v-col md="6" cols="12">
                  <v-text-field v-model="userData.PARIBU_FEE_MAKER" label="Paribu Maker Fee" dense type="number"></v-text-field>
                </v-col>
                <v-col md="6" cols="12"></v-col>

                <v-col md="6" cols="12">
                  <v-text-field v-model="userData.PARIBU_FEE_TAKER" label="Paribu Taker Fee" dense type="number"></v-text-field>
                </v-col>
                <v-col md="6" cols="12"></v-col>
              </v-row>
            </v-card-text>
            <v-divider></v-divider>
            <v-card-text>
              <v-row>
                <v-col md="6" cols="12">
                  <v-text-field v-model="userData.BINANCE_APIKEY" label="Binance Api Key" dense></v-text-field>
                </v-col>
                <v-col md="6" cols="12"></v-col>

                <v-col md="6" cols="12">
                  <v-text-field v-model="userData.BINANCE_SECRET" label="Binance Api Secret" dense></v-text-field>
                </v-col>
                <v-col md="6" cols="12"></v-col>

                <v-col md="6" cols="12">
                  <v-text-field v-model="userData.BINANCE_FEE_MAKER" label="Binance Maker Fee" dense type="number"></v-text-field>
                </v-col>
                <v-col md="6" cols="12"></v-col>

                <v-col md="6" cols="12">
                  <v-text-field v-model="userData.BINANCE_FEE_TAKER" label="Binance Taker Fee" dense type="number"></v-text-field>
                </v-col>
                <v-col md="6" cols="12"></v-col>
              </v-row>
            </v-card-text>
            <v-divider></v-divider>
            <v-card-actions class="pa-5">
              <v-btn color="primary" @click="submitTwo"> Değişiklikleri Kaydet</v-btn>
            </v-card-actions>
          </v-card>
        </v-tab-item>
        -->

        <v-tab-item>
          <v-card flat class="mt-5">
            <v-card-text>
              <v-row>
                <v-col md="6" cols="12">
            <v-checkbox v-model="userData.TELEGRAM_ACTIVE" label="Telegram Bildirimleri" class="mt-2"></v-checkbox>
                </v-col>
                <v-col md="6" cols="12"></v-col>

                <v-col md="6" cols="12">
                  <v-text-field v-model="userData.TELEGRAM_USER_ID" label="Telegram Chat Id" persistent-hint hint="@ArbitrajBildirimleriBot botundaki Chat Id" ></v-text-field>
                </v-col>
                <v-col md="6" cols="12"></v-col>

                <v-col md="6" cols="12">
                  <v-text-field v-model="userData.TELEGRAM_PERCENT" label="Bildirim Yüzdesi" persistent-hint hint="Bu eşik değerin üzerindeki fırsatlar bildirilir."  type="number"></v-text-field>
                </v-col>
                <v-col md="6" cols="12"></v-col>

                <v-col md="6" cols="12">
                  <v-text-field v-model="userData.TELEGRAM_INTERVAL" label="Bildirim Sıklığı" persistent-hint hint="Aynı paritedeki bildirimler arası süre (dakika). Minimum: 5"  type="number"></v-text-field>
                </v-col>
                <v-col md="6" cols="12"></v-col>

                <v-col md="6" cols="12">
                      <v-checkbox v-model="telegramChannels.B2TC" label="Binance > BtcTürk (Klasik)" hide-details class="mt-2"></v-checkbox>
                      <v-checkbox v-model="telegramChannels.B2PC" label="Binance > Paribu (Klasik)" hide-details class="mt-2"></v-checkbox>
                      <v-checkbox v-model="telegramChannels.T2BC" label="BtcTürk > Binance (Klasik)" hide-details class="mt-2"></v-checkbox>
                      <v-checkbox v-model="telegramChannels.T2PC" label="BtcTürk > Paribu (Klasik)" hide-details class="mt-2"></v-checkbox>
                      <v-checkbox v-model="telegramChannels.P2BC" label="Paribu > Binance (Klasik)" hide-details class="mt-2"></v-checkbox>
                      <v-checkbox v-model="telegramChannels.P2TC" label="Paribu > BtcTürk (Klasik)" hide-details class="mt-2"></v-checkbox>
                </v-col>
                <v-col md="6" cols="12"></v-col>
                <v-col md="6" cols="12">
                      <v-checkbox v-model="telegramChannels.B2TX" label="Binance > BtcTürk (Çapraz)" hide-details class="mt-2"></v-checkbox>
                      <v-checkbox v-model="telegramChannels.B2PX" label="Binance > Paribu (Çapraz)" hide-details class="mt-2"></v-checkbox>
                      <v-checkbox v-model="telegramChannels.T2BX" label="BtcTürk > Binance (Çapraz)" hide-details class="mt-2"></v-checkbox>
                      <v-checkbox v-model="telegramChannels.T2PX" label="BtcTürk > Paribu (Çapraz)" hide-details class="mt-2"></v-checkbox>
                      <v-checkbox v-model="telegramChannels.P2BX" label="Paribu > Binance (Çapraz)" hide-details class="mt-2"></v-checkbox>
                      <v-checkbox v-model="telegramChannels.P2TX" label="Paribu > BtcTürk (Çapraz)" hide-details class="mt-2"></v-checkbox>
                </v-col>
                <v-col md="6" cols="12"></v-col>

                <v-col md="6" cols="12"></v-col>
              </v-row>
            </v-card-text>
            <v-divider></v-divider>
            <v-card-actions class="pa-5">
              <v-btn color="primary" @click="submitThree"> Değişiklikleri Kaydet</v-btn>
            </v-card-actions>
          </v-card>
        </v-tab-item>
      </v-tabs-items>

      <v-dialog v-model="dialogError" persistent max-width="400">
        <v-card>
          <v-card-title class="headline">Hata</v-card-title>
          <v-card-text class="d-flex mt-4">
            <v-icon color="error" large>mdi-alert-circle-outline</v-icon>
            <span class="ml-2 pt-2">{{ dialogErrorMessage }}</span>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="primary" text @click="dialogError = false">OK</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <v-dialog v-model="dialogSuccess" persistent max-width="400">
        <v-card>
          <v-card-title class="headline">Başarılı</v-card-title>
          <v-card-text class="d-flex mt-4">
            <v-icon color="success" large>mdi-check-circle-outline</v-icon>
            <span class="ml-2 pt-2">{{ dialogSuccessMessage }}</span>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="primary" text @click="dialogSuccess = false">OK</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <v-snackbars :objects.sync="snackMessages" bottom right>
        <template v-slot:action="{ close }">
          <v-btn text @click="close()">Kapat</v-btn>
        </template>
      </v-snackbars>
    </v-card>
  </div>
</template>

<script>
import { mapState } from 'vuex';
import { mdiEyeOutline, mdiEyeOffOutline, mdiArrowRightBoldCircle } from '@mdi/js';

export default {
  components: {},

  data: () => ({
    tab: 0,
    pageLoading: true,
    cardLoading: false,
    isPasswordVisible: false,

    passw: '',
    icons: {
      mdiEyeOutline,
      mdiEyeOffOutline,
      mdiArrowRightBoldCircle,
    },

    dialogError: false,
    dialogErrorMessage: '',

    dialogSuccess: false,
    dialogSuccessMessage: '',

    snackMessages: [],
    telegramChannels: {},
  }),

  methods: {
    getUserData: function (e) {
      this.$api
        .accountData()
        .then((response) => {})
        .catch((error) => {})
        .finally(() => {
          this.telegramChannels = JSON.parse(this.userData.TELEGRAM_CHANNELS);
          this.pageLoading = false;
        });
    },

    submitOne: function (e) {
      this.cardLoading = true;

      this.$api
        .setAccountData(this.userData.EMAIL, this.userData.NAME, this.userData.SURNAME, this.passw)
        .then((response) => {
          if (response.data.success) {
            this.snackMessages.push({ message: 'Değişiklikler kaydedildi', color: 'green', timeout: 15000 });
          } else {
            this.dialogErrorMessage = 'Bilinmeyen Hata';
            let errcode = response?.data?.error?.code;
            let errmessage = response?.data?.error?.message;
            if (errcode) this.dialogErrorMessage = errcode + ': ';
            if (errmessage) this.dialogErrorMessage += errmessage;
            this.dialogError = true;
          }
        })
        .catch((error) => {
          this.dialogErrorMessage = 'Bilinmeyen Hata';
          let errcode = error?.response?.data?.error?.code;
          let errmessage = error?.response?.data?.error?.message;
          if (errcode) this.dialogErrorMessage = errcode + ': ';
          if (errmessage) this.dialogErrorMessage += errmessage;
          this.dialogError = true;
        })
        .finally(() => {
          this.cardLoading = false;
        });
    },

    submitTwo: function (e) {
      this.cardLoading = true;

      this.$api
        .setAccountApi(this.userData)
        .then((response) => {
          if (response.data.success) {
            this.snackMessages.push({ message: 'Değişiklikler kaydedildi', color: 'green', timeout: 15000 });
          } else {
            this.dialogErrorMessage = 'Bilinmeyen Hata';
            let errcode = response?.data?.error?.code;
            let errmessage = response?.data?.error?.message;
            if (errcode) this.dialogErrorMessage = errcode + ': ';
            if (errmessage) this.dialogErrorMessage += errmessage;
            this.dialogError = true;
          }
        })
        .catch((error) => {
          this.dialogErrorMessage = 'Bilinmeyen Hata';
          let errcode = error?.response?.data?.error?.code;
          let errmessage = error?.response?.data?.error?.message;
          if (errcode) this.dialogErrorMessage = errcode + ': ';
          if (errmessage) this.dialogErrorMessage += errmessage;
          this.dialogError = true;
        })
        .finally(() => {
          this.cardLoading = false;
        });
    },
    
    submitThree: function (e) {
      this.cardLoading = true;
      this.userData.TELEGRAM_CHANNELS = JSON.stringify(this.telegramChannels);

      this.$api
        .setAccountTelegram(this.userData)
        .then((response) => {
          if (response.data.success) {
            this.snackMessages.push({ message: 'Değişiklikler kaydedildi', color: 'green', timeout: 15000 });
          } else {
            this.dialogErrorMessage = 'Bilinmeyen Hata';
            let errcode = response?.data?.error?.code;
            let errmessage = response?.data?.error?.message;
            if (errcode) this.dialogErrorMessage = errcode + ': ';
            if (errmessage) this.dialogErrorMessage += errmessage;
            this.dialogError = true;
          }
        })
        .catch((error) => {
          this.dialogErrorMessage = 'Bilinmeyen Hata';
          let errcode = error?.response?.data?.error?.code;
          let errmessage = error?.response?.data?.error?.message;
          if (errcode) this.dialogErrorMessage = errcode + ': ';
          if (errmessage) this.dialogErrorMessage += errmessage;
          this.dialogError = true;
        })
        .finally(() => {
          this.cardLoading = false;
        });
    },

  },

  computed: {
    ...mapState(['session', 'userData']),
  },

  created() {
    //console.log("created");
    this.getUserData();
  },

  mounted() {
    //console.log("mounted");
  },

  updated() {
    //console.log("updated");
  },

  destroyed() {
    //console.log("destroyed");
  },
};
</script>
