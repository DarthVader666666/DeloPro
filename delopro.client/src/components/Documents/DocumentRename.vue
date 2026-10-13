<script setup>
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'
import { ref } from 'vue'
import { useStore } from 'vuex'

const props = defineProps({
	node: {
		typeof: Object,
		default: null,
	},
})

const emit = defineEmits(['hideSettings'])
const store = useStore()
const newName = ref(props.node.data.name)

async function renameDocument() {
	if (newName.value === props.node.data.name) {
		emit('hideSettings')
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
		emit('hideSettings')
	}
}
</script>
<template>
	<div style="display: flex; flex-direction: column; gap: 5px">
		<InputText
			:id="`${props.node.data.path}_${props.node.data.type}_rename-input`"
			type="text"
			v-model="newName"
			@keydown.enter="renameDocument()"
		/>
		<div style="display: flex; justify-content: space-between">
			<Button
				style="width: 90px"
				@click="renameDocument()"
				severity="primary"
				icon="pi pi-check"
				title="Ок"
			>
				Ок
			</Button>
			<Button
				style="width: 90px"
				@click="emit('hideSettings')"
				severity="contrast"
				title="Отмена"
			>
				Отмена
			</Button>
		</div>
	</div>
</template>
<style scoped></style>
