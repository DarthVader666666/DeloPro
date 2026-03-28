<script setup>
import { helper } from '@/helper/helper'
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
	userDeleted: {
		type: Boolean,
		default: false,
	},
	size: {
		type: Number,
		default: 10,
	},
})
</script>

<template>
	<Avatar
		v-if="props.userDeleted"
		:image="helper.getImagePath('icon') + 'deleted-user.png'"
		:style="{
			minWidth: `${props.size}rem`,
			minHeight: `${props.size}rem`,
			'--icon-size': `${props.size / 2}rem`,
		}"
		class="avatar-icon"
		shape="circle"
	></Avatar>
	<Avatar
		v-else-if="props.avatarBase64"
		:image="props.avatarBase64"
		:style="{ minWidth: `${props.size}rem`, minHeight: `${props.size}rem` }"
		shape="circle"
	/>
	<Avatar
		v-else-if="props.avatarPath"
		:image="props.avatarPath"
		:style="{ minWidth: `${props.size}rem`, minHeight: `${props.size}rem` }"
		shape="circle"
	/>
	<Avatar
		v-else
		icon="pi pi-user"
		:style="{
			minWidth: `${props.size}rem`,
			minHeight: `${props.size}rem`,
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
