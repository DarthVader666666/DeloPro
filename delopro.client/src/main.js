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

// Import PrimeVue components you're using
import TreeTable from 'primevue/treetable'
import Column from 'primevue/column'

axios.defaults.withCredentials = true

async function bootstrap() {
	store.commit('setPending', true)

	try {
		if (!store.getters.getCurrentUser && (await helper.checkAuthentication())) {
			await store.dispatch('downloadCurrentUser')
		}

		await store.dispatch('downloadChapters')
		await store.dispatch('downloadDocumentNodes')
		await store.dispatch('downloadImageNames')
	} finally {
		store.commit('setPending', false)
	}
}

const app = createApp(App)

// Configure PrimeVue
app.use(PrimeVue, {
	theme: {
		preset: Aura,
		options: {
			darkModeSelector: '.fake-dark-selector',
			cssLayer: false,
		},
	},
})

// Register PrimeVue components globally
app.component('TreeTable', TreeTable)
app.component('Column', Column)

// Use other plugins
app.use(router)
app.use(Toast, { timeout: 2000, position: POSITION.TOP_CENTER })
app.use(store)

app.mount('#app')

bootstrap()
