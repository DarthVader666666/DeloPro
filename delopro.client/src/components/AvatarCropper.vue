<script setup>
import { ref } from 'vue';
import Button from 'primevue/button';
import { Cropper, CircleStencil } from 'vue-advanced-cropper';
import 'vue-advanced-cropper/dist/style.css';
import { helper } from '@/helper/helper';

const cropper = ref(null);

const props = defineProps({
    avatar:
    {
        type: Uint8Array,
        default: null
    }
})

const emit = defineEmits(['setAvatar','switchAvatarMode','switchEditMode']);

async function handleCrop() {
	const { canvas } = cropper.value.getResult();

	if (canvas) {
    const base64 = canvas.toDataURL("image/png");
    const bytes = helper.base64ToBytes(base64);

    emit('setAvatar', bytes);
    emit('switchAvatarMode', false);

        // To download:
        // const link = document.createElement('a');
        // link.download = 'avatar.png';
        // link.href = canvas.toDataURL();
        // link.click();
	}
};
</script>

<template>
	<div style="align-content: center;">
		<Cropper
			ref="cropper"
			:src="helper.bytesToBase64(props.avatar)"
			:stencil-props="{ aspectRatio: 1 }"
			:stencil-component="CircleStencil"
		/>
		<Button severity="secondary" raised @click="handleCrop">OK</Button>

	</div>
</template>

<style>
.vue-advanced-cropper {
	height: 400px;
	width: 400px;
}
</style>
