<script setup>
import { ref } from 'vue';
import Button from 'primevue/button';
import { Cropper, CircleStencil } from 'vue-advanced-cropper';
import 'vue-advanced-cropper/dist/style.css';
import { helper } from '@/helper/helper';
import { useStore } from 'vuex';

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

const store = useStore();
const emit = defineEmits(['setAvatarBase64', 'switchToEditMode', 'setIsSaveDisabled']);

async function handleCrop() {
	const { canvas } = cropper.value.getResult();

	if (canvas) {
        const blob = await new Promise(resolve => canvas.toBlob(resolve, "image/png"));
        const file = new File([blob], `user_${props.user.userId}_${helper.getCurrentDate(true)}.png`, { type: "image/png" });

        const formData = new FormData();
        formData.append('avatar', file);

        await store.dispatch('uploadAvatar', formData);
        emit('setAvatarBase64', null);
        emit('switchToEditMode');
	}
};

function handleCancel() {
    emit('switchToEditMode');
    emit('setAvatarBase64', null);
};

</script>

<template>
	<div style="display: flex; justify-content: center; padding: 10px;">
		<Cropper
			ref="cropper"
			:src="props.avatarBase64"
			:stencil-props="{ aspectRatio: 1 }"
			:stencil-component="CircleStencil"
            :auto-zoom="false"
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
