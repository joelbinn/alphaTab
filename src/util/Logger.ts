import { LogLevel } from "@src/LogLevel";

export class Logger {
    public static logLevel: LogLevel = LogLevel.Info;

    private static format(category: string, msg: string): string {
        return `[AlphaTab][${category}] ${msg}`;
    }

    private static shouldLog(level: LogLevel): boolean {
        return Logger.logLevel !== LogLevel.None && level >= Logger.logLevel;
    }

    public static debug(category: string, msg: string, ...details: unknown[]): void {
        if (Logger.shouldLog(LogLevel.Debug)) {
            console.debug(Logger.format(category, msg), ...details);
        }
    }

    public static warning(category: string, msg: string, ...details: unknown[]): void {
        if (Logger.shouldLog(LogLevel.Warning)) {
            console.warn(Logger.format(category, msg), ...details);
        }
    }

    public static info(category: string, msg: string, ...details: unknown[]): void {
        if (Logger.shouldLog(LogLevel.Info)) {
            console.info(Logger.format(category, msg), ...details);
        }
    }

    public static error(category: string, msg: string, ...details: unknown[]): void {
        if (Logger.shouldLog(LogLevel.Error)) {
            console.error(Logger.format(category, msg), ...details);
        }
    }
}