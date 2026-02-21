<script setup>
import Button from 'primevue/button'
import { useStore } from 'vuex'
import { computed, reactive, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { helper } from '@/helper/helper'
import SpinningCircle from '@/components/SpinningCircle.vue'
import CaptchaComponent from '@/components/CaptchaComponent.vue'
import ConfirmAgreement from '@/components/ConfirmAgreement.vue'
import InputComponent from '@/components/InputComponent.vue'

const placeholder = 'Должен быть указан Email и/или Номер телефона'

const store = useStore()
const router = useRouter()

const isCaptchaMatch = ref(false)
const pending = computed(() => store.getters.getPending)
const invalid = ref(false)
const isAgreementChecked = ref(false)
const messageForm = reactive({
	name: null,
	email: null,
	phone: null,
	text: null,
	dateSent: null,
})

watch(messageForm, (oldValue, newValue) => {
	if (newValue.email || newValue.phone) {
		invalid.value = false
		const email = document.getElementById('email')
		const phone = document.getElementById('phone')
		email.setAttribute('placeholder', '')
		phone.setAttribute('placeholder', '')
	}
})

async function sendFeedback() {
	if (!(messageForm.email || messageForm.phone)) {
		invalid.value = true
		const email = document.getElementById('email')
		const phone = document.getElementById('phone')

		email.setAttribute('placeholder', placeholder)
		phone.setAttribute('placeholder', placeholder)

		return
	}

	if (!messageForm.email) {
		messageForm.email = ''
	}

	if (!messageForm.phone) {
		messageForm.phone = ''
	}

	var formData = new FormData()
	formData.append('name', messageForm.name)
	formData.append('email', messageForm.email)
	formData.append('phone', messageForm.phone)
	formData.append('text', messageForm.text)
	formData.append('dateSent', helper.getCurrentDateString())

	const result = await store.dispatch('sendFeedback', formData)

	if (result) {
		router.push('/')
	} else {
		isCaptchaMatch.value = false
	}
}

function setCaptchaMatch(isMatch) {
	isCaptchaMatch.value = isMatch
}
</script>

<template>
	<div v-if="!pending">
		<form
			class="feedback-container"
			@submit.prevent="sendFeedback"
		>
			<InputComponent
				title="Ваше имя"
				:required="true"
				v-model="messageForm.name"
				:titleFont="{ fontWeight: 'normal' }"
			></InputComponent>

			<h3 style="margin: 20px 0 0 0">Email или номер телефона</h3>

			<InputComponent
				title="Ваш Email"
				:required="true"
				:invalid="invalid"
				v-model="messageForm.email"
				type="email"
				:titleFont="{ fontWeight: 'normal' }"
			></InputComponent>
			<InputComponent
				title="Ваш номер телефона"
				type="tel"
				:invalid="invalid"
				v-model="messageForm.phone"
				:titleFont="{ fontWeight: 'normal' }"
			></InputComponent>
			<InputComponent
				title="Ваше сообщение"
				:is-textarea="true"
				:required="true"
				v-model="messageForm.text"
				:titleFont="{ fontWeight: 'normal' }"
			></InputComponent>
			<CaptchaComponent
				style="padding: 10px 0 10px 0"
				@captcha-match="setCaptchaMatch"
			></CaptchaComponent>
			<ConfirmAgreement
				@agreement-checked="isAgreementChecked = !isAgreementChecked"
				:isAgreementChecked="isAgreementChecked"
			></ConfirmAgreement>
			<div style="padding-top: 10px; display: flex; gap: 10px">
				<Button
					severity="secondary"
					:disabled="!(isCaptchaMatch && isAgreementChecked)"
					type="submit"
					raised
				>
					Отправить
				</Button>
				<Button
					severity="contrast"
					raised
					@click="router.push('/')"
				>
					Отменить
				</Button>
			</div>
		</form>
	</div>
	<SpinningCircle
		v-else
		title="Сообщение отправляется..."
	></SpinningCircle>
</template>
<style scoped>
.feedback-container {
	width: 60%;
	padding: 10px;
}

@media (max-width: 1100px) {
	.feedback-container {
		width: 100%;
	}
}
</style>
