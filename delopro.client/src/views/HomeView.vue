<script setup>
import { useStore } from 'vuex'
import { computed } from 'vue'
import { RouterLink } from 'vue-router'
import { helper } from '@/helper/helper'
import SpinningCircle from '@/components/SpinningCircle.vue'

const store = useStore()
const chapters = computed(() => store.getters.getChapters)
const pending = computed(() => store.getters.getPending)
</script>

<template>
	<div>
		<div class="chapters-header">
			<h2>Документационное обеспечение управления</h2>
		</div>
		<SpinningCircle v-if="pending"></SpinningCircle>
		<div
			v-else
			class="chapter-links"
		>
			<div
				v-for="(chapter, index) in chapters"
				:key="index"
				class="chapter"
			>
				<RouterLink
					:to="
						`/chapters/${chapter.chapterId}` +
						`${chapter.themes.length > 0 ? '/' + chapter.themes[0].themeId : ''}`
					"
				>
					<img
						:src="helper.getImagePath('chapter') + chapter.imagePath"
						width="150px"
						height="auto"
					/>
					<p>{{ chapter.chapterTitle }}</p>
				</RouterLink>
			</div>
		</div>
	</div>
</template>

<style scoped>
.chapters-header {
	text-align: center;
	margin: 15px;
	padding: 1px;
	background: var(--MENU-BCKGND-CLR);
	color: var(--MENU-TEXT-COLOR);
}

.chapter-links {
	display: flex;
	flex-flow: row wrap;
	justify-content: space-around;
	padding: 15px;
	gap: 30px;
}

.chapter {
	padding: 10px;
	max-width: 130px;
	max-height: 150px;
	display: flex;
	flex-direction: column;
	align-items: center;
	text-decoration: none;
}

.chapter a {
	text-decoration: none;
	color: var(--TEXT-COLOR);
}

.chapter p {
	font-size: medium;
	text-align: center;
	font-weight: bold;
}

.chapter img:hover {
	-webkit-transform: scale(1.1);
	-moz-transform: scale(1.1);
	-o-transform: scale(1.1);
	transform: scale(1.1);

	cursor: pointer;
}

.chapter img {
	-webkit-transition: all 0.2s ease-in-out;
	-moz-transition: all 0.2s ease-in-out;
	-o-transition: all 0.2s ease-in-out;
	transition: all 0.2s ease-in-out;
	filter: drop-shadow(var(--PNG-IMAGE-SHADOW));
}

@media (max-width: 800px) {
	.chapter img {
		max-width: 120px;
		max-height: auto;
	}

	.chapter p {
		font-size: small;
	}

	h2 {
		font-size: medium;
	}

	.chapters-header {
		margin: 0;
	}
}
</style>
