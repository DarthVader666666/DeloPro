<script setup>
import { helper } from '@/helper/helper'
import { computed, onMounted, ref } from 'vue'
import { useStore } from 'vuex'
import { FilterMatchMode } from '@primevue/core/api'
import DataTable from 'primevue/datatable'
import InputText from 'primevue/inputtext'
import Column from 'primevue/column'
import Tag from 'primevue/tag'
import SpinningCircle from '@/components/SpinningCircle.vue'
import UserModal from '@/components/UserModal.vue'
import AvatarImage from '@/components/Account/AvatarImage.vue'

const store = useStore()
const pending = computed(() => store.getters.getPending)
const users = computed(() => store.getters.getUsers)

const filters = ref({
	nickname: { value: null, matchMode: FilterMatchMode.CONTAINS },
	fullName: { value: null, matchMode: FilterMatchMode.CONTAINS },
	email: { value: null, matchMode: FilterMatchMode.CONTAINS },
	registerDate: { value: null, matchMode: FilterMatchMode.CONTAINS },
})

const showUserModal = ref(false)

onMounted(() => {
	showUserModal.value = false
	store.commit('setUser', null)
})

async function onRowSelect(event) {
	const userId = event.data.userId
	await store.dispatch('downloadUser', userId)
	showUserModal.value = true
}

function setShowUserModal() {
	showUserModal.value = false
	store.commit('setUser', null)
}
</script>

<template>
	<SpinningCircle v-if="pending"></SpinningCircle>
	<div
		v-else
		class="users-container"
	>
		<DataTable
			:value="users"
			paginator
			:rows="10"
			:rowsPerPageOptions="[5, 10, 20, 50]"
			v-model:filters="filters"
			filterDisplay="row"
			:globalFilterFields="['nickname', 'email', 'fullName', 'registerDate', 'roles', 'status']"
			stripedRows
			showGridlines
			selectionMode="single"
			@rowSelect="onRowSelect"
		>
			<Column
				field="nickname"
				header="Никнэйм"
				sortable
			>
				<template #body="{ data }">
					<div style="display: flex; align-items: center; gap: 20px">
						<AvatarImage
							:avatar-path="data.avatarPath"
							:size="3.5"
						></AvatarImage>
						<span>{{ data.nickname }}</span>
					</div>
				</template>
				<template #filter="{ filterModel, filterCallback }">
					<InputText
						v-model="filterModel.value"
						type="text"
						@input="filterCallback()"
						style="width: 100%"
						placeholder="Поиск"
					/>
				</template>
			</Column>
			<Column
				field="fullName"
				header="Имя"
				sortable
			>
				<template #body="{ data }">
					<div style="display: flex; align-items: center; gap: 20px">
						<span>{{ data.fullName }}</span>
					</div>
				</template>
				<template #filter="{ filterModel, filterCallback }">
					<InputText
						v-model="filterModel.value"
						type="text"
						@input="filterCallback()"
						style="width: 100%"
						placeholder="Поиск"
					/>
				</template>
			</Column>
			<Column
				field="email"
				header="Email"
				sortable
			>
				<template #body="{ data }">
					<div style="display: flex; align-items: center; gap: 20px">
						<span>{{ data.email }}</span>
					</div>
				</template>
				<template #filter="{ filterModel, filterCallback }">
					<InputText
						v-model="filterModel.value"
						type="text"
						@input="filterCallback()"
						style="width: 100%"
						placeholder="Поиск"
					/>
				</template>
			</Column>
			<Column
				field="registerDate"
				header="Дата регистрации"
				sortable
			>
				<template #body="{ data }">
					{{ helper.getDateStringForUI(data.registerDate, true) }}
				</template>
				<template #filter="{ filterModel, filterCallback }">
					<InputText
						v-model="filterModel.value"
						type="text"
						@input="filterCallback()"
						style="width: 100%"
						placeholder="Поиск"
					/>
				</template>
			</Column>
			<Column
				field="roles"
				header="Роли"
				sortable
			>
				<template #body="{ data }">
					{{ data.roles }}
				</template>
			</Column>
			<Column
				field="status"
				header="Статус"
				sortable
			>
				<template #body="{ data }">
					<Tag
						:value="helper.userStatuses[data.status]"
						:severity="helper.getUserTagSeverity(data.status)"
					></Tag>
				</template>
			</Column>
		</DataTable>
		<UserModal
			v-model:visible="showUserModal"
			@set-show-user-modal="setShowUserModal"
		></UserModal>
	</div>
</template>

<style scoped>
.users-container {
	padding: 20px;
}

.users-container:deep(td) {
	white-space: nowrap;
	max-width: 200px;
	overflow: hidden;
	text-overflow: ellipsis;
}

@media (max-width: 800px) {
	.users-container {
		padding: 3px;
	}
}
</style>
