<script setup>
import { useStore } from 'vuex'
import Button from 'primevue/button'
import { RouterLink, useRouter } from 'vue-router'
import { helper } from '@/helper/helper'
import { computed, ref, watch } from 'vue'
import SpinningCircle from './SpinningCircle.vue'
import CommentModal from './CommentModal.vue'
import ThemeContent from './ThemeContent.vue'
import CommentsComponent from './CommentsComponent.vue'

const store = useStore()
const router = useRouter()
const emit = defineEmits(['removeTheme'])

const isAdmin = computed(() => store.getters.isAdmin)
const isOwner = computed(() => store.getters.isOwner)
const isAuthenticated = computed(() => store.getters.isAuthenticated)
const pending = computed(() => store.getters.getPending)
const commentsCount = computed(() => store.getters.getCommentsCount)

const showCommentModal = ref(false)
const showComments = ref(false)

const props = defineProps({
	theme: {
		typeof: Object,
		default: null,
	},
	useDeleteButtons: {
		typeof: Boolean,
		default: false,
	},
	useShortMode: {
		typeof: Boolean,
		default: false,
	},
	searchResult: {
		type: Object,
		default: null,
	},
})

watch(pending, () => {
	showComments.value = false
})

function setShowCommentModal(value) {
	showCommentModal.value = value
}

function setShowComments() {
	if (commentsCount.value) {
		showComments.value = !showComments.value
	}
}
</script>

<template>
	<div
		:id="`theme_${props.theme.themeId}`"
		:ref="`theme_${props.theme.themeId}`"
	>
		<div class="theme-header">
			<div class="theme-title">
				<RouterLink
					:class="!props.useShortMode && `disabled`"
					:to="`/chapters/${store.state.chapter.chapterId}/${props.theme.themeId}`"
					:disabled="true"
				>
					{{ props.theme.themeTitle }}
				</RouterLink>
			</div>
			<div
				v-if="!props.useShortMode"
				class="theme-buttons"
			>
				<Button
					v-if="isAdmin || isOwner"
					rounded
					text
					icon="pi pi-pencil"
					severity="contrast"
					title="Редактировать"
					@click="router.push(`/edit-theme/${props.theme.themeId}`)"
				></Button>
				<Button
					v-if="isAuthenticated"
					title="Комментировать"
					text
					severity="contrast"
					rounded
					icon="pi pi-comment"
					@click="setShowCommentModal(true)"
				></Button>

				<Button
					text
					severity="contrast"
					rounded
					:label="commentsCount ? `${commentsCount}` : ''"
					icon="pi pi-comments"
					title="Комментарии"
					id="comments-button"
					:class="{ active: showComments }"
					@click="setShowComments"
				></Button>
			</div>
			<span class="date">
				{{ helper.getDateStringForUI(props.theme.dateCreated) }}
			</span>

			<Button
				v-if="useDeleteButtons && (store.getters.isAdmin || store.getters.isOwner)"
				icon="pi pi-times"
				text
				severity="danger"
				title="Удалить тему"
				rounded
				@click="() => emit('removeTheme', props.theme)"
			></Button>
		</div>
		<div
			v-if="pending"
			style="display: flex; flex-direction: column; align-items: center"
		>
			<SpinningCircle></SpinningCircle>
		</div>
		<div v-else-if="!props.useShortMode">
			<ThemeContent
				v-if="!showComments"
				:content="props.theme.content"
				:searchResult="props.searchResult"
			></ThemeContent>
			<CommentsComponent v-else></CommentsComponent>
		</div>
	</div>
	<CommentModal
		v-model:visible="showCommentModal"
		:themeId="props.theme.themeId"
		@setShowCommentModal="setShowCommentModal"
	></CommentModal>
</template>

<style lang="scss" scoped>
.theme-header {
	display: flex;
	flex: row;
	justify-content: space-between;
	align-items: center;
	font-size: large;
	background: var(--THEME-HEADER-BCKGND-GRADIENT);
	padding: 6px;
	min-height: 34px;
}

.theme-header a {
	text-decoration: none;
	margin-left: 5px;
	color: var(--TEXT-COLOR);

	&:hover {
		text-decoration: underline;
	}
}

.theme-title {
	width: 70%;
}

.theme-buttons {
	display: flex;
}

.theme-buttons :deep(.p-button-icon) {
	font-size: 1.3rem;
	opacity: 0.7;
	color: var(--TEXT-COLOR);
}

.theme-buttons :deep(.p-button-label) {
	font-size: 0.6rem;
	color: var(--TEXT-COLOR);
	border: solid 1px rgba(128, 128, 128, 0.562);
	border-radius: 50%;
	background: var(--COLUMNS-BCKGND-CLR);
	width: 17px;
	height: 17px;
	align-content: center;
	position: absolute;
	left: 55%;
	top: 10%;
}

:deep(#comments-button.active) {
	background-color: rgba(255, 255, 255, 0.9);
}

.disabled {
	pointer-events: none;
}

.disabled:hover {
	text-decoration: none;
}

.date {
	font-size: small;
}

.highlighted {
	background-color: yellow;
}

@media (max-width: 1500px) {
	.theme-content:deep(img) {
		max-width: 500px;
		height: auto;
	}
}

@media (max-width: 800px) {
	.theme-content:deep(img) {
		max-width: 300px;
		height: auto;
	}

	.theme-header span {
		display: none;
	}

	.theme-title {
		width: 90%;
	}

	.comment-button {
		display: none;
	}
}
</style>
