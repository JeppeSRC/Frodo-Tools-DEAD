using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using FDconverterCLI;

namespace FDconverter {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window {

        void Sort(ref List<int> indices) {
            List<int> tmp = new List<int>(indices);

            for (int i = 0; i < indices.Count; i++) {
                 int max = 0;

                for (int j = 0; j < tmp.Count; j++) {
                    if (tmp[j] > max) max = tmp[j];
                }

                tmp.Remove(max);

                indices[i] = max;
            }
        }

        private ObservableCollection<FDFile> files;

        private string exePath;

        public MainWindow() {
            InitializeComponent();
            files = new ObservableCollection<FDFile>();

            lvFiles.ItemsSource = files;
            exePath = Assembly.GetEntryAssembly().Location;
            exePath = exePath.Remove(exePath.Length - 17, 17);

            FileType[] types = new FileType[3];

            types[(int)FileType.Unknown] = FileType.Unknown;
            types[(int)FileType.Image] = FileType.Image;
            types[(int)FileType.Model] = FileType.Model;


            TextureChannel[] channels = new TextureChannel[(int)TextureChannel.NUM];

            for (int i = 0; i < (int)TextureChannel.NUM; i++) {
                channels[i] = (TextureChannel)i;
            }

            TextureChannelType[] channelTypes = new TextureChannelType[(int)TextureChannelType.NUM];

            for (int i = 0; i < (int)TextureChannelType.NUM; i++) {
                channelTypes[i] = (TextureChannelType)i;
            }

            cbType.ItemsSource = types;
            cbImageChannels.ItemsSource = channels;
            cbImageSizes.ItemsSource = channelTypes;

            imageOptionsGroup.Visibility = Visibility.Hidden;
            modelOptionsGroup.Visibility = Visibility.Hidden;

            tvDstPath.Text = exePath + "ouput\\";
            Directory.CreateDirectory("output");
        }

