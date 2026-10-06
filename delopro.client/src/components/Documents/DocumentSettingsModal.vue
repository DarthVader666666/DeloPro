<script setup>
import { useStore } from 'vuex'
import { computed, ref } from 'vue'
import { useToast } from 'vue-toastification'
import Dialog from 'primevue/dialog'
import FileUpload from 'primevue/fileupload'
import Button from 'primevue/button'
import Select from 'primevue/select'

const props = defineProps({
	node: {
		typeof: Object,
		default: null,
	},
})

const emit = defineEmits('showSettings')

const toast = useToast()
const store = useStore()

const newName = ref(null)
const editedNode = ref(props.node)
const editedNodeId = ref(
	editedNode.value && `${editedNode.value.data.path}_${editedNode.value.data.type}`,
)
const moveFolder = ref(null)
const newFolderName = ref(null)

const folderPaths = computed(() => store.getters.getFolderPaths)

async function createFolder() {
	if (!(props.node.data.path && newFolderName.value)) {
		return
	}

	const folderPathModel = {
		folderPath: props.node.data.path + '\\' + newFolderName.value,
	}

	const success = await store.dispatch('createFolder', folderPathModel)

	if (success) {
		hideSettings()
	}

	resetTempValues()
}

function hideSettings() {
	if (!editedNode.value) {
		return
	} else if (!editedNodeId.value) {
		editedNodeId.value = `${editedNode.value.data.path}_${editedNode.value.data.type}`
	}
}

// async function showSettings() {
// 	if (editedNode.value) {
// 		hideSettings()
// 	}

// 	editedNode.value = props.node
// 	editedNodeId.value = `${props.node.data.path}_${props.node.data.type}`
// }

function showRenameInput() {
	hideSettings()

	newName.value = editedNode.value.data.name

	const renameInput = document.getElementById(`${editedNodeId.value}_rename-input`)
	renameInput.focus()
}

function showNewFolderInput() {
	hideSettings()

	const newFolderInput = document.getElementById(`${editedNodeId.value}_new-folder-input`)
	newFolderInput.focus()
}

function showPathSelector() {
	hideSettings()

	const name = document.getElementById(`${editedNodeId.value}_name`)
	const pathSelector = document.getElementById(`${editedNodeId.value}_path-selector`)

	name.style.display = 'none'
	pathSelector.style.display = 'inline-flex'
}

function cancel() {
	resetTempValues()
	hideSettings()
}

async function renameDocument() {
	if (newName.value === props.node.data.name) {
		cancel()
		return
	}

	const updateModel = {
		newName: newName.value,
		oldName: props.node.data.name,
		path: props.node.data.path,
		type: props.node.data.type,
	}

	const success = await store.dispatch('updateDocument', updateModel)
	newName.value = null

	if (success) {
		editedNode.value = null
		editedNodeId.value = null
		hideSettings()
	}
}

async function uploadFiles(event) {
	const files = event.files
	let uploadModel = new FormData()
	files.forEach((file) => uploadModel.append('files', file))

	if (!props.node.data.path) {
		hideSettings()
		return
	}

	uploadModel.append('folderName', props.node.data.path)
	await store.dispatch('uploadDocuments', uploadModel)

	hideSettings()
}

async function deleteDocument() {
	if (
		!window.confirm(
			`${editedNode.value.data.type === 'file' ? `Файл "${editedNode.value.data.name}" будет удален` : `Папка "${editedNode.value.data.name}" и всё её содержимое будет удалено`}, вы уверены?`,
		)
	) {
		return
	}

	const deleteModel = {
		path: editedNode.value.data.path,
		type: editedNode.value.data.type,
	}

	const success = await store.dispatch('deleteDocument', deleteModel)

	if (success) {
		//hideButtons()
		editedNode.value = null
		editedNodeId.value = null
	}
}

async function moveFile() {
	const oldPath = editedNode.value.data.path.replace('...', '')
	let fileName = '\\' + editedNode.value.data.path.split('\\').at(-1)
	const newPath = moveFolder.value.replace('...', '') + fileName

	const moveModel = {
		oldPath: oldPath,
		newPath: newPath,
	}

	// hideButtons()

	const success = await store.dispatch('moveDocument', moveModel)

	if (success) {
		resetTempValues()
		editedNode.value = null
		editedNodeId.value = null
	}
}

function resetTempValues() {
	moveFolder.value = null
	newFolderName.value = null
	newName.value = null
}

async function copyUrlToClipboard() {
	hideSettings()

	const url =
		store.getters.serverUrl.replace('api', '') + editedNode.value.data.path.replace('\\', '/')
	navigator.clipboard.writeText(url)
	toast.success(`Ссылка для "${editedNode.value.data.name}" скопирована`)
}

