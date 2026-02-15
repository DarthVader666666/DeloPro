<script setup>
    const props = defineProps({
        propertyValue:
        {
            type: String,
            default: null
        },
        propertyName:
        {
            type: String,
            default: null
        }
    })
</script>

<template>
    <div>
        <span class="account-property-name">{{ props.propertyName }}:</span>
        <span v-if="props.propertyValue">{{ props.propertyValue  }}</span>
        <span v-else class="account-property-undefined">не указано</span>
    </div>
</template>

<style>
    .account-property-undefined {
        font-size: 0.9rem;
        font-style: italic;
        color: gray;
    }
</style>
