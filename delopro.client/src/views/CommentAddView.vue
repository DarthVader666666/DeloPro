<script setup>
import Textarea from 'primevue/textarea'
import Button from 'primevue/button'
import { onMounted, reactive } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useStore } from 'vuex'

const route = useRoute()
const router = useRouter()
const store = useStore()

const comment = reactive({
	themeId: route.query['themeId'],
	text: null,
})

onMounted(async () => {
	await store.dispatch('downloadTheme', route.query['themeId'])
})

function addEmoji(emoji) {
	comment.text += emoji
}

function handleTextChange() {}
</script>

<template>
	<div>
		<div style="display: flex; align-items: center; justify-content: space-between">
			<div style="padding-left: 15px; min-width: 60%">
				<span>{{ store.getters.getTheme?.themeTitle }}</span>
			</div>
			<div style="text-align: end">
				<Button
					severity="secondary"
					raised
					label="Сохранить"
					style="margin: 10px; width: 100px"
				></Button>
				<Button
					severity="contrast"
					raised
					label="Отменить"
					@click="router.back()"
					style="margin: 10px; width: 100px"
				></Button>
			</div>
		</div>
		<div style="margin: 8px">
			<Textarea
				v-model="comment.text"
				style="max-width: 100%; min-width: 100%; min-height: 200px; max-height: 200px"
				maxlength="1200"
			></Textarea>
			<div class="emoji-picker">
				<span @click="addEmoji('😀')">😀</span>
				<span @click="addEmoji('🔥')">🔥</span>
				<span @click="addEmoji('❤️')">❤️</span>
			</div>
		</div>
	</div>
</template>

<style scoped>
.emoji-picker :hover {
	cursor: pointer;
}
</style>
