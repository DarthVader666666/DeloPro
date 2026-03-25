<script setup>
import { computed } from 'vue'
import { useStore } from 'vuex'
import { useRouter } from 'vue-router'
import { Form } from '@primevue/forms'
import Editor from 'primevue/editor'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'

const store = useStore()
const router = useRouter()

const chapter = computed(() => store.getters.getChapter)
const theme = computed(() => store.getters.getTheme)

function handleThemeChange() {
	document.getElementById('save-button').disabled = false
}

async function updateTheme() {
	const themeUpdateForm = {
		theme: theme.value,
		chapterId: chapter.value.chapterId,
	}

	await store.dispatch('updateTheme', themeUpdateForm)
}
</script>

<template>
	<div
		v-if="theme"
		class="theme-edit-container"
	>
		<Form
			@submit="updateTheme"
			class="edit-theme-form"
			id="form"
		>
			<div class="upper-part">
				<InputText
					v-model="theme.themeTitle"
					type="text"
					placeholder="Заголовок темы"
					required
					@input="handleThemeChange"
					maxlength="100"
				/>
				<div class="buttons">
					<Button
						type="submit"
						raised
						severity="secondary"
						id="save-button"
						disabled
					>
						<i class="pi pi-save"></i>
						<span>Сохранить</span>
					</Button>
					<Button
						type="button"
						@click="router.back()"
						raised
						severity="contrast"
					>
						<i class="pi pi-ban"></i>
						<span>Отменить</span>
					</Button>
				</div>
			</div>
			<Editor
				v-model.content="theme.content"
				editorStyle="height: 650px"
				@text-change="handleThemeChange"
			/>
		</Form>
	</div>
</template>

<style scoped>
.theme-edit-container {
	display: flex;
	flex-direction: column;
	gap: 10px;
}

.upper-part {
	display: flex;
	flex-direction: row;
	gap: 10px;
	justify-content: space-between;
}

.upper-part input {
	width: 70%;
}

.buttons {
	display: flex;
	flex-direction: row;
	justify-content: end;
	gap: 10px;
}

.buttons button {
	padding: 6px;
}

.edit-theme-form {
	display: flex;
	flex-direction: column;
	gap: 10px;
	width: 100%;
	padding-bottom: 10px;
}

@media (max-width: 800px) {
	.buttons button {
		padding: 10px;
		margin-right: 5px;
	}

	.buttons span {
		display: none;
	}
}
</style>
