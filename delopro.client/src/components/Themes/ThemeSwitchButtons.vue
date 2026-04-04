<script setup>
import Button from 'primevue/button'
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from 'vuex'

const store = useStore()
const router = useRouter()

const props = defineProps({
	theme: {
		type: Object,
	},
})

const isAdminOrOwner = computed(() => store.getters.isAdmin || store.getters.isOwner)
const themeIds = computed(() => store.getters.getThemes.map((x) => x.themeId))
const themeIndex = computed(() => themeIds.value.indexOf(props.theme?.themeId ?? 0))
const themeNumber = computed(() => themeIndex.value + 1)

const width = ref(window.innerWidth)

function updateWidth() {
	width.value = window.innerWidth
}

onMounted(() => {
	if (!isAdminOrOwner.value) {
		window.addEventListener('resize', updateWidth)
	}
})
onUnmounted(() => {
	if (!isAdminOrOwner.value) {
		window.removeEventListener('resize', updateWidth)
	}
})

const bottomValue = computed(() => {
	if (isAdminOrOwner.value) return '5%'
	if (width.value >= 1100) return '5%'
	return '12%'
})

function previousTheme() {
	if (themeIndex.value != 0 && props.theme) {
		router.push(`/chapters/${props.theme.chapterId}/themes/${themeIds.value[themeIndex.value - 1]}`)
	}
}

function nextTheme() {
	if (!(themeIndex.value >= themeIds.value.length - 1)) {
		router.push(`/chapters/${props.theme.chapterId}/themes/${themeIds.value[themeIndex.value + 1]}`)
	}
}
</script>

<template>
	<div
		class="theme-buttons"
		:style="{ bottom: bottomValue }"
	>
		<Button
			@click="previousTheme"
			icon="pi pi-arrow-left"
			rounded
			raised
		></Button>
		<div class="theme-counter">
			<span>{{ `${themeNumber} из ${themeIds.length}` }}</span>
		</div>
		<Button
			@click="nextTheme"
			icon="pi pi-arrow-right"
			rounded
			raised
		></Button>
	</div>
</template>

<style scoped>
.theme-buttons {
	display: flex;
	justify-content: space-between;
	align-items: center;
	left: 50%;
	transform: translateX(-50%);
	gap: 10%;
	width: 30%;
	position: fixed;
	z-index: 1;
}

.theme-buttons button {
	height: 50px;
	width: 50px;
	background: rgba(0, 50, 90, 0.5);
	border-width: 0;
	&:hover {
		background: rgba(0, 50, 90, 0.7);
		border-width: 0;
	}
}

.theme-counter {
	padding: 10px;
	border-radius: 20px;
	background: rgba(0, 50, 90, 0.5);
	color: white;
}

@media (min-width: 1100px) {
	.theme-buttons {
		bottom: 12%;
	}
}

@media (max-width: 1100px) {
	.theme-buttons {
		justify-content: center;
		width: inherit;
		position: fixed;
	}
}
</style>
