<script setup>
import TreeTable from 'primevue/treetable'
import Column from 'primevue/column'
import { computed, ref } from 'vue'
import { useStore } from 'vuex'

import DocumentSettingsModal from './DocumentSettingsModal.vue'

const props = defineProps({
	scrollHeight: {
		type: String,
		default: '100vh',
	},
})

const store = useStore()

const isAdmin = computed(() => store.getters.isAdmin)
const isOwner = computed(() => store.getters.isOwner)
const documentNodes = computed(() => store.getters.getDocumentNodes)

const expandedNodes = { docs: true }

const showDocumentSettings = ref(false)
const editedNode = ref(null)
const interval = ref(null)

function downloadFile(node) {
	if (node.data.path)
		window.open(store.getters.serverUrl.replace('api', '') + node.data.path.replace('\\', '/'))
}

function disableArrowKeysEvents(event) {
	if ([38, 40, 37, 39].includes(event.keyCode)) {
		event.preventDefault()
	}
}

function showSettings(node, value) {
	editedNode.value = node
	showDocumentSettings.value = value != undefined ? value : false
}

function startCountdown() {
	interval.value = setTimeout(() => showSettings(), 1200)
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
							@touchstart="startCountdown(node)"
							@touchend="stopCountdown"
							@touchmove="stopCountdown"
							@contextmenu.prevent="showSettings(node, true)"
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
			v-model:visible="showDocumentSettings"
			:node="editedNode"
			@showSettings="showSettings"
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

@media (max-width: 1100px) {
	.tree-table:deep(tr) {
		height: 28px;
	}

	.tree-table {
		font-size: small;
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
