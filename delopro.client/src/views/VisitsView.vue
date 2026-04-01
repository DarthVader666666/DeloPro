<script setup>
import Chart from 'primevue/chart'
import { computed } from 'vue'
import { useStore } from 'vuex'
import InputText from 'primevue/inputtext'
import { helper } from '@/helper/helper'

const store = useStore()
const visitResponse = computed(() => store.getters.getVisits)

const chartData = computed(() => ({
	labels: visitResponse.value.labels,
	datasets: visitResponse.value.datasets,
}))

const chartOptions = computed(() => setChartOptions())
const dateRange = computed(() => store.getters.getVisitsDateRange)

async function handleFromDateChange(fromDate) {
	const today = new Date()
	const currentDate = new Date(today.getFullYear(), today.getMonth(), today.getDate())
	let fromDateValue = fromDate

	if (new Date(fromDate) > currentDate) {
		fromDateValue = helper.getDateStringForInput(today)
	}

	await store.dispatch('downloadVisits', {
		fromDate: fromDateValue,
		toDate: dateRange.value.toDate,
	})
}

async function handleToDateChange(toDate) {
	const today = new Date()
	const currentDate = new Date(today.getFullYear(), today.getMonth(), today.getDate())
	let toDateValue = toDate

	if (new Date(toDate) > currentDate) {
		toDateValue = helper.getDateStringForInput(currentDate)
	}

	await store.dispatch('downloadVisits', {
		fromDate: dateRange.value.fromDate,
		toDate: toDateValue,
	})
}

function setChartOptions() {
	const documentStyle = getComputedStyle(document.documentElement)
	const textColor = documentStyle.getPropertyValue('--p-text-color')
	const textColorSecondary = documentStyle.getPropertyValue('--p-text-muted-color')
	const surfaceBorder = '#d3d3d3'

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
	<div class="chart-container">
		<Chart
			style="height: 55%; padding: 10px"
			type="line"
			:data="chartData"
			:options="chartOptions"
		></Chart>
		<div class="from-to">
			<div>
				<label style="padding: 5px">с:</label>
				<InputText
					style="padding: 5px"
					v-model="dateRange.fromDate"
					type="date"
					@change="handleFromDateChange($event.target.value)"
				></InputText>
			</div>
			<div>
				<label style="padding: 5px">по:</label>
				<InputText
					style="padding: 5px"
					v-model="dateRange.toDate"
					type="date"
					@change="handleToDateChange($event.target.value)"
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
