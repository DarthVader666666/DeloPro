import store from '@/vuex/store'
import axios from 'axios'

export const helper = {
	getUnicodeByteArray(text) {
		const utf8Encode = new TextEncoder()
		return Object.values(utf8Encode.encode(text))
	},
	validateEmail(email) {
		const result = email.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/)
		return result
	},
	timeoutAsync(ms) {
		return new Promise((resolve) => setTimeout(resolve, ms))
	},
	getCurrentDateString(hmsFormat = false) {
		const today = new Date()
		return this.getDateStringCore(today, hmsFormat)
	},
	getDateStringForUI(dateString, short = false) {
		if (!dateString) {
			return null
		}

		const date = new Date(dateString)

		if (short) {
			return date.toLocaleDateString('ru-RU', { day: 'numeric', month: 'numeric', year: 'numeric' })
		} else {
			return date.toLocaleDateString('ru-RU', {
				day: 'numeric',
				month: 'long',
				year: 'numeric',
				hour: '2-digit',
				minute: '2-digit',
			})
		}
	},
	getDateStringForInput(dateString) {
		const date = new Date(dateString)
		return this.getDateStringCore(date).split('T')[0]
	},
	getDateStringCore(date, hmsFormat = false) {
		const day = String(date.getDate()).padStart(2, '0')
		const month = String(date.getMonth() + 1).padStart(2, '0')
		const year = date.getFullYear()
		const hours = withLeadingZero(date.getHours())
		const minutes = withLeadingZero(date.getMinutes())
		const seconds = withLeadingZero(date.getSeconds())

		function withLeadingZero(value) {
			if (String(value).length < 2) {
				return `0${value}`
			}

			return value
		}

		return (
			year +
			'-' +
			month +
			'-' +
			day +
			'T' +
			(!hmsFormat
				? hours + ':' + minutes + ':' + seconds
				: hours + 'h' + minutes + 'm' + seconds + 's')
		)
	},
	getQueryString(array, key) {
		const queryString = array
			.map((value) => `${key}=${value}&`)
			.join('')
			.slice(0, -1)
		return '?' + queryString
	},
	getImagePath() {
		return store.getters.environment === 'development'
			? '/src/assets/chapters/chapter-'
			: '/chapters/chapter-'
	},
	scrollToTheme(themeId) {
		if (themeId) {
			let links = document.getElementsByClassName('link active')

			for (let item of links) {
				item.classList.remove('active')
			}

			document.getElementById(`listItem_${themeId}`).classList.add('active')
		}
	},
	closeMenu(event, ids, hasSelect = false) {
		let isValidClick = false

		if (hasSelect) {
			let select = document.getElementsByClassName('p-select-list-container')[0]

			// if (select.contains(event.target)) {
			// 	isValidClick = true
			// }
			if (anyChildren(event, select)) isValidClick = true
		}

		ids.forEach((id) => {
			// const element = document.getElementById(id)
			// if (element?.contains(event.target)) {
			// 	isValidClick = true
			// }
			if (anyChildren(event, document.getElementById(id))) {
				isValidClick = true
			}
		})

		return isValidClick

		function anyChildren(event, element) {
			if (element && element.children.length) {
				if (event.target === element) {
					return true
				}

				for (let i = 0; i < element.children.length; i++) {
					if (event.target === element.children[i]) {
						return true
					} else {
						if (anyChildren(event, element.children[i])) {
							return true
						}
					}
				}
			}

			return false
		}
	},
	userStatuses: ['Подтвержден', 'Не подтвержден', 'Удален'],
	getUserTagSeverity(status) {
		switch (status) {
			case 0:
				return 'success'

			case 1:
				return 'warn'

			case 2:
				return 'danger'

			default:
				return null
		}
	},
	roles: ['Owner', 'Admin', 'User'],
	getFutureDate(days) {
		let date = this.getCurrentDateString()
		const result = new Date(date)
		result.setDate(result.getDate() + days)
		return result
	},
	darkenContainers: [
		document.getElementsByClassName('main-container'),
		document.getElementsByClassName('search-bar'),
		document.getElementsByClassName('title'),
	],
	lightenContainers: [document.getElementsByClassName('message')],
	darkenBackground() {
		this.darkenContainers.forEach((items) => {
			for (let item of items) {
				item.style.opacity = 0.8
				item.style.filter = 'brightness(50%)'
			}
		})
	},
	lightenBackground() {
		this.darkenContainers.forEach((items) => {
			for (let item of items) {
				item.style.removeProperty('opacity')
				item.style.removeProperty('filter')

				let children = item.children

				for (let item of children) {
					item.style.removeProperty('opacity')
					item.style.removeProperty('filter')
				}
			}
		})
	},
	trimTags(htmlTag) {
		return htmlTag.replace(/<\/?[^>]+(>|$)/g, '').trim()
	},
	decodeHtml(htmlString) {
		const txt = document.createElement('textarea')
		txt.innerHTML = htmlString
		return txt.value
	},
	base64ToBytes(base64) {
		const binary = atob(base64.split(',')[1])
		const len = binary.length
		const bytes = new Uint8Array(len)

		for (let i = 0; i < len; i++) {
			bytes[i] = binary.charCodeAt(i)
		}

		return bytes
	},
	bytesToBase64(bytes) {
		let binary = ''
		const len = bytes.byteLength

		for (let i = 0; i < len; i++) {
			binary += String.fromCharCode(bytes[i])
		}

		const r = 'data:image/png;base64,' + btoa(binary)
		return r
	},
	async fileToBytesAsync(file) {
		return new Promise((resolve, reject) => {
			const reader = new FileReader()
			reader.onload = () => {
				const arrayBuffer = reader.result
				const bytes = new Uint8Array(arrayBuffer)
				resolve(bytes)
			}

			reader.onerror = reject
			reader.readAsArrayBuffer(file)
		})
	},
	async fileToBase64Async(file) {
		if (!file) {
			return null
		}

		return new Promise((resolve, reject) => {
			const reader = new FileReader()

			reader.onload = () => {
				resolve(reader.result)
			}

			reader.onerror = (error) => {
				reject(error)
			}

			reader.readAsDataURL(file)
		})
	},
	clearSession() {
		const keys = store.state.sessionStorageKeys

		for (const key in keys) {
			sessionStorage.removeItem(keys[key])
		}

		// store.commit('setCurrentUser', null)
	},
	async isAuthenticated() {
		return await axios
			.get(`${store.state.serverUrl}/authentication/checkauthentication`, { withCredentials: true })
			.then((response) => response.data)
	},
	resetObject(obj) {
		Object.keys(obj).forEach((key) =>
			obj[key] === false || obj[key] === true ? (obj[key] = false) : (obj[key] = null),
		)
	},
}
