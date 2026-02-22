<script setup>
import Textarea from 'primevue/textarea'
import InputText from 'primevue/inputtext'

const props = defineProps({
	title: {
		type: String,
		required: true,
	},
	modelValue: {
		type: String,
		default: null,
	},
	type: {
		type: String,
		default: 'text',
	},
	maxlength: {
		type: Number,
		default: 50,
	},
	required: {
		type: Boolean,
		default: false,
	},
	disabled: {
		type: Boolean,
		default: false,
	},
	placeholder: {
		type: String,
		default: null,
	},
	inputHandler: {
		type: Function,
		required: false,
	},
	errorText: {
		type: String,
		default: null,
	},
	showError: {
		type: Boolean,
		default: false,
	},
	isCorrect: {
		type: Boolean,
		default: false,
	},
	isTextarea: {
		type: Boolean,
		default: false,
	},
	showRedStar: {
		type: Boolean,
		default: false,
	},
	invalid: {
		type: Boolean,
		default: false,
	},
	titleFont: {
		type: Object,
		default: () => ({
			fontWeight: 'bold',
		}),
	},
})

const emit = defineEmits(['update:modelValue'])

function onInput(event) {
	const value = event.target.value

	emit('update:modelValue', value)

	if (props.inputHandler) {
		props.inputHandler(value)
	}
}
</script>
<template>
	<div class="input-container">
		<span :style="props.titleFont">
			{{ props.title }}:
			<span
				v-if="props.showRedStar"
				style="color: red"
			>
				*
			</span>
			<span
				v-if="props.showError"
				style="color: red; font-weight: lighter"
			>
				{{ props.errorText }}
			</span>
			<i
				style="color: green; position: absolute; padding-left: 10px; font-size: 1.2rem"
				v-if="isCorrect"
				class="pi pi-check"
			></i>
		</span>
		<Textarea
			style="height: 120px"
			v-if="props.isTextarea"
			:placeholder="props.placeholder"
			:value="props.modelValue"
			:disabled="props.disabled"
			@input.prevent="onInput"
			:required="props.required"
		></Textarea>
		<InputText
			v-else
			:type="props.type"
			:placeholder="props.placeholder"
			:value="props.modelValue"
			:disabled="props.disabled"
			@input.prevent="onInput"
			:maxlength="props.maxlength"
			:required="props.required"
			:invalid="props.invalid"
		></InputText>
	</div>
</template>

<style>
.input-container {
	display: flex;
	flex-direction: column;
	padding-top: 8px;
}

.input-container span {
	padding: 3px;
}
</style>
