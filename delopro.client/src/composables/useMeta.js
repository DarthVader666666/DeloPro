// composables/useMeta.js
import { onMounted } from 'vue'

export function useMeta(metaData) {
	onMounted(() => {
		// Set title
		if (metaData.title) {
			document.title = metaData.title
		}

		// Set HTML attributes
		if (metaData.htmlAttrs) {
			Object.entries(metaData.htmlAttrs).forEach(([key, value]) => {
				document.documentElement.setAttribute(key, value)
			})
		}

		// Set meta tags
		if (metaData.meta) {
			metaData.meta.forEach((meta) => {
				let selector = meta.name ? `meta[name="${meta.name}"]` : `meta[property="${meta.property}"]`
				let element = document.querySelector(selector)

				if (!element) {
					element = document.createElement('meta')
					if (meta.name) {
						element.setAttribute('name', meta.name)
					} else if (meta.property) {
						element.setAttribute('property', meta.property)
					}
					document.head.appendChild(element)
				}

				element.setAttribute('content', meta.content)
			})
		}
	})
}
