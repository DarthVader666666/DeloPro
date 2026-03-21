<script setup>
import { onMounted, reactive } from 'vue'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'
import { useStore } from 'vuex'

const store = useStore()
const emit = defineEmits(['setShowLogIn'])

const loginRequestForm = reactive({
	nicknameOrEmail: null,
	password: null,
	remember: false,
})

async function handleLogIn(loginRequestForm) {
	await store.dispatch('logIn', loginRequestForm)
	emit('setShowLogIn', false)
}

onMounted(() => {
	const loginInput = document.getElementById('log-in-input')
	loginInput.focus()
})
</script>

<template>
	<form
		class="slide-container"
		@submit="handleLogIn(loginRequestForm)"
		@keydown.enter.prevent="() => handleLogIn(loginRequestForm)"
		id="log-in-form"
	>
		<div class="upper-part">
			<div class="log-in-input">
				<label>Логин:</label>
				<InputText
					v-model="loginRequestForm.nicknameOrEmail"
					type="text"
					placeholder="Почта или никнэйм"
					required
					id="log-in-input"
				/>
			</div>
			<div class="log-in-input">
				<label>Пароль:</label>
				<InputText
					v-model="loginRequestForm.password"
					type="password"
					placeholder="Пароль"
					required
				/>
			</div>
		</div>

		<div class="bottom-part">
			<div class="remember">
				<label for="remember-checkbox">Запомнить</label>
				<input
					v-model="loginRequestForm.remember"
					type="checkbox"
					id="remember-checkbox"
				/>
			</div>
			<Button
				type="submit"
				severity="secondary"
				icon="pi pi-sign-in"
				label="Войти"
				raised
			></Button>
		</div>
		<RouterLink
			to="/recover-password"
			@click="() => emit('setShowLogIn', false)"
			class="forgot-password"
		>
			Забыли пароль?
		</RouterLink>
	</form>
</template>

<style scoped>
.log-in-input {
	display: flex;
	flex-direction: column;
}

.log-in-input input {
	border-radius: 4px;
}

.log-in-input:hover:deep(input) {
	cursor: text;
}

.upper-part {
	display: flex;
	flex-direction: column;
	gap: 10px;
}

.bottom-part {
	display: flex;
	align-items: center;
	margin-top: 8px;
}

.bottom-part button {
	height: 30px;
	padding: 8px;
	margin-left: 22px;
	border-radius: 4px;
}

.remember {
	display: flex;
	flex-direction: row;
	align-items: center;
	gap: 5px;
}

.remember label,
input {
	&:hover {
		cursor: pointer;
	}
}

.forgot-password {
	color: var(--MENU-TEXT-COLOR);
	opacity: 0.6;
}

label {
	color: var(--MENU-TEXT-COLOR);
}

input[type='text'],
input[type='password'] {
	height: 30px;
}
</style>
