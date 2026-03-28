<script setup>
import ChapterCreateUpdateForm from '@/components/Chapters/ChapterCreateUpdateForm.vue'
import { useStore } from 'vuex'
import { helper } from '@/helper/helper'

const store = useStore()

async function createChapter(newChapter) {
	let formData = new FormData()

	formData.append('chapterTitle', newChapter.chapterTitle)
	formData.append('imagePath', newChapter.imagePath)
	formData.append('dateCreated', helper.getCurrentDateString())

	await store.dispatch('createChapter', formData)
}
</script>

<template>
	<ChapterCreateUpdateForm
		:isCreateForm="true"
		:doClearChapter="true"
		:createChapter="createChapter"
	/>
</template>

<style scoped></style>
