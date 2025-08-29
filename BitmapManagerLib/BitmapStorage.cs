using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace BitmapManager
{
    public class BitmapMetadata
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime UtcStoredTime { get; set; }
        public string FilePath { get; set; }
    }

    public class BitmapStorage
    {
        private readonly string _storageDirectory;
        private readonly Dictionary<string, BitmapMetadata> _metadataStore;

        public BitmapStorage(string storageDirectory)
        {
            _storageDirectory = storageDirectory;
            _metadataStore = new Dictionary<string, BitmapMetadata>();

            // Ensure storage directory exists
            if (!Directory.Exists(_storageDirectory))
            {
                Directory.CreateDirectory(_storageDirectory);
            }

            // Load existing metadata
            LoadMetadata();
        }

        public string StoreBitmap(Bitmap bitmap, string name)
        {
            string id = Guid.NewGuid().ToString();
            string fileName = $"{id}.png";
            string filePath = Path.Combine(_storageDirectory, fileName);

            // Save bitmap to file
            bitmap.Save(filePath, ImageFormat.Png);

            // Create metadata
            var metadata = new BitmapMetadata
            {
                Id = id,
                Name = name,
                UtcStoredTime = DateTime.UtcNow,
                FilePath = filePath
            };

            _metadataStore[id] = metadata;
            SaveMetadata();

            return id;
        }

        public BitmapMetadata GetBitmapMetadata(string id)
        {
            return _metadataStore.ContainsKey(id) ? _metadataStore[id] : null;
        }

        public Bitmap LoadBitmap(string id)
        {
            if (!_metadataStore.ContainsKey(id))
            {
                return null;
            }

            string filePath = _metadataStore[id].FilePath;
            if (!File.Exists(filePath))
            {
                return null;
            }

            return new Bitmap(filePath);
        }

        public IEnumerable<BitmapMetadata> GetAllMetadata()
        {
            return _metadataStore.Values.OrderByDescending(m => m.UtcStoredTime);
        }

        private void LoadMetadata()
        {
            string metadataFile = Path.Combine(_storageDirectory, "metadata.json");
            if (!File.Exists(metadataFile))
            {
                return;
            }

            try
            {
                string json = File.ReadAllText(metadataFile);
                var metadataList = System.Text.Json.JsonSerializer.Deserialize<List<BitmapMetadata>>(json);
                foreach (var metadata in metadataList)
                {
                    _metadataStore[metadata.Id] = metadata;
                }
            }
            catch
            {
                // If metadata file is corrupted, start fresh
                _metadataStore.Clear();
            }
        }

        private void SaveMetadata()
        {
            string metadataFile = Path.Combine(_storageDirectory, "metadata.json");
            var metadataList = _metadataStore.Values.ToList();
            string json = System.Text.Json.JsonSerializer.Serialize(metadataList);
            File.WriteAllText(metadataFile, json);
        }
    }
}