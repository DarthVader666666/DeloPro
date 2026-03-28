<script setup>
import HeaderComponent from './components/HeaderComponent.vue'
import FooterComponent from './components/FooterComponent.vue'
import SearchBar from './components/SearchBar.vue'
import MainComponent from './components/MainComponent.vue'
import { useStore } from 'vuex'
import { computed } from 'vue'

const store = useStore()
const showSearchBar = computed(() => store.state.showSearchBar)
</script>

<template>
	<div class="app-container">
		<HeaderComponent />
		<SearchBar v-if="showSearchBar" />
		<MainComponent />
		<FooterComponent />
	</div>
</template>

<style scoped>
.app-container {
	padding: var(--APP-PADDING);
}

@media (max-width: 1600px) {
	.app-container {
		padding: 0;
	}
}
</style>
