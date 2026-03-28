<script setup>
import ThemeComponent from '@/components/Themes/ThemeComponent.vue'
import ThemeSwitchComponent from '@/components/Themes/ThemeSwitchButtons.vue'
import { computed } from 'vue'
import Button from 'primevue/button'
import { helper } from '@/helper/helper'
import { useRoute, useRouter } from 'vue-router'
import { useStore } from 'vuex'

const store = useStore()
const route = useRoute()
const router = useRouter()

const isAdmin = computed(() => store.getters.isAdmin)
const isOwner = computed(() => store.getters.isOwner)
const chapter = computed(() => store.getters.getChapter)
const theme = computed(() => store.getters.getTheme)
const isCommentsMode = computed(() => route.name === 'comments')

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
</script>

<template>
	<div class="chapter-container">
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
						@click="router.push(`/chapters/${chapter.chapterId}/edit`)"
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
			:isCommentsMode="isCommentsMode"
		></ThemeComponent>
		<ThemeSwitchComponent
			v-if="!isCommentsMode"
			:theme="theme"
		></ThemeSwitchComponent>
	</div>
</template>

<style scoped>
.chapter-container {
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

@media (max-width: 800px) {
	.chapter-title span {
		display: none;
	}
}
</style>
