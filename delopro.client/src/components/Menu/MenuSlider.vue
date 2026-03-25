<script setup>
import { useStore } from 'vuex'
import MenuOption from './MenuOption.vue'
import { computed, ref } from 'vue'
import MenuAccount from './MenuAccount.vue'
import MenuAccountSettings from './MenuAccountSettings.vue'

const store = useStore()
const isAuthenticated = computed(() => store.getters.isAuthenticated)
const currentUser = computed(() => store.getters.getCurrentUser)
const showAccountSettings = ref(false)

const props = defineProps({
	options: {
		type: Array,
		default: () => [],
	},
	accountOptions: {
		type: Array,
		default: () => [],
	},
})

function setShowAccountSettings(value) {
	showAccountSettings.value = value !== undefined ? value : !showAccountSettings.value
}
</script>

<template>
	<div
		class="slide-container"
		id="slide-menu"
	>
		<div v-if="isAuthenticated">
			<MenuAccount
				:currentUser="currentUser"
				@setShowAccountSettings="setShowAccountSettings"
			></MenuAccount>
			<hr style="width: 100%" />
		</div>

		<div
			v-for="(option, index) in props.options"
			:key="index"
		>
			<MenuOption
				v-if="
					option.roles.includes('Any') ||
					(isAuthenticated
						? option.roles.some((role) => currentUser?.roles?.includes(role))
						: option.roles.length === 0)
				"
				:routeName="option.path"
				:label="option.label"
				:icon="option.icon"
				:clickHandler="option.clickHandler"
				:id="option.id"
			></MenuOption>
		</div>
		<MenuAccountSettings
			v-show="showAccountSettings"
			style="top: 0"
			@setShowAccountSettings="setShowAccountSettings"
			:accountOptions="props.accountOptions"
		></MenuAccountSettings>
	</div>
</template>
