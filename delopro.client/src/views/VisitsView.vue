<script setup>
import Chart from 'primevue/chart'
import { computed, onMounted, reactive, ref } from 'vue'
import { useStore } from 'vuex'
import InputText from 'primevue/inputtext'

const store = useStore()
const visitResponse = computed(() => store.getters.getVisits)

const chartData = ref()
const chartOptions = ref()
const dateRangeForm = reactive({
	fromDate: null,
	toDate: null,
})

onMounted(async () => {
	await store.dispatch('downloadVisits', dateRangeForm)
	chartData.value = setChartData()
	chartOptions.value = setChartOptions()
})

async function handleDateChange() {
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
		aspectRatio: 0.6,
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
