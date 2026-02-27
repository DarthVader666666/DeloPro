<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from 'vuex'
import Button from 'primevue/button'
import { helper } from '@/helper/helper'
import MenuOption from './MenuOption.vue'

const router = useRouter()
const store = useStore()

const props = defineProps({
	options: {
		type: Array,
		default: () => [],
	},
})

const currentUser = computed(() => store.getters.getCurrentUser)

// const unreadMessagesCount = computed(() => store.getters.getUnreadMessagesCount)

const showMenu = ref(false)
const showUserAccountSettings = ref(false)

onMounted(() => {
	window.addEventListener('click', (event) => {
		if (!helper.closeMenu(event, ['account-settings', 'account-button']))
			showUserAccountSettings.value = false
	})
})

function handleLogout() {
	if (!window.confirm('Вы уверены, что хотите выйти?')) {
		return
	}

	store.dispatch('logOut')

	showUserAccountSettings.value = false
}
</script>

<template>
	<div class="account">
		<Button
			@click="
				() => {
					showUserAccountSettings = !showUserAccountSettings
					showMenu = false
				}
			"
			severity="secondary"
			rounded
			id="account-button"
		>
			<img
				v-if="isAuthenticated && currentUser.avatarPath"
				:src="currentUser.avatarPath"
				width="50px"
				height="50px"
			/>
			<i
				v-else
				class="pi pi-user"
				style="font-size: x-large"
			></i>
		</Button>
		<span>{{ currentUser.nickname }}</span>
	</div>
	<MenuOption
		v-for="(option, index) in props.options"
		:key="index"
		:path="option.path"
		:label="option.label"
		:icon="option.icon"
		:clickHandler="option.clickHandler"
	></MenuOption>
	<div
		v-if="showUserAccountSettings"
		class="slide-container"
		id="account-settings"
	>
		<div style="text-align: center">
			<span style="font-size: large">
				{{ currentUser.nickname }}
			</span>
			<Button
				@click="showUserAccountSettings = false"
				severity="contrast"
				rounded
				text
				icon="pi pi-times"
				style="position: absolute; right: 5px; top: 5px; height: 25px; width: 25px"
			></Button>
		</div>
		<div style="padding-top: 20px">
			<Button
				@click="
					() => {
						showUserAccountSettings = false
						router.push(`/account`)
					}
				"
				text
				label="Личный кабинет"
				style="padding: 12px"
			></Button>
			<Button
				@click="handleLogout"
				text
				label="Выйти"
				icon="pi pi-sign-out"
				id="logout-button"
				style="padding: 12px"
			></Button>
		</div>
	</div>
</template>

<style scoped>
.account {
	position: absolute;
	right: 20px;
	padding-bottom: 8px;
	display: flex;
	flex-direction: column;
	font-size: medium;
	align-items: center;
}

.account button {
	height: 50px;
	width: 50px;
	border-radius: 50%;
}

.unread-messages-count {
	right: 0;
	background: red;
	color: white !important;
	font-size: small;
	font-weight: normal !important;
	padding: 3px 0 0 0;
	border-radius: 50%;
	height: 20px;
	width: 20px;
}

@media (max-width: 800px) {
	.account {
		position: relative;
		padding: 0 0 15px 0;
		right: 0;
		margin: auto;
	}
}
</style>
