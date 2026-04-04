<script setup>
import { RouterLink } from 'vue-router'
import MenuComponent from './Menu/MenuComponent.vue'
import MenuBurger from './Menu/MenuBurger.vue'
import MenuLoginForm from './Menu/MenuLoginForm.vue'
import MenuAccount from './Menu/MenuAccount.vue'
import MenuAccountSettings from './Menu/MenuAccountSettings.vue'
import MenuSlider from './Menu/MenuSlider.vue'
import { useStore } from 'vuex'
import { computed, onMounted, ref, watch } from 'vue'
import { helper } from '@/helper/helper'

const store = useStore()

const isAuthenticated = computed(() => store.getters.isAuthenticated)
const isAdmin = computed(() => store.getters.isAdmin)
const isOwner = computed(() => store.getters.isOwner)
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
		routeName: 'home',
		id: 'home_menu_button',
		label: 'Главная',
		icon: 'pi pi-home',
		roles: ['Any'],
		clickHandler: () => setShowSlideMenu(),
	},
	{
		routeName: 'documents',
		id: 'documents_menu_button',
		label: 'Документы',
		icon: 'pi pi-folder-open',
		roles: ['Any'],
		clickHandler: () => setShowSlideMenu(),
	},
	{
		routeName: 'feedback',
		id: 'feedback_menu_button',
		label: 'Напишите нам',
		icon: 'pi pi-send',
		roles: [],
		clickHandler: () => setShowSlideMenu(),
	},
	{
		routeName: 'register',
		id: 'register_menu_button',
		label: 'Зарегистрироваться',
		icon: 'pi pi-user-plus',
		roles: [],
		clickHandler: () => setShowSlideMenu(),
	},
	{
		routeName: 'sign-in',
		id: 'sign-in_menu_button',
		label: 'Войти',
		icon: 'pi pi-sign-in',
		clickHandler: () => setShowLogIn(),
		roles: [],
	},
	{
		routeName: 'create-chapter',
		id: 'create-chapter_menu_button',
		label: 'Cоздать раздел',
		icon: 'pi pi-plus-circle',
		roles: ['Owner', 'Admin'],
		clickHandler: () => setShowSlideMenu(),
	},
	{
		routeName: 'users',
		id: 'users_menu_button',
		label: 'Пользователи',
		icon: 'pi pi-users',
		roles: ['Owner', 'Admin'],
		clickHandler: () => setShowSlideMenu(),
	},
	{
		routeName: 'visits',
		id: 'visits_menu_button',
		label: 'Статистика посещений',
		icon: 'pi pi-chart-line',
		roles: ['Owner', 'Admin'],
		clickHandler: () => setShowSlideMenu(),
	},
	{
		routeName: 'messages',
		id: 'messages_menu_button',
		label: 'Сообщения',
		icon: 'pi pi-envelope',
		roles: ['Owner'],
		clickHandler: () => setShowSlideMenu(),
	},
]

const accountOptions = [
	{
		routeName: 'account',
		id: 'account_acc_button',
		label: 'Личный кабинет',
		icon: 'pi pi-user',
		clickHandler: () => setShowAccountSettings(false),
		roles: ['Owner', 'Admin', 'User'],
	},
	{
		routeName: null,
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
		<div
			v-if="!(isAdmin || isOwner)"
			class="contacts"
		>
			<img :src="helper.getImagePath('icon') + 'email.svg'" />
			<span>airlex34@gmail.com</span>
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
<style scoped>
.header-container {
	display: flex;
	flex-direction: row;
	justify-content: space-between;
	position: relative;
	background: var(--MENU-BACKGROUND);
	box-shadow: var(--COMPONENT-BOX-SHADOW);
	height: var(--HEADER-HEIGHT);
}

.contacts {
	display: flex;
	gap: 5px;
	position: absolute;
	background-color: transparent;
	color: var(--MENU-TEXT-COLOR);
	left: 50%;
	transform: translateX(-50%);

	padding: 18px 0 0 0;
	align-items: center;
}

.contacts span,
img {
	font-size: 1.1rem;
	opacity: 0.8;
}

.contacts img {
	background-color: var(--MENU-TEXT-COLOR);
	border-radius: 50px;
	width: 30px;
	height: auto;
	padding: 2px;

	&:hover {
		opacity: 0.8;
	}
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
		color: var(--LOGO-HOVER-COLOR);
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

@media (max-width: 500px) {
	.contacts {
		top: 22%;
		left: 55%;
		transform: translateX(-45%);
	}

	.contacts span {
		font-size: 0.9rem;
	}

	.contacts img {
		width: 20px;
	}
}
</style>
