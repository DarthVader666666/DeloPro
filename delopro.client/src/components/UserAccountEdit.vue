<script setup>
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import Textarea from 'primevue/textarea';
import { reactive, watch } from 'vue';
import { useStore } from 'vuex';
import axios from 'axios';
import { useToast } from 'vue-toastification';
import UserAccountAvatar from './UserAccountAvatar.vue';

const store = useStore();
const toast = useToast();

const props = defineProps(
{
    user:
    {
        type: Object,
        default: null
    },
    avatarFile:
    {
        type: File,
        default: null
    },
    avatarBase64:
    {
        type: String,
        default: null
    },
    isSaveDisabled:
    {
        type: Boolean,
        default: true
    }
});

let updatedUser = reactive({
    nickname: props.user.nickname,
    firstName: props.user.firstName,
    lastName: props.user.lastName,
    birthDate: props.user.birthDate,
    country: props.user.country,
    city: props.user.city,
    userTitle: props.user.userTitle,
    info: props.user.info,
    email: props.user.email,
    phone: props.user.phone,
    deleteAvatar: false
});

watch(updatedUser, () => {
    emit('setIsSaveDisabled', false);
});

const emit = defineEmits(['switchToInfoMode','switchToAvatarMode','setAvatarFile', 'setIsSaveDisabled']);

async function onFileChange(e) {
  const file = e.target.files[0];

  if (file) {
    emit('setAvatarFile', file);
    emit('switchToAvatarMode');
  }
}

async function handleUserAccountUpdate() {
    const formData = new FormData();

    formData.append('user', JSON.stringify(updatedUser));

    if(props.avatarFile) {
        formData.append('avatar', props.avatarFile);
    }
    else {
        formData.append('avatar', null);
    }

    const url = store.state.serverUrl;

    const propmise = axios.put(`${url}/useraccount/updatecurrentuser`, formData,
    {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    })
    .then(response => {
        if(response.status === 200) {
            toast.success(`Параметры пользователя ${updatedUser.value.nickname} обновлены`);
            store.dispatch('downloadCurrentUser');
        }
    })
    .catch(error => {
        if(error.response) {
            toast.error(error.response.data.errorText)
        }
    });

    await propmise;

    emit('switchToInfoMode');
}

function handleDeleteAvatar() {
    if (window.confirm("Вы уверены, что хотите удалить аватар?")) {
        emit('setAvatarFile', null);
        updatedUser.deleteAvatar = true;
    }
}

</script>

<template>
    <form @submit.prevent="handleUserAccountUpdate">
        <div class="user-account-properties">
            <div class="user-account-header">
                <div style="position: relative;">
                  <input type="file" id="fileInput" @change="onFileChange" accept="image/*" hidden />

                  <UserAccountAvatar :user="props.user" :avatarBase64="props.avatarBase64"></UserAccountAvatar>

                  <label for="fileInput" id="avatar-label" title="Загрузить фото">
                        <div class="avatar-button" style="bottom: 30%; left: 55%;">
                            <i class="pi pi-camera" style="font-size: 2rem;" ></i>
                        </div>
                  </label>
                  <div class="avatar-button" style="bottom: 30%; left: 10%;" title="Удалить фото" @click="handleDeleteAvatar">
                        <i class="pi pi-times" style="font-size: 1.7rem; padding-top: 5px;"></i>
                  </div>

                </div>
                <div class="user-account-short-info">
                    <span style="font-weight: bold; font-size: large">{{ props.user.nickname }}</span>
                    <span>{{ props.user.firstName }}</span>
                    <span>{{ props.user.lastName }}</span>
                    <span>Роль: {{ props.user.roles }}</span>
                    <span v-if="updatedUser.registerDate">Дата регистрации: {{ updatedUser.registerDate }}</span>
                    <div style="padding-top: 10px;">
                        <Button type="submit" raised severity="secondary" label="Сохранить" style="width: 100px; margin-bottom: 10px; margin-right: 10px;" :disabled="props.isSaveDisabled"/>
                        <Button raised severity="contrast" label="Отменить" style="width: 100px;" @click="emit('switchToInfoMode')"/>
                    </div>
                </div>
            </div>
            <div class="user-account-input">
                <span>Никнэйм:</span>
                <InputText type="text" placeholder="Никнэйм" v-model="updatedUser.nickname"></InputText>
            </div>
            <div class="user-account-input">
                <span>Email:</span>
                <InputText type="text" placeholder="Email" v-model="updatedUser.email"></InputText>
            </div>
            <div class="user-account-input">
                <span>Телефон:</span>
                <InputText type="phone" placeholder="Телефон" v-model="updatedUser.phone"></InputText>
            </div>
            <div class="user-account-input">
                <span>Имя:</span>
                <InputText type="text" placeholder="Имя" v-model="updatedUser.firstName"></InputText>
            </div>
            <div class="user-account-input">
                <span>Фамилия:</span>
                <InputText type="text" placeholder="Фамилия" v-model="updatedUser.lastName"></InputText>
            </div>
            <div class="user-account-input">
                <span>Дата рождения:</span>
                <InputText type="date" v-model="updatedUser.birthDate"></InputText>
            </div>
            <div class="user-account-input">
                <span>Страна:</span>
                <InputText type="text" placeholder="Страна" v-model="updatedUser.country"></InputText>
            </div>
            <div class="user-account-input">
                <span>Город:</span>
                <InputText type="text" placeholder="Город" v-model="updatedUser.city"></InputText>
            </div>
            <div class="user-account-input">
                <span>Должность:</span>
                <InputText type="text" placeholder="Должность" v-model="updatedUser.userTitle"></InputText>
            </div>
            <div class="user-account-input">
                <span>О себе:</span>
                <Textarea v-model="updatedUser.info" placeholder="Напишите о себе"></Textarea>
            </div>
        </div>
    </form>
</template>

<style>
    .avatar-button :hover{
        cursor: pointer;
        opacity: 0.6;
    }

    .avatar-button {
        align-content: center;
        text-align: center;
        position: absolute;
        background-color: lightgray;
        opacity: 0.4;
        border-radius: 50%;
        width: 50px;
        height: 50px;
    }
</style>
