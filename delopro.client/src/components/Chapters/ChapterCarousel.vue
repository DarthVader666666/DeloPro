<script setup>
import Carousel from 'primevue/carousel'
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'

import { useStore } from 'vuex'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import { helper } from '@/helper/helper'

const store = useStore()
const router = useRouter()
const chapters = computed(() => store.getters.getChapters)
const isAdmin = computed(() => store.getters.isAdmin)
const isOwner = computed(() => store.getters.isOwner)

const responsiveOptions = ref([
	{
		breakpoint: '2400px',
		numVisible: 4,
		numScroll: 1,
	},
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
	{
		breakpoint: '500px',
		numVisible: 1,
		numScroll: 1,
	},
])

const width = ref(window.innerWidth)
const isMobile = computed(() => width.value <= 500)

const onResize = () => {
	width.value = window.innerWidth
}

const chunkedChapters = ref([])

onMounted(() => {
	window.addEventListener('resize', onResize)
	chunkedChapters.value = isMobile.value ? chunkArray(chapters.value, 3) : chapters.value
})

onBeforeUnmount(() => {
	window.removeEventListener('resize', onResize)
})

watch([chapters, isMobile], () => {
	chunkedChapters.value = isMobile.value ? chunkArray(chapters.value, 3) : chapters.value
})

function chunkArray(arr, size = 3) {
	const result = []
	for (let i = 0; i < arr.length; i += size) {
		result.push(arr.slice(i, i + size))
	}
	return result
}
</script>

<template>
	<Carousel
		v-if="isMobile"
		:value="chunkedChapters"
		:responsiveOptions="responsiveOptions"
		:key="chunkedChapters.length + '-mobile'"
	>
		<template #item="slotProps">
			<div class="chapter-mobile-group">
				<div
					v-for="chapter in slotProps.data"
					:key="chapter.chapterId"
					class="chapter"
				>
					<div
						class="chapter-image"
						@click.prevent="
							router.push(
								`/chapters/${chapter.chapterId}/themes` +
									`${chapter.themes.length > 0 ? '/' + chapter.themes[0].themeId : ''}`,
							)
						"
					>
						<img
							:src="helper.getImagePath('chapter') + chapter.imagePath"
							loading="lazy"
							decoding="async"
						/>
						<p>{{ chapter.chapterTitle }}</p>
						<Button
							v-if="isAdmin || isOwner"
							text
							rounded
							severity="contrast"
							icon="pi pi-pen-to-square"
							title="Редактировать"
							@click.stop="router.push(`/chapters/${chapter.chapterId}/edit`)"
						/>
					</div>
				</div>
			</div>
		</template>
	</Carousel>
	<Carousel
		v-else
		:value="chapters"
		:responsiveOptions="responsiveOptions"
		:key="chapters.length + '-desktop'"
		:numVisible="6"
		:numScroll="1"
	>
		<template #item="slotProps">
			<div class="chapter">
				<div
					class="chapter-image"
					@click.prevent="
						router.push(
							`/chapters/${slotProps.data.chapterId}/themes` +
								`${slotProps.data.themes.length > 0 ? '/' + slotProps.data.themes[0].themeId : ''}`,
						)
					"
				>
					<img
						:src="helper.getImagePath('chapter') + slotProps.data.imagePath"
						loading="lazy"
						decoding="async"
					/>
					<p>{{ slotProps.data.chapterTitle }}</p>
					<Button
						v-if="isAdmin || isOwner"
						text
						rounded
						severity="contrast"
						icon="pi pi-pen-to-square"
						title="Редактировать"
						@click.stop="router.push(`/chapters/${slotProps.data.chapterId}/edit`)"
					/>
				</div>
			</div>
		</template>
	</Carousel>
</template>

<style scoped>
.chapter-mobile-group {
	display: flex;
	flex-direction: column;
	align-items: center;
}

.chapter {
	padding: 10px;
	-webkit-transition: all 0.2s ease-in-out;
	-moz-transition: all 0.2s ease-in-out;
	-o-transition: all 0.2s ease-in-out;
	transition: all 0.2s ease-in-out;
	filter: drop-shadow(var(--PNG-IMAGE-SHADOW));

	&:hover {
		-webkit-transform: scale(1.05);
		-moz-transform: scale(1.05);
		-o-transform: scale(1.05);
		transform: scale(1.05);
		opacity: 0.8;
		cursor: pointer;
	}
}

.chapter-image {
	position: relative;
}

.chapter-image img {
	height: 200px;
	width: 100%;
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

.chapter-image button {
	position: absolute;
	top: 5%;
	right: 3%;
	background: rgba(210, 210, 210, 0.6);
}

:deep(.p-button-text.p-button-secondary) {
	background: lightgray;
}

:deep(.p-carousel-indicator-active .p-carousel-indicator-button) {
	background: rgb(50, 50, 50);
}

:deep(.p-carousel-indicator-button) {
	background: lightgray;
}

/* :deep(.p-carousel-content) {
	padding-top: 20px;
	background: rgb(255, 255, 255, 0.3);
	border-radius: 15px 15px 0 0;
} */

/* :deep(.p-carousel-indicator-list) {
	background: rgb(255, 255, 255, 0.3);
	border-radius: 0 0 15px 15px;
} */

@media (min-width: 1800px) {
	.chapter-image img {
		height: 230px;
	}
}

@media (max-width: 800px) {
	.chapter-image p {
		font-size: 0.9rem;
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
		width: 240px;
	}

	.chapter-image img {
		height: 160px;
	}
}
</style>
