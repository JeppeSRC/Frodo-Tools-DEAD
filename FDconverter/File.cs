using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace FDconverter {

    public enum FileType  {
        Unknown,
        Image,
        Model
    };

    public class FDFile : INotifyPropertyChanged {

        private struct FileExtension {
            public FileExtension(string extension, FileType type) { this.extension = extension; this.type = type; }
            public string extension { get; }
            public FileType type { get; }
        };

        private static FileExtension[] extensions = { new FileExtension(".png", FileType.Image),
                                                      new FileExtension(".jpg", FileType.Image),
                                                      new FileExtension(".bmp", FileType.Image)};

        private static FileType GetFileTypeFromExtension(String filename) {

            for (uint i = 0; i < extensions.Length; i++) {
                if (filename.EndsWith(extensions[i].extension)) {
                    return extensions[i].type;
                }
            }

            return FileType.Unknown;
        }

        private int _progress;
        private FileType _type;
        private bool _included;

        public string path { get; }
        public long size { get; }
        public string sizeString { get; set; }
        public FileType type { get { return _type; } set { _type = value; OnPropertyChanged(); } }
        public int progress { get { return _progress; } set { _progress = value; OnPropertyChanged(); } }
        public bool included { get { return _included; } set { _included = value; OnPropertyChanged(); Console.WriteLine("SA"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public FDFile(string path, long size) {
            this.path = path;
            this.size = size;
            this.type = GetFileTypeFromExtension(path);
            progress = 0;
            included = true;

            if (size >= 1024 * 1024 * 1024) {
                sizeString = string.Format("{0:F2} GB", size / (1024.0f * 1024.0f * 1024.0f));
            } else if (size >= 1024 * 1024) {
                sizeString = string.Format("{0:F2} MB", size / (1024.0f * 1024.0f));
            } else if (size >= 1024) {
                sizeString = string.Format("{0:F2} KB", size / (1024.0f));
            } else {
                sizeString = string.Format("{0:F2} Bytes", size);
            }
        }

       protected virtual void OnPropertyChanged([CallerMemberName] string name = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
       }

    }
}
