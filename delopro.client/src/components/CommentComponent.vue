<script setup>
import { nextTick, reactive, ref } from 'vue'
import { useStore } from 'vuex'
import AvatarImage from './Account/AvatarImage.vue'
import Button from 'primevue/button'
import Textarea from 'primevue/textarea'
import { helper } from '@/helper/helper'
import EmojiPicker from './EmojiPicker.vue'

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

const showEmojiPicker = ref(false)

const updatedComment = reactive({
	commentId: props.comment.commentId,
	themeId: props.comment.themeId,
	text: props.comment.text,
})

const editArea = ref(null)

function autoResize() {
	const el = editArea.value?.$el || editArea.value
	if (!el) return

	el.style.height = 'auto'
	el.style.height = el.scrollHeight + 'px'
}

function setShowEmojiPicker(value) {
	showEmojiPicker.value = value != undefined ? value : !showEmojiPicker.value
}

function addEmoji(emoji) {
	updatedComment.text += emoji
}

function isYourComment(userId) {
	return userId === props.currentUser?.userId
}

async function deleteComment() {
	if (window.confirm('Вы уверены, что хотите удалить комментарий?')) {
		await store.dispatch('deleteComment', props.comment)
	}
}

function onKeyDown(event) {
	if (event.key === 'Escape') {
		cancelUpdate()
	}
}

async function updateComment() {
	if (props.comment.text != updatedComment.text) {
		await store.dispatch('updateComment', updatedComment)
	}
	setShowTextarea()
}

function cancelUpdate() {
	updatedComment.text = props.comment.text
	setShowTextarea()
	setShowEmojiPicker(false)
}

function setShowTextarea() {
	const editingId = props.editingId === props.comment.commentId ? null : props.comment.commentId
	emit('setEditingId', editingId)
	setShowEmojiPicker(editingId === null)

	nextTick(() => {
		const el = document.getElementById(`textarea_${props.comment.commentId}`)
		if (el) {
			el.focus()
			el.style.height = 'auto'
			el.style.height = el.scrollHeight + 'px'
		}
	})

	autoResize()
}
</script>

<template>
	<div
		:id="`comment_${props.comment.commentId}`"
		class="comment"
		:class="{ editing: props.editingId === props.comment.commentId }"
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
			</div>
		</div>
		<Textarea
			v-if="props.editingId === props.comment.commentId"
			:id="`textarea_${props.comment.commentId}`"
			ref="editArea"
			class="edit-area"
			maxlength="1000"
			v-model="updatedComment.text"
			@keydown="onKeyDown($event)"
			@focus="setShowEmojiPicker(false)"
		></Textarea>
		<span
			v-else
			:id="`span_${props.comment.commentId}`"
			class="comment-text"
		>
			{{ props.comment.text }}
		</span>
		<div class="comment-footer">
			<div>
				<EmojiPicker
					v-if="props.editingId === props.comment.commentId"
					:show-emoji-picker="showEmojiPicker"
					:width="85"
					:height="100"
					:bottom="-110"
					:left="14"
					@addEmoji="addEmoji"
					@setShowEmojiPicker="setShowEmojiPicker"
				></EmojiPicker>
			</div>

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
	max-width: 85%;
	width: fit-content;
	display: inline-block;
	border-radius: 10px;
	box-shadow: var(--INPUT-BOX-SHADOW);
	background-color: white;
	margin-left: var(--margin-value);
	word-break: break-word;
}

.comment.editing {
	width: 85% !important;
	display: block;
}

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
	position: relative;
	display: flex;
	justify-content: space-between;
	padding-top: 10px;
	font-size: 0.7rem;
}

.comment-text {
	white-space: pre-wrap;
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

.edit-area {
	overflow: scroll;
	resize: none;
	min-height: 6rem;
	width: 100%;
}
</style>
