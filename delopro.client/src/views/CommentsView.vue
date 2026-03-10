<script setup>
import Button from 'primevue/button'
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useStore } from 'vuex'

const store = useStore()
const router = useRouter()
const route = useRoute()
const isAuthenticated = computed(() => store.getters.isAuthenticated)
</script>
<template>
	<div class="comments">
		<div style="display: flex; align-items: center; justify-content: space-between; padding: 5px">
			<Button
				icon="pi pi-arrow-left"
				rounded
				text
				severity="contrast"
				title="Назад"
				@click="router.back()"
			></Button>
			<Button
				v-if="isAuthenticated"
				severity="secondary"
				raised
				label="Комментировать"
				@click="
					router.push({
						name: 'add-comment',
						query: route.query['themeId'],
					})
				"
			></Button>
		</div>
	</div>
</template>

<style scoped>
.comments {
	display: flex;
	flex-direction: column;
}
</style>
