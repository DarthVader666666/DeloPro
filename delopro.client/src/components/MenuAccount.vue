<script setup>
import Button from 'primevue/button'

const props = defineProps({
	currentUser: {
		type: Object,
		default: null,
	},
})

const emit = defineEmits(['setShowAccountSettings'])
</script>

<template>
	<div style="display: flex; flex-direction: column; font-size: medium; align-items: center">
		<Button
			style="height: 50px; width: 50px; border-radius: 50%"
			@click="() => emit('setShowAccountSettings')"
			severity="secondary"
			rounded
			id="account-button"
		>
			<img
				v-if="props.currentUser?.avatarPath"
				:src="props.currentUser?.avatarPath"
				width="50px"
				height="50px"
			/>
			<i
				v-else
				class="pi pi-user"
				style="font-size: x-large"
			></i>
		</Button>
		<span>{{ props.currentUser?.nickname }}</span>
	</div>
</template>

<style scoped>
.unread-messages-count {
	right: 0;
	background: red;
	color: white !important;
	font-size: small;
	font-weight: normal !important;
	padding: 3px 0 0 0;
	border-radius: 50%;
	height: 20px;
	width: 20px;
}
</style>
