<script setup>
import Button from 'primevue/button';
import UserAccountProperty from './UserAccountProperty.vue';

    const props = defineProps(
    {
       user: 
        {
            type: Object,
            default: {}
        }
    });

    const emit = defineEmits(['switchEditMode']);
</script>

<template>
    <div class="user-account-properties">
        <div class="user-account-header">
            <div>
                <img v-if="props.user.avatar" :src="`data:image/png;base64,' + ${(props.user.avatar ?? '')}`" class="user-account-avatar">
                <i v-else class="user-account-avatar pi pi-user" style="font-size: 5rem; color: rgb(71, 85, 105, 1); background-color: rgb(241,245,249,1)"></i>
            </div>
            <div class="user-account-short-info">
                <span style="font-weight: bold; font-size: large">{{ props.user.nickname }}</span>
                <span>{{ props.user.firstName }}</span>
                <span>{{ props.user.lastName }}</span>
                <span>Роль: {{ props.user.roles }}</span>
                <span v-if="props.user.registerDate">Дата регистрации: {{ props.user.registerDate }}</span>
                <div style="padding-top: 10px;">
                    <Button @click="emit('switchEditMode', true)" severity="contrast" raised>Редактировать</Button>
                </div>
            </div>
            
        </div>
        <UserAccountProperty propertyName="Никнэйм" :propertyValue="props.user.nickname" />
        <UserAccountProperty propertyName="Email" :propertyValue="props.user.email" />
        <UserAccountProperty propertyName="Телефон" :propertyValue="props.user.phone" />
        <UserAccountProperty propertyName="Имя" :propertyValue="props.user.firstName" />
        <UserAccountProperty propertyName="Фамилия" :propertyValue="props.user.lastName" />
        <UserAccountProperty propertyName="Дата рождения" :propertyValue="props.user.birthDate" />
        <UserAccountProperty propertyName="Страна" :propertyValue="props.user.country" />
        <UserAccountProperty propertyName="Город" :propertyValue="props.user.city" />
        <UserAccountProperty propertyName="Должность" :propertyValue="props.user.userTitle" />
        <UserAccountProperty propertyName="О себе" :propertyValue="props.user.info" />
    </div>
</template>
