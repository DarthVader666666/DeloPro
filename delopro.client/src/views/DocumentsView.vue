<script setup>
import DocumentsNavigator from '@/components/Documents/DocumentsNavigator.vue'
import { ref } from 'vue'
import { useStore } from 'vuex'

const store = useStore()
const documentNode = ref(store.getters.getDocumentNodes[0])
</script>

<template>
	<div style="padding: 20px">
		<DocumentsNavigator
			:node="documentNode"
			:disable-files="false"
			:useModal="true"
		></DocumentsNavigator>
	</div>
</template>

<style scoped></style>
