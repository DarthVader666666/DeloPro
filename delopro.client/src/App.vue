<script setup>
import HeaderComponent from './components/HeaderComponent.vue'
import FooterComponent from './components/FooterComponent.vue'
import SearchBar from './components/SearchBar.vue'
import MainComponent from './components/MainComponent.vue'
import { useStore } from 'vuex'
import { computed } from 'vue'
import { useMeta } from './composables/useMeta'

// Use custom meta composable
useMeta({
	title: 'DeloPro | Профессиональное делопроизводство',
	htmlAttrs: {
		lang: 'ru',
	},
	meta: [
		{
			name: 'description',
			content:
				'Профессиональные консультации по документационному обеспечению управления, оформлению документов и архивному хранению. Будьте всегда в курсе всех требований, предъявляемых к делопроизводству организации.',
		},
		{
			name: 'keywords',
			content:
				'делопроизводство, документационное обеспечение управления, документооборот, система регистрации и учёта документов, архивное хранение документов, оформление, документ, документы, архив, секретарское дело',
		},
		{
			property: 'og:title',
			content: 'DeloPro | Профессиональное делопроизводство',
		},
		{
			property: 'og:description',
			content:
				'Профессиональные консультации по делопроизводству, оформлению документов и архивному хранению.',
		},
		{
			property: 'og:image',
			content: 'https://delopro.site/src/assets/delopro.png',
		},
	],
})

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
	padding: 2%;
}

@media (max-width: 1400px) {
	.app-container {
		padding-left: 0;
		padding-right: 0;
	}
}

@media (max-width: 1000px) {
	.app-container {
		padding: 0;
	}
}
</style>
