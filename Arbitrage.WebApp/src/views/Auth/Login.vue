<template>
  <div class="auth-wrapper auth-v1">
    <div class="auth-inner">
      <v-card class="auth-card" :loading="loading">
        <!-- {{$t('api.e1355')}} -->

        <!-- login form -->
        <v-card-text>
          <v-form>
            <v-text-field class="mb-5 mt-10" v-model="email" label="Email"></v-text-field>
            <v-text-field class="mb-5" v-model="passw" :type="isPasswordVisible ? 'text' : 'password'" label="Şifre" :append-icon="isPasswordVisible ? icons.mdiEyeOffOutline : icons.mdiEyeOutline" @click:append="isPasswordVisible = !isPasswordVisible"></v-text-field>
            <v-btn block color="primary" class="mt-6" @click="onSubmit"> Giriş </v-btn>
          </v-form>
        </v-card-text>
      </v-card>
    </div>

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
  </div>
</template>

<script>
import { mapState } from 'vuex';
import { mdiEyeOutline, mdiEyeOffOutline } from '@mdi/js';

export default {
  data: () => ({
    isPasswordVisible: false,
    email: '',
    passw: '',
    icons: {
      mdiEyeOutline,
      mdiEyeOffOutline,
    },

    loading: false,

    dialogError: false,
    dialogErrorMessage: '',

    dialogSuccess: false,
    dialogSuccessMessage: '',

    loginAccessToken: '',
  }),

  methods: {
    onSubmit: function (e) {
      this.loading = true;

      this.$api
        .authLogin(this.email, this.passw)
        .then((response) => {
          if (response.data.success) {
            this.loginAccessToken = response.data.data.accessToken;
              const sessObj = {
                loggedIn: true,
                accountId: response.data.data.accountId,
                accessToken: response.data.data.accessToken,
              };

              this.$store.commit('SESSION_LOGIN', sessObj);
              // this.$router.push({ name: 'home' }).catch((err) => {});
              this.getLoggedInAccountData();
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
          this.loading = false;
        });
    },

    getLoggedInAccountData: function () {
      this.$api
        .accountData()
        .then((response) => {
          if (response.data.success) {
            this.$router.push({ name: 'dashboard' }).catch((err) => {});
          } else {
            this.$router.push({ name: 'auth-logout' }).catch((err) => {});
          }
        })
        .catch((error) => {
          this.dialogErrorMessage = 'Bilinmeyen Hata';
          let errcode = error?.response?.data?.error?.code;
          let errmessage = error?.response?.data?.error?.message;
          if (errcode) this.dialogErrorMessage = errcode + ': ';
          if (errmessage) this.dialogErrorMessage += errmessage;
          this.dialogError = true;
        });
    },
  },

  computed: {
    ...mapState(['session']),
  },
};
</script>

<style lang="scss">
@import '~@/plugins/vuetify/default-preset/preset/pages/auth.scss';
</style>
