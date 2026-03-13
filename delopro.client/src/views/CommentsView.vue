<script setup>
import CommentModal from '@/components/CommentModal.vue'
import Button from 'primevue/button'
import { computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useStore } from 'vuex'

const store = useStore()
const router = useRouter()
const route = useRoute()
const isAuthenticated = computed(() => store.getters.isAuthenticated)
const showCommentModal = ref(false)

function setShowCommentModal(value) {
	showCommentModal.value = value
}
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
				icon="pi pi-comment"
				label="Комментировать"
				@click="setShowCommentModal(true)"
			></Button>
		</div>
		<CommentModal
			v-model:visible="showCommentModal"
			:themeId="route.query['themeId']"
			@setShowCommentModal="setShowCommentModal"
		></CommentModal>
	</div>
</template>

<style scoped>
.comments {
	display: flex;
	flex-direction: column;
}
</style>
