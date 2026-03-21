<script setup>
import { onMounted, reactive, computed, ref } from 'vue'
import { helper } from '@/helper/helper.js'
import { useStore } from 'vuex'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Select from 'primevue/select'
import { useRouter } from 'vue-router'

const props = defineProps({
	chapter: {
		typeof: Object,
		require: false,
		default: {
			chapterId: null,
			chapterTitle: null,
			imagePath: null,
			userId: null,
			dateCreated: null,
			dateDeleted: null,
			themes: [],
		},
	},
	createChapter: {
		typeof: Function,
		require: false,
	},
	doClearChapter: {
		typeof: Boolean,
		default: false,
	},
	isCreateForm: {
		typeof: Boolean,
		default: false,
	},
})

const store = useStore()
const router = useRouter()
const emit = defineEmits(['cancel', 'updateChapter'])
const chapter = reactive(props.chapter)
const imageNames = computed(() => store.getters.getImageNames)
const imagePath = ref(null)

onMounted(() => {
	if (props.doClearChapter) {
		helper.resetObject(chapter)
	}
})

function handleCancel() {
	const chapterId = chapter.chapterId

	if (chapterId) {
		emit('cancel')
	} else {
		router.push('/')
	}
}

function handleSave(chapter) {
	if (props.isCreateForm) {
		props.createChapter(chapter)
	} else {
		emit('updateChapter', chapter)
	}
}

function handleInput() {
	document.getElementById('save-button').disabled = false
}

function handleSelect(value) {
	imagePath.value = helper.getImagePath('chapter') + value
	handleInput()
}

function onMouseEnter(option) {
	imagePath.value = helper.getImagePath('chapter') + option
}

function onMouseLeave() {
	imagePath.value = helper.getImagePath('chapter') + chapter.imagePath
}

async function handleDeleteChapter() {
	if (!window.confirm('Этот раздел и его темы будут удалены. Вы уверены?')) {
		return
	}

	await store.dispatch('deleteChapter', chapter)
}
</script>

<template>
	<div class="chapter-create-update">
		<form
			@submit.prevent="handleSave(chapter)"
			id="chapter-create-update-form"
		>
			<InputText
				v-model="chapter.chapterTitle"
				@input="handleInput"
				type="text"
				required
				placeholder="Заголовок раздела"
			/>
			<img
				:src="imagePath ? imagePath : helper.getImagePath('chapter') + chapter.imagePath"
				width="150px"
				height="120px"
			/>
			<Select
				class="select"
				v-model="chapter.imagePath"
				@update:model-value="handleSelect"
				:options="imageNames"
				placeholder="Путь к картинке"
				appendTo="self"
			>
				<template #option="slotProps">
					<div
						@mouseenter="onMouseEnter(slotProps.option)"
						@mouseleave="onMouseLeave()"
						style="width: 100%; height: 100%; padding-left: 5px; align-content: center"
					>
						{{ slotProps.option }}
					</div>
				</template>
			</Select>
		</form>
		<div class="chapter-buttons">
			<div class="save-cancel-buttons">
				<Button
					type="submit"
					form="chapter-create-update-form"
					disabled
					raised
					severity="secondary"
					label="Сохранить"
					id="save-button"
				/>
				<Button
					type="button"
					@click="handleCancel"
					raised
					severity="contrast"
					label="Отменить"
				/>
			</div>
			<div>
				<Button
					v-if="chapter.chapterId"
					severity="danger"
					@click="handleDeleteChapter"
				>
					<i class="pi pi-trash"></i>
					<span>Удалить</span>
				</Button>
			</div>
		</div>
	</div>
</template>

<style scoped>
.select :deep(li) {
	padding: 0;
	height: 30px;
}

.select :deep(ul) {
	border: 0;
}

.chapter-create-update {
	display: flex;
	justify-content: space-between;
}

.chapter-create-update form {
	display: flex;
	flex-direction: column;
	gap: 20px;
	width: 60%;
}

.chapter-buttons {
	display: flex;
	flex-direction: column;
	justify-content: space-between;
	align-items: end;
}

.save-cancel-buttons {
	display: flex;
	flex-direction: column;
	gap: 10px;
}

.chapter-buttons button {
	width: 100px;
}

@media (max-width: 1100px) {
	.chapter-create-update {
		margin: 20px;
	}
}
</style>
