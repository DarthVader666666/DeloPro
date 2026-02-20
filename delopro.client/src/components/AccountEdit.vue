<script setup>
import Button from 'primevue/button'
import { computed, reactive, ref, watch } from 'vue'
import { useStore } from 'vuex'
import AccountAvatar from './AccountAvatar.vue'
import { helper } from '@/helper/helper'
import InputComponent from './InputComponent.vue'

const store = useStore()

const props = defineProps({
	user: {
		type: Object,
		default: null,
	},
	avatarBase64: {
		type: String,
		default: null,
	},
	isSaveDisabled: {
		type: Boolean,
		default: true,
	},
})

const updatedUser = reactive({
	nickname: props.user.nickname,
	firstName: props.user.firstName,
	lastName: props.user.lastName,
	birthDate: props.user.birthDate?.slice(0, 10),
	country: props.user.country,
	city: props.user.city,
	userTitle: props.user.userTitle,
	info: props.user.info,
	email: props.user.email,
	phone: props.user.phone,
	registerDate: props.user.registerDate,
})

const showNicknameError = ref(false)
const showEmailError = ref(false)
const needLogout = computed(
	() => updatedUser.nickname != props.user.nickname || updatedUser.email != props.user.email,
)

const isOldPasswordCorrect = ref(false)
const oldPassword = ref(null)
const newPassword = ref(null)
const repeatNewPassword = ref(null)
const isRepeatPasswordCorrect = computed(
	() => repeatNewPassword.value && newPassword.value === repeatNewPassword.value,
)

const emit = defineEmits([
	'switchToInfoMode',
	'switchToAvatarMode',
	'setAvatarBase64',
	'setIsSaveDisabled',
])

watch(updatedUser, () => {
	emit('setIsSaveDisabled', false)
})

watch(showNicknameError, (newValue) => {
	emit('setIsSaveDisabled', newValue)
})

watch(showEmailError, (newValue) => {
	emit('setIsSaveDisabled', newValue)
})

async function onFileChange(e) {
	const file = e.target.files[0]

	if (file) {
		emit('setAvatarBase64', file)
		emit('switchToAvatarMode')
	}

	e.target.value = ''
}

async function handleAccountUpdate() {
	if (needLogout.value) {
		if (
			!window.confirm('Внимание! После обновления данных необходимо будет заново войти в систему')
		) {
			return
		}
	}

	if (!updatedUser.birthDate) {
		updatedUser.birthDate = null
	}

	await store.dispatch('updateCurrentUser', updatedUser)

	if (!needLogout.value) {
		emit('switchToInfoMode')
	} else {
		await store.dispatch('logOut')
	}
}

function handleDeleteAvatar() {
	if (!props.user.avatarPath) {
		return
	}

	if (window.confirm('Вы уверены, что хотите удалить аватар?')) {
		store.dispatch('deleteAvatar')
		emit('setAvatarBase64', null)
	}
}

async function changePassword() {
	if (window.confirm('Внимание! После смены пароля необходимо будет заново войти в систему')) {
		await store.dispatch('changePassword', newPassword.value)
		await store.dispatch('logOut')
	}
}

async function handleCancel() {
	;((updatedUser.nickname = props.user.nickname),
		(updatedUser.firstName = props.user.firstName),
		(updatedUser.lastName = props.user?.lastName),
		(updatedUser.birthDate = props.user.birthDate?.slice(0, 10)),
		(updatedUser.country = props.user.country),
		(updatedUser.city = props.user.city),
		(updatedUser.userTitle = props.user.userTitle),
		(updatedUser.info = props.user.info),
		(updatedUser.email = props.user.email),
		(updatedUser.phone = props.user.phone))

    showEmailError.value = false
    showNicknameError.value = false
	await helper.timeoutAsync(20)
	emit('switchToInfoMode')
}

async function doesUserExist(nickname, email) {
	await helper.timeoutAsync(500)
	return await store.dispatch('checkUserExists', { nickname: nickname, email: email })
}

async function handleNicknameMatch(nickname) {
	if (nickname === props.user.nickname) {
		return
	}
	showNicknameError.value = await doesUserExist(nickname, null)
}

async function handleEmailMatch(email) {
	if (email === props.user.email) {
		return
	}
	if (helper.validateEmail(email)) {
		showEmailError.value = await doesUserExist(null, email)
	} else {
		showEmailError.value = false
	}
}

async function checkOldPassword(password) {
	const result = await store.dispatch('checkPassword', password)
	isOldPasswordCorrect.value = result && password.length > 0
}

async function deleteAccount() {
	if (window.confirm('Внимание! Ваш аккаунт будет полностью удалён')) {
		await store.dispatch('deleteAccount')
		await store.dispatch('logOut')
	}
}
</script>

