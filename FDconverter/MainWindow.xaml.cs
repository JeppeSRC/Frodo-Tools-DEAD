using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            exePath = exePath.Remove(exePath.Length - 15, 15);

            FileType[] types = new FileType[3];

            types[(int)FileType.Unknown] = FileType.Unknown;
            types[(int)FileType.Image] = FileType.Image;
            types[(int)FileType.Model] = FileType.Model;

            cbType.ItemsSource = types;

            tvDstPath.Text = exePath + "ouput\\";
        }

        private bool AddFolder(string folder) {
            List<string> f = null;
            try {
                 f = new List<string>(Directory.EnumerateDirectories(folder));
            } catch (Exception e) {
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
                try {
                    FileStream file = new FileStream(files[i], FileMode.Open);
                    this.files.Add(new FDFile(files[i], file.Length));
                    file.Close();
                } catch(Exception e) {
                    if (AddFolder(files[i]) == false) MessageBox.Show(e.GetType().ToString(), string.Format("Exception Opening \"{0}\"", files[i]), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
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

        private void lvFiles_Drop(object sender, DragEventArgs e) {
            AddFiles((string[])e.Data.GetData(DataFormats.FileDrop));
        }

        private void lvFiles_KeyDown(object sender, KeyEventArgs e) { 
            if (e.Key == Key.Delete) {
                var tmp = lvFiles.SelectedItems;

                List<int> indices = new List<int>();

                for (int i = 0; i < tmp.Count; i++) {
                    for (int j = 0; j < files.Count; j++) {
                        if (tmp[i] == files[j]) indices.Add(j);
                    }
                }

                Sort(ref indices);

                for (int i = 0; i < indices.Count; i++) {
                    files.RemoveAt(indices[i]);
                }
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
            var tmp = lvFiles.SelectedItems;

            List<int> indices = new List<int>();

            for (int i = 0; i < tmp.Count; i++) {
                for (int j = 0; j < files.Count; j++) {
                    if (tmp[i] == files[j]) indices.Add(j);
                }
            }

            Sort(ref indices);

            for (int i = 0; i < indices.Count; i++) {
                files.RemoveAt(indices[i]);
            }
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
            for (int i = 0; i < lvFiles.SelectedItems.Count; i++) {
                ((FDFile)lvFiles.SelectedItems[i]).type = (FileType)cbType.SelectedIndex;
            }

            SetOptionsGroupVisibility((FileType)cbType.SelectedIndex);
        }

        private void lvFiles_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            FDFile tmp = (FDFile)lvFiles.SelectedItems[0];

            cbType.SelectionChanged -= cbType_SelectionChanged;

            cbType.SelectedIndex = 0;

            for (int i = 1; i < lvFiles.SelectedItems.Count; i++) {
                FDFile file = (FDFile)lvFiles.SelectedItems[i];
                if (tmp.type != file.type) {
                    cbType.SelectedIndex = -1;
                }
            }

            if (cbType.SelectedIndex == 0) {
                cbType.SelectedIndex = (int)tmp.type;
                SetOptionsGroupVisibility(tmp.type);
            } else {
                imageOptionsGroup.Visibility = Visibility.Hidden;
                modelOptionsGroup.Visibility = Visibility.Hidden;
            }

            cbType.SelectionChanged += cbType_SelectionChanged;

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
        }

        private void ckbItem_Unchecked(object sender, RoutedEventArgs e) {
            UpdateIncludeAllCheckbox();
        }
    }
}
