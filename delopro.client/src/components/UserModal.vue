<script setup>
import Dialog from 'primevue/dialog'
import { computed, reactive, ref } from 'vue'
import { helper } from '@/helper/helper'
import Button from 'primevue/button'
import { useStore } from 'vuex'
import Tag from 'primevue/tag'
import Select from 'primevue/select'
import MultiSelect from 'primevue/multiselect'

const emit = defineEmits(['setShowUserModal'])
const store = useStore()

const updatedUser = reactive({
	userId: null,
	deletionDate: null,
	status: null,
	roles: [],
})

const user = computed(() => {
	const user = store.getters.getUser
	defineUpdatedUserFileds(user)
	return user
})

const selectedRoles = ref([])
const disableSave = ref(true)

function defineUpdatedUserFileds(user = null) {
	updatedUser.userId = user.userId
	updatedUser.deletionDate = user.deletionDate
	updatedUser.status = user.status
	updatedUser.roles = user.roles

	selectedRoles.value = helper.roles.filter((x) => user.roles.includes(helper.roles.indexOf(x)))
}

function handleUpdateStatus(status) {
	disableSave.value = false
	updatedUser.status = helper.userStatuses.indexOf(status)

	if (status === 'Удален') {
		let deletionDate = helper.getFutureDate(30)
		updatedUser.deletionDate = deletionDate
	} else {
		updatedUser.deletionDate = null
	}
}

async function handleUpdateRoles(roles) {
	disableSave.value = false
	updatedUser.roles = roles.map((x) => helper.roles.indexOf(x))
}

function handleCancel() {
	disableSave.value = true
	emit('setShowUserModal')
}

async function updateUser() {
	await store.dispatch('updateUser', updatedUser)
	disableSave.value = true
	emit('setShowUserModal')
}

async function deleteUser() {
	if (window.confirm('Пользователь будет полностью удалён! Вы уверены?')) {
		await store.dispatch('deleteUser', user.value.userId)
		disableSave.value = true
		emit('setShowUserModal')
	}
}
</script>

<template>
	<Dialog
		modal
		@hide="handleCancel"
		:draggable="false"
		:style="{ width: '35rem' }"
	>
		<template #header>
			<span style="color: red">
				{{
					updatedUser.deletionDate &&
					'Пользователь будет удален ' + helper.getDateStringForUI(updatedUser.deletionDate)
				}}
			</span>
		</template>

		<div style="display: flex; flex-direction: row; justify-content: space-between">
			<div
				style="
					display: flex;
					flex-direction: column;
					gap: 20px;
					align-items: center;
					justify-content: start;
					min-width: 50%;
				"
			>
				<span style="font-size: 1.5rem">{{ user.nickname }}</span>
				<div>
					<img
						v-if="user.avatarPath"
						:src="user.avatarPath"
						style="border-radius: 50%; height: 150px; width: 150px"
					/>
					<i
						v-else
						class="pi pi-user avatar"
						style="height: 120px; width: 120px; font-size: 3.5rem"
					></i>
				</div>
				<div
					style="
						display: flex;
						flex-direction: column;
						gap: 5px;
						align-items: center;
						text-align: center;
					"
				>
					<span style="font-weight: bold">Статус:</span>
					<Select
						@update:model-value="handleUpdateStatus"
						:options="helper.userStatuses"
						class="selector"
					>
						<template #value>
							<Tag
								:value="helper.userStatuses[updatedUser.status]"
								:severity="helper.getUserTagSeverity(updatedUser.status)"
							></Tag>
						</template>
						<template #option="slotProps">
							<Tag
								:value="slotProps.option"
								:severity="helper.getUserTagSeverity(helper.userStatuses.indexOf(slotProps.option))"
							></Tag>
						</template>
					</Select>
					<span style="font-weight: bold">Роли:</span>
					<MultiSelect
						@update:model-value="handleUpdateRoles"
						v-model:model-value="selectedRoles"
						:options="helper.roles"
						class="selector"
					/>
				</div>
			</div>

			<div
				style="
					display: flex;
					flex-direction: column;
					gap: 15px;
					align-items: center;
					justify-content: space-between;
				"
			>
				<div class="user-fields">
					<span v-if="user.email">
						<span style="font-weight: bold">Email:</span>
						<span>{{ ' ' + user.email }}</span>
					</span>
					<span v-if="user.phone">
						<span style="font-weight: bold">Телефон:</span>
						<span>{{ ' ' + user.phone }}</span>
					</span>
					<span v-if="user.firstName">
						<span style="font-weight: bold">Имя:</span>
						<span>{{ ' ' + user.firstName }}</span>
					</span>
					<span v-if="user.lastName">
						<span style="font-weight: bold">Фамилия:</span>
						<span>{{ ' ' + user.lastName }}</span>
					</span>
					<span v-if="user.birthDate">
						<span style="font-weight: bold">Дата рождения:</span>
						<span>{{ ' ' + helper.getDateStringForUI(user.birthDate, true) }}</span>
					</span>
					<span v-if="user.country">
						<span style="font-weight: bold">Страна:</span>
						<span>{{ ' ' + user.country }}</span>
					</span>
					<span v-if="user.city">
						<span style="font-weight: bold">Город:</span>
						<span>{{ ' ' + user.city }}</span>
					</span>
					<span v-if="user.userTitle">
						<span style="font-weight: bold">Должность:</span>
						<span>{{ ' ' + user.userTitle }}</span>
					</span>
					<span v-if="user.info">
						<span style="font-weight: bold">О себе:</span>
						<span>{{ ' ' + user.info }}</span>
					</span>
				</div>
				<div
					style="
						display: flex;
						flex-direction: column;
						align-items: center;
						justify-content: center;
						background-color: var(--COLUMNS-BCKGND-CLR);
						align-content: center;
						text-align: center;
						padding: 15px;
						border-radius: 10px;
					"
				>
					<span style="font-weight: bold">Удалить пользователя</span>
					<Button
						style="margin: 10px"
						severity="danger"
						label="Удалить"
						@click="deleteUser"
					></Button>
				</div>
			</div>
		</div>

		<div class="buttons">
			<Button
				type="button"
				@click="updateUser"
				raised
				severity="secondary"
				:disabled="disableSave"
			>
				<i class="pi pi-save"></i>
				<span>Сохранить</span>
			</Button>
			<Button
				type="button"
				@click="handleCancel"
				raised
				severity="contrast"
			>
				<i class="pi pi-ban"></i>
				<span>Отменить</span>
			</Button>
		</div>
	</Dialog>
</template>

<style scoped>
.user-fields {
	display: flex;
	flex-direction: column;
	gap: 5px;
}

.user-fields:deep(div) {
	display: flex;
	flex-direction: row;
	gap: 5px;
}

.selector {
	width: 150px;
	height: 35px;
}

.selector:deep(span) {
	padding: 3px;
	width: 100%;
	text-align: center;
}

ul li span {
	width: 100%;
	text-align: center;
}

.buttons {
	display: flex;
	flex-direction: row;
	margin-top: 20px;
	justify-content: center;
	gap: 20px;
}
</style>
