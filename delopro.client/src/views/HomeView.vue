<script setup>
import { useStore } from 'vuex'
import { computed, ref } from 'vue'
import { RouterLink } from 'vue-router'
import { helper } from '@/helper/helper'
import SpinningCircle from '@/components/SpinningCircle.vue'
import Carousel from 'primevue/carousel'

const store = useStore()
const chapters = computed(() => store.getters.getChapters)
const pending = computed(() => store.getters.getPending)

const responsiveOptions = ref([
	{
		breakpoint: '2000px',
		numVisible: 4,
		numScroll: 1,
	},
	{
		breakpoint: '1800px',
		numVisible: 3,
		numScroll: 1,
	},
	{
		breakpoint: '1400px',
		numVisible: 3,
		numScroll: 1,
	},
	{
		breakpoint: '1000px',
		numVisible: 3.5,
		numScroll: 1,
	},
	{
		breakpoint: '800px',
		numVisible: 3.2,
		numScroll: 1,
	},
	{
		breakpoint: '600px',
		numVisible: 2.5,
		numScroll: 1,
	},
	{
		breakpoint: '500px',
		numVisible: 2,
		numScroll: 1,
	},
])
</script>

<template>
	<div>
		<div class="chapters-header">
			<h2>Документационное обеспечение управления</h2>
		</div>

		<SpinningCircle v-if="pending"></SpinningCircle>

		<div
			class="chapter-links"
			v-else
		>
			<Carousel
				:value="chapters"
				:responsiveOptions="responsiveOptions"
			>
				<template #item="slotProps">
					<div class="chapter">
						<RouterLink
							:to="
								`/chapters/${slotProps.data.chapterId}` +
								`${slotProps.data.themes.length > 0 ? '/' + slotProps.data.themes[0].themeId : ''}`
							"
						>
							<div class="chapter-image">
								<img
									:src="helper.getImagePath('chapter') + slotProps.data.imagePath"
									width="220px"
									height="200px"
								/>
								<p>{{ slotProps.data.chapterTitle }}</p>
							</div>
						</RouterLink>
					</div>
				</template>
			</Carousel>
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
	padding-top: 15px;
	margin: 0 10px 0 10px;
}

.chapter {
	max-height: 200px;
	max-width: 220px;
}

.chapter p {
	position: absolute;
	font-size: medium;
	text-align: center;
	font-weight: bold;
	bottom: 10%;
	color: rgb(240, 240, 240);
	text-shadow: 2px 2px 3px rgba(0, 0, 0);
}

.chapter:hover {
	-webkit-transform: scale(1.1);
	-moz-transform: scale(1.1);
	-o-transform: scale(1.1);
	transform: scale(1.1);
	opacity: 0.8;
	cursor: pointer;
}

.chapter {
	-webkit-transition: all 0.2s ease-in-out;
	-moz-transition: all 0.2s ease-in-out;
	-o-transition: all 0.2s ease-in-out;
	transition: all 0.2s ease-in-out;
	filter: drop-shadow(var(--PNG-IMAGE-SHADOW));
}

@media (max-width: 1000px) {
	.chapter {
		max-width: 180px;
		max-height: 160px;
	}
	.chapter img {
		max-width: 180px;
		max-height: 160px;
	}
}

@media (max-width: 800px) {
	.chapter {
		max-width: 140px;
		max-height: 130px;
	}
	.chapter img {
		max-width: 140px;
		max-height: 130px;
	}
}

@media (max-width: 600px) {
	.chapter {
		max-width: 120px;
		max-height: 110px;
	}
	.chapter img {
		max-width: 120px;
		max-height: 110px;
	}

	.chapter p {
		font-size: small;
	}

	h2 {
		font-size: medium;
	}

	.chapters-header {
		margin: 0;
	}
}
</style>
