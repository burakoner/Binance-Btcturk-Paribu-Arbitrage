<template></template>

<script>
export default {
  created() {
    if (this.$route.params.routeName) {
      this.$router
        .push({
          name: this.$route.params.routeName,
          params: this.$route.params.routeParams
        })
        .catch(err => {})
    } else {
      this.$router
        .push({
          name: 'home'
        })
        .catch(err => {})
    }
  }
}
</script>
