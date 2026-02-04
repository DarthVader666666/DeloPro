<script setup>
import { ref } from 'vue';
import Button from 'primevue/button';
import { Cropper, CircleStencil } from 'vue-advanced-cropper';
import 'vue-advanced-cropper/dist/style.css';

const cropper = ref(null);

const props = defineProps({
    avatar:
    {
        type: String,
        default: null
    }
})

const emit = defineEmits(['setAvatar','switchAvatarMode','switchEditMode']);

const handleCrop = () => {
	const { canvas } = cropper.value.getResult();
	if (canvas) {
        emit('setAvatar', canvas);
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
			:src="props.avatar"
			:stencil-props="{ aspectRatio: 1 }"
			:stencil-component="CircleStencil"
		/>
		<Button severity="secondary" raised @click="handleCrop">OK</Button>
        
        <!-- <img :src="croppedResult" v-if="croppedResult" alt="Cropped" /> -->
	</div>
</template>

<style>
.vue-advanced-cropper {
	height: 400px;
	width: 400px;
}
</style>
