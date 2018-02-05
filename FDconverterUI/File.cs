using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;


namespace FDconverter {

    public enum FileType  {
        Unknown,
        Image,
        Model
    };

    public class FDFile : INotifyPropertyChanged {

        public static FDFile CreateFile(string path) {
            FileType type = GetFileTypeFromExtension(path);

            switch(type) {
                case FileType.Image:
                    return new FDTextureFile(path);
                case FileType.Model:
                    break;
            }

            return new FDFile(path, FileType.Unknown);
        }

        private struct FileExtension {
            public FileExtension(string extension, FileType type) { this.extension = extension; this.type = type; }
            public string extension { get; }
            public FileType type { get; }
        };

        private static FileExtension[] extensions = { new FileExtension(".png", FileType.Image),
                                                      new FileExtension(".jpg", FileType.Image),
                                                      new FileExtension(".bmp", FileType.Image),
                                                      new FileExtension(".obj", FileType.Model)};

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
        private bool? _included;

        public string path { get; }
        public long size { get; }
        public string sizeString { get; set; }
        public FileType type { get { return _type; } set { _type = value; OnPropertyChanged(); } }
        public int progress { get { return _progress; } set { _progress = value; OnPropertyChanged(); } }
        public bool? included { get { return _included; } set { _included = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected FileStream file;

        protected FDFile(FDFile other, FileType type) {
            file = other.file;
            path = other.path;
            size = other.size;
            this.type = type;
            progress = -1;
            included = other.included;
            sizeString = other.sizeString;
        } 

        protected FDFile(string path, FileType type) {
            file = new FileStream(path, FileMode.Open);
            this.path = path;
            this.size = file.Length;
            this.type = type;
            progress = -1;
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
