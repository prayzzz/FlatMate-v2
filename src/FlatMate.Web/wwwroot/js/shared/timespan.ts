export default class Timespan {
    public static fromHours(hours: number): number {
        return hours * Timespan.fromMinutes(60);
    }

    public static fromMinutes(minutes: number): number {
        return minutes * Timespan.fromSeconds(60);
    }

    public static fromSeconds(seconds: number): number {
        return seconds * 1000;
    }
}
