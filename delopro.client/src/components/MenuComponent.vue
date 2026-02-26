<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from 'vuex'
import Button from 'primevue/button'
import MenuAuthenticated from './MenuAuthenticated.vue'
import MenuInitial from './MenuInitial.vue'
import { helper } from '@/helper/helper'

const router = useRouter()
const store = useStore()
const isAuthenticated = computed(() => store.getters.isAuthenticated)
const currentUser = computed(() => store.getters.getCurrentUser)
const showMenu = ref(false)

onMounted(async () => {
	window.addEventListener('click', (event) => {
		if (!helper.closeMenu(event, ['menu', 'burger-button'])) showMenu.value = false
	})

	window.addEventListener('resize', handleScreenSizeChange)
})

watch(showMenu, (newValue) => {
	const menu = document.getElementById('menu')

	if (newValue) {
		menu.classList.remove('menu')
		menu.classList.add('slide-container')
		helper.darkenBackground()
	} else {
		menu.classList.remove('slide-container')
		menu.classList.add('menu')
		helper.lightenBackground()
	}
})

const handleScreenSizeChange = () => {
	if (document.documentElement.clientWidth > 800) {
		showMenu.value = false
	}
}

function setShowMenu(value) {
	if (value === undefined) {
		showMenu.value = !showMenu.value
	} else {
		showMenu.value = value
	}
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
		<Button
			@click="
				() => {
					showMenu = false
					router.push('/')
				}
			"
			severity="contrast"
			text
			label="Главная"
			id="home-button"
		></Button>
		<MenuAuthenticated v-if="isAuthenticated"></MenuAuthenticated>
		<MenuInitial
			:setShowMenu="setShowMenu"
			v-else
		></MenuInitial>
	</div>
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

.slide-container {
	display: flex;
	flex-direction: column;
	position: fixed;
	top: var(--HEADER-HEIGHT);
	right: 0;
	z-index: 1;
	background-color: var(--MENU-BCKGND-CLR);
	padding: 15px;
	border-radius: 3px;
	box-shadow: var(--MENU-BOX-SHADOW);
	animation-name: slide;
	animation-duration: 0.2s;
	transform: translateX(0%);
	min-width: 220px;
}

.slide-container button span {
	font-weight: bold;
	color: var(--TEXT-COLOR);
	border-radius: 0;
}

.slide-container button {
	border-radius: 0;
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
