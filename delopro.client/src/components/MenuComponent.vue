<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useStore } from 'vuex'
import Button from 'primevue/button'
import { helper } from '@/helper/helper'
import MenuOption from './MenuOption.vue'
import LoginForm from './LoginForm.vue'

const store = useStore()

const isAuthenticated = computed(() => store.getters.isAuthenticated)
const currentUser = computed(() => store.getters.getCurrentUser)
const showMenu = ref(false)
const showLogIn = ref(false)

const options = [
	{
		path: 'home',
		label: 'Главная',
		icon: null,
		clickHandler: null,
		roles: ['Any'],
	},
	{
		path: 'feedback',
		label: 'Обратная связь',
		icon: null,
		clickHandler: null,
		roles: [],
	},
	{
		path: 'register',
		label: 'Регистрация',
		icon: null,
		clickHandler: null,
		roles: [],
	},
	{
		path: null,
		label: 'Войти',
		icon: 'pi pi-sign-in',
		clickHandler: () => setShowLogIn(),
		roles: [],
	},
	{
		path: 'create-chapter',
		label: 'Cоздать раздел',
		icon: null,
		clickHandler: null,
		roles: ['Owner', 'Admin'],
	},
	{
		path: 'users',
		label: 'Пользователи',
		icon: null,
		clickHandler: null,
		roles: ['Owner', 'Admin'],
	},
	{
		path: 'visits',
		label: 'Статистика посещений',
		icon: null,
		clickHandler: null,
		roles: ['Owner', 'Admin'],
	},
	{
		path: 'messages',
		label: 'Сообщения',
		icon: null,
		clickHandler: null,
		roles: ['Owner'],
	},
]

onMounted(async () => {
	// window.addEventListener('click', (event) => {
	// 	if (!helper.closeMenu(event, ['menu', 'burger-button'])) showMenu.value = false
	// })

	window.addEventListener('click', (event) => {
		if (!helper.closeMenu(event, ['log-in-form', 'sign-in_menu_button'])) {
			showLogIn.value = false
		}
	})

	window.addEventListener('resize', handleScreenSizeChange)
})

watch(showMenu, (newValue) => {
	const menu = document.getElementById('menu')

	if (newValue) {
		menu.classList.remove('menu')
		menu.classList.add('slide-container')
	} else {
		menu.classList.remove('slide-container')
		menu.classList.add('menu')
	}
})

function handleScreenSizeChange() {
	if (document.documentElement.clientWidth > 800) {
		showMenu.value = false
	}
}

function setShowLogIn(value) {
	showLogIn.value = value !== undefined ? value : !showLogIn.value
}

function setShowMenu(value) {
	showMenu.value = value = value !== undefined ? value : !showMenu.value
}
</script>

<template>
	<div class="menu-burger">
		<span
			v-if="isAuthenticated"
			style="font-weight: bold"
		>
			{{ currentUser.nickname }}
		</span>
		<Button
			@click="setShowMenu"
			security="contrast"
			rounded
			text
			id="burger-button"
		>
			<i class="pi pi-bars"></i>
		</Button>
	</div>
	<div
		class="menu"
		id="menu"
	>
		<div
			v-for="(option, index) in options"
			:key="index"
		>
			<MenuOption
				v-if="
					option.roles.includes('Any') ||
					(isAuthenticated
						? option.roles.some((role) => currentUser?.roles?.includes(role))
						: option.roles.length === 0)
				"
				:path="option.path"
				:label="option.label"
				:icon="option.icon"
				:clickHandler="option.clickHandler"
				:id="option.path ?? 'sign-in' + '_menu_button'"
			></MenuOption>
		</div>
	</div>
	<LoginForm
		v-if="showLogIn"
		@setShowLogIn="setShowLogIn"
	></LoginForm>
</template>

<style>
.menu {
	display: flex;
	flex-direction: row;
	align-items: end;
	gap: 0px;
}

.menu button {
	border-radius: 0;
}

.menu span {
	font-weight: bold;
	color: var(--TEXT-COLOR);
}

.menu-burger {
	display: none;
	align-content: center;
}

.menu-burger button {
	margin: 0 10px 0 10px;
	padding: 12px;
}

.menu-burger button {
	border-width: 1px;
	border-color: rgba(0, 0, 0, 0.332);
}

.menu-burger i {
	color: var(--TEXT-COLOR);
	font-size: x-large;
}

/* .slide-container button:not(.account button) {
	width: 100%;
	border-radius: 0;
} */

@media (max-width: 800px) {
	.menu {
		display: none;
	}

	.menu-burger {
		display: flex;
		align-items: center;
	}

	/* .slide-container {
		flex-direction: column-reverse;
	} */
}
</style>
