<script setup>
import Button from 'primevue/button'
import { computed, ref } from 'vue'
import { useStore } from 'vuex'
import DocumentsNavigator from './DocumentsNavigator.vue'

const props = defineProps({
	node: {
		typeof: Object,
		default: null,
	},
})

const store = useStore()
const emit = defineEmits(['hideSettings'])

const documentNodes = computed(() => store.getters.getDocumentNodes)
const selectedFolder = ref(null)
const initialFolder = ref(getFolderByKey(documentNodes.value, props.node.parentKey))

function getFolderByKey(nodes, key) {
	for (const node of nodes.filter((n) => n.data.type === 'folder' || n.data.type === 'root')) {
		if (node.key === key) {
			return node
		}

		if (node.children && node.children.length > 0) {
			const foundNode = getFolderByKey(node.children, key)

			if (foundNode) {
				return foundNode
			}
		}
	}

	return null
}

async function openFolder(node) {
	selectedFolder.value = node
}

function moveFile() {
	const fileName = props.node.data.name
	store.dispatch('moveDocument', {
		oldPath: props.node.data.path,
		newPath: selectedFolder.value.data.path + '\\' + fileName,
	})

	emit('hideSettings')
}
</script>
<template>
	<div
		style="
			display: flex;
			flex-direction: column;
			justify-content: space-between;
			min-width: 400px;
			min-height: 300px;
		"
	>
		<div>
			<DocumentsNavigator
				:node="initialFolder"
				:disable-files="true"
				:useModal="false"
				@onRowSelect="openFolder"
			></DocumentsNavigator>
		</div>
		<div style="display: flex; justify-content: space-between">
			<Button
				@click="moveFile"
				style="width: 90px"
				severity="secondary"
				raised
			>
				ОК
			</Button>
			<Button
				style="width: 90px"
				@click="emit('hideSettings')"
				severity="contrast"
				raised
				title="Отмена"
			>
				Отмена
			</Button>
		</div>
	</div>
</template>
<style scoped>
.path-selector {
	height: 20px;
}

.path-selector:deep(span) {
	padding: 2px 0 2px 4px;
	border-radius: 20%;
}
</style>
