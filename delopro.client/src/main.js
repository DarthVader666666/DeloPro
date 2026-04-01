import './assets/main.css'
import 'primeicons/primeicons.css'
import 'vue-toastification/dist/index.css'
import router from './router/router.js'
import App from './App.vue'
import store from './vuex/store.js'
import Toast, { POSITION } from 'vue-toastification'
import { createApp } from 'vue'
import PrimeVue from 'primevue/config'
import Aura from '@primevue/themes/aura'
import axios from 'axios'
import { helper } from './helper/helper'

axios.defaults.withCredentials = true

axios.interceptors.request.use((config) => {
	config.loadingTimer = setTimeout(() => {
		store.commit('incrementPending')
	}, 100)

	return config
})

axios.interceptors.response.use(
	(response) => {
		clearTimeout(response.config.loadingTimer)
		store.commit('decrementPending')
		return response
	},
	(error) => {
		clearTimeout(error.config.loadingTimer)
		store.commit('decrementPending')
		return Promise.reject(error)
	},
)

async function bootstrap() {
	sessionStorage.clear()

	if (!store.getters.getCurrentUser && (await helper.checkAuthentication())) {
		await store.dispatch('downloadCurrentUser')
	}

	await store.dispatch('downloadChapters')
	await store.dispatch('downloadDocumentNodes')
	await store.dispatch('downloadImageNames')
}

createApp(App)
	.use(PrimeVue, {
		theme: {
			preset: Aura,
			options: {
				darkModeSelector: '.fake-dark-selector',
			},
		},
	})
	.use(router)
	.use(Toast, { timeout: 2000, position: POSITION.TOP_CENTER })
	.use(store)
	.mount('#app')

bootstrap()
