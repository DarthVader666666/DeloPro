<script setup>
import TreeTable from 'primevue/treetable'
import Column from 'primevue/column'
import { computed, nextTick, ref } from 'vue'
import { useStore } from 'vuex'

import DocumentSettingsModal from './DocumentSettingsModal.vue'
import { helper } from '@/helper/helper.js'

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

const showSettingsModal = ref(false)
const editedNode = ref(null)
const interval = ref(null)

function downloadFile(node) {
	if (window.confirm('Открыть/скачать файл?')) {
		if (node.data.path) {
			window.open(store.getters.serverUrl.replace('api', '') + node.data.path.replace('\\', '/'))
		}
	}
}

function showSettings(node) {
	editedNode.value = node
	showSettingsModal.value = false
	nextTick(() => (showSettingsModal.value = true))
}

function hideSettings() {
	editedNode.value = null
	showSettingsModal.value = false
}

function startCountdown(node) {
	interval.value = setTimeout(() => showSettings(node), 1200)
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
		>
			<Column
				field="name"
				expander
			>
				<template #body="{ node }">
					<div
						style="display: flex; flex-direction: row; align-items: center; padding: 4px 0 4px 0"
					>
						<div
							@touchstart="startCountdown($event, node)"
							@touchend="stopCountdown"
							@touchmove="stopCountdown"
							@contextmenu.prevent="showSettings(node)"
						>
							<img
								v-if="node.data.type === 'folder'"
								:src="helper.getImagePath('icon') + 'folder.png'"
								style="width: 1.6em; height: 1.4em; vertical-align: -0.3em; padding-right: 3px"
							/>
							<i
								v-else-if="node.data.type === 'root'"
								class="icon pi pi-ellipsis-h"
							></i>
							<img
								v-else
								:src="
									helper.getImagePath('icon') + helper.getDocumentIconName(node.data.name) + '.png'
								"
								style="width: 1.6em; height: 1.4em; vertical-align: -0.3em; padding-right: 3px"
							/>

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
	<DocumentSettingsModal
		v-if="isAdmin || isOwner"
		v-model:visible="showSettingsModal"
		:node="editedNode"
		@hideSettings="hideSettings"
	></DocumentSettingsModal>
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

.file,
.folder,
.icon {
	&:hover {
		cursor: pointer;
		color: gray;
	}
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
