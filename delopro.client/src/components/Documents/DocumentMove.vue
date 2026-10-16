<script setup>
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import { computed, ref } from 'vue'
import { useStore } from 'vuex'

const props = defineProps({
	node: {
		typeof: Object,
		default: null,
	},
})

const store = useStore()
const emit = defineEmits(['hideSettings'])

const documentNodes = computed(() => store.getters.getDocumentNodes)
const showFolders = ref(true)

const isRootFolder = computed(() => props.node.parentKey === 'docs')

const selectedFolder = ref(
	isRootFolder.value
		? documentNodes.value[0]
		: getFolderByKey(documentNodes.value, props.node.parentKey),
)
const folders = computed(() => getFolders(selectedFolder.value))

function isParentFolder(node) {
	return folders.value.indexOf(node) === 0 && node.key === selectedFolder.value.parentKey
}

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

function getFolders(folderNode) {
	var childFolders = folderNode.children.filter((c) => c.data.type === 'folder')
	var parentFolder = getFolderByKey(documentNodes.value, folderNode.parentKey)

	return parentFolder ? [parentFolder].concat(childFolders) : childFolders
}

async function openFolder(event) {
	selectedFolder.value = event.data
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
			<span style="padding-bottom: 10px">{{ selectedFolder.data.path }}</span>
			<DataTable
				v-if="showFolders"
				:value="folders"
				stripedRows
				showGridlines
				selectionMode="single"
				:showHeaders="false"
				@rowDblclick="openFolder($event)"
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
