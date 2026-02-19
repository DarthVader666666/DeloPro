<script setup>
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'
import Textarea from 'primevue/textarea'
import { computed, reactive, ref, watch } from 'vue'
import { useStore } from 'vuex'
import AccountAvatar from './AccountAvatar.vue'
import { helper } from '@/helper/helper'

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

	store.dispatch('updateCurrentUser', updatedUser)

	if (!needLogout.value) {
		emit('switchToInfoMode')
	} else {
		await store.dispatch('logOut')
		await store.dispatch('logIn')
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
		(updatedUser.phone = props.user.phone),
		(updatedUser.deleteAvatar = false))

	await helper.timeoutAsync(20)
	emit('switchToInfoMode')
}

async function doesUserExist(nickname, email) {
	await helper.timeoutAsync(500)
	return await store.dispatch('checkUserExists', { nickname: nickname, email: email })
}

async function handleNicknameMatch(event) {
	const nickname = event.target.value
	if (nickname === props.user.nickname) {
		return
	}
	showNicknameError.value = await doesUserExist(nickname, null)
}

async function handleEmailMatch(event) {
	const email = event.target.value
	if (email === props.user.email) {
		return
	}
	if (helper.validateEmail(email)) {
		showEmailError.value = await doesUserExist(null, email)
	} else {
		showEmailError.value = false
	}
}
</script>

<template>
	<form @submit.prevent="handleAccountUpdate">
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
						:avatarPath="props.user.avatarPath"
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
					<span style="font-weight: bold; font-size: large">{{ props.user.nickname }}</span>
					<span style="font-size: 1.2rem">
						{{ `${props.user.firstName ?? ''} ${props.user.lastName ?? ''}` }}
					</span>
					<span style="font-style: italic; color: gray">{{ props.user.roles.join(',') }}</span>
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
			<div class="account-input">
				<span>
					Никнэйм:
					<span
						v-if="showNicknameError"
						style="color: red; font-weight: lighter"
					>
						Никнэйм занят
					</span>
				</span>
				<InputText
					type="text"
					placeholder="Никнэйм"
					v-model="updatedUser.nickname"
					@input.prevent="handleNicknameMatch"
					maxlength="30"
					required
				></InputText>
			</div>
			<div class="account-input">
				<span>
					Email:
					<span
						v-if="showEmailError"
						style="color: red; font-weight: lighter"
					>
						Email занят
					</span>
				</span>
				<InputText
					type="email"
					placeholder="Email"
					v-model="updatedUser.email"
					@input.prevent="handleEmailMatch"
					maxlength="50"
					required
				></InputText>
			</div>
			<div class="account-input">
				<span>Телефон:</span>
				<InputText
					type="phone"
					placeholder="Телефон"
					v-model="updatedUser.phone"
				></InputText>
			</div>
			<div class="account-input">
				<span>Имя:</span>
				<InputText
					type="text"
					placeholder="Имя"
					v-model="updatedUser.firstName"
				></InputText>
			</div>
			<div class="account-input">
				<span>Фамилия:</span>
				<InputText
					type="text"
					placeholder="Фамилия"
					v-model="updatedUser.lastName"
				></InputText>
			</div>
			<div class="account-input">
				<span>Дата рождения:</span>
				<InputText
					type="date"
					v-model="updatedUser.birthDate"
				></InputText>
			</div>
			<div class="account-input">
				<span>Страна:</span>
				<InputText
					type="text"
					placeholder="Страна"
					v-model="updatedUser.country"
				></InputText>
			</div>
			<div class="account-input">
				<span>Город:</span>
				<InputText
					type="text"
					placeholder="Город"
					v-model="updatedUser.city"
				></InputText>
			</div>
			<div class="account-input">
				<span>Должность:</span>
				<InputText
					type="text"
					placeholder="Должность"
					v-model="updatedUser.userTitle"
				></InputText>
			</div>
			<div class="account-input">
				<span>О себе:</span>
				<Textarea
					v-model="updatedUser.info"
					placeholder="Напишите о себе"
				></Textarea>
			</div>
		</div>
	</form>
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
</style>
