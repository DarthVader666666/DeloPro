<script setup>
import DocumentsComponent from '@/components/Documents/DocumentsComponent.vue'
import Button from 'primevue/button'
import { useRouter } from 'vue-router'

const router = useRouter()
</script>
<template>
	<div class="right-container">
		<div style="height: 48px; text-align: center">
			<Button
				raised
				severity="secondary"
				style="border-radius: 30px; margin-top: 8px; margin-bottom: 4px"
				@click="router.push({ name: 'documents' })"
			>
				<span>Документы</span>
			</Button>
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

@media (max-width: 1100px) {
	.right-container {
		display: none;
	}
}
</style>
