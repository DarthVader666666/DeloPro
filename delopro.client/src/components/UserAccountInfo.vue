<script setup>
import Button from 'primevue/button';
import UserAccountProperty from './UserAccountProperty.vue';
import UserAccountAvatar from './UserAccountAvatar.vue';
import { useRouter } from 'vue-router';

    const props = defineProps(
    {
        user:
        {
            type: Object,
            default: null
        },
        avatarBase64:
        {
            type: String,
            default: null
        },
    });

    const router = useRouter();
    const emit = defineEmits(['switchToEditMode']);
</script>

<template>
    <div class="user-account-properties">
        <div class="user-account-header">
            <UserAccountAvatar :avatarPath="props.user.avatarPath" :avatarBase64="props.avatarBase64"></UserAccountAvatar>

            <div class="user-account-short-info">
                <span style="font-weight: bold; font-size: large">{{ props.user.nickname }}</span>
                <span style="font-size: 1.2rem;">{{ `${props.user.firstName ?? ''} ${props.user.lastName ?? ''}` }}</span>
                <span style="font-style: italic; color: gray">{{ props.user.roles.join(',') }}</span>
                <span v-if="props.user.registerDate">Дата регистрации: {{ props.user.registerDate }}</span>
                <div style="padding-top: 10px;">
                    <Button @click="async () => emit('switchToEditMode')" severity="contrast" raised>Редактировать</Button>
                </div>
            </div>            
        </div>
        <Button severity="contrast" text rounded 
            style="position: absolute; top: 0; right: 0; height: 45px;"
            @click="() => router.back()">
            <i class="pi pi-times" style="font-size: 1.3rem; padding-top: 3px;"></i>
        </Button>
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
