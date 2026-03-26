<script setup>
import Carousel from 'primevue/carousel'
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'
import { helper } from '../helper/helper.js'
import { useStore } from 'vuex'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'

const store = useStore()
const router = useRouter()
const chapters = computed(() => store.getters.getChapters)
const isAdmin = computed(() => store.getters.isAdmin)
const isOwner = computed(() => store.getters.isOwner)

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

onMounted(() => {
	window.addEventListener('resize', onResize)
})

onBeforeUnmount(() => {
	window.removeEventListener('resize', onResize)
})

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
	<Carousel
		:value="chunkedChapters"
		:responsiveOptions="responsiveOptions"
	>
		<template #item="slotProps">
			<div
				v-if="isMobile"
				class="chapter-mobile-group"
			>
				<div
					v-for="chapter in slotProps.data"
					:key="chapter.chapterId"
					class="chapter"
				>
					<div
						@click="
							router.push(
								`/chapters/${chapter.chapterId}/themes` +
									`${chapter.themes.length > 0 ? '/' + chapter.themes[0].themeId : ''}`,
							)
						"
					>
						<div class="chapter-image">
							<img :src="helper.getImagePath('chapter') + chapter.imagePath" />
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
			</div>
			<div
				v-else
				class="chapter"
			>
				<div
					@click="
						router.push(
							`/chapters/${slotProps.data.chapterId}/themes` +
								`${slotProps.data.themes.length > 0 ? '/' + slotProps.data.themes[0].themeId : ''}`,
						)
					"
				>
					<div class="chapter-image">
						<img :src="helper.getImagePath('chapter') + slotProps.data.imagePath" />
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
			</div>
		</template>
	</Carousel>
</template>

<style scoped>
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
	-webkit-transition: all 0.2s ease-in-out;
	-moz-transition: all 0.2s ease-in-out;
	-o-transition: all 0.2s ease-in-out;
	transition: all 0.2s ease-in-out;
	filter: drop-shadow(var(--PNG-IMAGE-SHADOW));

	&:hover {
		-webkit-transform: scale(1.1);
		-moz-transform: scale(1.1);
		-o-transform: scale(1.1);
		transform: scale(1.1);
		opacity: 0.8;
		cursor: pointer;
	}
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

.chapter-image button {
	position: absolute;
	top: 7%;
	right: 0;
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

@media (min-width: 1600px) {
	.chapter {
		height: 240px;
		width: 300px;
	}
	.chapter-image img {
		height: 220px;
		width: 280px;
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
</style>
