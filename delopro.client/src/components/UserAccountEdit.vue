<script setup>
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import Textarea from 'primevue/textarea';
import { onMounted, reactive, ref } from 'vue';
import { useStore } from 'vuex';
import axios from 'axios';
import { useToast } from 'vue-toastification';

const store = useStore();
const toast = useToast();

const props = defineProps(
{
    user: 
    {
        type: Object,
        default: {}
    },
    avatar:
    {
        type: Object,
        default: null
    }
});

const updatedUser = reactive({
    nickname: props.user.nickname,
    firstName: props.user.firstName,
    lastName: props.user.lastName,
    birthDate: props.user.birthDate,
    country: props.user.country,
    city: props.user.city,
    userTitle: props.user.userTitle,
    info: props.user.info,
    avatar: props.user.avatar,
    email: props.user.email,
    phone: props.user.phone
});

const emit = defineEmits(['switchEditMode','switchAvatarMode','setAvatar']);

onMounted(() => {
    updatedUser.nickname = props.user.nickname;
    updatedUser.firstName = props.user.firstName;
    updatedUser.lastName = props.user.lastName;
    updatedUser.birthDate = props.user.birthDate;
    updatedUser.country = props.user.country;
    updatedUser.city = props.user.city;
    updatedUser.userTitle = props.user.userTitle;
    updatedUser.info = props.user.info;
    updatedUser.avatar = props.user.avatar;
    updatedUser.email = props.user.email;
    updatedUser.phone = props.user.phone;

    console.log(updatedUser)
});

const onFileChange = (e) => {
  const file = e.target.files[0]

  if (file) {
    emit('setAvatar', URL.createObjectURL(file));
    emit('switchAvatarMode', true);
  }
}

async function handleUserAccountUpdate() {
    if(props.avatar) {
        updatedUser.avatar = props.avatar.ToBlob();
    }

    console.log(updatedUser);

    const url = store.state.serverUrl;

    await axios.put(`${url}/useraccount/updatecurrentuser`,  updatedUser,
    {
        headers: {
            'Content': 'application/json',
            'Accept': '*/*'
        }
    })
    .then(response => {
        if(response.status === 200) {
            toast.success(`Параметры пользователя ${updatedUser.value.nickname} обновлены`);
            store.dispatch('downloadCurrentUser');
            router.push('/user-account');
        }
    })
    .catch(error => {
        if(error.response) {
            toast.error(error.response.data.errorText)
        }
    });
}

</script>

<template>
    <form @submit.prevent="handleUserAccountUpdate">
        <div class="user-account-properties">
            <div class="user-account-header">
                <div style="position: relative;">
                  <input type="file" id="fileInput" @change="onFileChange" accept="image/*" hidden />

                  <img v-if="props.user.avatar" :src="props.user.avatar.toDataURL()" class="user-account-avatar">
                  <img v-else-if="props.avatar" :src="props.avatar.toDataURL()" class="user-account-avatar">

                  <i v-else class="user-account-avatar pi pi-user" style="font-size: 5rem; color: rgb(71, 85, 105, 1); background-color: rgb(241,245,249,1)"></i>

                  <label for="fileInput" id="avatar-label">
                        <div class="avatar-camera-icon">
                            <i class="pi pi-camera" style="font-size: 3rem;" ></i>
                        </div>
                  </label>
                </div>
                <div class="user-account-short-info">
                    <span style="font-weight: bold; font-size: large">{{ updatedUser.nickname }}</span>
                    <span>{{ updatedUser.firstName }}</span>
                    <span>{{ updatedUser.lastName }}</span>
                    <span>Роль: {{ props.user.roles }}</span>
                    <span v-if="updatedUser.registerDate">Дата регистрации: {{ updatedUser.registerDate }}</span>
                    <div style="padding-top: 10px;">
                        <Button type="submit" raised severity="secondary" label="Сохранить" style="width: 100px; margin-bottom: 10px; margin-right: 10px;"/>
                        <Button raised severity="contrast" label="Отменить" style="width: 100px;" @click="emit('switchEditMode', false)"/>      
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
    #avatar-label :hover{
        cursor: pointer;
        opacity: 0.6;
    }

    .avatar-camera-icon {
        align-content: center; 
        text-align: center; 
        position: absolute; 
        right: 27%; 
        bottom: 27%; 
        background-color: lightgray; 
        opacity: 0.4; 
        border-radius: 50%; 
        width: 70px;
        height: 70px;
    }
</style>