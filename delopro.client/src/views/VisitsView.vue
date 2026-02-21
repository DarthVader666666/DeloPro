<script setup>
import Chart from 'primevue/chart'
import { computed, onMounted, reactive, ref } from 'vue'
import { useStore } from 'vuex'
import InputText from 'primevue/inputtext'
import { helper } from '@/helper/helper'

const store = useStore()
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
	<div class="chart-component">
		<Chart
			style="height: 60%"
			type="line"
			:data="chartData"
			:options="chartOptions"
		></Chart>
		<div class="from-to">
			<div>
				<label>с:</label>
				<InputText
					v-model="dateRangeForm.fromDate"
					type="date"
					@change="handleDateChange"
				></InputText>
			</div>
			<div>
				<label>по:</label>
				<InputText
					v-model="dateRangeForm.toDate"
					type="date"
					@change="handleDateChange"
				></InputText>
			</div>
		</div>
	</div>
</template>

<style>
.chart-component {
	display: flex;
	flex-direction: column;
	overflow-x: scroll;
}

.from-to {
	display: flex;
	gap: 10px;
	padding-top: 3%;
	justify-content: center;
}
</style>
