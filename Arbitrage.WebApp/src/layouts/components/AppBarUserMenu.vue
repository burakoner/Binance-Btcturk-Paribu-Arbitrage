<template>
  <v-btn icon @click="logout">
    <v-icon>{{ icons.mdiLogout }}</v-icon>
  </v-btn>
</template>

<script>
import { mdiLogout } from '@mdi/js';

export default {
  setup() {
    return {
      icons: {
        mdiLogout,
      },
    };
  },

  methods:{
    logout: function () {
      this.$router
        .push({
          name: 'auth-logout',
        })
        .catch((err) => {});
    },
  }
};
</script>
