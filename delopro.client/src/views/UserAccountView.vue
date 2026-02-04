<script setup>
import AvatarCropper from '@/components/AvatarCropper.vue';
import UserAccountEdit from '@/components/UserAccountEdit.vue';
import UserAccountInfo from '@/components/UserAccountInfo.vue';
import { computed, ref } from 'vue';
import { useStore } from 'vuex';

const store = useStore();
const user = computed(() => store.getters.getUser);
const editMode = ref(false);
const avatarMode = ref(false);
const avatar = ref(null);

function switchEditMode(value) {
    editMode.value = value;
}

function switchAvatarMode(value) {
    avatarMode.value = value;
}

function setAvatar(canvas) {
    avatar.value = canvas;
}

</script>

<template>
    <div>
        <UserAccountEdit v-if="user && editMode && !avatarMode" :user="user" :avatar="avatar" @switch-edit-mode="switchEditMode" @switch-avatar-mode="switchAvatarMode" @set-avatar="setAvatar"></UserAccountEdit>
        <UserAccountInfo v-if="user && !editMode && !avatarMode" :user="user" @switch-edit-mode="switchEditMode"></UserAccountInfo>
        <AvatarCropper v-if="avatarMode" :avatar="avatar" @switch-edit-mode="switchEditMode" @switch-avatar-mode="switchAvatarMode" @set-avatar="setAvatar"></AvatarCropper>
    </div>    
</template>
