<script setup>
import Dialog from 'primevue/dialog'
import Textarea from 'primevue/textarea'
import Button from 'primevue/button'
import { computed, reactive, ref } from 'vue'
import { useStore } from 'vuex'
import AvatarImage from './Account/AvatarImage.vue'
import EmojiPicker from './EmojiPicker.vue'

const props = defineProps({
	themeId: {
		type: Number,
	},
})

const store = useStore()
const currentUser = computed(() => store.getters.getCurrentUser)
const showEmojiPicker = ref(false)

const comment = reactive({
	themeId: props.themeId,
	text: '',
})

function addEmoji(emoji) {
	comment.text += emoji
}

async function onSave() {
	setShowEmojiPicker(false)
	await store.dispatch('createComment', comment)
	comment.text = ''
	emit('setShowCommentModal', false)
}

function onCancel() {
	comment.text = ''
	setShowEmojiPicker(false)
	emit('setShowCommentModal', false)
}

function setShowEmojiPicker(value) {
	showEmojiPicker.value = value != undefined ? value : !showEmojiPicker.value
}

const emit = defineEmits(['setShowCommentModal'])
</script>
<template>
	<Dialog
		modal
		@hide="onCancel"
		:draggable="false"
		:style="{ width: '35rem', position: 'relative' }"
	>
		<template #header>
			<div style="display: flex; gap: 10px; align-items: center">
				<AvatarImage
					:avatar-path="currentUser.avatarPath"
					:size="3.5"
				></AvatarImage>
				<div>
					<span class="font-bold whitespace-nowrap">
						{{ currentUser.nickname }}
					</span>
				</div>
			</div>
		</template>
		<Textarea
			@focus="setShowEmojiPicker(false)"
			v-model="comment.text"
			style="width: 100%; height: 200px; resize: none"
			maxlength="1000"
			placeholder="Ваш комментарий"
			required
		></Textarea>
		<EmojiPicker
			:width="88"
			:height="100"
			:bottom="30"
			:left="6"
			:show-emoji-picker="showEmojiPicker"
			@addEmoji="addEmoji"
			@setShowEmojiPicker="setShowEmojiPicker"
		></EmojiPicker>
		<template #footer>
			<Button
				label="Сохранить"
				severity="secondary"
				raised
				@click="onSave"
			></Button>
			<Button
				label="Отменить"
				severity="contrast"
				raised
				@click="onCancel"
			></Button>
		</template>
	</Dialog>
</template>
<style scoped></style>
