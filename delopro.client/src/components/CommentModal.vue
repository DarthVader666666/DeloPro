<script setup>
import Dialog from 'primevue/dialog'
import Avatar from 'primevue/avatar'
import Textarea from 'primevue/textarea'
import Button from 'primevue/button'
import { computed, reactive, ref } from 'vue'
import { useStore } from 'vuex'

const props = defineProps({
	themeId: {
		type: Number,
	},
})

const store = useStore()
const currentUser = computed(() => store.getters.getCurrentUser)
const showEmojiPicker = ref(true)
const emojis = [
	'🙂',
	'😊',
	'😁',
	'😆',
	'😉',
	'😍',
	'😎',
	'😐',
	'🤔',
	'☹️',
	'😞',
	'🤢',
	'🤮',
	'😡',
	'💩',
	'🤡',
	'💀',
	'🔥',
	'❤️',
	'👋',
	'👌',
	'✌️',
	'🤘',
	'🫵',
	'👈',
	'👉',
	'👆',
	'👇',
	'👍',
	'👎',
	'✊',
	'🤝',
	'👏',
]

const comment = reactive({
	themeId: props.themeId,
	text: '',
})

function addEmoji(emoji) {
	comment.text += emoji
}

function onSave() {
	comment.text = ''
	emit('setShowCommentModal', false)
}

function onCancel() {
	comment.text = ''
	emit('setShowCommentModal', false)
}

const emit = defineEmits(['setShowCommentModal'])
</script>
<template>
	<Dialog
		modal
		@hide="onCancel"
		:draggable="false"
		:style="{ width: '35rem' }"
	>
		<template #header>
			<div style="display: flex; gap: 10px; align-items: center">
				<div>
					<Avatar
						:image="currentUser.avatarPath"
						shape="circle"
						style="width: 50px; height: 50px"
					/>
				</div>
				<div>
					<span class="font-bold whitespace-nowrap">
						{{ currentUser.nickname }}
					</span>
				</div>
			</div>
		</template>
		<div>
			<Textarea
				v-model="comment.text"
				style="width: 100%; height: 200px; resize: none"
				placeholder="Ваш комментарий"
				required
			></Textarea>
			<div style="display: flex">
				<!-- <Button
					rounded
					text
					severity="contrast"
					@click="() => (showEmojiPicker = !showEmojiPicker)"
				>
					<i
						class="pi pi-face-smile"
						style="font-size: 1.5rem; opacity: 0.6"
					></i>
				</Button> -->
				<div
					v-if="showEmojiPicker"
					class="emoji-picker"
				>
					<span
						v-for="(emoji, index) in emojis"
						:key="index"
						style="padding: 4px; font-size: 1.5rem"
						@click="addEmoji(emoji)"
					>
						{{ emoji }}
					</span>
				</div>
			</div>
		</div>

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
<style scoped>
.emoji-picker {
	overflow-y: scroll;
	height: 40px;
	width: 90%;
	border-radius: 10px;
	padding: 3px;
	background: var(--TEXT-BCKGND-CLR);
}

.emoji-picker :hover {
	cursor: pointer;
}
</style>
