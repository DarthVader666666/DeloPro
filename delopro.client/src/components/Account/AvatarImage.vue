<script setup>
import Avatar from 'primevue/avatar'

const props = defineProps({
	avatarPath: {
		type: String,
		default: null,
	},
	avatarBase64: {
		type: String,
		default: null,
	},
	size: {
		type: Number,
		default: 10,
	},
})
</script>

<template>
	<Avatar
		v-if="props.avatarBase64"
		:image="props.avatarBase64"
		:style="{ width: `${props.size}rem`, height: `${props.size}rem` }"
		shape="circle"
	/>
	<Avatar
		v-else-if="props.avatarPath"
		:image="props.avatarPath"
		:style="{ width: `${props.size}rem`, height: `${props.size}rem` }"
		shape="circle"
	/>
	<Avatar
		v-else
		icon="pi pi-user"
		:style="{
			width: `${props.size}rem`,
			height: `${props.size}rem`,
			'--icon-size': `${props.size / 2}rem`,
		}"
		class="avatar-icon"
		shape="circle"
	/>
</template>

<style>
.avatar-icon .pi {
	font-size: var(--icon-size);
	display: flex;
	align-items: center;
	justify-content: center;
}
</style>
