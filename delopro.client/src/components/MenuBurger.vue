<script setup>
import Button from 'primevue/button'

const props = defineProps({
	nickname: {
		type: String,
		default: null,
	},
})

const emit = defineEmits(['setShowMenu'])
</script>
<template>
	<div class="burger">
		<span style="font-weight: bold">
			{{ props.nickname ?? '' }}
		</span>
		<Button
			@click="emit('setShowMenu', true)"
			security="contrast"
			rounded
			text
			id="burger-button"
		>
			<i class="pi pi-bars"></i>
		</Button>
	</div>
</template>

<style scoped>
.burger {
	display: none;
	align-content: center;
}

.burger button {
	margin: 0 10px 0 10px;
	padding: 12px;
	border-width: 1px;
	border-color: rgba(0, 0, 0, 0.332);
}

.burger i {
	color: var(--TEXT-COLOR);
	font-size: x-large;
}

@media (max-width: 800px) {
	.burger {
		display: flex;
		align-items: center;
	}
}
</style>
