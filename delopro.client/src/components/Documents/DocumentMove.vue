<script setup>
import Button from 'primevue/button'
import Select from 'primevue/select'
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

const moveFolder = ref(null)
const folderPaths = computed(() => store.getters.getFolderPaths)

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
		moveFolder.value = null
	}
}
</script>
<template>
	<div style="display: flex; flex-direction: column; gap: 5px">
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
		<div style="display: flex; justify-content: space-between">
			<Button style="width: 90px">Ок</Button>
			<Button
				style="width: 90px"
				@click="emit('hideSettings')"
				severity="contrast"
				title="Назад"
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
