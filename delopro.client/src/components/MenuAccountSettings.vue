<script setup>
import MenuOption from './MenuOption.vue'
import Button from 'primevue/button'

const props = defineProps({
	options: {
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
			style="width: 25px; height: 25px; position: absolute; right: 5%"
			@click="() => emit('setShowAccountSettings')"
		></Button>
		<div style="padding-top: 25px">
			<MenuOption
				v-for="(option, index) in props.options"
				:key="index"
				:path="option.path"
				:label="option.label"
				:icon="option.icon"
				:clickHandler="option.clickHandler"
				:id="(option.path ?? index) + '_acc_button'"
			></MenuOption>
		</div>
	</div>
</template>

<style scoped></style>
