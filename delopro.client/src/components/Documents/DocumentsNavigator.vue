<script setup>
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import { computed, nextTick, ref, watch } from 'vue'
import { useStore } from 'vuex'
import DocumentSettingsModal from './DocumentSettingsModal.vue'

const props = defineProps({
	node: {
		type: Object,
		default: null,
	},
	disableFiles: {
		type: Boolean,
		default: false,
	},
	useModal: {
		type: Boolean,
		default: false,
	},
})

const emit = defineEmits(['onRowSelect'])
const store = useStore()

const showFolders = ref(true)
const selectedFolderOverride = ref(null)
const breadCrumbs = ref(store.getters.getDocumentNodes)
const showSettingsModal = ref(false)
const editedNode = ref(null)

const isAdmin = computed(() => store.getters.isAdmin)
const isOwner = computed(() => store.getters.isOwner)

const documentNodes = computed(() => store.getters.getDocumentNodes)
const isRootFolder = computed(() => !props.node?.parentKey)

const selectedFolder = computed(() => {
	if (selectedFolderOverride.value) {
		return selectedFolderOverride.value
	}

	if (!props.node) {
		return null
	}

	if (isRootFolder.value) {
		return documentNodes.value[0] ?? null
	}

	return getFolderByKey(documentNodes.value, props.node.parentKey) ?? null
})

const documents = computed(() =>
	documentNodes.value && selectedFolder.value ? getDocuments(selectedFolder.value) : [],
)

watch(selectedFolderOverride, () => updateDocumentBreadCrumbs())
watch(
	documentNodes,
	(newNodes) => {
		if (!selectedFolderOverride.value) return

		const key = selectedFolderOverride.value.key
		const fresh = getFolderByKey(newNodes, key)

		// Only reassign if we found a fresh node and it's a different object
		if (fresh && fresh !== selectedFolderOverride.value) {
			selectedFolderOverride.value = fresh
		}
	},
	{ deep: true },
)

function isParentFolder(node) {
	return (
		documents.value.indexOf(node) === 0 &&
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

function getDocuments(folderNode) {
	if (!folderNode?.children) {
		return []
	}

	const childFolders = folderNode.children
	const parentFolder = getFolderByKey(documentNodes.value, folderNode.parentKey)

	return parentFolder ? [parentFolder, ...childFolders] : childFolders
}

function fillBreadCrumbs(folder) {
	breadCrumbs.value.push(folder)
	const parent = getFolderByKey(documentNodes.value, folder.parentKey)

	if (parent) {
		fillBreadCrumbs(parent)
	}
}

function onDocumentSelect(node) {
	if (props.disableFiles && node.data.type === 'file') {
		return
	}

	if (node.data.type === 'file') {
		if (window.confirm('Окрыть/скачать файл?')) {
			window.open(store.getters.serverUrl.replace('api', '') + node.data.path.replace('\\', '/'))
		}
	} else {
		selectedFolderOverride.value = node
		updateDocumentBreadCrumbs()
		emit('onRowSelect', node)
	}
}

function updateDocumentBreadCrumbs() {
	breadCrumbs.value = []
	fillBreadCrumbs(selectedFolder.value)
	breadCrumbs.value.reverse()
}

function showSettings(event) {
	event.originalEvent.preventDefault()
	const node = event.data

	editedNode.value = node
	showSettingsModal.value = false
	nextTick(() => (showSettingsModal.value = true))
}

function hideSettings() {
	showSettingsModal.value = false
}
</script>
<template>
	<div style="padding: 10px">
		<span
			v-for="node in breadCrumbs"
			:key="node.key"
		>
			<span
				@click="onDocumentSelect(node)"
				class="bread-crumb"
			>
				{{ node.data.name === '' ? '...' : node.data.name }}
			</span>
			<span>{{ ' / ' }}</span>
		</span>
	</div>
	<DataTable
		v-if="showFolders"
		:value="documents"
		stripedRows
		showGridlines
		selectionMode="single"
		:showHeaders="false"
		@rowSelect="onDocumentSelect($event.data)"
		@rowContextmenu="showSettings($event)"
	>
		<Column field="text">
			<template #body="{ data }">
				<span
					:title="data.data.path"
					:style="props.disableFiles && data.data.type === 'file' ? { opacity: 0.5 } : {}"
				>
					<i
						:class="isParentFolder(data) ? 'pi pi-ellipsis-h' : data.icon"
						:title="data.data.path"
					></i>
					{{ isParentFolder(data) ? '' : data.data.name }}
				</span>
			</template>
		</Column>
	</DataTable>
	<DocumentSettingsModal
		v-if="props.useModal && (isAdmin || isOwner)"
		v-model:visible="showSettingsModal"
		:node="editedNode"
		@hideSettings="hideSettings"
		style="min-width: 15rem"
	></DocumentSettingsModal>
</template>
<style scoped>
.path-selector {
	height: 20px;
}

.path-selector:deep(span) {
	padding: 2px 0 2px 4px;
	border-radius: 20%;
}

.bread-crumb {
	&:hover {
		cursor: pointer;
		text-decoration: underline;
	}
}
</style>
