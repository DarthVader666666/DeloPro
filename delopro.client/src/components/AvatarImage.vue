<script setup>
const props = defineProps({
    avatarPath:
    {
        type: String,
        default: null
    },
    avatarBase64:
    {
        type: String,
        default: null
    }
});
</script>

<template>
    <img v-if="props.avatarBase64" :src="props.avatarBase64" class="account-avatar">
    <img v-else-if="props.avatarPath" :src="props.avatarPath" class="account-avatar">
    <i v-else class="pi pi-user account-avatar" style="font-size: 5rem;"></i>
</template>
