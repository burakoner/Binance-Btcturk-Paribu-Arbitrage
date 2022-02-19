<template>
  <div class="auth-wrapper auth-v1">
    <div class="auth-inner">
      <v-card class="auth-card" :loading="loading">


        <v-card-text style="text-align: center">
          <p class="text-2xl font-weight-semibold text--primary mb-2">Çıkış Başarılı</p>
          <p class="mb-2">Kullanıcı çıkışı tamamlandı</p>
        </v-card-text>

        <!-- create new account  -->
        <v-card-text class="d-flex align-center justify-center flex-wrap mt-2">
          <router-link :to="{ name: 'auth-login' }"> Giriş Sayfası </router-link>
        </v-card-text>

      </v-card>
    </div>
  </div>
</template>

<script>
import { mapState } from 'vuex';
export default {
  data: () => ({
    loading: true,
  }),

  methods: {
    goToHome: function () {
      this.$router
        .push({
          name: 'home',
        })
        .catch((err) => {});
    },

    goToLogin: function () {
      this.$router
        .push({
          name: 'auth-login',
        })
        .catch((err) => {});
    },
  },

  mounted: function () {
    this.loading = true;

    this.$api
      .authLogout()
      .then((response) => {})
      .catch((error) => {})
      .finally(() => {
        this.$store.commit('SESSION_LOGOUT');
        this.loading = false;
        this.goToLogin();
      });
  },
};
</script>

<style lang="scss">
@import '~@/plugins/vuetify/default-preset/preset/pages/auth.scss';
</style>
