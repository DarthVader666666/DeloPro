<script setup>
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import LoginForm from './LoginForm.vue'
import { onMounted, ref, watch } from 'vue'
import { useStore } from 'vuex'
import { helper } from '@/helper/helper'

const router = useRouter()
const store = useStore()

const showLogIn = ref(false)

const props = defineProps({
	setShowMenu: {
		type: Function,
	},
})

onMounted(async () => {
	window.addEventListener('click', (event) => {
		if (!helper.closeMenu(event, ['login-form', 'login-button'])) {
			showLogIn.value = false
		}
	})
})

watch(showLogIn, (newValue) => {
	if (newValue) {
		helper.darkenBackground()
	} else {
		helper.lightenBackground()
	}
})

function setShowLogIn() {
	props.setShowMenu(false)
	showLogIn.value = !showLogIn.value
}

async function handleLogIn(loginRequestForm) {
	await store.dispatch('logIn', loginRequestForm)
	showLogIn.value = false
}
</script>

<template>
	<Button
		@click="
			() => {
				setShowLogIn()
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
				setShowLogIn()
				router.push('/register')
			}
		"
		severity="contrast"
		text
		label="Регистрация"
		id="register-button"
	></Button>

	<Button
		@click="setShowLogIn()"
		severity="contrast"
		text
		label="Войти"
		icon="pi pi-sign-in"
		id="login-button"
	></Button>
	<LoginForm
		v-if="showLogIn"
		:setShowLogIn="setShowLogIn"
		:handleLogIn="handleLogIn"
	></LoginForm>
</template>

<style scoped></style>
