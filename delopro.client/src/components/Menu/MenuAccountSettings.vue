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
		<div class="close-account-settings-button">
			<Button
				style="width: 30px; height: 30px; color: var(--MENU-TEXT-COLOR)"
				text
				icon="pi pi-times"
				rounded
				@click="emit('setShowAccountSettings')"
			></Button>
		</div>

		<MenuOption
			v-for="(option, index) in props.accountOptions"
			:key="index"
			:routeName="option.routeName"
			:label="option.label"
			:icon="option.icon"
			:clickHandler="option.clickHandler"
			:id="option.id"
		></MenuOption>
	</div>
</template>
<style scoped>
.slide-container {
	padding-top: 10px;
}

.close-account-settings-button {
	text-align: end;

	&:hover {
		color: var(--TEXT-COLOR);
	}
}
</style>
