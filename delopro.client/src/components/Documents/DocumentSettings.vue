<script setup>
import { useStore } from 'vuex'
import { useToast } from 'vue-toastification'
import Button from 'primevue/button'
import FileUpload from 'primevue/fileupload'

const props = defineProps({
	node: {
		typeof: Object,
		default: null,
	},
})

const emit = defineEmits(['hideSettings', 'showCreateFolder', 'showRename', 'showMove'])

const toast = useToast()
const store = useStore()

async function uploadFiles(event) {
	const files = event.files
	let uploadModel = new FormData()
	files.forEach((file) => uploadModel.append('files', file))
	uploadModel.append('folderName', props.node.data.path)
	await store.dispatch('uploadDocuments', uploadModel)

	emit('hideSettings')
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

	await store.dispatch('deleteDocument', deleteModel)

	emit('hideSettings')
}

async function copyUrlToClipboard() {
	const url = store.getters.serverUrl.replace('api', '') + props.node.data.path.replace('\\', '/')
	navigator.clipboard.writeText(url)
	toast.success(`Ссылка для "${props.node.data.name}" скопирована`)

	emit('hideSettings')
}
</script>
<template>
	<div>
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
				@click="emit('showMove')"
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
				@click="emit('showCreateFolder')"
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
				@click="emit('showRename')"
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
</template>
<style scoped>
.setting-buttons {
	display: flex;
	flex-direction: column;
	align-items: start;
}
</style>
