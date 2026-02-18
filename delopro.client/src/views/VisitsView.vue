<script setup>
import Chart from 'primevue/chart'
import { computed, onMounted, ref } from 'vue'
import { useStore } from 'vuex'

const store = useStore()
const visitResponse = computed(() => store.getters.getVisits)

const chartData = ref()
const chartOptions = ref()

onMounted(async () => {
	await store.dispatch('downloadVisits')

	chartData.value = setChartData()
	chartOptions.value = setChartOptions()
})

const setChartData = () => {
	const documentStyle = getComputedStyle(document.documentElement)

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
	<div class="chart">
		<Chart
			style="height: 60%"
			type="line"
			:data="chartData"
			:options="chartOptions"
		></Chart>
	</div>
</template>

<style>
.chart {
	overflow-x: scroll;
}
</style>
