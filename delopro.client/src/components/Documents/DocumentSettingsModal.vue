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
const nodeId = computed(() => props.node && `${props.node.data.path}_${props.node.data.type}`)
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
	if (!props.node) {
		return
	} else if (!nodeId.value) {
		nodeId.value = `${props.node.data.path}_${props.node.data.type}`
	}
}

function showRenameInput() {
	hideSettings()

	newName.value = props.node.data.name

	const renameInput = document.getElementById(`${nodeId.value}_rename-input`)
	renameInput.focus()
}

function showNewFolderInput() {
	hideSettings()

	const newFolderInput = document.getElementById(`${nodeId.value}_new-folder-input`)
	newFolderInput.focus()
}

function showPathSelector() {
	hideSettings()

	const name = document.getElementById(`${nodeId.value}_name`)
	const pathSelector = document.getElementById(`${nodeId.value}_path-selector`)

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
		nodeId.value = null
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
			`${props.node.data.type === 'file' ? `Файл "${props.node.data.name}" будет удален` : `Папка "${props.node.data.name}" и всё её содержимое будет удалено`}, вы уверены?`,
		)
	) {
		return
	}

	const deleteModel = {
		path: props.node.data.path,
		type: props.node.data.type,
	}

	const success = await store.dispatch('deleteDocument', deleteModel)

	if (success) {
		nodeId.value = null
	}
}

async function moveFile() {
	const oldPath = props.node.data.path.replace('...', '')
	let fileName = '\\' + props.node.data.path.split('\\').at(-1)
	const newPath = moveFolder.value.replace('...', '') + fileName

	const moveModel = {
		oldPath: oldPath,
		newPath: newPath,
	}

	const success = await store.dispatch('moveDocument', moveModel)

	if (success) {
		resetTempValues()
	}
}

function resetTempValues() {
	moveFolder.value = null
	newFolderName.value = null
	newName.value = null
}

async function copyUrlToClipboard() {
	hideSettings()

	const url = store.getters.serverUrl.replace('api', '') + props.node.data.path.replace('\\', '/')
	navigator.clipboard.writeText(url)
	toast.success(`Ссылка для "${props.node.data.name}" скопирована`)
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
			<div
				v-if="props.node.data.type != 'folder' && props.node.data.type != 'root'"
				class="setting-buttons"
			>
				<Button
					@click="copyUrlToClipboard"
					text
					rounded
					severity="contrast"
					icon="pi pi-link"
					title="Копировать ссылку"
					label="Копировать ссылку"
				></Button>
				<Button
					@click="showPathSelector"
					text
					rounded
					severity="contrast"
					icon="pi pi-file-export"
					title="Переместить"
					label="Переместить"
				></Button>
			</div>
			<div
				v-if="props.node.data.type == 'folder' || props.node.data.type == 'root'"
				class="setting-buttons"
			>
				<div
					style="
						display: inline-flex;
						align-items: center;
						position: relative;
						cursor: pointer;
						padding: 6px 12px;
						border-radius: 4px;
						background: transparent;
						transition: background-color 0.2s;
					"
					onmouseover="this.style.backgroundColor = 'rgba(248,250,252,0)'"
					onmouseout="this.style.backgroundColor = 'transparent'"
				>
					<FileUpload
						mode="basic"
						name="files"
						:multiple="true"
						:maxFileSize="20000000"
						:auto="true"
						customUpload
						@select="uploadFiles($event)"
						style="
							position: absolute;
							top: 0;
							left: 0;
							width: 100%;
							height: 100%;
							opacity: 0;
							cursor: pointer;
							z-index: 1;
						"
					/>
					<i
						class="pi pi-upload"
						style="margin-right: 8px; color: black"
					></i>
					<span style="font-size: 1rem; color: black; font-weight: 500">Добавить файлы</span>
				</div>

				<Button
					@click="showNewFolderInput"
					text
					rounded
					severity="contrast"
					icon="pi pi-folder-plus"
					title="Добавить папку"
					label="Добавить папку"
				></Button>
			</div>
			<div
				v-if="props.node.data.type != 'root'"
				class="setting-buttons"
			>
				<Button
					@click="showRenameInput"
					text
					rounded
					severity="contrast"
					icon="pi pi-pencil"
					title="Переименовать"
					label="Переименовать"
				></Button>
				<Button
					v-if="props.node.data.type != 'root'"
					@click="deleteDocument"
					rounded
					severity="danger"
					text
					icon="pi pi-trash"
					title="Удалить"
					label="Удалить"
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
				@click="() => emit('showSettings', $event, props.node, false)"
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
				@keydown.esc="() => emit('showSettings', $event, props.node, false)"
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
				@click="() => emit('showSettings', $event, props.node, false)"
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
				@keydown.esc="() => emit('showSettings', $event, props.node, false)"
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
				@click="() => emit('showSettings', $event, props.node, false)"
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

<style scoped>
.setting-buttons {
	display: flex;
	flex-direction: column;
	align-items: start;
}
</style>
