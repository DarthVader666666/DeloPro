import { createStore } from 'vuex'
import axios from 'axios'
import { useToast } from 'vue-toastification'
import router from '@/router/router'
import { helper } from '@/helper/helper'
// vueQuery ?
const toast = useToast()

const store = createStore({
	state: {
		captcha: null,
		serverUrl: import.meta.env.VITE_API_SERVER_URL,
		environment: import.meta.env.VITE_API_ENVIRONMENT,
		roles: [],
		nickname: null,
		chapter: null,
		chapters: [],
		chapterNodes: [],
		theme: null,
		themes: [],
		documents: [],
		documentNodes: [],
		folderPaths: [],
		messages: [],
		message: null,
		unreadMessagesCount: 0,
		searchResult: [],
		showSearchBar: true,
		title: null,
		imageNames: [],
		showChapterList: true,
		showRightColumn: false,
		pending: false,
		users: [],
		user: null,
		currentUser: null,
		visits: [],
		sessionStorageKeys: {
			chaptersKey: 'chapters',
			chapterNodesKey: 'chapterNodes',
			documentsKey: 'documents',
			documentNodesKey: 'documentNodes',
			currentUserKey: 'currentUser',
			usersKey: 'users',
		},
	},
	getters: {
		// CHAPTERS
		getChapter(state) {
			return state.chapter
		},
		getChapters(state) {
			return state.chapters
		},
		getChapterNodes(state) {
			return state.chapterNodes
		},
		getShowChapterList(state) {
			return state.showChapterList
		},
		getImageNames(state) {
			return state.imageNames
		},
		getTheme(state) {
			return state.theme
		},
		getThemes(state) {
			return state.themes
		},

		// ADMINISTRATION
		getUsers(state) {
			return state.users
		},
		getUser(state) {
			return state.user
		},

		// ACCOUNT
		getCurrentUser(state) {
			return state.currentUser
		},
		getRoles(state) {
			return state.currentUser?.roles ?? []
		},
		getNickname(state) {
			return state.currentUser?.nickname ?? null
		},
		isAdmin(state) {
			return state.currentUser?.roles?.includes('Admin')
		},
		isOwner(state) {
			return state.currentUser?.roles?.includes('Owner')
		},
		isUser(state) {
			return state.currentUser?.roles?.includes('User')
		},
		isAuthenticated(state) {
			return state.currentUser != null
		},

		// DOCUMENTS
		getDocuments(state) {
			return state.documents
		},
		getDocumentNodes(state) {
			return state.documentNodes
		},
		getFolderPaths(state) {
			state.folderPaths = ['...']
			state.documentNodes.forEach((node) => getPaths(node.children))

			function getPaths(nodes) {
				nodes.forEach((node) => {
					if (node.data.type === 'folder') {
						state.folderPaths.push(node.data.path.split('\\').slice(1).join('\\'))
						getPaths(node.children)
					}
				})
			}

			return state.folderPaths
		},

		// MESSAGES
		getMessages(state) {
			return state.messages
		},
		getMessage(state) {
			return state.message
		},
		getUnreadMessagesCount(state) {
			return state.unreadMessagesCount
		},

		// SEARCH
		getSearchResult(state) {
			return state.searchResult
		},

		// SHARED
		serverUrl(state) {
			return state.serverUrl
		},
		environment(state) {
			return state.environment
		},
		getPending(state) {
			return state.pending
		},
		getShowRightColumn(state) {
			return state.showRightColumn
		},
		getTitle(state) {
			return state.title
		},
		getCaptcha(state) {
			return state.captcha
		},

		// VISITS
		getVisits(state) {
			return state.visits
		},
	},
	mutations: {
		// ACCOUNT
		setRoles(state, roles) {
			state.roles = roles
		},
		setNickname(state, userNickname) {
			state.nickname = userNickname
		},
		setCurrentUser(state, currentUser) {
			state.currentUser = currentUser
			sessionStorage.setItem(state.sessionStorageKeys.currentUserKey, JSON.stringify(currentUser))
		},

		// ADMINISTRATION
		setUsers(state, users) {
			state.users = users
			sessionStorage.setItem(state.sessionStorageKeys.usersKey, JSON.stringify(users))
		},
		setUser(state, value) {
			state.user = value
		},

		// SEARCH
		renderSearchBar(state) {
			state.title = null
			state.showSearchBar = true
		},
		setTitle(state, value) {
			state.title = value
			state.showSearchBar = false
		},
		setSearchResult(state, searchResult) {
			state.searchResult = searchResult
		},

		// CHAPTERS
		setChapter(state, chapter) {
			state.chapter = chapter
		},
		setChapters(state, chapters) {
			sessionStorage.setItem(state.sessionStorageKeys.chaptersKey, JSON.stringify(chapters))
			state.chapters = chapters
		},
		setChapterNodes(state, chapterNodes) {
			sessionStorage.setItem(state.sessionStorageKeys.chapterNodesKey, JSON.stringify(chapterNodes))
			state.chapterNodes = chapterNodes
		},
		setShowChapterList(state, value) {
			state.showChapterList = value
		},
		setTheme(state, theme) {
			state.theme = theme
		},
		setThemes(state, themes) {
			state.themes = themes
		},

		// DOCUMENTS
		setDocuments(state, documents) {
			sessionStorage.setItem(state.sessionStorageKeys.documentsKey, JSON.stringify(documents))
			state.documents = documents
		},
		setDocumentNodes(state, documentNodes) {
			sessionStorage.setItem(
				state.sessionStorageKeys.documentNodesKey,
				JSON.stringify(documentNodes),
			)
			state.documentNodes = documentNodes
		},

		// MESSAGES
		setMessages(state, messages) {
			state.messages = messages
		},
		setMessage(state, message) {
			state.message = message
		},
		setUnreadMessagesCount(state, count) {
			state.unreadMessagesCount = count
		},
		setMessageById(state, messageId) {
			state.message = state.messages.find((x) => x.messageId === messageId)
		},

		// SHARED
		setShowRightColumn(state, value) {
			state.showRightColumn = value
		},
		setCaptcha(state, value) {
			state.captcha = value
		},
		setPending(state, value) {
			state.pending = value
		},
		setImageNames(state, value) {
			state.imageNames = value
		},

		// VISITS
		setVisits(state, value) {
			state.visits = value
		},
	},
	actions: {
		// CHAPTERS
		async downloadChapter({ commit, state }, chapterId) {
			await axios
				.get(`${state.serverUrl}/chapters/getchapter/${chapterId}`)
				.then(async (response) => {
					if (response.status === 200) {
						const chapter = response.data
						commit('setChapter', chapter)
						commit('setThemes', chapter.themes)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},
		async downloadChapters({ dispatch, commit, state }) {
			const storedChapters = sessionStorage.getItem(state.sessionStorageKeys.chaptersKey)

			if (!storedChapters) {
				axios
					.get(`${state.serverUrl}/chapters/getchapters`)
					.then(async (response) => {
						if (response.status === 200) {
							const chapters = response.data
							commit('setChapters', chapters)
							await dispatch('downloadChapterNodes')
						}
					})
					.catch((error) => {
						if (error.response) {
							toast.error(error.response.data.errorText)
						}
					})
			} else {
				commit('setChapters', JSON.parse(storedChapters))
				await dispatch('downloadChapterNodes')
			}
		},
		async downloadChapterNodes({ commit, state }) {
			const storedChapterNodes = sessionStorage.getItem(state.sessionStorageKeys.chapterNodesKey)

			if (!storedChapterNodes) {
				axios
					.get(`${state.serverUrl}/chapters/getchapternodes`)
					.then((response) => {
						if (response.status === 200) {
							commit('setChapterNodes', response.data)
						}
					})
					.catch((error) => {
						if (error.response) {
							toast.error(error.response.data.errorText)
						}
					})
			} else {
				commit('setChapterNodes', JSON.parse(storedChapterNodes))
			}
		},
		async createChapter({ dispatch, state }, formData) {
			await axios
				.post(`${store.getters.serverUrl}/chapters/createchapter`, formData, {
					headers: {
						'Content-Type': 'multipart/form-data',
						Accept: '',
					},
				})
				.then(async (response) => {
					if (response.status === 200) {
						toast.success('Раздел создан')
						sessionStorage.removeItem(state.sessionStorageKeys.chaptersKey)
						sessionStorage.removeItem(state.sessionStorageKeys.chapterNodesKey)
						await dispatch('downloadChapters')
						await dispatch('downloadChapterNodes')
						router.push(`/chapters/${response.data.chapterId}`)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},
		async deleteChapter({ dispatch, state }, chapter) {
			await axios
				.delete(`${state.serverUrl}/chapters/deletechapter/` + chapter.chapterId, null)
				.then(async (response) => {
					if (response.status === 200) {
						toast.success('Раздел успешно удален')
						sessionStorage.removeItem(state.sessionStorageKeys.chaptersKey)
						sessionStorage.removeItem(state.sessionStorageKeys.chapterNodesKey)
						await dispatch('downloadChapters')
						await dispatch('downloadChapterNodes')
						router.push(`/`)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},
		async updateChapter({ dispatch, state }, chapter) {
			await axios
				.put(`${state.serverUrl}/chapters/updatechapter`, chapter, {
					headers: {
						Content: 'application/json',
						Accept: '*/*',
					},
				})
				.then(async (response) => {
					if (response.status === 200) {
						toast.success('Раздел успешно обновлен')
						sessionStorage.removeItem(state.sessionStorageKeys.chaptersKey)
						sessionStorage.removeItem(state.sessionStorageKeys.chapterNodesKey)
						await dispatch('downloadChapter', chapter.chapterId)
						await dispatch('downloadChapters')
						router.push(
							`/chapters/${chapter.chapterId}${chapter.themes.length > 0 ? '/' + chapter.themes[0].themeId : ''}`,
						)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},
		async downloadTheme({ commit, state }, themeId) {
			let url = `${state.serverUrl}/themes/gettheme/`

			if (themeId) {
				url += `${themeId}`
			} else if (state.chapter.themes.length > 0) {
				url += `${state.chapter.themes[0].themeId}`
			} else {
				return
			}

			commit('setPending', true)

			try {
				const theme = await axios
					.get(url)
					.then((response) => response.data)
					.catch((error) => {
						if (error.response) {
							toast.error(error.response.data.errorText)
						}
					})

				commit('setTheme', theme)
			} finally {
				commit('setPending', false)
			}
		},
		async deleteTheme({ dispatch, state }, theme) {
			axios
				.delete(`${state.serverUrl}/themes/deletetheme/${theme.themeId}`, null, {
					headers: {
						Content: 'application/json',
						Accept: '*/*',
					},
				})
				.then(async (response) => {
					if (response.status === 200) {
						toast.success('Тема успешно удалена')
						sessionStorage.removeItem(state.sessionStorageKeys.chaptersKey)
						sessionStorage.removeItem(state.sessionStorageKeys.chapterNodesKey)
						await dispatch('downloadChapters')
						await dispatch('downloadChapter', theme.chapterId)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},
		async createTheme({ dispatch, state }, newTheme) {
			axios
				.post(`${state.serverUrl}/themes/createtheme`, newTheme, {
					headers: {
						Content: 'application/json',
						Accept: '*/*',
					},
				})
				.then(async (response) => {
					if (response.status === 200) {
						toast.success('Тема успешно добавлена')
						sessionStorage.removeItem(state.sessionStorageKeys.chaptersKey)
						sessionStorage.removeItem(state.sessionStorageKeys.chapterNodesKey)
						await dispatch('downloadChapters')
						await dispatch('downloadChapter', newTheme.chapterId)
					}
				})
				.catch((error) => {
					toast.error(error.response.data.errorText)
				})
		},
		async updateTheme({ state }, themeUpdateForm) {
			await axios
				.put(`${state.serverUrl}/themes/updatetheme`, themeUpdateForm.theme, {
					headers: {
						Content: 'application/json',
						Accept: '*/*',
					},
				})
				.then((response) => {
					if (response.status === 200) {
						toast.success('Тема успешно обновлена')
						sessionStorage.removeItem(state.sessionStorageKeys.chaptersKey)
						sessionStorage.removeItem(state.sessionStorageKeys.chapterNodesKey)
						store.dispatch('downloadChapters')
						store.dispatch('downloadChapter', themeUpdateForm.chapterId)
						store.dispatch('downloadTheme', themeUpdateForm.theme.themeId)

						router.push(`/chapters/${themeUpdateForm.chapterId}/${themeUpdateForm.theme.themeId}`)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},

		// DOCUMENTS
		async downloadDocuments({ dispatch, commit, state }) {
			const storedDocuments = sessionStorage.getItem(state.sessionStorageKeys.documentsKey)

			if (!storedDocuments) {
				axios
					.get(`${state.serverUrl}/documents/getdocuments`)
					.then(async (response) => {
						if (response.status === 200) {
							commit('setDocuments', response.data)
							await dispatch('downloadDocumentNodes')
						}
					})
					.catch((error) => {
						if (error.response) {
							toast.error(error.response.data.errorText)
						}
					})
			} else {
				commit('setDocuments', JSON.parse(storedDocuments))
				await dispatch('downloadDocumentNodes')
			}
		},
		async downloadDocumentNodes({ commit, state }) {
			const storedDocumentNodes = sessionStorage.getItem(state.sessionStorageKeys.documentNodesKey)

			if (!storedDocumentNodes) {
				await axios
					.get(`${state.serverUrl}/documents/getdocumentnodes`)
					.then((response) => {
						if (response.status === 200) {
							commit('setDocumentNodes', response.data)
						}
					})
					.catch((error) => {
						if (error.response) {
							toast.error(error.response.data.errorText)
						}
					})
			} else {
				commit('setDocumentNodes', JSON.parse(storedDocumentNodes))
			}
		},
		async deleteDocument({ dispatch, state }, deleteModel) {
			return axios
				.post(`${state.serverUrl}/documents/deletedocument`, deleteModel)
				.then(async (response) => {
					if (response.status === 200) {
						toast.success(response.data.okText)
						//sessionStorage.removeItem(state.sessionStorageKeys.documentsKey)
						sessionStorage.removeItem(state.sessionStorageKeys.documentNodesKey)
						await dispatch('downloadDocumentNodes')
						return true
					} else {
						return false
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
					return false
				})
		},
		async uploadDocuments({ dispatch, state }, upladForm) {
			await axios
				.post(`${state.serverUrl}/documents/uploaddocuments`, upladForm, {
					headers: {
						'Content-Type': 'multipart/form-data',
					},
				})
				.then(async (response) => {
					if (response.status === 200) {
						toast.success(response.data.okText)
						//sessionStorage.removeItem(state.sessionStorageKeys.documentsKey)
						sessionStorage.removeItem(state.sessionStorageKeys.documentNodesKey)
						await dispatch('downloadDocumentNodes')
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},
		async updateDocument({ dispatch, state }, updateModel) {
			return axios
				.put(`${store.state.serverUrl}/documents/updatedocument`, updateModel)
				.then(async (response) => {
					if (response.status === 200) {
						toast.success(response.data.okText)
						//sessionStorage.removeItem(state.sessionStorageKeys.documentsKey)
						sessionStorage.removeItem(state.sessionStorageKeys.documentNodesKey)
						await dispatch('downloadDocumentNodes')
						return true
					} else {
						return false
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
					return false
				})
		},
		async moveDocument({ dispatch, state }, moveModel) {
			return axios
				.post(`${state.serverUrl}/documents/move`, moveModel)
				.then(async (response) => {
					if (response.status === 200) {
						toast.success(response.data.okText)
						//sessionStorage.removeItem(state.sessionStorageKeys.documentsKey)
						sessionStorage.removeItem(state.sessionStorageKeys.documentNodesKey)
						await dispatch('downloadDocumentNodes')
						return true
					} else {
						return false
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
					return false
				})
		},
		async createFolder({ dispatch, state }, folderPathModel) {
			return axios
				.post(`${state.serverUrl}/documents/addfolder`, folderPathModel)
				.then(async (response) => {
					if (response.status === 200) {
						toast.success(response.data.okText)
						//sessionStorage.removeItem(state.sessionStorageKeys.documentsKey)
						sessionStorage.removeItem(state.sessionStorageKeys.documentNodesKey)
						await dispatch('downloadDocumentNodes')
						return true
					} else {
						return false
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
					return false
				})
		},

		// MESSAGES
		async downloadMessages({ commit, state }, isRead) {
			await axios
				.get(`${state.serverUrl}/feedback/getmessages/${isRead}`)
				.then((response) => {
					if (response.status === 200) {
						commit('setMessages', response.data)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},
		async downloadMessage({ commit, state }, messageId) {
			await axios
				.get(`${state.serverUrl}/feedback/getmessage/${messageId}`)
				.then((response) => {
					if (response.status === 200) {
						commit('setMessage', response.data)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},
		async downloadUnreadMessagesCount({ commit, state }) {
			await axios.get(`${state.serverUrl}/feedback/getunreadmessagescount`).then((response) => {
				if (response.status === 200) {
					commit('setUnreadMessagesCount', response.data)
				}
			})
		},

		// SEARCH
		async downloadSearchResult({ commit, state }, searchLine) {
			await axios
				.post(`${state.serverUrl}/search/getsearchresult`, {
					searchLine: searchLine,
				})
				.then((response) => {
					if (response.status === 200) {
						commit('setSearchResult', response.data)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},

		// CAPTCHA
		async downloadCaptcha({ commit, state }) {
			await axios
				.get(`${state.serverUrl}/captcha/getcaptcha`)
				.then((response) => {
					if (response.status === 200) {
						commit('setCaptcha', response.data)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},

		// IMAGES
		async downloadImageNames({ commit, state }) {
			await axios
				.get(`${state.serverUrl}/home/getimagenames`)
				.then((response) => {
					if (response.status === 200) {
						commit('setImageNames', response.data)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},

		// ADMINISTRATION
		async downloadUsers({ commit, state }) {
			await axios
				.get(`${state.serverUrl}/administration/getusers`)
				.then((response) => {
					if (response.status === 200) {
						commit('setUsers', response.data)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},
		async downloadUser({ commit, state }, userId) {
			await axios
				.get(`${state.serverUrl}/administration/getuser/${userId}`)
				.then((response) => {
					if (response.status === 200) {
						commit('setUser', response.data)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data?.errorText)
					}
				})
		},

		// ACCOUNT
		async sendConfirmationEmail({ state }, accountForm) {
			return await axios
				.post(`${state.serverUrl}/authentication/register`, accountForm, {
					headers: {
						'Content-Type': 'application/json',
					},
				})
				.then((response) => {
					if (response.status === 200) {
						toast.success(response.data.okText)
						return true
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
					return false
				})
		},
		async checkUserExists({ state }, { nickname, email }) {
			return await axios
				.get(
					`${state.serverUrl}/authentication/userexists?` +
						(nickname ? `nickname=${nickname}` : `email=${email}`),
				)
				.then((response) => {
					if (response.status === 200) {
						return response.data.userExists
					}
				})
				.catch((error) => {
					if (error) {
						return false
					}
				})
		},
		async downloadCurrentUser({ commit, state }) {
			const storedCurrentUser = sessionStorage.getItem(state.sessionStorageKeys.currentUserKey)

			if (!storedCurrentUser) {
				const url = '/account/getcurrentuser'
				await axios
					.get(state.serverUrl + url)
					.then((response) => {
						if (response.status === 200 && response.data) {
							commit('setCurrentUser', response.data)
						}
					})
					.catch((error) => {
						if (error.response) {
							toast.error(error.response.data?.errorText ?? `${error.message}: ${url}`)
						}
					})
			} else {
				commit('setCurrentUser', JSON.parse(storedCurrentUser))
			}
		},
		async logIn({ dispatch, state }, loginRequestForm) {
			const nickname = (
				helper.validateEmail(loginRequestForm.nicknameOrEmail)
					? ''
					: loginRequestForm.nicknameOrEmail
			).trimEnd()
			const email = helper.validateEmail(loginRequestForm.nicknameOrEmail)
				? loginRequestForm.nicknameOrEmail
				: null

			axios
				.post(`${state.serverUrl}/authentication/login`, {
					nickname: nickname,
					email: email,
					password: loginRequestForm.password,
					remember: loginRequestForm.remember,
				})
				.then(async (response) => {
					if (response.status === 200) {
						await dispatch('downloadCurrentUser')
						toast.success(`Вы вошли, как ${response.data.nickname}`)
						router.push('/')
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},
		logOut() {
			axios
				.post(`${store.getters.serverUrl}/authentication/logout/`, {
					headers: {
						'Content-Type': 'application/json',
					},
				})
				.then((response) => {
					if (response.status === 200) {
						helper.clearSession()
						router.push('/')
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},
		async updateCurrentUser({ dispatch, state }, updatedUser) {
			await axios
				.post(`${state.serverUrl}/account/updatecurrentuser`, updatedUser, {
					headers: {
						'Content-Type': 'application/json',
					},
				})
				.then(async (response) => {
					if (response.status === 200) {
						sessionStorage.removeItem(state.sessionStorageKeys.currentUserKey)
						sessionStorage.removeItem(state.sessionStorageKeys.usersKey)
						await dispatch('downloadCurrentUser')
						await dispatch('downloadUsers')
						toast.success(response.data.okText)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},
		async uploadAvatar({ dispatch, state }, formData) {
			await axios
				.post(`${store.getters.serverUrl}/account/uploadavatar`, formData, {
					headers: {
						'Content-Type': 'multipart/form-data',
					},
				})
				.then(async (response) => {
					if (response.status === 200) {
						toast.success(response.data.okText)
						sessionStorage.removeItem(state.sessionStorageKeys.currentUserKey)
						sessionStorage.removeItem(state.sessionStorageKeys.usersKey)
						await dispatch('downloadCurrentUser')
						await dispatch('downloadUsers')
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},
		async deleteAvatar({ dispatch, state }) {
			await axios
				.delete(`${state.serverUrl}/account/deleteavatar`)
				.then(async (response) => {
					if (response.status === 200) {
						toast.success(response.data.okText)
						sessionStorage.removeItem(state.sessionStorageKeys.currentUserKey)
						sessionStorage.removeItem(state.sessionStorageKeys.usersKey)
						await dispatch('downloadCurrentUser')
						await dispatch('downloadUsers')
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},

		// VISITS
		async downloadVisits({ commit, state }) {
			await axios
				.get(`${state.serverUrl}/visits/getvisits`)
				.then(async (response) => {
					if (response.status === 200) {
						commit('setVisits', response.data)
					}
				})
				.catch((error) => {
					if (error.response) {
						toast.error(error.response.data.errorText)
					}
				})
		},
	},
})

export default store
