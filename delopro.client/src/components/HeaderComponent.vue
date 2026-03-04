<script setup>
import { RouterLink } from 'vue-router'
import MenuComponent from './MenuComponent.vue'
import MenuBurger from './MenuBurger.vue'
import MenuLoginForm from './MenuLoginForm.vue'
import MenuAccount from './MenuAccount.vue'
import MenuAccountSettings from './MenuAccountSettings.vue'
import { useStore } from 'vuex'
import { computed, onMounted, ref, watch } from 'vue'
import { helper } from '@/helper/helper'
import MenuSlider from './MenuSlider.vue'

const store = useStore()

const isAuthenticated = computed(() => store.getters.isAuthenticated)
const currentUser = computed(() => store.getters.getCurrentUser)

const isSlideMenu = ref(false)
const showSlideMenu = ref(false)
const showLogIn = ref(false)
const showAccountSettings = ref(false)
const changeBackground = computed(
	() => showLogIn.value || showAccountSettings.value || showSlideMenu.value,
)

watch(changeBackground, (newValue) => {
	if (newValue) {
		helper.darkenBackground()
	} else {
		helper.lightenBackground()
	}
})

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
		if (!helper.closeMenu(event, ['slide-menu', 'burger-button'])) {
			showSlideMenu.value = false
		}
	})

	// window.addEventListener('click', (event) => {
	// 	if (!helper.closeMenu(event, ['log-in-form', 'sign-in_menu_button'])) {
	// 		showLogIn.value = false
	// 	}
	// })

	window.addEventListener('click', (event) => {
		if (!helper.closeMenu(event, ['account-settings', 'account-button']))
			showAccountSettings.value = false
	})

	window.addEventListener('resize', handleScreenSizeChange)
})

function handleScreenSizeChange() {
	if (document.documentElement.clientWidth > 1100) {
		isSlideMenu.value = false
	} else {
		isSlideMenu.value = true
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

async function setShowSlideMenu(value) {
	showSlideMenu.value = value = value !== undefined ? value : !showSlideMenu.value
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
	<div class="header-container">
		<div class="logo">
			<RouterLink to="/"><h1>DeloPro</h1></RouterLink>
		</div>
		<MenuSlider
			v-if="isSlideMenu && showSlideMenu"
			:options="options"
		></MenuSlider>
		<MenuComponent
			v-else
			:options="options"
		></MenuComponent>
		<MenuBurger
			:nickname="currentUser?.nickname"
			@setShowSlideMenu="setShowSlideMenu"
		></MenuBurger>
		<MenuLoginForm
			v-if="showLogIn"
			@setShowLogIn="setShowLogIn"
		></MenuLoginForm>
		<MenuAccount
			v-if="isAuthenticated && !isSlideMenu"
			:currentUser="currentUser"
			@setShowAccountSettings="setShowAccountSettings"
		></MenuAccount>
		<MenuAccountSettings
			v-if="showAccountSettings"
			:options="accountOptions"
		></MenuAccountSettings>
	</div>
</template>
<style>
/* .slide-container button:not(.account button) {
	width: 100%;
	border-radius: 0;
} */

/* @media (max-width: 800px) {
	.menu {
		display: none;
	}

	.slide-container {
		flex-direction: column-reverse;
	}
} */

.header-container {
	display: flex;
	flex-direction: row;
	justify-content: center;
	background-image: var(--BCKGND-GRADIENT);
	box-shadow: var(--COMPONENT-BOX-SHADOW);
	border-radius: 0 0 5px 5px;
	height: var(--HEADER-HEIGHT);
}

.logo {
	position: absolute;
	left: 0;
	text-shadow: 3px 3px rgba(22, 22, 22, 0.651);
	height: 18px;
	margin-left: 10px;
	width: 10px;
}

.logo a {
	text-decoration: none;
	color: var(--LOGO-COLOR);
	font-size: 18px;
}

.logo a:hover {
	color: var(--LOGO-COLOR);
}

label {
	font-weight: bold;
}

@media (max-width: 1100px) {
	.header-container {
		justify-content: end;
	}
}
</style>
