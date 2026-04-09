<script setup>
import Dialog from 'primevue/dialog'
import SpinningCircle from './SpinningCircle.vue'
</script>
<template>
	<Dialog
		modal
		:closable="false"
		class="pending-modal"
		style="background: transparent; width: 100%; border: none; box-shadow: none"
	>
		<SpinningCircle />
	</Dialog>
</template>

<style>
.pending-modal .p-dialog-content {
	overflow: hidden;
}
</style>
