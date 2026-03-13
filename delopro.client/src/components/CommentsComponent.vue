<script setup>
import { computed } from 'vue'
import { useStore } from 'vuex'
import AvatarImage from './Account/AvatarImage.vue'
import Button from 'primevue/button'
import { helper } from '@/helper/helper'

const store = useStore()
const currentUser = computed(() => store.getters.getCurrentUser)
const isAdmin = computed(() => store.getters.isAdmin)
const isOwner = computed(() => store.getters.isOwner)
const comments = computed(() => store.getters.getComments)

async function deleteComment(comment) {
	if (window.confirm('Вы уверены, что хотите удалить комментарий?')) {
		await store.dispatch('deleteComment', comment)
	}
}
</script>

<template>
	<div class="components">
		<div
			v-for="(comment, index) in comments"
			:key="index"
			class="comment"
		>
			<div class="comment-header">
				<div class="left-part">
					<AvatarImage
						:avatar-path="comment.avatarPath"
						:size="3"
					></AvatarImage>
					<span>{{ comment.nickname }}</span>
				</div>
				<div class="right-part">
					<Button
						v-if="comment.userId === currentUser?.userId"
						text
						rounded
						severity="contrast"
						icon="pi pi-pencil"
					></Button>
					<Button
						v-if="isAdmin || isOwner"
						text
						rounded
						@click="deleteComment(comment)"
						severity="contrast"
						icon="pi pi-trash"
					></Button>
					<span>{{ helper.getDateStringForUI(comment.dateCreated, true) }}</span>
				</div>
			</div>
			<span>{{ comment.text }}</span>
		</div>
	</div>
</template>

<style scoped>
.components {
	padding-top: 10px;
	display: flex;
	flex-direction: column;
	gap: 12px;
	animation-name: slide-down;
	animation-duration: 0.2s;
	transform: translateY(0%);
}

.comment {
	padding: 12px;
	width: 80%;
	border-radius: 10px;
	box-shadow: var(--INPUT-BOX-SHADOW);
	background-color: white;
}

.comment-header {
	position: relative;
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding-bottom: 10px;
}

.left-part {
	display: flex;
	gap: 5px;
	align-items: center;
}

.right-part {
	right: 0;
	top: 0px;
	position: absolute;
	display: flex;
	align-items: center;
	font-size: small;
}

.right-part :deep(.p-button) {
	opacity: 0.8;
	margin: 0;
	color: var(--TEXT-COLOR);
}

@keyframes slide-down {
	0% {
		transform: translateY(-20%);
	}
	100% {
		transform: translateY(0%);
	}
}
</style>
