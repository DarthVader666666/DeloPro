<script setup>
import TreeTable from 'primevue/treetable'
import Column from 'primevue/column'
import { computed, nextTick, ref } from 'vue'
import { useStore } from 'vuex'

import DocumentSettingsModal from './DocumentSettingsModal.vue'

const props = defineProps({
	scrollHeight: {
		type: String,
		default: '100vh',
	},
})

const store = useStore()

const coords = ref({ x: 0, y: 0 })

const settingsStyle = computed(() => {
	return {
		position: 'absolute',
		left: `${coords.value.x - 250}px`,
		top: `${coords.value.y - 100}px`,
		minWidth: '15rem',
	}
})

const isAdmin = computed(() => store.getters.isAdmin)
const isOwner = computed(() => store.getters.isOwner)
const documentNodes = computed(() => store.getters.getDocumentNodes)

const expandedNodes = { docs: true }

const showSettingsModal = ref(false)
const editedNode = ref(null)
const interval = ref(null)

function downloadFile(node) {
	if (node.data.path) {
		window.open(store.getters.serverUrl.replace('api', '') + node.data.path.replace('\\', '/'))
	}
}

function disableArrowKeysEvents(event) {
	if ([38, 40, 37, 39].includes(event.keyCode)) {
		event.preventDefault()
	}
}

function showSettings(event, node) {
	coords.value = {
		x: event.clientX,
		y: event.clientY,
	}

	editedNode.value = node
	showSettingsModal.value = false
	nextTick(() => (showSettingsModal.value = true))
}

function hideSettings() {
	editedNode.value = null
	showSettingsModal.value = false
}

function startCountdown(event, node) {
	interval.value = setTimeout(() => showSettings(event, node), 1200)
}

function stopCountdown() {
	clearTimeout(interval.value)
}
</script>

<template>
	<div class="items">
		<TreeTable
			:value="documentNodes"
			v-model:expandedKeys="expandedNodes"
			scrollable
			:scrollHeight="props.scrollHeight"
			class="tree-table"
			@keydown="disableArrowKeysEvents"
		>
			<Column
				field="name"
				expander
			>
				<template #body="{ node }">
					<div style="display: flex; flex-direction: row; align-items: center; gap: 5px">
						<!-- Document name -->
						<div
							@touchstart="startCountdown($event, node)"
							@touchend="stopCountdown"
							@touchmove="stopCountdown"
							@contextmenu.prevent="showSettings($event, node)"
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
					</div>
				</template>
			</Column>
		</TreeTable>
	</div>
	<div v-if="isAdmin || isOwner">
		<DocumentSettingsModal
			v-model:visible="showSettingsModal"
			:node="editedNode"
			@hideSettings="hideSettings"
			:style="settingsStyle"
		></DocumentSettingsModal>
	</div>
</template>

<style scoped>
.items {
	text-align: start;
	padding: 10px;
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
	background: transparent;
}

.file:hover {
	cursor: pointer;
	color: gray;
}

.folder:hover {
	cursor: pointer;
	color: gray;
}

@media (max-width: 1100px) {
	.tree-table:deep(tr) {
		height: 28px;
	}

	.tree-table {
		font-size: small;
	}
}
</style>
