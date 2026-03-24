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
		numVisible: 3,
		numScroll: 1,
	},
	{
		breakpoint: '500px',
		numVisible: 2.5,
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
								<img :src="helper.getImagePath('chapter') + slotProps.data.imagePath" />
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

.chapter-links :deep(.p-button-text.p-button-secondary) {
	background: lightgray;
}

.chapter {
	height: 220px;
	width: 240px;
	padding: 18px;
}

.chapter-image {
	position: relative;
}

.chapter-image img {
	height: 180px;
	width: 220px;
}

.chapter-image p {
	position: absolute;
	font-size: medium;
	text-align: center;
	left: 0;
	right: 0;
	bottom: 10%;
	font-weight: bold;
	color: var(--MENU-TEXT-COLOR);
	text-shadow:
		2px -2px 3px rgba(0, 0, 0, 1),
		-2px 2px 3px rgba(0, 0, 0, 1),
		2px 2px 3px rgba(0, 0, 0, 1);
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

:deep(.p-carousel-indicator-active .p-carousel-indicator-button) {
	background: rgb(50, 50, 50);
}

:deep(.p-carousel-indicator-button) {
	background: lightgray;
}

@media (max-width: 1000px) {
	.chapter {
		width: 200px;
		height: 180px;
	}
	.chapter-image img {
		width: 180px;
		height: 150px;
	}
}

@media (max-width: 800px) {
	.chapter {
		width: 180px;
		height: 150px;
	}
	.chapter-image img {
		width: 150px;
		height: 130px;
	}
	.chapter-image p {
		font-size: small;
	}
}

@media (max-width: 600px) {
	.chapter {
		width: 150px;
		height: 130px;
	}
	.chapter-image img {
		width: 130px;
		height: 100px;
	}
	.chapter-image p {
		font-size: small;
	}
	h2 {
		font-size: medium;
	}
	.chapters-header {
		margin: 0;
	}
}

@media (max-width: 500px) {
	.chapter {
		width: 120px;
		height: 100px;
	}
	.chapter-image img {
		width: 100px;
		height: 80px;
	}
}
</style>
