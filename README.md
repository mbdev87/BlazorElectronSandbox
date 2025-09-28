# Blazor WASM Log Analytics with Arrow JS

This project demonstrates a high-performance log analytics solution using:

- **Blazor WebAssembly** for the frontend
- **Apache Arrow JS** via TypeScript for client-side parquet processing
- **ASP.NET Core Web API** for serving log data
- **FluentUI** for the component library

## Architecture

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│  ASP.NET Core   │────│   TypeScript     │────│  Blazor WASM    │
│     Web API     │    │   Arrow JS       │    │   FluentUI      │
│                 │    │                  │    │                 │
│ • Serves data   │    │ • Parquet parse  │    │ • Virtualized   │
│ • Mock logs     │    │ • Search/filter  │    │   grid          │
│ • JSON/Parquet  │    │ • Pagination     │    │ • Search UI     │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

## Project Structure

```
FluentBlazorLogViewExample/
├── TypeScript/
│   └── parquetProcessor.ts     # Arrow JS processing logic
├── Components/
│   └── LogViewer.razor         # Main log viewer component
├── Services/
│   └── ParquetLogService.cs    # C# interop service
├── Pages/
│   └── LogAnalytics.razor      # Log analytics page
└── wwwroot/js/                 # Compiled TypeScript output

LogAnalyticsApi/
├── Controllers/
│   └── LogsController.cs       # Sample log data endpoints
└── ...
```

## Getting Started

### 1. Start the API Backend

```bash
cd LogAnalyticsApi
dotnet run
```

The API will be available at `https://localhost:7071`

### 2. Start the Blazor WASM Frontend

```bash
cd FluentBlazorLogViewExample
dotnet run
```

The frontend will be available at `https://localhost:7032`

### 3. Try the Log Viewer

1. Navigate to the "Log Analytics" page
2. Click "Load Sample Logs"
3. The system will:
   - Download sample log data from the API
   - Parse it using Arrow JS (with JSON fallback)
   - Display it in a virtualized FluentUI DataGrid
4. Try searching for terms like "ERROR", "GameEngine", or "build"

## Key Features

### Client-Side Processing
- **Zero backend compute cost** for log analysis
- **Leverages powerful developer workstations** (256GB RAM, Threadrippers)
- **Arrow JS** for efficient columnar data processing
- **TypeScript** for type safety and maintainability

### Performance Features
- **Virtualized rendering** for handling large datasets
- **Paginated data loading** to manage memory
- **Efficient search/filtering** on client-side
- **Fallback to JSON** when parquet parsing fails

### UI Features
- **FluentUI components** for professional look
- **Real-time search** with highlighting
- **Log level badges** with color coding
- **Responsive layout** with virtualized scrolling

## Development

### TypeScript Compilation
The project auto-compiles TypeScript during build, but you can also run:

```bash
cd FluentBlazorLogViewExample
npm run build    # Compile once
npm run watch    # Watch for changes
```

### Adding More Log Sources
To add new log sources, extend the `LogsController.cs` to serve your parquet files:

```csharp
[HttpGet("custom/{fileName}")]
public IActionResult GetCustomLogs(string fileName)
{
    // Serve your parquet files
}
```

### Memory Management
For very large datasets (1M+ rows), consider:
- Implementing data chunking in `parquetProcessor.ts`
- Adding IndexedDB caching for offline usage
- Implementing virtual scrolling with smaller page sizes

## Technology Decisions

### Why Arrow JS over DuckDB WASM?
- **Better Blazor integration** - fewer WASM runtime conflicts
- **More stable** - established JS interop patterns
- **Easier debugging** - standard browser dev tools
- **Less fragile** - avoids "WASM^3" complexity

### Why Client-Side Processing?
- **Cost efficiency** - offload compute to developer machines
- **Scalability** - zero backend scaling costs
- **Performance** - leverage powerful local hardware
- **Offline capability** - work without backend connectivity

## Future Enhancements

- **Actual parquet file support** (currently uses JSON fallback)
- **IndexedDB persistence** for large datasets
- **Advanced filtering UI** with date ranges, regex support
- **Export functionality** to CSV, Excel
- **Real-time log streaming** via SignalR
- **Memory usage monitoring** and optimization

## Sample Data

The API generates realistic game development logs with:
- **Error logs**: Texture loading failures, memory issues
- **Warning logs**: Performance degradation, deprecated APIs
- **Info logs**: Build completion, test results
- **Debug logs**: Method traces, system health

Perfect for testing large-scale log analysis scenarios!