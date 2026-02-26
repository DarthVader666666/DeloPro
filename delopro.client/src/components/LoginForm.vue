<script setup>
import { onMounted, reactive } from 'vue'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'

const props = defineProps({
	setShowLogIn: {
		type: Function,
	},
	handleLogIn: {
		type: Function,
		required: true,
	},
})

const loginRequestForm = reactive({
	nicknameOrEmail: null,
	password: null,
	remember: false,
})

onMounted(() => {
	const loginInput = document.getElementById('login-input')
	loginInput.focus()
})
</script>

<template>
	<form
		class="slide-container"
		@submit.prevent="() => props.handleLogIn(loginRequestForm)"
		@keydown.enter.prevent="() => props.handleLogIn(loginRequestForm)"
		id="login-form"
	>
		<div class="login-input">
			<label>Логин:</label>
			<InputText
				v-model="loginRequestForm.nicknameOrEmail"
				type="text"
				placeholder="Почта или никнэйм"
				required
				id="login-input"
			/>
		</div>
		<div class="login-input">
			<label>Пароль:</label>
			<InputText
				v-model="loginRequestForm.password"
				type="password"
				placeholder="Пароль"
				required
			/>
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
				form="login-form"
			></Button>
		</div>
		<RouterLink
			to="/recover-password"
			@click="props.setShowLogIn()"
		>
			Забыли пароль?
		</RouterLink>
	</form>
</template>

<style scoped>
.login-input {
	display: flex;
	flex-direction: column;
	gap: 2px;
}

.login-input input {
	border-radius: 4px;
}

.login-input:hover:deep(input) {
	cursor: text;
}

.bottom-part {
	display: flex;
	flex-direction: row;
	align-items: center;
	margin-top: 8px;
}

.bottom-part button {
	font-size: medium;
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

.remember label:hover,
input:hover {
	cursor: pointer;
}

label {
	font-weight: bold;
	color: var(--TEXT-COLOR);
}

input[type='text'],
input[type='password'] {
	font-size: medium;
	height: 30px;
}
</style>
