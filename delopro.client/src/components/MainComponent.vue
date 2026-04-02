<script setup>
import LeftColumn from './LeftColumn.vue'
import RightColumn from './RightColumn.vue'
import { RouterView } from 'vue-router'
import { useStore } from 'vuex'
import { computed } from 'vue'
import SpinningCircle from './SpinningCircle.vue'

const store = useStore()
const pending = computed(() => store.getters.getPending)
const title = computed(() => store.getters.getTitle)
const showRightColumn = computed(() => store.getters.getShowRightColumn)
</script>

<template>
	<div
		v-if="title"
		class="title"
	>
		<h2>{{ title }}</h2>
	</div>
	<div class="main-container">
		<LeftColumn />
		<SpinningCircle v-if="pending" />
		<RouterView
			v-show="!pending"
			class="central-container"
		/>
		<RightColumn v-if="showRightColumn" />
	</div>
</template>

<style scoped>
.main-container {
	display: flex;
	flex-direction: row;
	min-height: var(--MAIN-COMPONENT-MIN-HEIGHT);
	width: var(--MAIN-COMPONENT-WIDTH);
	box-shadow: var(--COMPONENT-BOX-SHADOW);
}

.central-container {
	width: var(--CENTRAL-COLUMN-WIDTH);
	background-color: var(--CENTRAL-BACKGROUND);
	padding: 10px;
	overflow-wrap: break-word;
}

.title {
	text-align: center;
	align-content: center;
	height: var(--TITLE-HEIGHT);
	margin: 10px 0 10px 0;
	background-color: var(--MENU-BACKGROUND);
	box-shadow: var(--COMPONENT-BOX-SHADOW);
}

.title h2 {
	color: var(--MENU-TEXT-COLOR);
	margin: 0 10px 0 10px;
}

@media (max-width: 1100px) {
	.central-container {
		width: 100%;
		padding: 10px 0 0 0;
	}
}
</style>
