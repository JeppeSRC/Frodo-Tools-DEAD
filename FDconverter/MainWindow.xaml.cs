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

        public MainWindow() {
            InitializeComponent();

            files = new ObservableCollection<FDFile>();

            lvFiles.ItemsSource = files;
            
        }

        private void AddFiles(String[] files) {
            for (int i = 0; i < files.Length; i++) {
                try {
                    FileStream file = new FileStream(files[i], FileMode.Open);
                    this.files.Add(new FDFile(files[i], file.Length));
                    file.Close();
                } catch(Exception e) {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private void lvFiles_Drop(object sender, DragEventArgs e) {
            String[] s = (String[])e.Data.GetData(DataFormats.FileDrop);
            AddFiles(s);           
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

        private void lvFiles_MouseDown(object sender, MouseButtonEventArgs e) {

        }
    }
}
