# Bitmap Storage Manager

A cross-platform C# application for storing and retrieving bitmaps with metadata management.

## Features

- **Cross-platform compatibility**: Works on both Linux and Windows
- **Bitmap storage**: Store bitmaps with unique IDs and metadata
- **Metadata tracking**: Stores name and UTC timestamp for each bitmap
- **Local file storage**: Uses local file system for persistence
- **WPF UI**: Windows application with graphical interface for bitmap management

## Project Structure

- `BitmapStorageLib/`: Core library containing bitmap storage functionality
- `BitmapStorageWPF/`: Windows WPF application for GUI interaction

## Requirements

- .NET 8.0 or later
- For Windows: Windows 10 or later (for WPF application)

## Building and Running

### Using .NET CLI

1. Restore dependencies:
   ```bash
   dotnet restore
   ```

2. Build the solution:
   ```bash
   dotnet build
   ```

3. Run the WPF application (Windows only):
   ```bash
   cd BitmapStorageWPF
   dotnet run
   ```

### Using Visual Studio

1. Open `BitmapStorageManager.sln` in Visual Studio
2. Build the solution
3. Set `BitmapStorageWPF` as the startup project
4. Run the application

## Usage

1. **Load Bitmap**: Click "Load Bitmap" to select an image file
2. **Store Bitmap**: Enter a name and click "Store Bitmap" to save it with metadata
3. **Retrieve Bitmap**: Enter an ID in the search box and click "Retrieve Bitmap"
4. **Browse Stored Bitmaps**: Use the list on the right to view and select stored bitmaps

## Storage Location

Bitmaps are stored in:
- Windows: `%USERPROFILE%\Documents\BitmapStorage\`
- Linux: `~/Documents/BitmapStorage/`

## API Reference

### BitmapStorage Class

- `StoreBitmap(Bitmap bitmap, string name)`: Stores a bitmap and returns its unique ID
- `GetBitmapMetadata(string id)`: Retrieves metadata for a bitmap by ID
- `LoadBitmap(string id)`: Loads a bitmap by ID
- `GetAllMetadata()`: Returns all stored bitmap metadata

### BitmapMetadata Class

- `Id`: Unique identifier for the bitmap
- `Name`: User-provided name for the bitmap
- `UtcStoredTime`: UTC timestamp when the bitmap was first stored
- `FilePath`: Local file system path to the bitmap file

## License

This project is open source. Feel free to use and modify as needed.