<script setup>
import { RouterLink } from 'vue-router'
import MenuComponent from './Menu/MenuComponent.vue'
import MenuBurger from './Menu/MenuBurger.vue'
import MenuLoginForm from './Menu/MenuLoginForm.vue'
import MenuAccount from './Menu/MenuAccount.vue'
import MenuAccountSettings from './Menu/MenuAccountSettings.vue'
import { useStore } from 'vuex'
import { computed, onMounted, ref, watch } from 'vue'
import { helper } from '@/helper/helper'
import MenuSlider from './Menu/MenuSlider.vue'

const store = useStore()

const isAuthenticated = computed(() => store.getters.isAuthenticated)
const currentUser = computed(() => store.getters.getCurrentUser)

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
		id: 'home_menu_button',
		label: 'Главная',
		icon: 'pi pi-home',
		roles: ['Any'],
		clickHandler: () => setShowSlideMenu(),
	},
	{
		path: 'feedback',
		id: 'feedback_menu_button',
		label: 'Обратная связь',
		icon: 'pi pi-send',
		roles: [],
		clickHandler: () => setShowSlideMenu(),
	},
	{
		path: 'register',
		id: 'register_menu_button',
		label: 'Регистрация',
		icon: 'pi pi-user-plus',
		roles: [],
		clickHandler: () => setShowSlideMenu(),
	},
	{
		path: 'sign-in',
		id: 'sign-in_menu_button',
		label: 'Войти',
		icon: 'pi pi-sign-in',
		clickHandler: () => setShowLogIn(),
		roles: [],
	},
	{
		path: 'create-chapter',
		id: 'create-chapter_menu_button',
		label: 'Cоздать раздел',
		icon: 'pi pi-plus-circle',
		roles: ['Owner', 'Admin'],
		clickHandler: () => setShowSlideMenu(),
	},
	{
		path: 'users',
		id: 'users_menu_button',
		label: 'Пользователи',
		icon: 'pi pi-users',
		roles: ['Owner', 'Admin'],
		clickHandler: () => setShowSlideMenu(),
	},
	{
		path: 'visits',
		id: 'visits_menu_button',
		label: 'Статистика посещений',
		icon: 'pi pi-chart-line',
		roles: ['Owner', 'Admin'],
		clickHandler: () => setShowSlideMenu(),
	},
	{
		path: 'messages',
		id: 'messages_menu_button',
		label: 'Сообщения',
		icon: 'pi pi-envelope',
		roles: ['Owner'],
		clickHandler: () => setShowSlideMenu(),
	},
]

const accountOptions = [
	{
		path: 'account',
		id: 'account_acc_button',
		label: 'Личный кабинет',
		icon: 'pi pi-user',
		clickHandler: () => setShowAccountSettings(false),
		roles: ['Owner', 'Admin', 'User'],
	},
	{
		path: null,
		id: 'sign-out_acc_button',
		label: 'Выйти',
		icon: 'pi pi-sign-out',
		clickHandler: () => handleLogout(),
		roles: ['Owner', 'Admin', 'User'],
	},
]

onMounted(() => {
	window.addEventListener('click', (event) => {
		if (!helper.closeMenu(event, ['log-in-form', 'sign-in_menu_button'])) {
			showLogIn.value = false
		}
	})
	window.addEventListener('click', (event) => {
		if (!helper.closeMenu(event, ['slide-menu', 'burger-button'])) {
			showSlideMenu.value = false
		}
	})
	window.addEventListener('click', (event) => {
		if (!helper.closeMenu(event, ['account-settings', 'account-button']))
			showAccountSettings.value = false
	})
})

function setShowLogIn(value) {
	showLogIn.value = value !== undefined ? value : !showLogIn.value
}

function setShowSlideMenu(value) {
	showSlideMenu.value = value !== undefined ? value : !showSlideMenu.value
}

function setShowAccountSettings(value) {
	showAccountSettings.value = value !== undefined ? value : !showAccountSettings.value
	setShowSlideMenu()
}

async function handleLogout() {
	if (!window.confirm('Вы уверены, что хотите выйти?')) {
		return
	}

	await store.dispatch('logOut')
}
</script>

<template>
	<div class="header-container">
		<div class="logo">
			<RouterLink to="/"><h1>DeloPro</h1></RouterLink>
		</div>
		<MenuSlider
			v-if="showSlideMenu"
			:options="options"
			:accountOptions="accountOptions"
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
		<div class="account">
			<MenuAccount
				v-if="isAuthenticated"
				:currentUser="currentUser"
				@setShowAccountSettings="setShowAccountSettings"
			></MenuAccount>
		</div>
		<MenuAccountSettings
			v-if="isAuthenticated && showAccountSettings"
			:accountOptions="accountOptions"
			@setShowAccountSettings="setShowAccountSettings"
		></MenuAccountSettings>
	</div>
</template>
<style>
.header-container {
	display: flex;
	flex-direction: row;
	justify-content: space-between;
	background: var(--MENU-BCKGND-CLR);
	box-shadow: var(--COMPONENT-BOX-SHADOW);
	height: var(--HEADER-HEIGHT);
}

.logo {
	padding-top: 15px;
	padding-left: 15px;
}

.logo a {
	text-decoration: none;
	color: var(--MENU-TEXT-COLOR);
	text-shadow: var(--LOGO-SHADOW);
	font-size: 0.8rem;
	&:hover {
		color: var(--LOGO-HOVER-TEXT-COLOR);
	}
}

.account {
	padding-top: 10px;
	padding-right: 20px;
}

@media (max-width: 1000px) {
	.account {
		display: none;
	}
}
</style>
