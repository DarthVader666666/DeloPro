<script setup>
import DocumentsComponent from './DocumentsComponent.vue'
</script>
<template>
	<div id="right-container">
		<div class="items-header">
			<span style="margin: 5px 0 6px 0; font-size: large">Документы</span>
		</div>
		<hr style="width: 90%" />
		<DocumentsComponent :scrollHeight="'85vh'"></DocumentsComponent>
	</div>
</template>

<style scoped>
.items-header {
	text-align: center;
	min-height: 37px;
	padding-top: 18px;
	padding-bottom: 8px;
}
</style>
