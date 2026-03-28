<script setup>
import { computed, ref, watch } from 'vue'
import { useStore } from 'vuex'
import Button from 'primevue/button'
import { useRouter } from 'vue-router'
import CommentComponent from './CommentComponent.vue'

const store = useStore()
const router = useRouter()
const currentUser = computed(() => store.getters.getCurrentUser)
const isAdmin = computed(() => store.getters.isAdmin)
const isOwner = computed(() => store.getters.isOwner)
const comments = computed(() => store.getters.getComments)
const editingId = ref(null)

const emit = defineEmits(['setShowComments'])

watch(comments, (newValue) => {
	if (!newValue.length) {
		emit('setShowComments')
	}
})

function setShowComments(value) {
	editingId.value = value != undefined ? value : null
}
</script>

<template>
	<div class="comments-header">
		<Button
			icon="pi pi-arrow-left"
			text
			rounded
			@click="router.back()"
		></Button>
		<h2>Комментарии</h2>
	</div>
	<div class="comments">
		<CommentComponent
			v-for="comment in comments"
			:key="comment.commentId"
			:comment="comment"
			:currentUser="currentUser"
			:isAdmin="isAdmin"
			:isOwner="isOwner"
			:editingId="editingId"
			@setEditingId="setShowComments"
		></CommentComponent>
	</div>
</template>

<style scoped>
.comments {
	padding-top: 10px;
	display: flex;
	flex-direction: column;
	gap: 3px;
	animation-name: slide-down;
	animation-duration: 0.2s;
	transform: translateY(0%);
}

.comments-header {
	display: flex;
	align-items: center;
	justify-content: center;
	position: relative;
}

.comments-header h2 {
	color: var(--MENU-BACKGROUND);
	text-align: center;
}

.comments-header button {
	position: absolute;
	color: var(--MENU-BACKGROUND);
	background: rgba(0, 50, 90, 0.2);
	left: 2%;
}

@keyframes slide-down {
	0% {
		transform: translateY(-20%);
	}
	100% {
		transform: translateY(0%);
	}
}
</style>
