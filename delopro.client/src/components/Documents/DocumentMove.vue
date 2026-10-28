<script setup>
import Button from 'primevue/button'
import { ref } from 'vue'
import { useStore } from 'vuex'
import DocumentsNavigator from './DocumentsNavigator.vue'

const props = defineProps({
	node: {
		typeof: Object,
		default: null,
	},
})

const store = useStore()
const emit = defineEmits(['hideSettings'])

const selectedFolder = ref(null)

async function openFolder(node) {
	selectedFolder.value = node
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
	<div class="move-container">
		<div style="max-height: 500px; overflow-y: scroll">
			<DocumentsNavigator
				:node="props.node"
				:disable-files="true"
				:useModal="false"
				@onRowSelect="openFolder"
			></DocumentsNavigator>
		</div>
		<div style="padding-top: 15px; display: flex; justify-content: space-between">
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

.move-container {
	display: flex;
	flex-direction: column;
	justify-content: space-between;
	width: 500px;
	height: 600px;
}

@media (max-width: 500px) {
	.move-container {
		width: 100%;
		height: 400px;
	}
}
</style>
