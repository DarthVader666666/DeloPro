<script setup>
import { useStore } from 'vuex'
import { computed } from 'vue'
import SpinningCircle from '@/components/SpinningCircle.vue'
import ChapterCarousel from '../components/ChapterCarousel.vue'

const store = useStore()
const pending = computed(() => store.getters.getPending)
</script>

<template>
	<div>
		<div class="chapters-header">
			<h2>Документационное обеспечение управления</h2>
		</div>

		<SpinningCircle v-if="pending" />
		<div
			class="chapter-links"
			v-else
		>
			<ChapterCarousel></ChapterCarousel>
		</div>
	</div>
</template>

<style scoped>
.chapters-header {
	text-align: center;
	margin: 15px;
	padding: 1px;
	background: var(--MENU-BCKGND-CLR);
	color: var(--MENU-TEXT-COLOR);
}

.chapter-links {
	height: 300px;
	padding-top: 15px;
	margin: 0 10px 0 10px;
}

@media (max-width: 800px) {
	h2 {
		font-size: medium;
	}
	.chapters-header {
		margin: 0;
	}
}
</style>
