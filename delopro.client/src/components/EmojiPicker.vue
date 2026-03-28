<script setup>
import Button from 'primevue/button'

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

const props = defineProps({
	showEmojiPicker: {
		type: Boolean,
		default: false,
	},
	width: {
		type: Number,
		default: 50,
	},
	height: {
		type: Number,
		default: 100,
	},
	left: {
		type: Number,
		default: 15,
	},
	bottom: {
		type: Number,
		default: 50,
	},
})

const emit = defineEmits(['addEmoji', 'setShowEmojiPicker'])
</script>

<template>
	<Button
		rounded
		text
		severity="contrast"
		@click="emit('setShowEmojiPicker')"
	>
		<i
			class="pi pi-face-smile"
			style="font-size: 1.5rem; opacity: 0.6"
		></i>
	</Button>
	<div
		v-if="props.showEmojiPicker"
		class="emoji-picker"
		:style="{
			width: `${props.width}%`,
			height: `${props.height}px`,
			left: `${props.left}%`,
			bottom: `${props.bottom}%`,
		}"
	>
		<span
			v-for="(emoji, index) in emojis"
			:key="index"
			style="padding: 4px; font-size: 1.5rem"
			@click="emit('addEmoji', emoji)"
		>
			{{ emoji }}
		</span>
	</div>
</template>

<style scoped>
.emoji-picker {
	position: absolute;
	overflow-y: scroll;
	border-radius: 10px;
	box-shadow: var(--INPUT-BOX-SHADOW);
	padding: 5px;
	background: var(--TEXT-BACKGROUND);
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
