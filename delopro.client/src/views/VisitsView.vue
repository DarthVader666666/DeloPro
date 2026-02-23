<script setup>
import Chart from 'primevue/chart'
import { computed, onMounted, reactive, ref } from 'vue'
import { useStore } from 'vuex'
import InputText from 'primevue/inputtext'
import { helper } from '@/helper/helper'
import SpinningCircle from '@/components/SpinningCircle.vue'

const store = useStore()
const pending = computed(() => store.getters.getPending)
const visitResponse = computed(() => store.getters.getVisits)
const currentDate = new Date()

const chartData = ref()
const chartOptions = ref()
const dateRangeForm = reactive({
	fromDate: null,
	toDate: null,
})

onMounted(async () => {
	const dateNow = new Date()
	dateRangeForm.fromDate = helper.getDateStringForInput(dateNow.setDate(dateNow.getDate() - 30))
	dateRangeForm.toDate = helper.getDateStringForInput(currentDate)

	await store.dispatch('downloadVisits', dateRangeForm)
	chartData.value = setChartData()
	chartOptions.value = setChartOptions()
})

async function handleDateChange() {
	const fromDate = new Date(dateRangeForm.fromDate)
	const toDate = new Date(dateRangeForm.toDate)

	if (toDate > currentDate) {
		dateRangeForm.toDate = helper.getDateStringForInput(currentDate)
	}

	if (fromDate > currentDate) {
		dateRangeForm.fromDate = helper.getDateStringForInput(currentDate)
	}

	await store.dispatch('downloadVisits', dateRangeForm)
	chartData.value = setChartData()
}

const setChartData = () => {
	return {
		labels: visitResponse.value.labels,
		datasets: visitResponse.value.datasets,
	}
}
const setChartOptions = () => {
	const documentStyle = getComputedStyle(document.documentElement)
	const textColor = documentStyle.getPropertyValue('--p-text-color')
	const textColorSecondary = documentStyle.getPropertyValue('--p-text-muted-color')
	const surfaceBorder = documentStyle.getPropertyValue('--p-content-border-color')

	return {
		maintainAspectRatio: false,
		plugins: {
			legend: {
				labels: {
					color: textColor,
				},
			},
		},
		scales: {
			x: {
				ticks: {
					color: textColorSecondary,
				},
				grid: {
					color: surfaceBorder,
				},
			},
			y: {
				ticks: {
					color: textColorSecondary,
				},
				grid: {
					color: surfaceBorder,
				},
			},
		},
	}
}
</script>

<template>
	<SpinningCircle v-if="pending"></SpinningCircle>
	<div
		v-else
		class="chart-container"
	>
		<Chart
			style="height: 55%"
			type="line"
			:data="chartData"
			:options="chartOptions"
		></Chart>
		<div class="from-to">
			<div>
				<label>с:</label>
				<InputText
					style="padding: 5px"
					v-model="dateRangeForm.fromDate"
					type="date"
					@change="handleDateChange"
				></InputText>
			</div>
			<div>
				<label>по:</label>
				<InputText
					style="padding: 5px"
					v-model="dateRangeForm.toDate"
					type="date"
					@change="handleDateChange"
				></InputText>
			</div>
		</div>
	</div>
</template>

<style>
.chart-container {
	display: flex;
	flex-direction: column;
	overflow-x: scroll;
}

.from-to {
	display: flex;
	justify-content: center;
	padding: 15px 0 0 15px;
}
</style>
