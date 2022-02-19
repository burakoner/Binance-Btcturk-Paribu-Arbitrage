<template>
  <v-btn icon @click="$vuetify.theme.dark = !$vuetify.theme.dark">
    <v-icon>{{ $vuetify.theme.dark ? icons.mdiWeatherSunny : icons.mdiWeatherNight }}</v-icon>
  </v-btn>
</template>

<script>
import { mdiWeatherNight, mdiWeatherSunny } from '@mdi/js';

export default {
  setup() {
    return {
      icons: {
        mdiWeatherNight,
        mdiWeatherSunny,
      },
    };
  },
};
</script>