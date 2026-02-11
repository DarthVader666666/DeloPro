<script setup>
import HeaderComponent from './components/HeaderComponent.vue';
import FooterComponent from './components/FooterComponent.vue';
import SearchBar from './components/SearchBar.vue';
import MainComponent from './components/MainComponent.vue';
import { useStore } from 'vuex';
import { computed, onMounted } from 'vue';
import { useCookies } from 'vue3-cookies';
import { helper } from './helper/helper';

const store = useStore();
const cookieManager = useCookies();
const showSearchBar = computed(() => store.state.showSearchBar);
const coockieName = store.getters.getCookieName;

onMounted(async () => {
    const cookie = cookieManager.cookies.get(coockieName);

    if(cookie) {   
        await store.dispatch('downloadCurrentUser');
    }
    else {
        helper.clearSession();
    }

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
