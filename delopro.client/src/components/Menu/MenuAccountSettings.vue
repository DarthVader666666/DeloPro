<script setup>
import MenuOption from './MenuOption.vue'
import Button from 'primevue/button'

const props = defineProps({
	accountOptions: {
		type: Array,
		default: () => [],
	},
})

const emit = defineEmits(['setShowAccountSettings'])
</script>

<template>
	<div
		class="slide-container"
		id="account-settings"
	>
		<Button
			text
			icon="pi pi-times"
			rounded
			class="close-account-settings-button"
			@click="emit('setShowAccountSettings')"
		></Button>
		<MenuOption
			v-for="(option, index) in props.accountOptions"
			:key="index"
			:path="option.path"
			:label="option.label"
			:icon="option.icon"
			:clickHandler="option.clickHandler"
			:id="option.id"
		></MenuOption>
	</div>
</template>
<style scoped>
.slide-container {
	padding-top: 40px;
}

.close-account-settings-button {
	position: absolute;
	margin-left: 220px;
	margin-top: -35px;
	width: 40px;
	color: var(--MENU-TEXT-COLOR);

	&:hover {
		color: var(--TEXT-COLOR);
	}
}
</style>
