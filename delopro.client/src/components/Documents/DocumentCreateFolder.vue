<script setup>
import { ref } from 'vue'
import { useStore } from 'vuex'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'

const props = defineProps({
	node: {
		typeof: Object,
		default: null,
	},
})

const store = useStore()
const emit = defineEmits(['hideSettings'])
const newFolderName = ref(null)

async function createFolder() {
	if (!(props.node.data.path && newFolderName.value)) {
		return
	}

	const folderPathModel = {
		folderPath: props.node.data.path + '\\' + newFolderName.value,
	}

	await store.dispatch('createFolder', folderPathModel)
	emit('hideSettings')
}
</script>
<template>
	<div style="display: flex; flex-direction: column; gap: 5px">
		<InputText
			:id="`${props.node.data.path}_${props.node.data.type}_new-folder-input`"
			type="text"
			v-model="newFolderName"
			@keydown.esc="() => emit('hideSettings')"
			@keydown.enter="createFolder()"
		/>
		<div style="display: flex; justify-content: space-between">
			<Button
				style="width: 90px"
				@click="createFolder()"
				severity="primary"
				title="Ок"
			>
				Ок
			</Button>
			<Button
				style="width: 90px"
				@click="() => emit('hideSettings')"
				severity="contrast"
				icon="pi pi-ban"
				title="Отмена"
			>
				Отмена
			</Button>
		</div>
	</div>
</template>
<style scoped></style>
