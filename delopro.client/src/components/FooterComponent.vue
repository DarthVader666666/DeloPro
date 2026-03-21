<script setup>
import { useStore } from 'vuex'

const store = useStore()
</script>
<template>
	<div class="footer-container">
		<h1>{{ store.getters.environment === 'production' ? '' : store.getters.environment }}</h1>
	</div>
</template>

<style scoped>
.footer-container {
	background: var(--MENU-BCKGND-CLR);
	color: var(--MENU-TEXT-COLOR);
	height: 4rem;
	align-content: center;
	box-shadow: 0 7px 15px -3px black;
}
</style>
