<script setup>
import { computed, ref, watch } from 'vue'
import { useStore } from 'vuex'
import CommentComponent from './CommentComponent.vue'

const store = useStore()
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

@keyframes slide-down {
	0% {
		transform: translateY(-20%);
	}
	100% {
		transform: translateY(0%);
	}
}
</style>
