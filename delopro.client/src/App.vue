<script setup>
import HeaderComponent from './components/HeaderComponent.vue';
import FooterComponent from './components/FooterComponent.vue';
import SearchBar from './components/SearchBar.vue';
import MainComponent from './components/MainComponent.vue';
import { useStore } from 'vuex';
import { computed, onMounted, onUnmounted } from 'vue';
import { helper } from './helper/helper';

const store = useStore();
const showSearchBar = computed(() => store.state.showSearchBar);

onMounted(async () => {
    if(await helper.isAuthenticated()) {
        await store.dispatch('downloadCurrentUser');
    }
    else {
        helper.clearSession();
    }

    await store.dispatch('downloadChapters');
    await store.dispatch('downloadDocuments');
    await store.dispatch('downloadImageNames');
})

</script>

<template>
  <HeaderComponent/>
  <SearchBar v-if="showSearchBar"/>
  <MainComponent/>
  <FooterComponent/>
</template>

<style scoped>
</style>
