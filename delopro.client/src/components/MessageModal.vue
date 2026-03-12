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
		<div style="display: flex; justify-content: space-between; padding: 20px 20px 0 20px">
			<span>{{ props.message.name }}</span>
			<span>{{ helper.getDateStringForUI(props.message.dateSent) }}</span>
		</div>
		<div style="white-space: pre-wrap; padding: 20px 0 0 20px">
			{{ props.message.contacts }}
		</div>
	</Dialog>
</template>
