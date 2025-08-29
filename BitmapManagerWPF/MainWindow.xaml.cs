using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using BitmapManager;

namespace BitmapManagerWPF
{
    public partial class MainWindow : Window
    {
        private BitmapStorage _bitmapStorage;
        private Bitmap _currentBitmap;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize storage in user's documents folder
            string storagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BitmapManager");
            _bitmapStorage = new BitmapStorage(storagePath);

            LoadBitmapList();
        }

        private void LoadBitmapBtn_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _currentBitmap = new Bitmap(openFileDialog.FileName);
                    DisplayBitmap(_currentBitmap);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading bitmap: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void StoreBitmapBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBitmap == null)
            {
                MessageBox.Show("Please load a bitmap first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = BitmapNameTxt.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a name for the bitmap.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string id = _bitmapStorage.StoreBitmap(_currentBitmap, name);
                MessageBox.Show($"Bitmap stored successfully with ID: {id}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadBitmapList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error storing bitmap: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RetrieveBitmapBtn_Click(object sender, RoutedEventArgs e)
        {
            string id = SearchIdTxt.Text.Trim();
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Please enter an ID to search.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var metadata = _bitmapStorage.GetBitmapMetadata(id);
                if (metadata == null)
                {
                    MessageBox.Show("Bitmap not found with the specified ID.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var bitmap = _bitmapStorage.LoadBitmap(id);
                if (bitmap == null)
                {
                    MessageBox.Show("Bitmap file not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _currentBitmap = bitmap;
                DisplayBitmap(bitmap);
                DisplayMetadata(metadata);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving bitmap: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BitmapListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (BitmapListBox.SelectedItem is BitmapMetadata metadata)
            {
                try
                {
                    var bitmap = _bitmapStorage.LoadBitmap(metadata.Id);
                    if (bitmap != null)
                    {
                        _currentBitmap = bitmap;
                        DisplayBitmap(bitmap);
                        DisplayMetadata(metadata);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading bitmap: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DisplayBitmap(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                BitmapImage.Source = bitmapImage;
            }
        }

        private void DisplayMetadata(BitmapMetadata metadata)
        {
            MetadataId.Text = $"ID: {metadata.Id}";
            MetadataName.Text = $"Name: {metadata.Name}";
            MetadataTime.Text = $"Stored Time: {metadata.UtcStoredTime.ToLocalTime()}";
        }

        private void LoadBitmapList()
        {
            try
            {
                var metadataList = _bitmapStorage.GetAllMetadata().ToList();
                BitmapListBox.ItemsSource = metadataList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bitmap list: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}