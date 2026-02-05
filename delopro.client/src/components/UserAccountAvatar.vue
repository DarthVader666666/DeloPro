<script setup>

    const props = defineProps({
        user: 
        {
            type: Object,
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
    <img v-if="props.user.avatar" :src="props.user.avatar" class="user-account-avatar">
    <img v-else-if="props.avatarBase64" :src="props.avatarBase64" class="user-account-avatar">
    <i v-else class="pi pi-user user-account-avatar" style="font-size: 5rem; color: rgb(71, 85, 105, 1); background-color: rgb(241, 245, 249, 1);"></i>
</template>

<style>
    
</style>