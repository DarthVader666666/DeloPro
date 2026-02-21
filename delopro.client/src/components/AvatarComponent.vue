<script setup>
import { useStore } from 'vuex'
import AvatarImage from './AvatarImage.vue'

const props = defineProps({
	avatarPath: {
		type: String,
		default: null,
	},
	avatarBase64: {
		type: String,
		default: null,
	},
	switchToAvatarCropper: {
		type: Function,
		required: true,
	},
	setAvatarBase64: {
		type: Function,
		required: true,
	},
})

const store = useStore()

async function onFileSelect(e) {
	const file = e.target.files[0]

	if (file) {
		await props.setAvatarBase64(file)
		props.switchToAvatarCropper()
	} else {
		e.target.value = ''
	}
}

function deleteAvatar() {
	if (!props.avatarPath) {
		return
	}

	if (window.confirm('Вы уверены, что хотите удалить аватар?')) {
		store.dispatch('deleteAvatar')
		props.setAvatarBase64(null)
	}
}
</script>

<template>
	<div style="position: relative">
		<input
			type="file"
			id="fileInput"
			@change="onFileSelect"
			accept="image/*"
			hidden
		/>
		<AvatarImage
			:avatarPath="props.avatarPath"
			:avatarBase64="props.avatarBase64"
		></AvatarImage>
		<label
			for="fileInput"
			id="avatar-label"
			title="Загрузить фото"
		>
			<div
				class="avatar-button"
				style="bottom: 30%; left: 55%"
			>
				<i
					class="pi pi-camera"
					style="font-size: 2rem"
				></i>
			</div>
		</label>
		<div
			class="avatar-button"
			style="bottom: 30%; left: 10%"
			title="Удалить фото"
			@click="deleteAvatar"
		>
			<i
				class="pi pi-times"
				style="font-size: 1.7rem; padding-top: 5px"
			></i>
		</div>
	</div>
</template>

<style>
.avatar-button :hover {
	cursor: pointer;
	opacity: 0.6;
}

.avatar-button {
	align-content: center;
	text-align: center;
	position: absolute;
	background-color: lightgray;
	opacity: 0.4;
	border-radius: 50%;
	width: 50px;
	height: 50px;
}
</style>
