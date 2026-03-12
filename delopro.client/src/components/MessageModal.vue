<script setup>
import { helper } from '@/helper/helper'
import Dialog from 'primevue/dialog'

const props = defineProps({
	message: {
		typeof: Object,
		default: null,
	},
})

const emit = defineEmits(['setShowMessageModal'])
</script>

<template>
	<Dialog
		modal
		@hide="emit('setShowMessageModal')"
		:draggable="false"
		:style="{ width: '35rem', maxHeight: '35rem' }"
	>
		<template #header>
			<span>
				Сообщение от
				<span style="font-weight: bold">{{ ' ' + props.message.name }}</span>
			</span>
		</template>
		<div style="word-break: break-word; overflow-wrap: break-word; text-align: justify">
			{{ props.message.text }}
		</div>
		<template #footer>
			<div class="footer-wrapper">
				<div class="footer-top">
					<span class="footer-name">{{ props.message.name }}</span>
					<span class="footer-date">{{ helper.getDateStringForUI(props.message.dateSent) }}</span>
				</div>
				<div class="footer-contacts">
					{{ props.message.contacts }}
				</div>
			</div>
		</template>
	</Dialog>
</template>
<style scoped>
.footer-wrapper {
	width: 100%;
	display: flex;
	flex-direction: column;
	gap: 6px;
	padding: 12px 20px;
}

.footer-top {
	display: flex;
	justify-content: space-between;
	width: 100%;
}

.footer-name {
	font-weight: bold;
}

.footer-date {
	font-style: italic;
}

.footer-contacts {
	white-space: pre-wrap;
	opacity: 0.8;
}
</style>
