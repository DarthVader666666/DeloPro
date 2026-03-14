<script setup>
import { computed, nextTick, ref, watch } from 'vue'
import { useStore } from 'vuex'
import AvatarImage from './Account/AvatarImage.vue'
import Button from 'primevue/button'
import Textarea from 'primevue/textarea'
import { helper } from '@/helper/helper'

const store = useStore()
const currentUser = computed(() => store.getters.getCurrentUser)
const isAdmin = computed(() => store.getters.isAdmin)
const isOwner = computed(() => store.getters.isOwner)
const comments = computed(() => store.getters.getComments)
const editingId = ref(null)

const emit = defineEmits(['setShowComments'])

watch(comments, (newValue) => {
	if (!newValue.length) {
		emit('setShowComments')
	}
})

async function deleteComment(comment) {
	if (window.confirm('Вы уверены, что хотите удалить комментарий?')) {
		await store.dispatch('deleteComment', comment)
	}
}

function onKeyDown(event, comment) {
	if (event.key === 'Enter' && !event.shiftKey) {
		event.preventDefault()
		updateComment(comment)
		editingId.value = null
	}
}

async function updateComment(comment) {}

function setShowTextarea(commentId) {
	editingId.value = editingId.value === commentId ? null : commentId

	nextTick(() => {
		const el = document.getElementById(`textarea_${commentId}`)
		if (el) {
			el.focus()
		}
	})
}

function isYourComment(userId) {
	return userId === currentUser?.value?.userId
}
</script>

<template>
	<div class="components">
		<div
			v-for="(comment, index) in comments"
			:key="index"
			:id="`comment_${comment.commentId}`"
			class="comment"
			:style="{ '--margin-value': isYourComment(comment.userId) ? 'auto' : '8px' }"
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
					<div
						v-if="isYourComment(comment.userId) || isAdmin || isOwner"
						class="comment-menu-buttons"
					>
						<Button
							v-if="isYourComment(comment.userId)"
							text
							rounded
							severity="contrast"
							@click="setShowTextarea(comment.commentId)"
							icon="pi pi-pencil"
						></Button>
						<Button
							v-if="isYourComment(comment.userId) || isAdmin || isOwner"
							text
							rounded
							@click="deleteComment(comment)"
							severity="contrast"
							icon="pi pi-trash"
						></Button>
					</div>

					<span>{{ helper.getDateStringForUI(comment.dateCreated, false, false) }}</span>

					<!-- <Button
						v-if="isYourComment(comment.userId) || isAdmin || isOwner"
						class="comment-menu-button"
						icon="pi pi-ellipsis-v"
						severity="contrast"
						text
					></Button> -->
				</div>
			</div>
			<Textarea
				v-if="editingId === comment.commentId"
				:id="`textarea_${comment.commentId}`"
				style="width: 100%"
				v-model="comment.text"
				@keydown="onKeyDown($event, comment)"
			></Textarea>
			<span
				v-else
				:id="`span_${comment.commentId}`"
			>
				{{ comment.text }}
			</span>
		</div>
	</div>
</template>

<style scoped>
.components {
	padding-top: 10px;
	display: flex;
	flex-direction: column;
	gap: 3px;
	animation-name: slide-down;
	animation-duration: 0.2s;
	transform: translateY(0%);
}

.comment {
	padding: 12px;
	margin: 8px;
	width: 85%;
	border-radius: 10px;
	box-shadow: var(--INPUT-BOX-SHADOW);
	background-color: white;
	margin-left: var(--margin-value);
}

/* .comment-menu-button {
	width: 20px;
	display: none;
} */

.comment-menu-buttons :deep(button) {
	width: 30px;
	height: 30px;
}

.comment-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding-bottom: 10px;
}

.left-part {
	display: flex;
	gap: 5px;
	align-items: center;
	font-size: 0.9rem;
}

.right-part {
	right: 0;
	display: flex;
	gap: 10px;
	align-items: center;
	font-size: 0.7rem;
}

.right-part :deep(.p-button) {
	opacity: 0.6;
	color: var(--TEXT-COLOR);
}

@media (max-width: 800px) {
	/* .comment-menu-button {
		display: block;
	} */

	/* .comment-menu-buttons {
		position: absolute;
		display: flex;
		flex-direction: column;
		background: white;
		border-radius: 10px;
		box-shadow: var(--GLOW-BOX-SHADOW);
	} */
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
