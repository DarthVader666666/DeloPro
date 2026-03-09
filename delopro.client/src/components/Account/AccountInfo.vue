<script setup>
import Button from 'primevue/button'
import PropertyComponent from '../PropertyComponent.vue'
import AvatarImage from './AvatarImage.vue'
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
	<div class="account-info">
		<div class="account-header">
			<AvatarImage
				:avatarPath="props.user.avatarPath"
				:avatarBase64="props.avatarBase64"
			></AvatarImage>

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
					Дата регистрации: {{ props.user.registerDate.slice(0, 10) }}
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
		<PropertyComponent
			propName="Никнэйм"
			:propValue="props.user.nickname"
		/>
		<PropertyComponent
			propName="Email"
			:propValue="props.user.email"
		/>
		<PropertyComponent
			propName="Телефон"
			:propValue="props.user.phone"
		/>
		<PropertyComponent
			propName="Имя"
			:propValue="props.user.firstName"
		/>
		<PropertyComponent
			propName="Фамилия"
			:propValue="props.user.lastName"
		/>
		<PropertyComponent
			propName="Дата рождения"
			:propValue="helper.getDateStringForUI(props.user.birthDate, true)"
		/>
		<PropertyComponent
			propName="Страна"
			:propValue="props.user.country"
		/>
		<PropertyComponent
			propName="Город"
			:propValue="props.user.city"
		/>
		<PropertyComponent
			propName="Должность"
			:propValue="props.user.userTitle"
		/>
		<PropertyComponent
			propName="О себе"
			:propValue="props.user.info"
		/>
	</div>
</template>
