<script setup>
import FileUpload from 'primevue/fileupload'
import Button from 'primevue/button'
import TreeTable from 'primevue/treetable'
import Column from 'primevue/column'
import Select from 'primevue/select'
import { computed, nextTick, ref } from 'vue'
import { useStore } from 'vuex'
import { useToast } from 'vue-toastification'

const store = useStore()
const toast = useToast()
const isAdmin = computed(() => store.getters.isAdmin)
const isOwner = computed(() => store.getters.isOwner)
const documentNodes = computed(() => store.getters.getDocumentNodes)
const folderPaths = computed(() => store.getters.getFolderPaths)

const newFolderName = ref(null)
const newName = ref(null)
const moveFolder = ref(null)
const editedNode = ref(null)
const editedNodeId = ref(null)
const expandedNodes = { docs: true }
const interval = ref(null)

async function createFolder(path) {
	if (!(path && newFolderName.value)) {
		return
	}

	const folderPathModel = {
		folderPath: path + '\\' + newFolderName.value,
	}

	const success = await store.dispatch('createFolder', folderPathModel)

	if (success) {
		hideButtons()
	}

	resetTempValues()
}

function hideButtons() {
	if (!editedNode.value) {
		return
	} else if (!editedNodeId.value) {
		editedNodeId.value = `${editedNode.value.data.path}_${editedNode.value.data.type}`
	}

	const name = document.getElementById(`${editedNodeId.value}_name`)
	const rename = document.getElementById(`${editedNodeId.value}_rename`)
	const newFolder = document.getElementById(`${editedNodeId.value}_new-folder`)
	const settings = document.getElementById(`${editedNodeId.value}_settings`)
	const closeSettings = document.getElementById(`${editedNodeId.value}_close-settings`)
	const pathSelector = document.getElementById(`${editedNodeId.value}_path-selector`)

	name.style.display = 'block'
	rename.style.display = 'none'
	newFolder.style.display = 'none'
	settings.style.display = 'none'
	closeSettings.style.display = 'none'
	pathSelector.style.display = 'none'
}

async function showSettings(node) {
	if (editedNode.value) {
		hideButtons()
	}

	editedNode.value = node
	editedNodeId.value = `${node.data.path}_${node.data.type}`

	const settingButtons = document.getElementById(`${editedNodeId.value}_settings`)
	const showSettings = document.getElementById(`${editedNodeId.value}_show-settings`)
	const closeSettings = document.getElementById(`${editedNodeId.value}_close-settings`)
	const name = document.getElementById(`${editedNodeId.value}_name`)

	await nextTick()

	settingButtons.style.display = 'inline-flex'
	showSettings.style.display = 'none'
	closeSettings.style.display = 'inline-flex'
	name.style.display = 'none'
}

function showRenameInput() {
	hideButtons()

	newName.value = editedNode.value.data.name

	document.getElementById(`${editedNodeId.value}_name`).style.display = 'none'
	document.getElementById(`${editedNodeId.value}_rename`).style.display = 'block'
	document.getElementById(`${editedNodeId.value}_settings`).style.display = 'none'

	const renameInput = document.getElementById(`${editedNodeId.value}_rename-input`)
	renameInput.focus()
}

function showNewFolderInput() {
	hideButtons()

	document.getElementById(`${editedNodeId.value}_name`).style.display = 'none'
	document.getElementById(`${editedNodeId.value}_new-folder`).style.display = 'block'
	document.getElementById(`${editedNodeId.value}_settings`).style.display = 'none'

	const newFolderInput = document.getElementById(`${editedNodeId.value}_new-folder-input`)
	newFolderInput.focus()
}

function showPathSelector() {
	hideButtons()

	const name = document.getElementById(`${editedNodeId.value}_name`)
	const pathSelector = document.getElementById(`${editedNodeId.value}_path-selector`)

	name.style.display = 'none'
	pathSelector.style.display = 'inline-flex'
}

