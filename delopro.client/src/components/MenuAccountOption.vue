<script setup>
import Button from 'primevue/button'

const props = defineProps({
	currentUser: {
		type: Object,
		default: null,
	},
})

const emit = defineEmits(['setShowAccountSettings'])

// const unreadMessagesCount = computed(() => store.getters.getUnreadMessagesCount)
</script>

<template>
	<div class="account">
		<Button
			@click="() => emit('setShowAccountSettings', true)"
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
.account {
	position: absolute;
	right: 20px;
	padding-bottom: 8px;
	display: flex;
	flex-direction: column;
	font-size: medium;
	align-items: end;
}

.account button {
	height: 50px;
	width: 50px;
	border-radius: 50%;
}

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

@media (max-width: 800px) {
	.account {
		position: relative;
		padding: 0 0 15px 0;
		right: 0;
		margin: auto;
	}
}
</style>
