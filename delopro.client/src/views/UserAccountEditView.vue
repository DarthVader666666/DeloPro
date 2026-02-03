<script setup>
import UserAccountEdit from '@/components/UserAccountEdit.vue';
import UserAccountInfo from '@/components/UserAccountInfo.vue';
import { computed, ref } from 'vue';
import { useStore } from 'vuex';

const store = useStore();
const user = computed(() => store.getters.getUser);
const editMode = ref(false);

function switchEditMode(value) {
    editMode.value = value;
}

</script>

<template>
    <div>
        <UserAccountEdit v-if="editMode" :user="user" @switch-edit-mode="switchEditMode"></UserAccountEdit>
        <UserAccountInfo v-if="!editMode" :user="user" @switch-edit-mode="switchEditMode"></UserAccountInfo>
    </div>    
</template>

<style>

</style>