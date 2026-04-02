<script setup>
import ChapterCarousel from '@/components/Chapters/ChapterCarousel.vue'
</script>

<template>
	<div>
		<div class="chapters-header">
			<h2>Документационное обеспечение управления в Республике Беларусь</h2>
		</div>
		<div style="padding: 10px; display: flex; flex-direction: column">
			<span style="font-size: 1.1rem">
				Поможем составить номенклатуру дел, подготовить документы для хранения в архиве. Подробно
				рассмотрим делопроизводственный процесс, включающий оформление, движение документов и
				контроль их исполнения.
			</span>

			<div class="services">
				<div style="">
					<i class="pi pi-star-fill"></i>
					<span>Делопроизводство в организации и оформление документов</span>
				</div>
				<div>
					<i class="pi pi-star-fill"></i>
					<span>Организация документооборота и подготовка к архивному хранению</span>
				</div>
				<div>
					<i class="pi pi-star-fill"></i>
					<span>Деловой этикет и искусство делового письма</span>
				</div>
				<div>
					<i class="pi pi-star-fill"></i>
					<span>Организация деловых встреч и переговоров, дресс-код</span>
				</div>
			</div>
		</div>
		<div class="chapter-links">
			<ChapterCarousel></ChapterCarousel>
		</div>
	</div>
</template>

<style scoped>
.chapters-header {
	text-align: center;
	margin: 15px 0 15px 0;
	padding: 1px;
	background: var(--MENU-BACKGROUND);
	color: var(--MENU-TEXT-COLOR);
}

.chapters-header h2 {
	padding: 0 2% 0 2%;
}

.chapter-links {
	margin: 0 10px 0 10px;
}

.services {
	display: flex;
	flex-direction: column;
	padding: 15px;
	gap: 8px;
}

.services div {
	display: flex;
	gap: 8px;
	align-items: center;
}

@media (max-width: 800px) {
	.chapters-header h2 {
		font-size: medium;
	}
	.chapters-header {
		margin: 0;
	}
}

@media (min-width: 1600px) {
	.chapter-links {
		height: 340px;
	}
}
</style>
