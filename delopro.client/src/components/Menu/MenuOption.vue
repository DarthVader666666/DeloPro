<script setup>
import Button from 'primevue/button'
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useStore } from 'vuex'

const router = useRouter()
const route = useRoute()
const store = useStore()
const unreadMessagesCount = computed(() => store.getters.getUnreadMessagesCount)

const props = defineProps({
	label: {
		type: String,
		required: true,
	},
	clickHandler: {
		type: Function,
	},
	path: {
		type: String,
		default: null,
	},
	id: {
		type: String,
	},
	icon: {
		type: String,
		default: null,
	},
	warning: {
		type: String,
		default: null,
	},
})

function clicked() {
	if (props.warning) {
		if (!window.confirm(props.warning)) {
			return
		}
	}

	if (props.path && props.path != 'sign-in') {
		router.push('/' + props.path)
	}

	if (props.clickHandler) {
		props.clickHandler()
	}
}
</script>

<template>
	<Button
		class="menu-button"
		:id="props.id"
		:style="route.path.includes(props.path) ? { backgroundColor: '#f8fafc' } : {}"
		@click="clicked"
		severity="contrast"
		text
	>
		<i
			v-if="props.icon"
			:class="props.icon"
		></i>
		<span style="position: relative">
			{{ props.label }}
			<span
				v-if="props.path === 'messages' && unreadMessagesCount"
				class="count"
			>
				{{ unreadMessagesCount > 99 ? '99+' : unreadMessagesCount }}
			</span>
		</span>
	</Button>
</template>

<style scoped>
.menu-button {
	border-radius: 0;
	font-weight: bold;
	color: var(--TEXT-COLOR);
}

.menu-button i {
	font-weight: bold;
}

.count {
	position: absolute;
	background: red;
	color: white;
	border-radius: 50%;
	font-size: 0.6rem;
	width: 20px;
	height: 17px;
	text-align: center;
	align-content: center;
	top: 50%;
	right: -12%;
}
</style>
