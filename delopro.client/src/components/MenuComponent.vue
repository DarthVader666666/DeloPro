<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useStore } from 'vuex'
import { helper } from '@/helper/helper'
import MenuOption from './MenuOption.vue'
import MenuLoginForm from './MenuLoginForm.vue'
import MenuBurger from './MenuBurger.vue'
import MenuAccountOption from './MenuAccountOption.vue'
import MenuAccountSettings from './MenuAccountSettings.vue'

const store = useStore()

const isAuthenticated = computed(() => store.getters.isAuthenticated)
const currentUser = computed(() => store.getters.getCurrentUser)
const showMenu = ref(false)
const showLogIn = ref(false)
const showAccountSettings = ref(false)

const options = [
	{
		path: 'home',
		label: 'Главная',

		roles: ['Any'],
	},
	{
		path: 'feedback',
		label: 'Обратная связь',
		roles: [],
	},
	{
		path: 'register',
		label: 'Регистрация',
		roles: [],
	},
	{
		label: 'Войти',
		icon: 'pi pi-sign-in',
		clickHandler: () => setShowLogIn(),
		roles: [],
	},
	{
		path: 'create-chapter',
		label: 'Cоздать раздел',
		roles: ['Owner', 'Admin'],
	},
	{
		path: 'users',
		label: 'Пользователи',
		roles: ['Owner', 'Admin'],
	},
	{
		path: 'visits',
		label: 'Статистика посещений',
		roles: ['Owner', 'Admin'],
	},
	{
		path: 'messages',
		label: 'Сообщения',
		roles: ['Owner'],
	},
]

const accountOptions = [
	{
		path: 'account',
		label: 'Личный кабинет',
		clickHandler: () => setShowAccountSettings(),
		roles: ['Owner', 'Admin', 'User'],
	},
	{
		path: null,
		label: 'Выйти',
		icon: 'pi pi-sign-out',
		clickHandler: () => handleLogout(),
		roles: ['Owner', 'Admin', 'User'],
	},
]

onMounted(async () => {
	window.addEventListener('click', (event) => {
		if (!helper.closeMenu(event, ['menu', 'burger-button'])) {
			showMenu.value = false
		}
	})

	// window.addEventListener('click', (event) => {
	// 	if (!helper.closeMenu(event, ['log-in-form', 'sign-in_menu_button'])) {
	// 		showLogIn.value = false
	// 	}
	// })

	// window.addEventListener('click', (event) => {
	// 	if (!helper.closeMenu(event, ['account-settings', 'account-button']))
	// 		showAccountSettings.value = false
	// })

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

let outsideClickHandler = null

async function setShowLogIn(value) {
	showLogIn.value = value !== undefined ? value : !showLogIn.value

	await helper.timeoutAsync(10)
	const logInModal = document.getElementById('log-in-form')

	if (showLogIn.value) {
		outsideClickHandler = (e) => {
			const clickedInside = logInModal.contains(e.target)
			if (!clickedInside) {
				setShowLogIn(false)
			}
		}
		document.addEventListener('click', outsideClickHandler)
	} else {
		if (outsideClickHandler) {
			document.removeEventListener('click', outsideClickHandler)
			outsideClickHandler = null
		}
	}
}

async function setShowMenu(value) {
	showMenu.value = value = value !== undefined ? value : !showMenu.value

	// await helper.timeoutAsync(10)
	// const menuModal = document.getElementById('menu')

	// if (showLogIn.value) {
	// 	outsideClickHandler = (e) => {
	// 		const clickedInside = menuModal.contains(e.target)
	// 		if (!clickedInside) {
	// 			setShowMenu(false)
	// 		}
	// 	}
	// 	document.addEventListener('click', outsideClickHandler)
	// } else {
	// 	if (outsideClickHandler) {
	// 		document.removeEventListener('click', outsideClickHandler)
	// 		outsideClickHandler = null
	// 	}
	// }
}

async function setShowAccountSettings(value) {
	showAccountSettings.value = value = value !== undefined ? value : !showAccountSettings.value

	await helper.timeoutAsync(10)
	const accountSettingsModal = document.getElementById('account-settings')

	if (showLogIn.value) {
		outsideClickHandler = (e) => {
			const clickedInside = accountSettingsModal.contains(e.target)
			if (!clickedInside) {
				setShowAccountSettings(false)
			}
		}
		document.addEventListener('click', outsideClickHandler)
	} else {
		if (outsideClickHandler) {
			document.removeEventListener('click', outsideClickHandler)
			outsideClickHandler = null
		}
	}
}

function handleLogout() {
	if (!window.confirm('Вы уверены, что хотите выйти?')) {
		return
	}

	store.dispatch('logOut')
	showAccountSettings.value = false
}
</script>

<template>
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
				:id="(option.path ?? index) + '_menu_button'"
			></MenuOption>
		</div>
	</div>
	<MenuBurger
		:nickname="currentUser?.nickname"
		@setShowMenu="setShowMenu"
	></MenuBurger>
	<MenuLoginForm
		v-if="showLogIn"
		@setShowLogIn="setShowLogIn"
	></MenuLoginForm>
	<MenuAccountOption
		v-if="isAuthenticated"
		:currentUser="currentUser"
		@setShowAccountSettings="setShowAccountSettings"
	></MenuAccountOption>
	<MenuAccountSettings
		v-if="showAccountSettings"
		:options="accountOptions"
	></MenuAccountSettings>
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

/* .slide-container button:not(.account button) {
	width: 100%;
	border-radius: 0;
} */

@media (max-width: 800px) {
	.menu {
		display: none;
	}

	/* .slide-container {
		flex-direction: column-reverse;
	} */
}
</style>
