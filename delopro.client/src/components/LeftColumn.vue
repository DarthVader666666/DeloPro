<script setup>
import { useStore } from 'vuex'
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import Tree from 'primevue/tree'

const store = useStore()
const router = useRouter()

const isAdmin = computed(() => store.getters.isAdmin)
const isOwner = computed(() => store.getters.isOwner)
const chapterNodes = computed(() => store.getters.getChapterNodes)

const selectedKey = ref(null)

function handleThemeClick(node) {
	if (!node.data) {
		return
	}

	router.push(node.data)
}

const expandedKeys = ref({})

function toggleNode(node) {
	if (node.children && node.children.length > 0) {
		if (expandedKeys.value[node.key]) {
			delete expandedKeys.value[node.key]
		} else {
			expandedKeys.value[node.key] = true
		}

		expandedKeys.value = { ...expandedKeys.value }
	}
}
</script>

<template>
	<div class="left-container">
		<div class="items">
			<div class="items-header">
				<Button
					v-if="isAdmin || isOwner"
					raised
					severity="secondary"
					style="border-radius: 30px"
					@click="router.push({ name: 'create-chapter' })"
				>
					<i class="pi pi-plus"></i>
					<span>Создать</span>
				</Button>
			</div>
			<hr />
			<Tree
				:value="chapterNodes"
				class="tree"
				v-model:selectionKeys="selectedKey"
				selectionMode="single"
				@nodeSelect="handleThemeClick"
				:expandedKeys="expandedKeys"
			>
				<template #default="slotProps">
					<div @click="toggleNode(slotProps.node)">
						{{ slotProps.node.label }}
					</div>
				</template>
			</Tree>
		</div>
	</div>
</template>

<style scoped>
.left-container {
	width: var(--LEFT-COLUMN-WIDTH);
	background-color: var(--COLUMNS-BACKGROUND);
	word-break: break-word;
	position: sticky;
	height: 100vh;
	top: 0;
}

.items {
	text-align: start;
	padding: 0.5rem;
}

.items-header {
	display: flex;
	flex-direction: row;
	justify-content: center;
	align-items: center;
	min-height: 39px;
}

.items-header button {
	padding: 5px;
}

.items-header a {
	text-decoration: none;
	color: black;
}

.tree {
	padding: 0;
	font-size: small;
	font-weight: bold;
	background: var(--COLUMNS-BACKGROUND);
}

.tree:deep(button) {
	height: 20px;
	width: 20px;
}

.tree:deep(div) {
	padding: 1px;
}

.tree:deep(ul li ul li div span div) {
	font-weight: lighter;
}

.tree:deep(*) {
	padding: 0;
	margin: 0;
	font-size: small;
}

@media (max-width: 1100px) {
	.left-container {
		display: none;
	}
}
</style>
