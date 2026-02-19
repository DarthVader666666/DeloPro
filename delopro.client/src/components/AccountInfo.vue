<script setup>
import Button from 'primevue/button'
import AccountProperty from './AccountProperty.vue'
import AccountAvatar from './AccountAvatar.vue'
import { useRouter } from 'vue-router'
import { helper } from '@/helper/helper'

const props = defineProps({
	user: {
		type: Object,
		default: null,
	},
	avatarBase64: {
		type: String,
		default: null,
	},
})

const router = useRouter()
const emit = defineEmits(['switchToEditMode'])
</script>

<template>
	<div class="account-properties">
		<div class="account-header">
			<AccountAvatar
				:avatarPath="props.user.avatarPath"
				:avatarBase64="props.avatarBase64"
			></AccountAvatar>

			<div class="account-short-info">
				<span style="font-weight: bold; font-size: large">{{ props.user.nickname }}</span>
				<span style="font-size: 1.2rem">
					{{ `${props.user.firstName ?? ''} ${props.user.lastName ?? ''}` }}
				</span>
				<span style="font-style: italic; color: gray">{{ props.user.roles.join(',') }}</span>
				<span
					v-if="props.user.registerDate"
					style="font-style: italic"
				>
					Дата регистрации: {{ helper.getDateStringForUI(props.user.registerDate, true) }}
				</span>
				<div style="padding-top: 10px">
					<Button
						@click="async () => emit('switchToEditMode')"
						severity="contrast"
						raised
					>
						Редактировать
					</Button>
				</div>
			</div>
		</div>
		<Button
			severity="contrast"
			text
			rounded
			style="position: absolute; top: 0; right: 0; height: 45px"
			@click="() => router.back()"
		>
			<i
				class="pi pi-times"
				style="font-size: 1.3rem; padding-top: 3px"
			></i>
		</Button>
		<AccountProperty
			propertyName="Никнэйм"
			:propertyValue="props.user.nickname"
		/>
		<AccountProperty
			propertyName="Email"
			:propertyValue="props.user.email"
		/>
		<AccountProperty
			propertyName="Телефон"
			:propertyValue="props.user.phone"
		/>
		<AccountProperty
			propertyName="Имя"
			:propertyValue="props.user.firstName"
		/>
		<AccountProperty
			propertyName="Фамилия"
			:propertyValue="props.user.lastName"
		/>
		<AccountProperty
			propertyName="Дата рождения"
			:propertyValue="helper.getDateStringForUI(props.user.birthDate, true)"
		/>
		<AccountProperty
			propertyName="Страна"
			:propertyValue="props.user.country"
		/>
		<AccountProperty
			propertyName="Город"
			:propertyValue="props.user.city"
		/>
		<AccountProperty
			propertyName="Должность"
			:propertyValue="props.user.userTitle"
		/>
		<AccountProperty
			propertyName="О себе"
			:propertyValue="props.user.info"
		/>
	</div>
</template>
