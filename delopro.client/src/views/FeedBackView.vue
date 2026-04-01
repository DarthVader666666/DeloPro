<script setup>
import Button from 'primevue/button'
import { useStore } from 'vuex'
import { computed, reactive, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { helper } from '@/helper/helper'
import CaptchaComponent from '@/components/CaptchaComponent.vue'
import PersonalDataAgreement from '@/components/PersonalDataAgreement.vue'
import InputComponent from '@/components/InputComponent.vue'

const store = useStore()
const router = useRouter()
const isAuthenticated = computed(() => store.getters.isAuthenticated)

const isCaptchaMatch = ref(false)
const invalid = ref(false)
const isAgreementChecked = ref(false)
const feedbackForm = reactive({
	name: null,
	email: null,
	phone: null,
	text: null,
	dateSent: null,
})

watch(feedbackForm, (newValue) => {
	if (newValue.email || newValue.phone) {
		invalid.value = false
	}
})

watch(isAuthenticated, (newValue) => {
	if (newValue) {
		router.push('/')
	}
})

async function sendFeedback() {
	if (!(feedbackForm.email || feedbackForm.phone)) {
		invalid.value = true
		return
	}

	if (!feedbackForm.email) {
		feedbackForm.email = ''
	}

	if (!feedbackForm.phone) {
		feedbackForm.phone = ''
	}

	var formData = new FormData()
	formData.append('name', feedbackForm.name)
	formData.append('email', feedbackForm.email)
	formData.append('phone', feedbackForm.phone)
	formData.append('text', feedbackForm.text)
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
	<div>
		<form
			class="feedback-container"
			@submit.prevent="sendFeedback"
		>
			<InputComponent
				title="Ваше имя"
				placeholder="Ваше имя"
				:required="true"
				v-model="feedbackForm.name"
			></InputComponent>

			<h3 style="margin: 20px 0 0 0">Email или номер телефона</h3>

			<InputComponent
				title="Ваш Email"
				placeholder="Ваш Email"
				:invalid="invalid"
				v-model="feedbackForm.email"
				type="email"
				:titleFont="{ fontWeight: 'normal' }"
			></InputComponent>
			<InputComponent
				title="Ваш номер телефона"
				placeholder="Ваш номер телефона"
				type="tel"
				:invalid="invalid"
				v-model="feedbackForm.phone"
				:titleFont="{ fontWeight: 'normal' }"
			></InputComponent>
			<InputComponent
				title="Ваше сообщение"
				placeholder="Ваше сообщение"
				:is-textarea="true"
				:required="true"
				v-model="feedbackForm.text"
				:maxlength="1500"
			></InputComponent>
			<CaptchaComponent
				style="padding: 10px 0 10px 0"
				@captcha-match="setCaptchaMatch"
			></CaptchaComponent>
			<PersonalDataAgreement
				@agreement-checked="isAgreementChecked = !isAgreementChecked"
				:isAgreementChecked="isAgreementChecked"
			></PersonalDataAgreement>
			<div style="padding-top: 10px; display: flex; gap: 10px">
				<Button
					severity="secondary"
					:disabled="
						!(isCaptchaMatch && isAgreementChecked && (feedbackForm.email || feedbackForm.phone))
					"
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
