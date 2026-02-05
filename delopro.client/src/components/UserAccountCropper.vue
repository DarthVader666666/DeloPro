<script setup>
import { ref } from 'vue';
import Button from 'primevue/button';
import { Cropper, CircleStencil } from 'vue-advanced-cropper';
import 'vue-advanced-cropper/dist/style.css';

const cropper = ref(null);

const props = defineProps({
    avatarBase64:
    {
        type: String,
        default: null
    },
    user:
    {
        type: Object,
        default: null
    }
})

const emit = defineEmits(['setAvatarFile', 'switchToEditMode', 'setIsSaveDisabled']);

async function handleCrop() {
	const { canvas } = cropper.value.getResult();

	if (canvas) {
        const blob = await new Promise(resolve => canvas.toBlob(resolve, "image/png"));
        const file = new File([blob], `${props.user.userId}.png`, { type: "image/png" });

        emit('setAvatarFile', file);
        emit('switchToEditMode');
        emit('setIsSaveDisabled', false);
	}
};

function handleCancel() {
    emit('switchToEditMode');
    emit('setAvatarFile', null);
};

</script>

<template>
	<div style="display: flex; justify-content: center; padding: 10px;">
		<Cropper
			ref="cropper"
			:src="props.avatarBase64"
			:stencil-props="{ aspectRatio: 1 }"
			:stencil-component="CircleStencil"
		/>
	</div>
    <div style="display: flex; flex-direction: row; gap: 10px; padding: 10px; justify-content: center;">
        <Button severity="secondary" raised @click="handleCrop" style="width: 90px;">OK</Button>
        <Button severity="contrast" raised @click="handleCancel">Отмена</Button>
    </div>
</template>

<style>
.vue-advanced-cropper {
	height: 400px;
	width: 400px;
}
</style>
