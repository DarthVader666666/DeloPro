<script setup>
import Dialog from 'primevue/dialog'
import Textarea from 'primevue/textarea'
import Button from 'primevue/button'
import { computed, reactive, ref } from 'vue'
import { useStore } from 'vuex'
import AvatarImage from './Account/AvatarImage.vue'

const props = defineProps({
	themeId: {
		type: Number,
	},
})

const store = useStore()
const currentUser = computed(() => store.getters.getCurrentUser)
const showEmojiPicker = ref(false)
const emojis = [
	'😊',
	'🙂',
	'😁',
	'😆',
	'😉',
	'😍',
	'😘',
	'🥰',
	'😎',
	'🫠',
	'😐',
	'🤐',
	'🤔',
	'☹️',
	'😞',
	'😢',
	'😭',
	'😨',
	'😱',
	'😦',
	'😮',
	'😴',
	'😵',
	'🤯',
	'😤',
	'🤢',
	'🤮',
	'😵‍💫',
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
		<div>
			<Textarea
				@focus="setShowEmojiPicker(false)"
				v-model="comment.text"
				style="width: 100%; height: 200px; resize: none"
				placeholder="Ваш комментарий"
				required
			></Textarea>
			<div style="display: flex">
				<Button
					rounded
					text
					severity="contrast"
					@click="setShowEmojiPicker()"
				>
					<i
						class="pi pi-face-smile"
						style="font-size: 1.5rem; opacity: 0.6"
					></i>
				</Button>
			</div>
		</div>
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
	position: absolute;
	width: 91%;
	height: 105px;
	overflow-y: scroll;
	border-radius: 10px;
	padding: 5px;
	bottom: 29%;
	background: var(--TEXT-BCKGND-CLR);
	animation-name: slide-up;
	animation-duration: 0.2s;
	transform: translateY(0%);
}

.emoji-picker :hover {
	cursor: pointer;
	background: gray;
	border-radius: 50%;
}

@keyframes slide-up {
	0% {
		transform: translateY(100%);
	}
}
</style>
