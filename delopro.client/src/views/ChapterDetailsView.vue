<script setup>
import ThemeComponent from '@/components/ThemeComponent.vue'
import { computed, ref } from 'vue'
import { useStore } from 'vuex'
import { useRouter } from 'vue-router'
import { useRoute } from 'vue-router'
import Button from 'primevue/button'
import { helper } from '@/helper/helper'

const store = useStore()
const router = useRouter()
const route = useRoute()

const isAdmin = computed(() => store.getters.isAdmin)
const isOwner = computed(() => store.getters.isOwner)
const chapter = computed(() => store.getters.getChapter)
const theme = computed(() => store.getters.getTheme)
const themeIds = computed(() => store.getters.getThemes.map((x) => x.themeId))
const themeIndex = computed(() => themeIds.value.indexOf(theme.value?.themeId ?? 0))
const themeNumber = computed(() => themeIndex.value + 1)
const showThemeButtons = ref(true)

const searchResult = computed(() => {
	if (route.query?.searchFragment) {
		return {
			searchFragment: helper.decodeHtml(route.query.searchFragment),
			index: route.query.index,
		}
	} else {
		return null
	}
})

function previousTheme() {
	if (themeIndex.value != 0 && theme.value) {
		router.push(`/chapters/${theme.value.chapterId}/${themeIds.value[themeIndex.value - 1]}`)
	}
}

function nextTheme() {
	if (!(themeIndex.value >= themeIds.value.length - 1)) {
		router.push(`/chapters/${theme.value.chapterId}/${themeIds.value[themeIndex.value + 1]}`)
	}
}

function setShowThemeButtons(value) {
	showThemeButtons.value = value != undefined ? value : false
}
</script>

<template>
	<div class="chapter-details-container">
		<div v-if="chapter">
			<div class="chapter-title">
				<h3>
					<Button
						text
						rounded
						severity="contrast"
						icon="pi pi-home"
						title="На главную"
						@click.prevent="() => router.push('/')"
					/>
					{{ chapter.chapterTitle }}
					<Button
						v-if="isAdmin || isOwner"
						text
						rounded
						severity="contrast"
						icon="pi pi-pen-to-square"
						title="Редактировать"
						@click="router.push(`/edit-chapter/${chapter.chapterId}`)"
					/>
				</h3>
				<span>{{ helper.getDateStringForUI(chapter.dateCreated) }}</span>
			</div>
			<hr style="margin: 5px" />
		</div>
		<ThemeComponent
			v-if="theme"
			:theme="theme"
			:searchResult="searchResult"
			@setShowThemeButtons="setShowThemeButtons"
		></ThemeComponent>
		<div
			v-if="showThemeButtons"
			class="theme-buttons"
		>
			<Button
				@click="previousTheme"
				icon="pi pi-arrow-left"
				rounded
				raised
			></Button>
			<div class="theme-counter">
				<span>{{ `${themeNumber} из ${themeIds.length}` }}</span>
			</div>
			<Button
				@click="nextTheme"
				icon="pi pi-arrow-right"
				rounded
				raised
			></Button>
		</div>
	</div>
</template>

<style scoped>
.chapter-details-container {
	display: flex;
	flex-direction: column;
}

.chapter-title {
	display: flex;
	flex-direction: row;
	padding: 0 10px 0 10px;
	align-items: center;
	justify-content: space-between;
}

.chapter-title h3 {
	margin: 0;
}

.chapter-title input {
	margin-top: 5px;
	height: 22px;
	font-size: 15px;
	font-weight: bold;
	width: 66%;
}

.chapter-title span {
	font-size: small;
}

.delete-button {
	margin: 10px 0 10px 0;
	float: right;
}

.ok-button {
	margin: 10px 0 10px 0;
	float: right;
	width: 90px;
}

.theme-buttons {
	display: flex;
	justify-content: center;
	align-items: center;
	gap: 10%;
	width: inherit;
	position: fixed;
	z-index: 1;
	bottom: 5%;
}

.theme-buttons button {
	height: 50px;
	width: 50px;
	background: rgba(0, 50, 90, 0.5);
	border-width: 0;
	&:hover {
		background: rgba(0, 50, 90, 0.7);
		border-width: 0;
	}
}

.theme-counter {
	padding: 10px;
	border-radius: 20px;
	background: rgba(0, 50, 90, 0.5);
	color: white;
}

@media (max-width: 1100px) {
	.theme-buttons {
		justify-content: center;
		width: inherit;
		position: fixed;
	}
}

@media (max-width: 800px) {
	.chapter-title span {
		display: none;
	}
}
</style>
