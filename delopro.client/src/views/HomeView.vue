<script setup>
import { useStore } from 'vuex'
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'
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
		numVisible: 3,
		numScroll: 1,
	},
	{
		breakpoint: '1200px',
		numVisible: 2,
		numScroll: 1,
	},
	{
		breakpoint: '1100px',
		numVisible: 3,
		numScroll: 1,
	},
	{
		breakpoint: '950px',
		numVisible: 3,
		numScroll: 1,
	},
	{
		breakpoint: '800px',
		numVisible: 2,
		numScroll: 1,
	},
])

const width = ref(window.innerWidth)

const onResize = () => {
	width.value = window.innerWidth
}

onMounted(() => {
	window.addEventListener('resize', onResize)
})

onBeforeUnmount(() => {
	window.removeEventListener('resize', onResize)
})

const isMobile = computed(() => width.value <= 500)

function chunkArray(arr, size = 3) {
	const result = []
	for (let i = 0; i < arr.length; i += size) {
		result.push(arr.slice(i, i + size))
	}
	return result
}

const chunkedChapters = computed(() =>
	isMobile.value ? chunkArray(chapters.value || [], 3) : chapters.value,
)
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
			<Carousel
				v-if="!isMobile"
				:value="chunkedChapters"
				:numVisible="1"
				:numScroll="1"
				:responsiveOptions="responsiveOptions"
			>
				<template #item="slotProps">
					<div class="chapter">
						<RouterLink
							:to="
								`/chapters/${slotProps.data.chapterId}/themes` +
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
			<Carousel
				v-else
				:value="chunkedChapters"
			>
				<template #item="slotProps">
					<div class="chapter-mobile-group">
						<div
							v-for="chapter in slotProps.data"
							:key="chapter.chapterId"
							class="chapter"
						>
							<RouterLink
								:to="
									`/chapters/${chapter.chapterId}/themes` +
									`${chapter.themes.length > 0 ? '/' + chapter.themes[0].themeId : ''}`
								"
							>
								<div class="chapter-image">
									<img :src="helper.getImagePath('chapter') + chapter.imagePath" />
									<p>{{ chapter.chapterTitle }}</p>
								</div>
							</RouterLink>
						</div>
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

.chapter-mobile-group {
	display: flex;
	flex-direction: column;
	align-items: center;
	margin-right: 6%;
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
		2px -2px 2px rgba(0, 0, 0, 1),
		-2px 2px 2px rgba(0, 0, 0, 1),
		2px 2px 2px rgba(0, 0, 0, 1);
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

@media (max-width: 800px) {
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
</style>
