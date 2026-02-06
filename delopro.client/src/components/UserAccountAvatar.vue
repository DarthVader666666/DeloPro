<script setup>
import { computed } from 'vue';
import { useStore } from 'vuex';

const store = useStore()
const user = computed(() => store.getters.getCurrentUser);

const props = defineProps({
    avatarBase64:
    {
        type: String,
        default: null
    }
});
</script>

<template>
    <img v-if="props.avatarBase64" :src="props.avatarBase64" class="user-account-avatar">
    <img v-else-if="user.avatarPath" :src="user.avatarPath" class="user-account-avatar">
    <i v-else class="pi pi-user user-account-avatar" style="font-size: 5rem; color: rgb(71, 85, 105, 1); background-color: rgb(241, 245, 249, 1);"></i>
</template>
