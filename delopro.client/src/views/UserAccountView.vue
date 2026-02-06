<script setup>
import UserAccountCropper from '@/components/UserAccountCropper.vue';
import UserAccountEdit from '@/components/UserAccountEdit.vue';
import UserAccountInfo from '@/components/UserAccountInfo.vue';
import { helper } from '@/helper/helper';
import { computed, ref, watch } from 'vue';
import { useStore } from 'vuex';

const store = useStore();

const modes = {
    info: 'INFO',
    edit: 'EDIT',
    avatar: 'AVATAR'
};

const user = computed(() => store.getters.getCurrentUser);
const avatarFile = ref(null);
const avatarBase64 = ref(null);
const currentMode = ref(modes.info);
const isSaveDisabled = ref(true);

watch(avatarFile, async (newValue) => {
    avatarBase64.value = await helper.fileToBase64Async(newValue);
});

function switchToInfoMode() {
    currentMode.value = modes.info;
    avatarFile.value = null;
    avatarBase64.value = null;
    isSaveDisabled.value = true;
}

function switchToEditMode() {
    currentMode.value = modes.edit;
}

function switchToAvatarMode() {
    currentMode.value = modes.avatar;
}

async function setAvatarFile(file) {
    avatarFile.value = file;
};

function setIsSaveDisabled(value) {
    isSaveDisabled.value = value;
}

</script>

<template>
    <div>
        <UserAccountInfo v-if="user && currentMode === modes.info"
            :user="user" :avatarBase64="avatarBase64"
            @switch-to-edit-mode="switchToEditMode">
        </UserAccountInfo>
        <UserAccountEdit v-show="user && currentMode === modes.edit"
            :user="user" :avatarFile="avatarFile" :avatarBase64="avatarBase64" :isSaveDisabled="isSaveDisabled"
            @switch-to-info-mode="switchToInfoMode" @switch-to-avatar-mode="switchToAvatarMode" @set-avatar-file="setAvatarFile" @set-is-save-disabled="setIsSaveDisabled">
        </UserAccountEdit>
        <UserAccountCropper v-if="user && currentMode === modes.avatar"
            :user="user" :avatarBase64="avatarBase64"
            @switch-to-edit-mode="switchToEditMode" @switch-to-avatar-mode="switchToAvatarMode" @set-avatar-file="setAvatarFile" @set-is-save-disabled="setIsSaveDisabled">
        </UserAccountCropper>
    </div>
</template>
