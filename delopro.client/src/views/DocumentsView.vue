<script setup>
import DocumentsNavigator from '@/components/Documents/DocumentsNavigator.vue'
import { ref } from 'vue'
import { useStore } from 'vuex'

const store = useStore()

const documentNode = ref(store.getters.getDocumentNodes[0])
const breadCrumbs = ref([documentNode.value])

function updateDocumentBreadCrumbs(node) {
	if (breadCrumbs.value.includes(node)) {
		const index = breadCrumbs.value.indexOf(node) + 1
		breadCrumbs.value.splice(index)
	} else {
		breadCrumbs.value.push(node)
	}
}

function navigate(node) {
	documentNode.value = node
	updateDocumentBreadCrumbs(node)
}
</script>

<template>
	<div style="padding: 20px">
		<div style="padding: 10px">
			<span
				v-for="node in breadCrumbs"
				:key="node.key"
			>
				<span @click="navigate(node)">{{ node.data.name + ' / ' }}</span>
			</span>
		</div>
		<DocumentsNavigator
			:disable-files="false"
			:node="documentNode"
			@update-document-bread-crumbs="updateDocumentBreadCrumbs"
		></DocumentsNavigator>
	</div>
</template>

<style scoped></style>