<template>
  <div class="account-properties">
		<div class="account-header">
			<div style="position: relative">
				<input
					type="file"
					id="fileInput"
					@change="onFileChange"
					accept="image/*"
					hidden
				/>
				<AccountAvatar
					:avatarPath="props.user?.avatarPath"
					:avatarBase64="props.avatarBase64"
				></AccountAvatar>
				<label
					for="fileInput"
					id="avatar-label"
					title="Загрузить фото"
				>
					<div
						class="avatar-button"
						style="bottom: 30%; left: 55%"
					>
						<i
							class="pi pi-camera"
							style="font-size: 2rem"
						></i>
					</div>
				</label>
				<div
					class="avatar-button"
					style="bottom: 30%; left: 10%"
					title="Удалить фото"
					@click="handleDeleteAvatar"
				>
					<i
						class="pi pi-times"
						style="font-size: 1.7rem; padding-top: 5px"
					></i>
				</div>
			</div>
			<div class="account-short-info">
				<span style="font-weight: bold; font-size: large">{{ props.user?.nickname }}</span>
				<span style="font-size: 1.2rem">
					{{ `${props.user?.firstName ?? ''} ${props.user?.lastName ?? ''}` }}
				</span>
				<span style="font-style: italic; color: gray">{{ props.user?.roles.join(',') }}</span>
				<span
					v-if="updatedUser.registerDate"
					style="font-style: italic"
				>
					Дата регистрации:
					{{ updatedUser.registerDate.slice(0, 10) }}
				</span>
				<div style="padding-top: 10px">
					<Button
						type="submit"
            form="account-form"
						raised
						severity="secondary"
						label="Сохранить"
						style="width: 100px; margin-bottom: 10px; margin-right: 10px"
						:disabled="props.isSaveDisabled"
					></Button>
					<Button
						raised
						severity="contrast"
						label="Отменить"
						style="width: 100px"
						@click="handleCancel"
					/>
				</div>
			</div>
		</div>
    <div class="account-container">
      <form @submit.prevent="handleAccountUpdate" id="account-form">
        <InputComponent
          :title="'Никнэйм'"
          :showError="showNicknameError"
          :errorText="'Никнэйм занят'"
          :placeholder="'Никнэйм'"
          v-model="updatedUser.nickname"
          :inputHandler="handleNicknameMatch"
          :maxlength="30"
          :required="true"
        >
        </InputComponent>
        <InputComponent
          :title="'Email'"
          :showError="showEmailError"
          :errorText="'Email занят'"
          :placeholder="'Email'"
          :type="'email'"
          v-model="updatedUser.email"
          :inputHandler="handleEmailMatch"
          :required="true"
        >
        </InputComponent>
        <InputComponent
          :title="'Телефон'"
          :placeholder="'Телефон'"
          :type="'tel'"
          v-model="updatedUser.phone"
        >
        </InputComponent>
        <InputComponent
          :title="'Имя'"
          :placeholder="'Имя'"
          v-model="updatedUser.firstName"
        >
        </InputComponent>
        <InputComponent
          :title="'Фамилия'"
          :placeholder="'Фамилия'"
          v-model="updatedUser.lastName"
        >
        </InputComponent>
        <InputComponent
          :title="'Дата рождения'"
          :type="'date'"
          v-model="updatedUser.birthDate"
        >
        </InputComponent>
        <InputComponent
          :title="'Страна'"
          :placeholder="'Страна'"
          v-model="updatedUser.country"
        >
        </InputComponent>
        <InputComponent
          :title="'Город'"
          :placeholder="'Город'"
          v-model="updatedUser.city"
        >
        </InputComponent>
        <InputComponent
          :title="'Должность'"
          :placeholder="'Должность'"
          v-model="updatedUser.userTitle"
        >
        </InputComponent>
        <InputComponent
          :title="'О себе'"
          :placeholder="'Напишите о себе'"
          v-model="updatedUser.info"
          :isTextarea="true"
        >
        </InputComponent>
	   </form>
     <div
		    style="background-color: var(--COLUMNS-BCKGND-CLR); padding: 10px"	>
				<h3 style="margin-top: 0">Сменить пароль</h3>
        <InputComponent
          :title="'Cтарый пароль'"
          :type="'password'"
          :placeholder="'Cтарый пароль'"
          :isCorrect="isOldPasswordCorrect"
          v-model="oldPassword"
          :maxlength="30"
          :inputHandler="checkOldPassword"
        >
        </InputComponent>
        <InputComponent
          :title="'Новый пароль'"
          :type="'password'"
          :placeholder="'Новый пароль'"
          :disabled="!oldPassword || !isOldPasswordCorrect"
          v-model="newPassword"
          :maxlength="30"
        >
        </InputComponent>
        <InputComponent
          :title="'Повторите новый пароль'"
          :type="'password'"
          :placeholder="'Повторите новый пароль'"
          :isCorrect="isRepeatPasswordCorrect"
          :disabled="!oldPassword || !isOldPasswordCorrect"
          v-model="repeatNewPassword"
          :maxlength="30"
        >
        </InputComponent>

        <div style="text-align: end;">
					<Button
						severity="secondary"
						raised
						style="margin-top: 10px; width: 100px"
						label="Сменить"
						:disabled="!(isOldPasswordCorrect && isRepeatPasswordCorrect)"
						@click="changePassword"
					></Button>
				</div>
			</div>
			<div
				style="background-color: var(--COLUMNS-BCKGND-CLR); padding: 10px"
			>
				<div class="delete-account">
					<h3>Удалить аккаунт</h3>
					<Button
						severity="danger"
						label="Удалить"
						raised
						style="width: 100px; height: 40px"
						@click="deleteAccount"
					></Button>
				</div>
			</div>
    </div>
	</div>
</template>

<style>
.avatar-button :hover {
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

.change-password {
	display: flex;
	flex-direction: column;
}

.delete-account {
	display: flex;
	flex-direction: row;
	align-items: center;
	justify-content: space-between;
}
</style>
