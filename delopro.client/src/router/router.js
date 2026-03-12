import RegisterView from '@/views/RegisterView.vue'
import HomeView from '@/views/HomeView.vue'
import ChapterCreateView from '@/views/ChapterCreateView.vue'
import ChapterDetailsView from '@/views/ChapterDetailsView.vue'
import ChapterEditView from '@/views/ChapterEditView.vue'
import ThemeEditView from '@/views/ThemeEditView.vue'
import FeedBackView from '@/views/FeedBackView.vue'
import { createRouter, createWebHistory } from 'vue-router'
import store from '@/vuex/store.js'
import MessagesView from '@/views/MessagesView.vue'
import SearchResultView from '@/views/SearchResultView.vue'
import PersonalDataAgreement from '@/views/PersonalDataAgreement.vue'
import RecoverPasswordView from '@/views/RecoverPasswordView.vue'
import UsersView from '@/views/UsersView.vue'
import { computed, ref } from 'vue'
import AccountView from '@/views/AccountView.vue'
import VisitsView from '@/views/VisitsView.vue'
import CommentsView from '@/views/CommentsView.vue'
import { helper } from '@/helper/helper'

const doScrollUp = ref(true)
const currentUser = computed(() => store.getters.getCurrentUser)
const isAuthenticated = computed(() => store.getters.isAuthenticated)

const router = createRouter({
	history: createWebHistory(),
	routes: [
		{
			path: '/',
			name: 'home',
			component: HomeView,
		},
		{
			path: '/register',
			name: 'register',
			component: RegisterView,
		},
		{
			path: '/create-chapter',
			name: 'create-chapter',
			component: ChapterCreateView,
		},
		{
			path: '/chapters/:chapterId/:themeId?',
			name: 'chapter-details',
			component: ChapterDetailsView,
		},
		{
			path: '/edit-chapter/:chapterId',
			name: 'edit-chapter',
			component: ChapterEditView,
		},
		{
			path: '/:catchAll(.*)', // any resource which doesn't exist
			name: 'home',
			component: HomeView,
		},
		{
			path: '/edit-theme/:themeId',
			name: 'edit-theme',
			component: ThemeEditView,
		},
		{
			path: '/feedback',
			name: 'feedback',
			component: FeedBackView,
		},
		{
			path: '/messages',
			name: 'messages',
			meta: { requiresAuth: true, roles: ['Owner'] },
			component: MessagesView,
		},
		{
			path: '/search-result',
			name: 'search-result',
			component: SearchResultView,
		},
		{
			path: '/personal-data-agreement',
			name: 'personal-data-agreement',
			component: PersonalDataAgreement,
		},
		{
			path: '/recover-password',
			name: 'recover-password',
			component: RecoverPasswordView,
		},
		{
			path: '/users',
			name: 'users',
			meta: { requiresAuth: true, roles: ['Owner', 'Admin'] },
			component: UsersView,
		},
		{
			path: '/account',
			name: 'account',
			meta: { requiresAuth: true, roles: ['Owner', 'Admin', 'User'] },
			component: AccountView,
		},
		{
			path: '/visits',
			name: 'visits',
			meta: { requiresAuth: true, roles: ['Owner', 'Admin'] },
			component: VisitsView,
		},
		{
			path: '/comments',
			name: 'comments',
			component: CommentsView,
		},
	],
})

router.beforeEach(async (to, from, next) => {
	if (to.meta.roles) {
		if (!to.meta.roles.some((r) => currentUser?.value?.roles?.includes(r) ?? false)) {
			return next('/')
		}
	} else {
		if (to.name === 'register') {
			if (isAuthenticated.value) {
				return next('/')
			}

			store.commit('setTitle', 'Заполните форму регистрации')
			const captchaInput = document.getElementById('captcha-input')

			if (captchaInput) {
				captchaInput.value = null
			}

			await store.dispatch('downloadCaptcha')
		}

		if (to.name === 'feedback') {
			if (isAuthenticated.value) {
				return next('/')
			}

			store.commit('setTitle', 'Напишите ваше сообщение')
			const captchaInput = document.getElementById('captcha-input')

			if (captchaInput) {
				captchaInput.value = null
			}

			await store.dispatch('downloadCaptcha')
		}
	}

	next()
})

router.afterEach(async (to) => {
	store.commit('setPending', true)
	store.commit('setShowRightColumn', false)

	if (to.name === 'chapter-details') {
		await store.dispatch('downloadChapter', to.params['chapterId'])

		if (to.params['themeId']) {
			await store.dispatch('downloadTheme', to.params['themeId'])
		}

		store.commit('renderSearchBar')
		store.commit('setShowChapterList', false)

		if (to.query.searchFragment) {
			doScrollUp.value = false
		}
	} else {
		store.commit('setShowChapterList', true)
		store.commit('setTheme', null)
	}

	if (to.name === 'edit-theme') {
		await store.dispatch('downloadTheme', to.params['themeId'])
		store.commit('setTitle', 'Редактирование темы')
	}

	if (to.name === 'edit-chapter') {
		await store.dispatch('downloadChapter', to.params['chapterId'])
		store.commit('setTitle', 'Редактирование раздела')
	}

	if (to.name === 'create-chapter') {
		store.commit('setTitle', 'Создание нового раздела')
	}

	if (to.name === 'home') {
		store.commit('renderSearchBar')
	}

	if (to.name === 'messages') {
		await store.dispatch('downloadMessages', false)
		store.commit('setTitle', 'Сообщения')
	}

	if (to.name === 'personal-data-agreement') {
		store.commit('setTitle', 'Соглашение о хранении и обработке данных')
	}

	if (to.name === 'recover-password') {
		store.commit('setTitle', 'Восстановление пароля')

		const captchaInput = document.getElementById('captcha-input')

		if (captchaInput) {
			captchaInput.value = null
		}

		await store.dispatch('downloadCaptcha')
	}

	if (to.name === 'users') {
		await store.dispatch('downloadUsers')
		store.commit('setTitle', 'Пользователи')
	}

	if (to.name === 'account') {
		store.commit('setTitle', 'Личный кабинет')
	}

	if (to.name === 'visits') {
		store.commit('setTitle', 'Статистика посещений')
	}

	if (to.name === 'comments') {
		await store.dispatch('downloadTheme', to.params['themeId'])
		await helper.timeoutAsync(10)
		const theme = store.getters.getTheme
		store.commit('setTitle', `${theme?.themeTitle}`)
	}

	if (to.name === 'add-comment') {
		store.commit('setTitle', 'Ваш комментарий')
	}

	if (store.getters.isOwner) {
		await store.dispatch('downloadUnreadMessagesCount')
	}

	if (doScrollUp.value) {
		window.scrollTo(0, 0)
	}

	doScrollUp.value = true
	store.commit('setPending', false)
})

export default router
