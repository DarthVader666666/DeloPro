<script setup>
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'
import { useStore } from 'vuex'
import { computed, ref } from 'vue'

import { useRouter } from 'vue-router'
import SpinningCircle from '@/components/SpinningCircle.vue'
import CaptchaComponent from '@/components/CaptchaComponent.vue'

const store = useStore()
const router = useRouter()

const pending = computed(() => store.getters.getPending)
const isCaptchaMatch = ref(false)
const email = ref(null)

async function sendRecoverPasswordRequest() {
	const result = store.dispatch('recoverPassword', email.value)

	if (!result) {
		email.value = null
	}

	isCaptchaMatch.value = false
}

function setCaptchaMatch(isMatch) {
	isCaptchaMatch.value = isMatch
}
</script>

<template>
	<div class="recover-password-container">
		<div v-if="!pending">
			<h3 style="padding: 11px">Новый пароль будет отправлен на ваш email</h3>
			<form
				@submit.prevent="sendRecoverPasswordRequest"
				class="send-message-form"
			>
				<div class="send-message-input">
					<span>Ваш Email:</span>
					<InputText
						type="email"
						v-model="email"
						required
					></InputText>
				</div>
				<CaptchaComponent @captcha-match="setCaptchaMatch"></CaptchaComponent>
				<div>
					<Button
						severity="secondary"
						:disabled="!(isCaptchaMatch && email)"
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
			text="Пожалуйста, подождите..."
		></SpinningCircle>
	</div>
</template>

<style scoped>
.recover-password-container form {
	padding: 10px;
}

.recover-password-container h1 {
	margin-top: 0;
}

.send-message-form {
	display: flex;
	flex-direction: column;
	gap: 10px;
	align-items: start;
}

.send-message-input {
	display: flex;
	flex-direction: column;
	max-width: 400px;
}

button {
	margin: 5px;
}

@media (max-width: 800px) {
	.recover-password-container {
		padding: 15px;
	}

	.send-message-input {
		width: 100%;
	}
}
</style>
