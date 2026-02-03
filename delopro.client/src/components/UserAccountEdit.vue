<script setup>
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import Textarea from 'primevue/textarea';

const props = defineProps(
{
   user: 
    {
        type: Object,
        define: null
    }
});

const emit = defineEmits(['switchEditMode']);

</script>

<template>
    <form @submit.prevent="() => {}">
        <div class="user-account-properties">
            <div class="user-account-header">
                <div>
                    <Button raised severity="secondary" style="border-radius: 50%; padding: 0;">
                        <img v-if="props.user.avatar" :src="`data:image/png;base64,' + ${(props.user.avatar ?? '')}`" class="user-account-avatar">
                        <i v-else class="user-account-avatar pi pi-user" style="font-size: 5rem;"></i>
                    </Button>
                </div>
                <div class="user-account-short-info">
                    <span style="font-weight: bold; font-size: large">{{ props.user.nickname }}</span>
                    <span>{{ props.user.firstName }}</span>
                    <span>{{ props.user.lastName }}</span>
                    <span>Роль: {{ props.user.roles }}</span>
                    <span v-if="props.user.registerDate">Дата регистрации: {{ props.user.registerDate }}</span>
                    <div>
                        <Button type="submit" disabled raised severity="secondary" label="Сохранить" style="margin-right: 15px;"/>
                        <Button type="button" @click="emit('switchEditMode', false)" raised severity="contrast" label="Отменить"/>      
                    </div>
                </div>                
            </div>
            <div class="user-account-input">
                <span>Никнэйм:</span>
                <InputText type="text" placeholder="Никнэйм" v-model="props.user.nickname"></InputText>
            </div>
            <div class="user-account-input">
                <span>Email:</span>
                <InputText type="text" placeholder="Email" v-model="props.user.email"></InputText>
            </div>
            <div class="user-account-input">
                <span>Телефон:</span>
                <InputText type="phone" placeholder="Телефон" v-model="props.user.phone"></InputText>
            </div>
            <div class="user-account-input">
                <span>Имя:</span>
                <InputText type="text" placeholder="Имя" v-model="props.user.firstName"></InputText>
            </div>
            <div class="user-account-input">
                <span>Фамилия:</span>
                <InputText type="text" placeholder="Фамилия" v-model="props.user.lastName"></InputText>
            </div>
            <div class="user-account-input">
                <span>Дата рождения:</span>
                <InputText type="date" v-model="props.user.birthDate"></InputText>
            </div>            
            <div class="user-account-input">
                <span>Страна:</span>
                <InputText type="text" placeholder="Страна" v-model="props.user.country"></InputText>
            </div>
            <div class="user-account-input">
                <span>Город:</span>
                <InputText type="text" placeholder="Город" v-model="props.user.city"></InputText>
            </div>
            <div class="user-account-input">
                <span>Должность:</span>
                <InputText type="text" placeholder="Должность" v-model="props.user.userTitle"></InputText>
            </div>
            <div class="user-account-input">
                <span>О себе:</span>
                <Textarea v-model="props.user.info" placeholder="Напишите о себе"></Textarea>
            </div>                        
        </div>
    </form>
</template>
