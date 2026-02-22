<script setup>
import { ref, computed } from 'vue'
import { helper } from '@/helper/helper.js'
import { useStore } from 'vuex'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import CaptchaComponent from '@/components/CaptchaComponent.vue'
import ConfirmAgreement from './ConfirmAgreement.vue'
import InputComponent from './InputComponent.vue'

const props = defineProps({
	pending: {
		type: Boolean,
		default: false,
	},
})

const emit = defineEmits(['register-user'])
const store = useStore()
const router = useRouter()

const isCaptchaMatch = ref(false)
const isAgreementChecked = ref(false)
const showNicknameError = ref(false)
const showEmailError = ref(false)
const repeatPassword = ref(null)
const registerModel = ref({
	nickname: null,
	email: null,
	firstName: null,
	password: null,
})

const isDisabledSendButton = computed(() => {
	return (
		!(
			registerModel.value.nickname &&
			registerModel.value.email &&
			registerModel.value.password &&
			isMatchPassword.value &&
			isCaptchaMatch.value &&
			isAgreementChecked.value
		) ||
		showNicknameError.value ||
		showEmailError.value ||
		showPasswordsError.value ||
		props.pending
	)
})

const isMatchPassword = computed(() => {
	return registerModel.value.password === repeatPassword.value
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
		nickname: registerModel.value.nickname,
		email: registerModel.value.email,
		firstName: registerModel.value.firstName,
		password: registerModel.value.password,
		registerDate: helper.getCurrentDateString(),
	}

	emit('register-user', registerRequest)
}

async function doesUserExist(nickname, email) {
	await helper.timeoutAsync(500)
	return await store.dispatch('checkUserExists', { nickname, email })
}

const handleNicknameMatch = async (event) => {
	const nickname = event.target.value
	showNicknameError.value = await doesUserExist(nickname, null)
}

const handleEmailMatch = async (event) => {
	const email = event.target.value

	if (helper.validateEmail(email)) {
		showEmailError.value = await doesUserExist(null, email)
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
				v-model="registerModel.firstName"
				:maxlength="30"
			></InputComponent>
			<InputComponent
				title="Никнэйм"
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
				v-model="registerModel.password"
				type="password"
				:maxLength="30"
				:required="true"
				:showRedStar="true"
			></InputComponent>
			<InputComponent
				title="Повторите пароль"
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
			<ConfirmAgreement
				@agreement-checked="isAgreementChecked = !isAgreementChecked"
				:isAgreementChecked="isAgreementChecked"
			></ConfirmAgreement>

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
</style>
