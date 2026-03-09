<script setup>
const props = defineProps({
	avatarPath: {
		type: String,
		default: null,
	},
	avatarBase64: {
		type: String,
		default: null,
	},
})
</script>

<template>
	<img
		v-if="props.avatarBase64"
		:src="props.avatarBase64"
		class="avatar"
	/>
	<img
		v-else-if="props.avatarPath"
		:src="props.avatarPath"
		class="avatar"
	/>
	<i
		v-else
		class="pi pi-user avatar"
		style="font-size: 5rem"
	></i>
</template>
