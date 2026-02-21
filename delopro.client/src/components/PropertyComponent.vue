<script setup>
const props = defineProps({
	propValue: {
		type: String,
		default: null,
	},
	propName: {
		type: String,
		default: null,
	},
})
</script>

<template>
	<div>
		<span class="account-property-name">{{ props.propName }}:</span>
		<span v-if="props.propValue">{{ props.propValue }}</span>
		<span
			v-else
			class="account-property-undefined"
		>
			не указано
		</span>
	</div>
</template>

<style>
.account-property-name {
	font-weight: bold;
	padding-right: 10px;
}
.account-property-undefined {
	font-size: 0.9rem;
	font-style: italic;
	color: gray;
}
</style>
