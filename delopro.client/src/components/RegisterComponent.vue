<script setup>
import { ref, computed, reactive } from 'vue'
import { helper } from '@/helper/helper.js'
import { useStore } from 'vuex'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import CaptchaComponent from '@/components/CaptchaComponent.vue'
import PersonalDataAgreement from './PersonalDataAgreement.vue'
import InputComponent from './InputComponent.vue'

const emit = defineEmits(['register-user'])
const store = useStore()
const router = useRouter()

const isCaptchaMatch = ref(false)
const isAgreementChecked = ref(false)
const showNicknameError = ref(false)
const showEmailError = ref(false)
const repeatPassword = ref(null)
const registerModel = reactive({
	nickname: null,
	email: null,
	firstName: null,
	password: null,
})

const isDisabledSendButton = computed(() => {
	return (
		!(
			registerModel.nickname &&
			registerModel.email &&
			registerModel.password &&
			isMatchPassword.value &&
			isCaptchaMatch.value &&
			isAgreementChecked.value
		) ||
		showNicknameError.value ||
		showEmailError.value ||
		showPasswordsError.value
	)
})

const isMatchPassword = computed(() => {
	return registerModel.password === repeatPassword.value
})

const showPasswordsError = computed(() => {
	if (!repeatPassword.value) {
		return false
	} else {
		return !isMatchPassword.value
	}
})

function sendRegisterRequest() {
	const registerRequest = {
		nickname: registerModel.nickname,
		email: registerModel.email,
		firstName: registerModel.firstName,
		password: registerModel.password,
		registerDate: helper.getCurrentDateString(),
	}

	emit('register-user', registerRequest)

	registerModel.password = null
	repeatPassword.value = null
	store.dispatch('downloadCaptcha')
	document.getElementById('captcha-input').value = null
	isAgreementChecked.value = false
	setCaptchaMatch(false)
}

const handleNicknameMatch = async (event) => {
	const nickname = event.target.value
	showNicknameError.value = await store.dispatch('doesUserExist', {
		nickname: nickname,
		email: null,
	})
}

const handleEmailMatch = async (event) => {
	const email = event.target.value

	if (helper.validateEmail(email)) {
		showEmailError.value = await store.dispatch('doesUserExist', {
			nickname: null,
			email: email,
		})
	}
}

function setCaptchaMatch(isMatch) {
	isCaptchaMatch.value = isMatch
}
</script>

<template>
	<div class="register-inputs">
		<form @submit.prevent="sendRegisterRequest">
			<InputComponent
				title="Имя"
				placeholder="Имя"
				v-model="registerModel.firstName"
				:maxlength="30"
			></InputComponent>
			<InputComponent
				title="Никнэйм"
				placeholder="Никнэйм"
				v-model="registerModel.nickname"
				errorText="Никнэйм занят"
				:showError="showNicknameError"
				:onInput="handleNicknameMatch"
				:maxLength="30"
				:required="true"
				:showRedStar="true"
			></InputComponent>
			<InputComponent
				title="Email"
				placeholder="Email"
				v-model="registerModel.email"
				errorText="Email занят"
				:showError="showEmailError"
				:onInput="handleEmailMatch"
				type="email"
				:required="true"
				:showRedStar="true"
			></InputComponent>
			<InputComponent
				title="Пароль"
				placeholder="Пароль"
				v-model="registerModel.password"
				type="password"
				:maxLength="30"
				:required="true"
				:showRedStar="true"
			></InputComponent>
			<InputComponent
				title="Повторите пароль"
				placeholder="Повторите пароль"
				v-model="repeatPassword"
				errorText="Пароли не совпадают"
				:showError="showPasswordsError"
				type="password"
				:maxLength="30"
				:required="true"
				:showRedStar="true"
			></InputComponent>

			<CaptchaComponent
				@captcha-match="setCaptchaMatch"
				style="padding: 10px 0 10px 0"
			></CaptchaComponent>
			<PersonalDataAgreement
				@agreement-checked="isAgreementChecked = !isAgreementChecked"
				:isAgreementChecked="isAgreementChecked"
			></PersonalDataAgreement>

			<hr />
			<div class="buttons">
				<Button
					severity="secondary"
					type="submit"
					:disabled="isDisabledSendButton"
				>
					Отправить
				</Button>
				<Button
					severity="contrast"
					type="button"
					@click="router.push('/')"
				>
					Отменить
				</Button>
			</div>
		</form>
	</div>
</template>

<style scoped>
.register-inputs {
	margin: 10px;
	display: flex;
	flex-direction: column;
	align-items: start;
	gap: 10px;
	width: 60%;
}

.register-input {
	display: flex;
	flex-direction: column;
}

.buttons {
	display: flex;
	flex-direction: row;
	gap: 15px;
	padding-left: 10px;
}

.error-message {
	position: absolute;
	margin-top: 20px;
	color: red;
	font-weight: lighter;
	font-size: x-small;
}

@media (max-width: 1100px) {
	.register-inputs {
		width: 95%;
	}
}
</style>
