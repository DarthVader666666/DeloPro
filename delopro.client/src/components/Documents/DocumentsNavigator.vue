<script setup>
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import { computed, ref } from 'vue'
import { useStore } from 'vuex'

const props = defineProps({
	node: { type: Object, default: null },
})

const store = useStore()
const documentNodes = computed(() => store.getters.getDocumentNodes)
const showFolders = ref(true)

// root detection: no parentKey, not `parentKey === 'docs'`
const isRootFolder = computed(() => !props.node?.parentKey)

const selectedFolderOverride = ref(null)

// computed so it re-evaluates when store/props change
const selectedFolder = computed(() => {
	if (selectedFolderOverride.value) return selectedFolderOverride.value
	if (!props.node) return null

	if (isRootFolder.value) return documentNodes.value[0] ?? null

	return getFolderByKey(documentNodes.value, props.node.parentKey) ?? null
})

const folders = computed(() => (selectedFolder.value ? getFolders(selectedFolder.value) : []))

function isParentFolder(node) {
	return (
		folders.value.indexOf(node) === 0 &&
		selectedFolder.value != null &&
		node.key === selectedFolder.value.parentKey
	)
}

function getFolderByKey(nodes, key) {
	if (!nodes || !key) return null
	for (const node of nodes.filter((n) => n.data?.type === 'folder' || n.data?.type === 'root')) {
		if (node.key === key) return node
		if (node.children?.length) {
			const found = getFolderByKey(node.children, key)
			if (found) return found
		}
	}
	return null
}

function getFolders(folderNode) {
	if (!folderNode?.children) {
		return []
	}

	const childFolders = folderNode.children
	const parentFolder = getFolderByKey(documentNodes.value, folderNode.parentKey)

	return parentFolder ? [parentFolder, ...childFolders] : childFolders
}

function onDocumentSelect(event) {
	const node = event.data

	if (node.data.type === 'file') {
		window.open(store.getters.serverUrl.replace('api', '') + node.data.path.replace('\\', '/'))
	} else {
		selectedFolderOverride.value = event.data
	}
}
</script>
<template>
	<DataTable
		v-if="showFolders"
		:value="folders"
		stripedRows
		showGridlines
		selectionMode="single"
		:showHeaders="false"
		@rowSelect="onDocumentSelect($event)"
	>
		<Column field="text">
			<template #body="{ data }">
				<span :title="data.data.path">
					<i
						:class="isParentFolder(data) ? 'pi pi-ellipsis-h' : data.icon"
						:title="data.data.path"
					></i>
					{{ isParentFolder(data) ? '' : data.data.name }}
				</span>
			</template>
		</Column>
	</DataTable>
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
