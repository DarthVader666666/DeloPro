<script setup>
import Textarea from 'primevue/textarea';
import InputText from 'primevue/inputtext';

const props = defineProps({
  title: {
    type: String,
    required: true
  },
  modelValue: {
    type: String,
    required: true
  },
  type: {
    type: String,
    default: 'text'
  },
  maxlength: {
    type: Number,
    default: 50
  },
  required: {
    type: Boolean,
    default: false
  },
  disabled: {
    type: Boolean,
    default: false
  },
  placeholder: {
    type: String,
    default: null
  },
  inputHandler: {
    type: Function,
    required: false
  },
  errorText: {
    type: String,
    default: null
  },
  showError: {
    type: Boolean,
    default: false
  },
  isCorrect: {
    type: Boolean,
    default: false
  },
  isTextarea: {
    type: Boolean,
    default: false
  }
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
  <div class="account-input">
		<span>
			{{ props.title }}:
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
    <Textarea v-if="props.isTextarea"
			:placeholder="props.placeholder"
			:value="props.modelValue"
      :disabled="props.disabled"
			@input.prevent="onInput"
			:required="props.required"
		></Textarea>
		<InputText v-else
			:type="props.type"
			:placeholder="props.placeholder"
			:value="props.modelValue"
      :disabled="props.disabled"
			@input.prevent="onInput"
			:maxlength="props.maxlength"
			:required="props.required"
		></InputText>
  </div>
</template>

<style>
.account-input {
	display: flex;
	flex-direction: column;
}

.account-input span {
	font-weight: bold;
	padding: 3px;
}
</style>
