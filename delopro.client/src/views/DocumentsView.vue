<script setup>
import DocumentsNavigator from '@/components/Documents/DocumentsNavigator.vue'
import { computed } from 'vue'
import { useStore } from 'vuex'

const store = useStore()
const documentNode = computed(() => store.getters.getDocumentNodes[0])
</script>

<template>
	<div style="padding: 20px">
		<DocumentsNavigator :node="documentNode"></DocumentsNavigator>
	</div>
</template>

<style scoped></style>
