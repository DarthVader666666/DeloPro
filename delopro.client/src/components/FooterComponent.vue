<script setup>
import { helper } from '@/helper/helper'
import Button from 'primevue/button'
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from 'vuex'

const store = useStore()
const router = useRouter()

const isAdminOrOwner = computed(() => store.getters.isAdmin || store.getters.isOwner)
</script>
<template>
	<div
		v-if="!isAdminOrOwner"
		class="footer-container"
	>
		<div class="footer-content">
			<div style="display: flex; flex-direction: column; gap: 5px">
				<span style="font-size: 1.1rem; opacity: 0.9">Пишите:</span>
				<div style="display: flex; gap: 5px; align-items: center">
					<img
						style="width: 20px"
						:src="helper.getImagePath('icon') + 'email.svg'"
					/>
					<span>airlex34@gmail.com</span>
				</div>
			</div>
			<Button
				style="max-width: 40%; height: 45px"
				label="Заказать консультацию"
				raised
				@click="router.push('/feedback')"
			></Button>
		</div>
	</div>
</template>

<style scoped>
.footer-container {
	background: var(--MENU-BACKGROUND);
	color: var(--MENU-TEXT-COLOR);
	height: 4rem;
	align-content: center;
	box-shadow: 0 7px 15px -3px black;
}

.footer-content {
	display: flex;
	justify-content: space-around;
	margin: 10px 15px 10px 15px;
}

@media (max-width: 1100px) {
	.footer-container {
		position: sticky;
		bottom: 0;
	}
}

@media (max-width: 600px) {
	.footer-content {
		justify-content: space-between;
	}
}
</style>
