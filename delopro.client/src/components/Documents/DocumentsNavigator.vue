<script setup>
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import { computed, nextTick, onMounted, ref, watch } from 'vue'
import { useStore } from 'vuex'
import DocumentSettingsModal from './DocumentSettingsModal.vue'
import { helper } from '@/helper/helper.js'

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

	if (!props.node || isRootFolder.value) {
		return documentNodes.value[0]
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

		if (fresh && fresh !== selectedFolderOverride.value) {
			selectedFolderOverride.value = fresh
		}
	},
	{ deep: true },
)

onMounted(() => {
	updateDocumentBreadCrumbs()
})

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

	const childNodes = folderNode.children
	const parentFolder = getFolderByKey(documentNodes.value, folderNode.parentKey)

	return parentFolder ? [parentFolder, ...childNodes] : [selectedFolder.value, ...childNodes]
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
			<template #body="{ data: node }">
				<span
					:title="node.data.path"
					:class="node.data.type === 'folder' ? 'folder' : ''"
					:style="props.disableFiles && node.data.type === 'file' ? { opacity: 0.5 } : {}"
				>
					<img
						v-if="node.data.type === 'folder' && !isParentFolder(node)"
						:src="helper.getImagePath('icon') + 'folder.png'"
						style="width: 1.6em; height: 1.4em; vertical-align: -0.3em; padding-right: 3px"
					/>
					<i
						v-else-if="node.data.type === 'root' || isParentFolder(node)"
						class="icon pi pi-ellipsis-h"
					></i>
					<img
						v-else
						:src="helper.getImagePath('icon') + helper.getDocumentIconName(node.data.name) + '.png'"
						style="width: 1.6em; height: 1.4em; vertical-align: -0.3em; padding-right: 3px"
					/>
					{{ isParentFolder(node) ? '' : node.data.name }}
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

.folder {
	font-weight: bold;
}
</style>
