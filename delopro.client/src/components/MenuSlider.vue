<script setup>
import { useStore } from 'vuex'
import MenuOption from './MenuOption.vue'
import { computed } from 'vue'
import MenuAccount from './MenuAccount.vue'

const store = useStore()
const isAuthenticated = computed(() => store.getters.isAuthenticated)
const currentUser = computed(() => store.getters.getCurrentUser)

const props = defineProps({
	options: {
		type: Array,
		default: () => [],
	},
	setShowAccountSettings: {
		type: Function,
	},
})
</script>

<template>
	<div
		class="slide-container"
		id="slide-menu"
	>
		<div v-if="isAuthenticated">
			<MenuAccount
				:currentUser="currentUser"
				@setShowAccountSettings="props.setShowAccountSettings"
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
				:path="option.path"
				:label="option.label"
				:icon="option.icon"
				:clickHandler="option.clickHandler"
				:id="(option.path ?? index) + '_menu_button'"
			></MenuOption>
		</div>
	</div>
</template>
