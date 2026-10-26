<script setup>
import { nextTick, ref } from 'vue'
import Dialog from 'primevue/dialog'
import DocumentSettings from './DocumentSettings.vue'
import DocumentMove from './DocumentMove.vue'
import DocumentRename from './DocumentRename.vue'
import DocumentCreateFolder from './DocumentCreateFolder.vue'

const props = defineProps({
	node: {
		typeof: Object,
		default: null,
	},
})

const emit = defineEmits(['hideSettings'])

const showSettings = ref(true)
const showRename = ref(false)
const showCreateFolder = ref(false)
const showMove = ref(false)

function showRenameHandler() {
	closeAll()
	showRename.value = true
	nextTick(() => {
		const renameInput = document.getElementById(
			`${props.node.data.path}_${props.node.data.type}_rename-input`,
		)
		renameInput.focus()
	})
}

function showCreateFolderHandler() {
	closeAll()
	showCreateFolder.value = true
	nextTick(() => {
		const newFolderInput = document.getElementById(
			`${props.node.data.path}_${props.node.data.type}_new-folder-input`,
		)
		newFolderInput.focus()
	})
}

function closeAll() {
	showSettings.value = false
	showRename.value = false
	showCreateFolder.value = false
	showMove.value = false
}
</script>

<template>
	<Dialog
		modal
		style="max-width: 90%; max-height: 90%"
		@show="
			() => {
				closeAll()
				showSettings = true
			}
		"
		@hide="
			() => {
				closeAll()
				showSettings = true
			}
		"
	>
		<template #header>
			<span>{{ props.node.data.name }}</span>
		</template>
		<DocumentSettings
			v-if="showSettings"
			:node="props.node"
			@hideSettings="() => emit('hideSettings')"
			@showRename="showRenameHandler"
			@showCreateFolder="showCreateFolderHandler"
			@showMove="
				() => {
					closeAll()
					showMove = true
				}
			"
		></DocumentSettings>
		<DocumentRename
			v-if="showRename"
			:node="props.node"
			@hideSettings="() => emit('hideSettings')"
		></DocumentRename>
		<DocumentCreateFolder
			v-if="showCreateFolder"
			:node="props.node"
			@hideSettings="() => emit('hideSettings')"
		></DocumentCreateFolder>
		<DocumentMove
			v-if="showMove"
			:node="props.node"
			@hideSettings="() => emit('hideSettings')"
		></DocumentMove>
	</Dialog>
</template>

<style scoped></style>
