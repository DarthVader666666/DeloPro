<script setup>
import { helper } from '@/helper/helper'
import { computed } from 'vue'
import { useStore } from 'vuex'
import { useRouter } from 'vue-router'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'

const store = useStore()
const router = useRouter()
const searchResult = computed(() => store.getters.getSearchResult)
</script>

<template>
	<div
		v-if="searchResult.length > 0"
		class="search-result-container"
	>
		<div>
			<h3>Результаты поиска</h3>
			<hr />
		</div>

		<DataTable
			:value="searchResult"
			paginator
			:rows="5"
			:rowsPerPageOptions="[5, 10, 20, 50]"
			class="table-class"
		>
			<Column>
				<template #body="slotProps">
					<div class="search-result-header">
						<RouterLink
							:to="`/chapters/${slotProps.data.chapterId}/themes/${slotProps.data.themeId}`"
						>
							{{ slotProps.data.themeTitle }}
						</RouterLink>
						<span class="date">{{ helper.getDateStringForUI(slotProps.data.dateCreated) }}</span>
					</div>
					<div
						v-html="slotProps.data.searchFragment"
						class="search-result-content"
						@click="
							router.push({
								path: `/chapters/${slotProps.data.chapterId}/themes/${slotProps.data.themeId}`,
								query: {
									searchFragment: helper.trimTags(slotProps.data.searchFragment),
									index: slotProps.data.index,
								},
							})
						"
					></div>
				</template>
			</Column>
		</DataTable>
	</div>
	<h1
		style="padding-left: 10px"
		v-else
	>
		Поиск не дал результатов
	</h1>
</template>

<style scoped>
.search-result-container {
	display: flex;
	flex-direction: column;
	justify-content: start;
	gap: 15px;
}

.search-result-header {
	display: flex;
	flex: row;
	justify-content: space-between;
	align-items: center;
	font-size: large;
	background: var(--THEME-HEADER-BCKGND-GRADIENT);
	padding: 6px;
	min-height: 34px;
}

.search-result-header a {
	text-decoration: none;
	margin-left: 5px;
	color: var(--MENU-TEXT-COLOR);
}

.search-result-header a:hover {
	text-decoration: underline;
}

.search-result-content :hover {
	cursor: pointer;
	background: lightgray;
}

.search-result-content {
	padding: 18px 20px 20px 20px;
	background: white;
}

.date {
	font-size: small;
	color: var(--MENU-TEXT-COLOR);
}

.table-class:deep(td) {
	padding-top: 0;
	background-color: var(--CENTRAL-BCKGND-CLR);
}

.table-class:deep(th) {
	display: none;
}

h1 {
	padding-left: 40px;
}

h3 {
	margin: 11px 11px 12px 11px;
}

@media (max-width: 800px) {
	.table-class:deep(td) {
		padding-left: 0;
		padding-right: 0;
	}

	.date {
		display: none;
	}
}
</style>
