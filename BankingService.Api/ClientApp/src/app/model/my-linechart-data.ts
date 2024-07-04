export interface MyLinechartData {
    name: string;
    series: DateKeyValueData[];
}

export interface DateKeyValueData {
    value: number,
    name: Date
}
