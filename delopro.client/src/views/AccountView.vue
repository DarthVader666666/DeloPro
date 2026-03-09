<script setup>
import AvatarCropper from '@/components/Account/AvatarCropper.vue'
import AccountEdit from '@/components/Account/AccountEdit.vue'
import AccountInfo from '@/components/Account/AccountInfo.vue'
import { helper } from '@/helper/helper'
import { computed, ref } from 'vue'
import { useStore } from 'vuex'

const store = useStore()

const modes = {
	info: 'INFO',
	edit: 'EDIT',
	cropper: 'CROPPER',
}

const user = computed(() => store.getters.getCurrentUser)
const avatarBase64 = ref(null)
const currentMode = ref(modes.info)
const isSaveDisabled = ref(true)

function switchToInfoMode() {
	currentMode.value = modes.info
	avatarBase64.value = null
	isSaveDisabled.value = true
}

function switchToEditMode() {
	currentMode.value = modes.edit
}

function switchToAvatarCropper() {
	currentMode.value = modes.cropper
}

async function setAvatarBase64(file) {
	avatarBase64.value = await helper.fileToBase64Async(file)
}

function setIsSaveDisabled(value) {
	isSaveDisabled.value = value
}
</script>

<template>
	<div>
		<AccountInfo
			v-if="user && currentMode === modes.info"
			:user="user"
			:avatarBase64="avatarBase64"
			@switch-to-edit-mode="switchToEditMode"
		></AccountInfo>
		<AccountEdit
			v-show="user && currentMode === modes.edit"
			:user="user"
			:avatarBase64="avatarBase64"
			:isSaveDisabled="isSaveDisabled"
			@switch-to-info-mode="switchToInfoMode"
			@switchToAvatarCropper="switchToAvatarCropper"
			@set-avatar-base64="setAvatarBase64"
			@set-is-save-disabled="setIsSaveDisabled"
		></AccountEdit>
		<AvatarCropper
			v-if="user && currentMode === modes.cropper"
			:user="user"
			:avatarBase64="avatarBase64"
			@switch-to-edit-mode="switchToEditMode"
			@set-avatar-base64="setAvatarBase64"
			@set-is-save-disabled="setIsSaveDisabled"
		></AvatarCropper>
	</div>
</template>
