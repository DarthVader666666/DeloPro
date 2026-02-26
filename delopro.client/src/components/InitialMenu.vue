<script setup>
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import LoginForm from './LoginForm.vue'
import { onMounted, ref, watch } from 'vue'
import { useStore } from 'vuex'
import { helper } from '@/helper/helper'

const router = useRouter()
const store = useStore()

const showLogin = ref(false)

onMounted(async () => {
	window.addEventListener('click', (event) => {
		if (!helper.closeMenu(event, ['login-form', 'login-button'])) {
			showLogin.value = false
		}
	})
})

watch(showLogin, (newValue) => {
	if (newValue) {
		helper.darkenBackground()
	} else {
		helper.lightenBackground()
	}
})

async function handleLogIn(loginRequestForm) {
	await store.dispatch('logIn', loginRequestForm)
	showLogin.value = false
}
</script>

<template>
	<Button
		@click="
			() => {
				showMenu = false
				router.push('/feedback')
			}
		"
		severity="contrast"
		text
		label="Обратная связь"
		id="feedback-button"
	></Button>
	<Button
		@click="
			() => {
				showMenu = false
				showLogin = false
				router.push('/register')
			}
		"
		severity="contrast"
		text
		label="Регистрация"
		id="register-button"
	></Button>

	<Button
		@click="() => (showLogin = !showLogin)"
		severity="contrast"
		text
		label="Войти"
		icon="pi pi-sign-in"
		id="login-button"
	></Button>
	<LoginForm
		v-if="showLogin"
		:handleLogIn="handleLogIn"
	></LoginForm>
</template>

<style scoped></style>
