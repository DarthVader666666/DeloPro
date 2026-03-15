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
	border-radius: 5px 5px 0 0;
	font-weight: bold;
	color: var(--TEXT-COLOR);
}

.menu-button i {
	font-weight: bold;
	margin-bottom: 3px;
}

.count {
	position: absolute;
	background: rgb(240, 0, 0);
	color: rgb(240, 240, 240);
	border-radius: 50%;
	font-size: 0.55rem;
	width: 17px;
	height: 17px;
	text-align: center;
	align-content: center;
	top: -38%;
	right: -12%;
	padding-right: 1%;
	border: solid 1px rgba(0, 0, 0, 0.2);
}

@media (max-width: 1000px) {
	.menu-button {
		border-radius: 0;
	}
}
</style>
