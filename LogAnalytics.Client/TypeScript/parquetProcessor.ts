import { Table, tableFromIPC } from 'apache-arrow';

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

export class ParquetLogProcessor {
    private table: Table | null = null;
    private jsonData: LogEntry[] | null = null;
    private readonly pageSize: number = 1000;

    async loadParquetFile(arrayBuffer: ArrayBuffer): Promise<void> {
        try {
            // Try to load as Arrow/Parquet first
            this.table = tableFromIPC(new Uint8Array(arrayBuffer));
            this.jsonData = null;
            console.log(`🎯 Loaded parquet file with ${this.table.numRows} rows`);
        } catch (error) {
            console.warn('⚠️ Failed to load as parquet, trying JSON format:', error);

            // Fallback to JSON (for our current setup)
            try {
                const textDecoder = new TextDecoder();
                const jsonString = textDecoder.decode(arrayBuffer);
                this.jsonData = JSON.parse(jsonString) as LogEntry[];
                this.table = null;
                console.log(`📊 Loaded JSON data with ${this.jsonData.length} entries`);
            } catch (jsonError) {
                console.error('💥 Failed to load as JSON as well:', jsonError);
                throw new Error('File format not supported. Expected parquet or JSON format.');
            }
        }
    }

    getPage(pageIndex: number, pageSize: number = this.pageSize): PagedResult<LogEntry> {
        if (!this.table && !this.jsonData) {
            throw new Error('No data loaded');
        }

        if (this.jsonData) {
            // Handle JSON data (current setup)
            const startIndex = pageIndex * pageSize;
            const endIndex = Math.min(startIndex + pageSize, this.jsonData.length);
            const pageData = this.jsonData.slice(startIndex, endIndex);

            return {
                data: pageData,
                totalCount: this.jsonData.length,
                pageIndex,
                pageSize,
                hasMore: endIndex < this.jsonData.length
            };
        }

        // Handle Arrow table data (future parquet support)
        const startRow = pageIndex * pageSize;
        const endRow = Math.min(startRow + pageSize, this.table!.numRows);

        const data: LogEntry[] = [];

        for (let i = startRow; i < endRow; i++) {
            const row = this.table!.get(i);
            data.push({
                timestamp: row?.timestamp?.toString() || '',
                level: row?.level?.toString() || '',
                message: row?.message?.toString() || '',
                source: row?.source?.toString() || '',
                threadId: row?.threadId || undefined,
                buildId: row?.buildId?.toString() || undefined
            });
        }

        return {
            data,
            totalCount: this.table!.numRows,
            pageIndex,
            pageSize,
            hasMore: endRow < this.table!.numRows
        };
    }

    search(query: string, pageIndex: number = 0, pageSize: number = this.pageSize): PagedResult<LogEntry> {
        if (!this.table && !this.jsonData) {
            throw new Error('No data loaded');
        }

        const allMatches: LogEntry[] = [];
        const queryLower = query.toLowerCase();

        if (this.jsonData) {
            // Search JSON data
            for (const entry of this.jsonData) {
                if (entry.message.toLowerCase().includes(queryLower) ||
                    entry.level.toLowerCase().includes(queryLower) ||
                    entry.source.toLowerCase().includes(queryLower)) {
                    allMatches.push(entry);
                }
            }
        } else if (this.table) {
            // Search Arrow table data
            for (let i = 0; i < this.table.numRows; i++) {
                const row = this.table.get(i);
                const entry: LogEntry = {
                    timestamp: row?.timestamp?.toString() || '',
                    level: row?.level?.toString() || '',
                    message: row?.message?.toString() || '',
                    source: row?.source?.toString() || '',
                    threadId: row?.threadId || undefined,
                    buildId: row?.buildId?.toString() || undefined
                };

                if (entry.message.toLowerCase().includes(queryLower) ||
                    entry.level.toLowerCase().includes(queryLower) ||
                    entry.source.toLowerCase().includes(queryLower)) {
                    allMatches.push(entry);
                }
            }
        }

        const startIndex = pageIndex * pageSize;
        const endIndex = Math.min(startIndex + pageSize, allMatches.length);
        const pageData = allMatches.slice(startIndex, endIndex);

        return {
            data: pageData,
            totalCount: allMatches.length,
            pageIndex,
            pageSize,
            hasMore: endIndex < allMatches.length
        };
    }

    getStats(): { totalRows: number; columns: string[] } | null {
        if (this.jsonData) {
            return {
                totalRows: this.jsonData.length,
                columns: ['timestamp', 'level', 'message', 'source', 'threadId', 'buildId']
            };
        }

        if (this.table) {
            return {
                totalRows: this.table.numRows,
                columns: this.table.schema.fields.map(field => field.name)
            };
        }

        return null;
    }
}

// Global instance for Blazor interop
let processor: ParquetLogProcessor | null = null;

// Export functions for webpack bundling and window attachment
export const ParquetProcessorAPI = {
    initialize: () => {
        processor = new ParquetLogProcessor();
        console.log('🚀 ParquetLogProcessor initialized!');
        return true;
    },

    loadFile: async (arrayBuffer: ArrayBuffer) => {
        if (!processor) throw new Error('Processor not initialized');
        await processor.loadParquetFile(arrayBuffer);
        return true;
    },

    getPage: (pageIndex: number, pageSize: number = 1000) => {
        if (!processor) throw new Error('Processor not initialized');
        return processor.getPage(pageIndex, pageSize);
    },

    search: (query: string, pageIndex: number = 0, pageSize: number = 1000) => {
        if (!processor) throw new Error('Processor not initialized');
        return processor.search(query, pageIndex, pageSize);
    },

    getStats: () => {
        if (!processor) return null;
        return processor.getStats();
    }
};

// Attach to window for Blazor interop
declare global {
    interface Window {
        ParquetLogProcessor: typeof ParquetProcessorAPI;
    }
}

window.ParquetLogProcessor = ParquetProcessorAPI;