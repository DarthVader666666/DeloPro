<script setup>
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from 'vuex'

const store = useStore()
const router = useRouter()
const searchLine = ref(null)

async function handleSearch() {
	if (!searchLine.value || searchLine.value.trim().length === 0) {
		searchLine.value = null
		return
	}

	await store.dispatch('downloadSearchResult', searchLine.value)
	searchLine.value = null
	router.push('/search-result')
}
</script>
<template>
	<div
		class="search-bar"
		id="search-bar"
	>
		<span style="padding-right: 10px; font-size: medium">Поиск</span>
		<InputText
			v-model="searchLine"
			@keydown.enter="handleSearch"
			placeholder="Введите, что хотите найти..."
		/>
		<Button
			@click="handleSearch"
			raised
			severity="secondary"
			reised
		>
			<i class="pi pi-search"></i>
			<span>Искать</span>
		</Button>
	</div>
</template>

<style scoped>
.search-bar {
	position: relative;
	height: var(--SEARCHBAR-HEIGHT);
	flex-direction: row;
	text-align: center;
	align-content: center;
	background-color: var(--SEARCHBAR-BACKGROUND);
	margin: 12px 0 12px 0;
	box-shadow: var(--COMPONENT-BOX-SHADOW);
	padding-right: 60px;
}

.search-bar label {
	margin-right: 10px;
}

.search-bar button {
	position: absolute;
	height: 36px;
	bottom: 12px;
}

.search-bar button i {
	margin-right: 5px;
}

.search-bar input {
	width: 40%;
	margin-right: 5px;
	box-shadow: var(--INPUT-BOX-SHADOW);
}

@media (max-width: 800px) {
	.search-bar input {
		width: 70%;
	}

	.search-bar button span {
		display: none;
	}

	.search-bar button {
		width: 40px;
	}
}
</style>
