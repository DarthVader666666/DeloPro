<script setup>
import Button from 'primevue/button'
import { useRouter } from 'vue-router'

const router = useRouter()

const props = defineProps({
	label: {
		type: String,
		required: true,
	},
	clickHandler: {
		type: Function,
	},
	path: {
		type: String,
		default: null,
	},
	icon: {
		type: String,
		default: null,
	},
})

function clicked() {
	if (props.path) {
		router.push(props.path)
	}

	if (props.clickHandler) {
		props.clickHandler(true)
	}
}
</script>

<template>
	<Button
		class="menu-button"
		@click="clicked"
		:label="props.label"
		:icon="props.icon"
		severity="contrast"
		text
	></Button>
</template>

<style scoped>
.menu-button {
	border-radius: 0;
	font-weight: bold;
	color: var(--TEXT-COLOR);
}
</style>
