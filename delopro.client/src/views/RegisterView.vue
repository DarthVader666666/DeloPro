<script setup>
import RegisterComponent from '@/components/RegisterComponent.vue'
import SpinningCircle from '@/components/SpinningCircle.vue'
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from 'vuex'
import Button from 'primevue/button'

const store = useStore()
const router = useRouter()
const pending = computed(() => store.getters.getPending)
const showNotification = ref(false)

async function registerUser(registerRequest) {
	const result = await store.dispatch('registerUser', registerRequest)
	store.commit('setTitle', null)
	showNotification.value = result
}
</script>

<template>
	<div
		v-if="showNotification"
		class="email-sent-notification"
	>
		<h3>Пользователь успешно зарегестрирован</h3>

		<h3>Проверьте свой Email</h3>

		<img
			src="/src/assets/email-sent.jpg"
			alt="email-sent.jpg"
		/>
		<Button
			severity="secondary"
			raised
			@click="router.push('/')"
		>
			Понятно
		</Button>
	</div>
	<div v-else>
		<SpinningCircle
			v-if="pending"
			style="padding-left: 10%"
			:text="'Пожалуйста, подождите...'"
		></SpinningCircle>
		<RegisterComponent
			v-else
			@register-user="registerUser"
		></RegisterComponent>
	</div>
</template>
