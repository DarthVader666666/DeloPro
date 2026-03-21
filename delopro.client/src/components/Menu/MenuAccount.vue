<script setup>
import AvatarImage from '../Account/AvatarImage.vue'

const props = defineProps({
	currentUser: {
		type: Object,
		default: null,
	},
})

const emit = defineEmits(['setShowAccountSettings'])
</script>

<template>
	<div class="account-menu-button">
		<AvatarImage
			:avatar-path="props.currentUser?.avatarPath"
			:size="3.5"
			id="account-button"
			@click="() => emit('setShowAccountSettings')"
			style="box-shadow: var(--MENU-BOX-SHADOW)"
		></AvatarImage>
		<span class="account-nickname">{{ props.currentUser?.nickname }}</span>
	</div>
</template>

<style lang="scss" scoped>
.account-menu-button {
	display: flex;
	flex-direction: column;
	align-items: center;
	gap: 3px;

	&:hover {
		cursor: pointer;
	}
}

.account-nickname {
	font-size: 0.9rem;
	color: var(--MENU-TEXT-COLOR);
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
</style>
