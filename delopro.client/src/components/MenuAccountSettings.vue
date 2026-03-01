<script setup>
import { onMounted, onUnmounted } from 'vue'
import MenuOption from './MenuOption.vue'
import { helper } from '@/helper/helper'

const props = defineProps({
	options: {
		type: Array,
		default: () => [],
	},
})

onMounted(() => {
	helper.darkenBackground()
})

onUnmounted(() => {
	helper.lightenBackground()
})
</script>

<template>
	<div
		class="slide-container"
		id="account-settings"
	>
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
</template>

<style scoped></style>
