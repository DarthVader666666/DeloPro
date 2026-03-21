<script setup>
import Button from 'primevue/button'

const props = defineProps({
	nickname: {
		type: String,
		default: null,
	},
})

const emit = defineEmits(['setShowSlideMenu'])
</script>
<template>
	<div class="burger">
		<span>
			{{ props.nickname ?? '' }}
		</span>
		<Button
			@click="emit('setShowSlideMenu')"
			security="contrast"
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
	color: var(--MENU-TEXT-COLOR);
}

.burger button {
	margin: 0 10px 0 10px;
	padding: 12px;
	border-width: 2px;
	border-color: var(--MENU-TEXT-COLOR);
	&:hover {
		border-width: 2px;
	}
}

.burger i {
	color: var(--MENU-TEXT-COLOR);
	font-size: x-large;
}

@media (max-width: 1000px) {
	.burger {
		display: flex;
		align-items: center;
	}
}
</style>
