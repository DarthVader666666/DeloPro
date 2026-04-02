<script setup>
import DocumentsComponent from './DocumentsComponent.vue'
</script>
<template>
	<div class="right-container">
		<div class="items-header">
			<span style="margin: 5px 0 6px 0; font-size: large">Документы</span>
		</div>
		<hr style="width: 90%" />
		<DocumentsComponent :scrollHeight="'85vh'"></DocumentsComponent>
	</div>
</template>

<style scoped>
.right-container {
	width: var(--RIGHT-COLUMN-WIDTH);
	background-color: var(--COLUMNS-BACKGROUND);
	position: sticky;
	height: 100vh;
	top: 0;
}

.items-header {
	text-align: center;
	min-height: 37px;
	padding-top: 18px;
	padding-bottom: 8px;
}

@media (max-width: 1100px) {
	.right-container {
		display: none;
	}
}
</style>
