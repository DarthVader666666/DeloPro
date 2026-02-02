<script setup>
import { computed } from 'vue';
import { useStore } from 'vuex';

var store = useStore();
var user = computed(() => store.getters.getUser);

</script>

<template>
    <div>
        {{ user }}
    </div>
    <form @submit.prevent="() => {}">
            <InputText type="text" placeholder="Имя">DFSFD</InputText>/>
            <Button type="submit" disabled raised severity="secondary" label="Сохранить"/>
            <Button type="button" @click="() => {}" raised severity="contrast" label="Отменить"/>      
        </form>
</template>

<style>
</style>