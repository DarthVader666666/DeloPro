<script setup>
import HeaderComponent from './components/HeaderComponent.vue'
import FooterComponent from './components/FooterComponent.vue'
import SearchBar from './components/SearchBar.vue'
import MainComponent from './components/MainComponent.vue'
import { useStore } from 'vuex'
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import PendingModal from './components/PendingModal.vue'

const store = useStore()
const showSearchBar = computed(() => store.state.showSearchBar)
const route = useRoute()
const pending = computed(() => store.getters.getPending)
</script>

<template>
	<div class="app-container">
		<HeaderComponent />
		<div class="search-bar">
			<SearchBar
				v-if="showSearchBar"
				@hide-modal="() => {}"
			/>
		</div>
		<MainComponent />
		<FooterComponent v-show="route.name != 'feedback'" />
		<PendingModal v-model:visible="pending"></PendingModal>
	</div>
</template>

<style>
.app-container {
	padding: var(--APP-PADDING);
}

.search-bar {
	animation-name: slide-down;
	animation-duration: 0.2s;
	transform: translateY(0%);
}

@keyframes slide-down {
	0% {
		transform: translateY(-30%);
	}
	100% {
		transform: translateY(0%);
	}
}

@media (max-width: 1000px) {
	.search-bar {
		display: none;
	}
}

@media (max-width: 1600px) {
	.app-container {
		padding: 0;
	}
}
</style>