function mouseEnterDocumentHandler(node) {
	const showSettings = document.getElementById(`${node.data.path}_${node.data.type}_show-settings`)

	if (!showSettings) {
		return
	}

	showSettings.style.display = 'inline-flex'
}

function mouseLeaveDocumentHandler(node) {
	const showSettings = document.getElementById(`${node.data.path}_${node.data.type}_show-settings`)

	if (!showSettings) {
		return
	}

	showSettings.style.display = 'none'
}

function cancel() {
	resetTempValues()
	hideButtons()
}

async function renameDocument(node) {
	if (newName.value === node.data.name) {
		cancel()
		return
	}

	const updateModel = {
		newName: newName.value,
		oldName: node.data.name,
		path: node.data.path,
		type: node.data.type,
	}

	const success = await store.dispatch('updateDocument', updateModel)
	newName.value = null

	if (success) {
		editedNode.value = null
		editedNodeId.value = null
		hideButtons()
	}
}

function downloadFile(node) {
	if (node.data.path)
		window.open(store.getters.serverUrl.replace('api', '') + node.data.path.replace('\\', '/'))
}

async function uploadFiles(event, path) {
	const files = event.files
	let uploadModel = new FormData()
	files.forEach((file) => uploadModel.append('files', file))

	if (!path) {
		hideButtons()
		return
	}

	uploadModel.append('folderName', path)
	await store.dispatch('uploadDocuments', uploadModel)

	hideButtons()
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
	hideButtons()

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

function disableArrowKeysEvents(event) {
	if ([38, 40, 37, 39].includes(event.keyCode)) {
		event.preventDefault()
	}
}

function startCountdown(node) {
	interval.value = setTimeout(() => showSettings(node), 1200)
}

function stopCountdown() {
	clearTimeout(interval.value)
}
</script>
<template>
	<div id="right-container">
		<div class="items">
			<div class="items-header">
				<strong style="margin: 5px 0 6px 0">Документы:</strong>
			</div>
			<hr />

			<TreeTable
				:value="documentNodes"
				v-model:expandedKeys="expandedNodes"
				scrollable
				scrollHeight="85vh"
				class="tree-table"
				@keydown="disableArrowKeysEvents"
			>
				<Column
					field="name"
					expander
				>
					<template #body="{ node }">
						<div
							style="display: flex; flex-direction: row; align-items: center; gap: 5px"
							@mouseleave="mouseLeaveDocumentHandler(node)"
						>
							<!-- Document name -->
							<div
								@mouseenter="mouseEnterDocumentHandler(node)"
								@touchstart="startCountdown(node)"
								@touchend="stopCountdown"
								@touchmove="stopCountdown"
								:id="`${node.data.path}_${node.data.type}_name`"
							>
								<i
									:class="node.icon"
									style="font-size: small; padding-right: 3px"
								></i>

								<span
									:title="node.data.size"
									:class="node.data.type"
									@click="node.data.type === 'file' ? downloadFile(node) : null"
									:style="node.data.type === 'folder' ? 'font-weight:bold;' : 'font-weight:normal;'"
								>
									{{ node.data.name }}
								</span>
							</div>

							<div v-if="isAdmin || isOwner">
								<!-- Settings -->
								<div
									class="setting-buttons"
									:id="`${node.data.path}_${node.data.type}_settings`"
								>
									<div
										v-if="
											editedNode && editedNode.data.type != 'folder' && node.data.type != 'root'
										"
									>
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
										v-if="node.data.type == 'folder' || node.data.type == 'root'"
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
										@select="uploadFiles($event, node.data.path)"
									/>
									<Button
										v-if="node.data.type == 'folder' || node.data.type == 'root'"
										@click="showNewFolderInput"
										text
										rounded
										severity="contrast"
										icon="pi pi-folder-plus"
										title="Добавить папку"
									></Button>

									<div v-if="node.data.type != 'root'">
										<Button
											@click="showRenameInput"
											text
											rounded
											severity="contrast"
											icon="pi pi-pencil"
											title="Переименовать"
										></Button>
										<Button
											v-if="node.data.type != 'root'"
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
									:id="`${node.data.path}_${node.data.type}_path-selector`"
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
										@click="showSettings(node)"
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
									:id="`${node.data.path}_${node.data.type}_rename`"
								>
									<input
										type="text"
										v-model="newName"
										class="settings-input"
										:id="`${node.data.path}_${node.data.type}_rename-input`"
										@keydown.stop="enableArrowKeysEvents"
										@keydown.esc="showSettings(node)"
										@keydown.enter="renameDocument(node)"
									/>

									<Button
										@click="renameDocument(node)"
										rounded
										severity="primary"
										text
										icon="pi pi-check"
										title="Ок"
									></Button>
									<Button
										@click="showSettings(node)"
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
									:id="`${node.data.path}_${node.data.type}_new-folder`"
								>
									<input
										type="text"
										v-model="newFolderName"
										class="settings-input"
										:id="`${node.data.path}_${node.data.type}_new-folder-input`"
										@keydown.stop="enableArrowKeysEvents"
										@keydown.esc="showSettings(node)"
										@keydown.enter="createFolder(node.data.path)"
									/>

									<Button
										@click="createFolder(node.data.path)"
										rounded
										severity="primary"
										text
										icon="pi pi-check"
										title="Ок"
									></Button>
									<Button
										@click="showSettings(node)"
										rounded
										severity="danger"
										text
										icon="pi pi-ban"
										title="Отмена"
									></Button>
								</div>

								<!-- Show settings -->
								<Button
									@click="showSettings(node)"
									text
									rounded
									severity="contrast"
									icon="pi pi-cog"
									title="Настройки"
									style="display: none"
									:id="`${node.data.path}_${node.data.type}_show-settings`"
								></Button>

								<!-- Close settings -->
								<Button
									@click="hideButtons"
									text
									rounded
									severity="contrast"
									icon="pi pi-times"
									title="Закрыть"
									style="display: none"
									:id="`${node.data.path}_${node.data.type}_close-settings`"
								></Button>
							</div>
						</div>
					</template>
				</Column>
			</TreeTable>
		</div>
	</div>
</template>

<style scoped>
.items {
	text-align: start;
	padding: 10px;
}

.items-header {
	display: flex;
	flex-direction: row;
	justify-content: space-between;
	align-items: center;
	min-height: 30px;
	padding: 6px 0 0 0;
}

.items-header button {
	padding: 5px;
}

.items-header a {
	text-decoration: none;
	color: black;
}

.tree-table {
	font-size: small;
}

.tree-table:deep(th) {
	display: none;
}

.tree-table:deep(tr) {
	height: 22px;
}

.tree-table:deep(td) {
	padding: 0;
	border: none;
}

.tree-table:deep(button) {
	height: 20px;
	width: 25px;
}

.tree-table:deep(button span) {
	font-size: small;
	background-color: transparent;
}

.tree-table:deep(*) {
	background: var(--COLUMNS-BCKGND-CLR);
}

.file:hover {
	cursor: pointer;
	color: gray;
}

.settings-input {
	max-width: 100px;
	background-color: white;
}

.setting-buttons {
	display: none;
	align-items: center;
	border: solid;
	border-width: 1px;
	background-color: white;
	gap: 0px;
}

.setting-buttons:deep(*) {
	background-color: white;
}

.path-selector {
	height: 20px;
}

.path-selector:deep(span) {
	padding: 2px 0 2px 4px;
	border-radius: 20%;
}

@media (max-width: 1500px) {
	.items-header a span {
		display: none;
	}
}

@media (max-width: 1100px) {
	.tree-table:deep(tr) {
		height: 28px;
	}

	.tree-table {
		font-size: medium;
	}

	.setting-buttons {
		height: 28px;
	}

	.setting-buttons:deep(button span) {
		font-size: 0.9rem;
	}

	.setting-buttons:deep(button) {
		margin: 3px;
	}

	.settings-input {
		height: 28px;
	}

	.settings-input {
		font-size: medium;
	}
}
</style>