        private bool AddFolder(string folder) {
            List<string> f = null;
            try {
                f = new List<string>(Directory.EnumerateDirectories(folder));
            } catch (IOException) {
                return false;
            }catch (Exception e) {
                MessageBox.Show(e.Message, string.Format("Exception enumarting folder \"{0}\"", folder), MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (f.Count >= 0) {
                AddFolders(f.ToArray());
            }

            f = new List<string>(Directory.EnumerateFiles(folder));

            AddFiles(f.ToArray());

            return true;
        }

        private void AddFolders(string[] folders) {
            for (int i = 0; i < folders.Length; i++) {
                AddFolder(folders[i]);
            }
        }

        private void AddFiles(string[] files) {
            for (int i = 0; i < files.Length; i++) {
               if (AddFolder(files[i]) == false) this.files.Add(FDFile.CreateFile(files[i]));
            }

            if (this.files.Count > 0 ) {
                btnStart.IsEnabled = true;
                cbType.IsEnabled = true;
                ckbIncluded.IsEnabled = true;
            }
        }

        private void SetOptionsGroupVisibility(FileType type) {
            switch (type) {
                case FileType.Unknown:
                    imageOptionsGroup.Visibility = Visibility.Hidden;
                    modelOptionsGroup.Visibility = Visibility.Hidden;
                    break;
                case FileType.Image:
                    imageOptionsGroup.Visibility = Visibility.Visible;
                    modelOptionsGroup.Visibility = Visibility.Hidden;
                    break;
                case FileType.Model:
                    modelOptionsGroup.Visibility = Visibility.Visible;
                    imageOptionsGroup.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void UpdateIncludeAllCheckbox() {
            if (lvFiles.SelectedItems.Count == 0) return;

            FDFile tmp = (FDFile)lvFiles.SelectedItems[0];
            
            if (lvFiles.SelectedItems.Count == 1) {
                ckbIncluded.IsChecked = tmp.included;
                return;
            }

            bool isAllIncluded = true;

            for (int i = 1; i < lvFiles.SelectedItems.Count; i++) {
                FDFile file = (FDFile)lvFiles.SelectedItems[i];

                if (tmp.included != file.included && isAllIncluded) {
                    isAllIncluded = false;
                }
            }

            if (!isAllIncluded) {
                ckbIncluded.IsChecked = null;
            } else {
                ckbIncluded.IsChecked = tmp.included;
            }
        }

        private List<int> GetFileIndices(List<FDFile> list) {

            List<int> indices = new List<int>();

            for (int i = 0; i < list.Count; i++) {
                for (int j = 0; j < files.Count; j++) {
                    if (list[i] == files[j]) indices.Add(j);
                }
            }

            Sort(ref indices);

            return indices;
        }

        private void RemoveFiles(List<FDFile> files) {

            for (int i = 0; i < files.Count; i++) {
                if (files[i].progress != -1) files.RemoveAt(i);
            }

            List<int> indices = GetFileIndices(files);

            for (int i = 0; i < indices.Count; i++) {
                this.files.RemoveAt(indices[i]);
            }
        }

        private void lvFiles_Drop(object sender, DragEventArgs e) {
            AddFiles((string[])e.Data.GetData(DataFormats.FileDrop));
        }

        private void lvFiles_KeyDown(object sender, KeyEventArgs e) { 
            if (e.Key == Key.Delete) {
                List<FDFile> tmp = lvFiles.SelectedItems.Cast<FDFile>().ToList();

                RemoveFiles(tmp);
            }
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) {
           System.Windows.Forms.OpenFileDialog diag = new System.Windows.Forms.OpenFileDialog();

            diag.CheckFileExists = true;
            diag.CheckPathExists = true;
            diag.Multiselect = true;
            diag.Title = "Add Files";

            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                AddFiles(diag.FileNames);
            }
        }

        private void Button_Click_Remove(object sender, RoutedEventArgs e) {
            List<FDFile> tmp = lvFiles.SelectedItems.Cast<FDFile>().ToList();

            RemoveFiles(tmp);
        }

        private void Button_Click_Destination(object sender, RoutedEventArgs e) {
            System.Windows.Forms.FolderBrowserDialog diag = new System.Windows.Forms.FolderBrowserDialog();

            diag.RootFolder = Environment.SpecialFolder.MyComputer;
            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                tvDstPath.Text = diag.SelectedPath + "\\";
            }
        }

        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (cbType.SelectedIndex == -1) return;

            List<FDFile> selected = lvFiles.SelectedItems.Cast<FDFile>().ToList();

            List<int> indices = GetFileIndices(selected);

            FileType type = (FileType)cbType.SelectedIndex;

            lvFiles.SelectionChanged -= lvFiles_SelectionChanged;

            for (int i = 0; i < indices.Count; i++) {
                FDFile current = files[indices[i]];
                switch (type) {
                    case FileType.Unknown:
                        current.type = (FileType)cbType.SelectedIndex;
                        break;
                    case FileType.Image:
                        files[indices[i]] = new FDTextureFile(current);
                        break;
                }
            }

           for (int i = 0; i < indices.Count; i++) {
                lvFiles.SelectedItems.Add(files[indices[i]]);
            }

            lvFiles.SelectionChanged += lvFiles_SelectionChanged;

            SetOptionsGroupVisibility((FileType)cbType.SelectedIndex);
        }

        private void lvFiles_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (lvFiles.SelectedItems.Count == 0) {
                btnCancel.IsEnabled = false;
                btnRemove.IsEnabled = false;
            } else {
                foreach (FDFile f in lvFiles.SelectedItems) {
                    if (f.included == null) {
                        btnCancel.IsEnabled = true;
                    } 
                }

                btnRemove.IsEnabled = true;
            }

            FDFile tmp = (FDFile)lvFiles.SelectedItems[0];

            cbType.SelectionChanged -= cbType_SelectionChanged;
            cbImageChannels.SelectionChanged -= cbImageChannels_SelectionChanged;
            cbImageSizes.SelectionChanged -= cbImageSizes_SelectionChanged;

            cbType.SelectedIndex = 0;
            cbImageChannels.SelectedIndex = 0;
            cbImageSizes.SelectedIndex = 0;

            for (int i = 1; i < lvFiles.SelectedItems.Count; i++) {
                FDFile file = (FDFile)lvFiles.SelectedItems[i];
                if (tmp.type != file.type) {
                    cbType.SelectedIndex = -1;
                }

                if (imageOptionsGroup.IsVisible) {
                    if (((FDTextureFile)tmp).channel != ((FDTextureFile)file).channel) {
                        cbImageChannels.SelectedIndex = -1;
                    }

                    if (((FDTextureFile)tmp).channelType != ((FDTextureFile)file).channelType) {
                        cbImageSizes.SelectedIndex = -1;
                    }
                }

                
            }

            if (cbType.SelectedIndex == 0) {
                cbType.SelectedIndex = (int)tmp.type;
                SetOptionsGroupVisibility(tmp.type);
            } else {
                imageOptionsGroup.Visibility = Visibility.Hidden;
                modelOptionsGroup.Visibility = Visibility.Hidden;
            }

            if (imageOptionsGroup.IsVisible) {
                if (cbImageChannels.SelectedIndex == 0) {
                    cbImageChannels.SelectedIndex = (int)((FDTextureFile)tmp).channel;
                }

                if (cbImageSizes.SelectedIndex == 0) {
                    cbImageSizes.SelectedIndex = (int)((FDTextureFile)tmp).channelType;
                }
            }

            cbType.SelectionChanged += cbType_SelectionChanged;
            cbImageChannels.SelectionChanged += cbImageChannels_SelectionChanged;
            cbImageSizes.SelectionChanged += cbImageSizes_SelectionChanged;

            UpdateIncludeAllCheckbox();
        }

        private void ckbIncluded_Checked(object sender, RoutedEventArgs e) {
            for (int i = 0; i < lvFiles.SelectedItems.Count; i++) {
                ((FDFile)lvFiles.SelectedItems[i]).included = true;
            }
        }

        private void ckbIncluded_Unchecked(object sender, RoutedEventArgs e) {
            for (int i = 0; i < lvFiles.SelectedItems.Count; i++) {
                ((FDFile)lvFiles.SelectedItems[i]).included = false;
            }
        }

        private void ckbItem_Checked(object sender, RoutedEventArgs e) {
            UpdateIncludeAllCheckbox();

            btnStart.IsEnabled = true;
        }

        private void ckbItem_Unchecked(object sender, RoutedEventArgs e) {
            UpdateIncludeAllCheckbox();

            foreach (FDFile f in files) {
                if (f.included == true) {
                    btnStart.IsEnabled = true;
                    return;
                }
            }

            btnStart.IsEnabled = false;
        }

        private void cbImageChannels_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            for (int i = 0; i < lvFiles.SelectedItems.Count; i++) {
                ((FDTextureFile)lvFiles.SelectedItems[i]).channel = (TextureChannel)cbImageChannels.SelectedIndex;
            }
        }

        private void cbImageSizes_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            for (int i = 0; i < lvFiles.SelectedItems.Count; i++) {
                ((FDTextureFile)lvFiles.SelectedItems[i]).channelType = (TextureChannelType)cbImageSizes.SelectedIndex;
            }
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e) {

        }

        private void btnStart_Click(object sender, RoutedEventArgs e) {
            foreach (FDFile file in files) {
                if (file.included == true) {
                    file.StartConversion(tvDstPath.Text);
                }
            }
        }
    }
}
