<script setup>
import ThemeList from '@/components/ThemeList.vue';
import ChapterCreateUpdateForm from '@/components/ChapterCreateUpdateForm.vue';
import axios from 'axios';
import { computed, ref } from 'vue';
import { useStore } from 'vuex';
import { helper } from '@/helper/helper';
import { useRouter } from 'vue-router';
import Editor from 'primevue/editor';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import { Form } from '@primevue/forms';

const store = useStore();
const router = useRouter();

const isAdmin = computed(() => store.getters.isAdmin);
const isOwner = computed(() => store.getters.isOwner);
const chapter = computed(() => store.state.chapter);

const newTheme = ref({
    themeTitle: null,
    content: null
});

const isFormActive = ref(false);

async function removeTheme(themeId) {
    if(!window.confirm('Вы уверены, что хотите удалить тему?')) {
        return;
    }
    
    await store.dispatch('deleteTheme', themeId);
    await store.dispatch('downloadChapter',  chapter.value.chapterId);

    clearNewTheme();
}

function changeFormStatus() {
    const editor = document.getElementById("editor");
    editor.classList.toggle('expanded');
    editor.classList.toggle('collapsed');

    isFormActive.value = !isFormActive.value;
    clearNewTheme();
}

async function addNewTheme() {
    newTheme.value.chapterId = chapter.value.chapterId;
    newTheme.value.dateCreated = helper.getCurrentDate();    

    await store.dispatch('createTheme', newTheme.value);
    await store.dispatch('downloadChapter',  chapter.value.chapterId);

    changeFormStatus();
    clearNewTheme();
}

function cancel() {
    router.push(`/chapters/${chapter.value.chapterId}${chapter.value.themes.length > 0 ? '/' + chapter.value.themes[0].themeId : '' }`)
}

function clearNewTheme() {
    newTheme.value.themeTitle = null;
    newTheme.value.content = null;
}

async function handleDeleteChapter() {
  if(!window.confirm('Этот раздел и его темы будут удалены. Вы уверены?')) {
      return;
  }

  await store.dispatch('deleteChapter', chapter.value);
}

async function updateChapter(updatedChapter) {
    chapter.value.chapterTitle = updatedChapter.chapterTitle;
    chapter.value.imagePath = updatedChapter.imagePath;

    await store.dispatch('updateChapter', chapter.value);
}

</script>

<template>
<div v-if="chapter && (isAdmin || isOwner)" class="edit-chapter-container">
    <div>
        <ChapterCreateUpdateForm v-if="!isFormActive" :chapter="chapter" @updateChapter="updateChapter" @cancel="cancel"/>
        <hr v-if="!isFormActive"/>
        <div class="add-new-theme">
            <h3>Темы:</h3>
            <Button @click="changeFormStatus" raised :severity="isFormActive ? 'contrast' : 'secondary'">
                <i :class="isFormActive ? 'pi pi-minus' : 'pi pi-plus'"></i><span>Новая тема</span>
            </Button>
            <Button v-if="isFormActive" form="form" type="submit" raised severity="secondary">
                <i class="pi pi-save"></i><span>Добавить</span>
            </Button>
        </div>
        <div id="expand-container">
            <div class="collapsed" id="editor">
                <Form @submit="addNewTheme(index)" class="new-theme-form" id="form">
                    <InputText v-model="newTheme.themeTitle" type="text" placeholder="Заголовок темы" required/>
                    <Editor v-model.content="newTheme.content" editorStyle="height: 650px"/>
                </Form>
            </div>
        </div>
    </div>
    <ThemeList v-if="!isFormActive" :removeTheme="removeTheme" :themes="chapter.themes"></ThemeList>
    <div class="delete-button">
        <Button v-if="!isFormActive && (isAdmin || isOwner)" severity="danger" @click="handleDeleteChapter">
            <i class="pi pi-trash"></i>
            <span>Удалить</span>
        </Button>
    </div>
</div>

</template>

<style scoped>

.edit-chapter-container {
    position: relative;
    display: flex;
    flex-direction: column;
}

.add-new-theme {
    padding-left: 15px;
    margin-bottom: 10px;
    display: flex;
    flex-direction: row;
    gap: 90px;
    align-items: center;
    height: 35px;
}

.new-theme-form {
    display: flex;
    flex-direction: column;
    gap: 5px;
    width: 100%;
    padding-bottom: 10px;
}

#expand-container {
    overflow: hidden;
}

#editor {
    margin-top: -100%;
    transition: all 1s;
}

#editor.expanded {
    margin-top: 0;
}

.expanded {
    animation-name: slide-in;
    animation-duration: 1s;
}

.collapsed {
    height: 300px;
    transform: translateY(-100%);
}

.delete-button {
    position: absolute;
    bottom: 0;
    right: 0;
    padding: 0 20px 20px 0;
}

@keyframes slide-in {
    100% {
        transform: translateY(0%)
    }
}

@media (max-width: 800px) {
    .add-new-theme span {
        display: none;
    }
}
</style>