function enableArrowKeysEvents(event) {
	if ([38, 40, 37, 39].includes(event.keyCode)) {
		event.stopPropagation()
		return true
	}
}
</script>

<template>
	<Dialog>
		<!-- Settings -->
		<div :id="`${props.node.data.path}_${props.node.data.type}_settings`">
			<div v-if="editedNode && editedNode.data.type != 'folder' && props.node.data.type != 'root'">
				<Button
					@click="copyUrlToClipboard"
					text
					rounded
					severity="contrast"
					icon="pi pi-link"
					title="Копировать ссылку"
				></Button>
				<Button
					@click="showPathSelector"
					text
					rounded
					severity="contrast"
					icon="pi pi-file-export"
					title="Переместить"
				></Button>
			</div>
			<FileUpload
				v-if="props.node.data.type == 'folder' || props.node.data.type == 'root'"
				mode="basic"
				name="files"
				:multiple="true"
				:maxFileSize="20000000"
				class="p-button-icon-only"
				chooseIcon="pi pi-upload"
				:auto="true"
				:chooseButtonProps="{
					severity: 'contrast',
					text: true,
					raised: false,
				}"
				customUpload
				title="Добавить файлы"
				@select="uploadFiles($event)"
			/>
			<Button
				v-if="props.node.data.type == 'folder' || props.node.data.type == 'root'"
				@click="showNewFolderInput"
				text
				rounded
				severity="contrast"
				icon="pi pi-folder-plus"
				title="Добавить папку"
			></Button>

			<div v-if="props.node.data.type != 'root'">
				<Button
					@click="showRenameInput"
					text
					rounded
					severity="contrast"
					icon="pi pi-pencil"
					title="Переименовать"
				></Button>
				<Button
					v-if="props.node.data.type != 'root'"
					@click="deleteDocument"
					rounded
					severity="danger"
					text
					icon="pi pi-trash"
					title="Удалить"
				></Button>
			</div>
		</div>

		<!-- Move File -->
		<div
			style="display: none"
			:id="`${props.node.data.path}_${props.node.data.type}_path-selector`"
		>
			<Select
				class="path-selector"
				:options="folderPaths"
				v-model="moveFolder"
				v-on:change="moveFile"
				placeholder="Путь..."
				appendTo="self"
			>
				<template #option="{ option }">
					<span style="font-size: small">
						{{ option }}
					</span>
				</template>
			</Select>
			<Button
				@click="() => emit('showSettings')"
				text
				rounded
				severity="contrast"
				icon="pi pi-arrow-left"
				title="Назад"
			></Button>
		</div>

		<!-- Rename -->
		<div
			style="display: none"
			:id="`${props.node.data.path}_${props.node.data.type}_rename`"
		>
			<input
				type="text"
				v-model="newName"
				class="settings-input"
				:id="`${props.node.data.path}_${props.node.data.type}_rename-input`"
				@keydown.stop="enableArrowKeysEvents"
				@keydown.esc="() => emit('showSettings', false)"
				@keydown.enter="renameDocument()"
			/>

			<Button
				@click="renameDocument()"
				rounded
				severity="primary"
				text
				icon="pi pi-check"
				title="Ок"
			></Button>
			<Button
				@click="() => emit('showSettings', false)"
				rounded
				severity="danger"
				text
				icon="pi pi-ban"
				title="Отмена"
			></Button>
		</div>

		<!-- New Folder -->
		<div
			style="display: none"
			:id="`${props.node.data.path}_${props.node.data.type}_new-folder`"
		>
			<input
				type="text"
				v-model="newFolderName"
				class="settings-input"
				:id="`${props.node.data.path}_${props.node.data.type}_new-folder-input`"
				@keydown.stop="enableArrowKeysEvents"
				@keydown.esc="() => emit('showSettings', false)"
				@keydown.enter="createFolder()"
			/>

			<Button
				@click="createFolder()"
				rounded
				severity="primary"
				text
				icon="pi pi-check"
				title="Ок"
			></Button>
			<Button
				@click="() => emit('showSettings', false)"
				rounded
				severity="danger"
				text
				icon="pi pi-ban"
				title="Отмена"
			></Button>
		</div>

		<!-- Close settings -->
		<Button
			@click="hideSettings"
			text
			rounded
			severity="contrast"
			icon="pi pi-times"
			title="Закрыть"
			style="display: none"
			:id="`${props.node.data.path}_${props.node.data.type}_close-settings`"
		></Button>
	</Dialog>
</template>

<style scoped></style>
