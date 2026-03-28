<script setup>
import { helper } from '@/helper/helper'
import { onMounted, ref } from 'vue'

const props = defineProps({
	content: {
		type: String,
	},
	searchResult: {
		type: Object,
		default: null,
	},
})

const themeContent = ref(null)

onMounted(() => {
	setTimeout(() => {
		if (themeContent.value) {
			helper.highlightSearchResult(themeContent.value, props.searchResult)
		}
	}, 20)
})
</script>

<template>
	<div
		v-html="props.content"
		class="theme-content"
		ref="themeContent"
	></div>
</template>

<style scoped>
.theme-content {
	padding: 18px 20px 20px 20px;
	background: white;
	overflow-x: scroll;
}

.theme-content:deep(img) {
	max-width: 98%;
	height: auto;
}

.theme-content:deep(.ql-align-center) {
	text-align: center;
}
</style>
