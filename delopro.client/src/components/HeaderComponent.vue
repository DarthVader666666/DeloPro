<script setup>
import { RouterLink } from 'vue-router'
import MenuComponent from './MenuComponent.vue'
</script>

<template>
	<div class="header-container">
		<div class="logo">
			<RouterLink to="/"><h1>DeloPro</h1></RouterLink>
		</div>
		<MenuComponent></MenuComponent>
	</div>
</template>
<style scoped>
.header-container {
	display: flex;
	flex-direction: row;
	justify-content: center;
	background-image: var(--BCKGND-GRADIENT);
	box-shadow: var(--COMPONENT-BOX-SHADOW);
	border-radius: 0 0 5px 5px;
	height: var(--HEADER-HEIGHT);
}

.logo {
	position: absolute;
	left: 0;
	text-shadow: 3px 3px rgba(22, 22, 22, 0.651);
	height: 18px;
	margin-left: 10px;
	width: 10px;
}

.logo a {
	text-decoration: none;
	color: var(--LOGO-COLOR);
	font-size: 18px;
}

.logo a:hover {
	color: var(--LOGO-COLOR);
}

label {
	font-weight: bold;
}

@media (max-width: 800px) {
	.header-container {
		justify-content: end;
	}
}
</style>
