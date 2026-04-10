<script setup>
import HeaderComponent from './components/HeaderComponent.vue'
import FooterComponent from './components/FooterComponent.vue'
import SearchBar from './components/SearchBar.vue'
import MainComponent from './components/MainComponent.vue'
import { useStore } from 'vuex'
import { computed, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import PendingModal from './components/PendingModal.vue'
import { helper } from './helper/helper'

const store = useStore()
const showSearchBar = computed(() => store.state.showSearchBar)
const route = useRoute()
const pending = computed(() => store.getters.getPending)
const showSpinner = ref(false)

watch(pending, async (newValue) => {
	if (newValue) {
		await helper.timeoutAsync(500)

		if (pending.value) {
			showSpinner.value = true
		}
	} else {
		showSpinner.value = false
	}
})
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
		<MainComponent :showSpinner="showSpinner" />
		<FooterComponent v-show="route.name != 'feedback'" />
		<PendingModal v-model:visible="showSpinner"></PendingModal>
	</div>
</template>

<style>
.app-container {
	padding: var(--APP-PADDING);
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
