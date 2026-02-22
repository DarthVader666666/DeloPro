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

const showEmailNotification = ref(false)

async function registerUser(registerRequest) {
	const result = await store.dispatch('registerUser', registerRequest)
	store.commit('setTitle', null)
	showEmailNotification.value = result
}
</script>

<template>
	<div
		class="email-sent-notification"
		v-if="showEmailNotification"
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
	<SpinningCircle
		v-else-if="pending"
		title="Пожалуйста, подождите..."
	/>
	<RegisterComponent
		v-else
		:pending="pending"
		@register-user="registerUser"
	/>
</template>
