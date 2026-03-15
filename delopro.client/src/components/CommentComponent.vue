<script setup>
import { nextTick, reactive } from 'vue'
import { useStore } from 'vuex'
import AvatarImage from './Account/AvatarImage.vue'
import Button from 'primevue/button'
import Textarea from 'primevue/textarea'
import { helper } from '@/helper/helper'

const store = useStore()
const emit = defineEmits(['setEditingId'])

const props = defineProps({
	comment: {
		type: Object,
	},
	currentUser: {
		type: Object,
	},
	isAdmin: {
		type: Boolean,
	},
	isOwner: {
		type: Boolean,
	},
	editingId: {
		type: Number,
		default: null,
	},
})

const updatedComment = reactive({
	commentId: props.comment.commentId,
	themeId: props.comment.themeId,
	text: props.comment.text,
})

function isYourComment(userId) {
	return userId === props.currentUser?.userId
}

async function deleteComment() {
	if (window.confirm('Вы уверены, что хотите удалить комментарий?')) {
		await store.dispatch('deleteComment', props.comment)
	}
}

function onKeyDown(event) {
	if (event.key === 'Enter') {
		if (props.comment.text === updatedComment.text) {
			setShowTextarea()
			return
		}

		event.preventDefault()
		updateComment()
	}

	if (event.key === 'Escape') {
		cancelUpdate()
	}
}

async function updateComment() {
	await store.dispatch('updateComment', updatedComment)
	setShowTextarea()
}

function cancelUpdate() {
	updatedComment.text = props.comment.text
	setShowTextarea()
}

function setShowTextarea() {
	emit('setEditingId', props.editingId === props.comment.commentId ? null : props.comment.commentId)

	nextTick(() => {
		const el = document.getElementById(`textarea_${props.comment.commentId}`)
		if (el) el.focus()
	})
}
</script>

<template>
	<div
		:id="`comment_${props.comment.commentId}`"
		class="comment"
		:style="{ '--margin-value': isYourComment(props.comment.userId) ? 'auto' : '8px' }"
	>
		<div class="comment-header">
			<div class="left-part">
				<AvatarImage
					:avatar-path="props.comment.avatarPath"
					:user-deleted="!props.comment.userId"
					:size="3"
				></AvatarImage>
				<span>{{ props.comment.nickname ?? 'Удалён' }}</span>
			</div>
			<div class="right-part">
				<div
					v-if="props.editingId === props.comment.commentId"
					class="comment-menu-buttons"
					style="background: lightgray; border-radius: 5px"
				>
					<Button
						v-if="isYourComment(props.comment.userId)"
						text
						rounded
						severity="contrast"
						@click="cancelUpdate"
						title="Отмена"
						icon="pi pi-ban"
					></Button>
					<Button
						v-if="isYourComment(props.comment.userId) || props.isAdmin || props.isOwner"
						text
						rounded
						severity="contrast"
						@click="updateComment()"
						title="Сохранить"
						icon="pi pi-save"
						:disabled="props.comment.text === updatedComment.text"
					></Button>
				</div>
				<div
					v-else-if="isYourComment(props.comment.userId) || isAdmin || isOwner"
					class="comment-menu-buttons"
				>
					<Button
						v-if="isYourComment(props.comment.userId)"
						text
						rounded
						severity="contrast"
						@click="setShowTextarea()"
						title="Редактировать"
						icon="pi pi-pencil"
					></Button>
					<Button
						v-if="isYourComment(props.comment.userId) || props.isAdmin || props.isOwner"
						text
						rounded
						severity="contrast"
						@click="deleteComment()"
						title="Удалить"
						icon="pi pi-trash"
					></Button>
				</div>

				<span>{{ helper.getDateStringForUI(props.comment.dateCreated, false, false) }}</span>

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
			v-if="props.editingId === props.comment.commentId"
			:id="`textarea_${props.comment.commentId}`"
			style="width: 100%; height: 5rem"
			v-model="updatedComment.text"
			@keydown="onKeyDown($event)"
		></Textarea>
		<span
			v-else
			:id="`span_${props.comment.commentId}`"
		>
			{{ props.comment.text }}
		</span>
		<div class="comment-footer">
			<span>
				{{
					props.comment.dateEdited
						? `Ред.: ${helper.getDateStringForUI(props.comment.dateEdited, false, false)}`
						: ''
				}}
			</span>
		</div>
	</div>
</template>

<style scoped>
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

.comment-footer {
	text-align: end;
	padding-top: 10px;
	font-size: 0.7rem;
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
</style>
