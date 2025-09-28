export interface LogEntry {
    timestamp: string;
    level: string;
    message: string;
    source: string;
    threadId?: number;
    buildId?: string;
}
export interface PagedResult<T> {
    data: T[];
    totalCount: number;
    pageIndex: number;
    pageSize: number;
    hasMore: boolean;
}
export declare class ParquetLogProcessor {
    private table;
    private jsonData;
    private readonly pageSize;
    loadParquetFile(arrayBuffer: ArrayBuffer): Promise<void>;
    getPage(pageIndex: number, pageSize?: number): PagedResult<LogEntry>;
    search(query: string, pageIndex?: number, pageSize?: number): PagedResult<LogEntry>;
    getStats(): {
        totalRows: number;
        columns: string[];
    } | null;
}
export declare const ParquetProcessorAPI: {
    initialize: () => boolean;
    loadFile: (arrayBuffer: ArrayBuffer) => Promise<boolean>;
    getPage: (pageIndex: number, pageSize?: number) => PagedResult<LogEntry>;
    search: (query: string, pageIndex?: number, pageSize?: number) => PagedResult<LogEntry>;
    getStats: () => {
        totalRows: number;
        columns: string[];
    } | null;
};
declare global {
    interface Window {
        ParquetLogProcessor: typeof ParquetProcessorAPI;
    }
}
//# sourceMappingURL=parquetProcessor.d.ts.map