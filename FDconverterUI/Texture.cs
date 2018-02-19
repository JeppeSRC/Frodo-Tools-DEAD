using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FDconverterCLI;

namespace FDconverter {

    class FDTextureFile : FDFile {

        private TextureChannel _channel;
        private TextureChannelType _type;

        public TextureChannel channel { get { return _channel; } set { _channel = value; OnPropertyChanged(); } }
        public TextureChannelType channelType{ get { return _type; } set { _type = value; OnPropertyChanged(); } }

        public FDTextureFile(FDFile file) : base(file, FileType.Image) {
            channel = TextureChannel.RGBA;
            channelType = TextureChannelType.Uint8;
        }

        public FDTextureFile(string path) : base(path, FileType.Image) {
            channel = TextureChannel.RGBA;
            channelType = TextureChannelType.Uint8;
        }

        private void ProgressCallback(int progress) {
            this.progress = progress;
        }

        public override void StartConversion(string outPath) {
            base.StartConversion(outPath);
            _converterThread = new Thread(new ParameterizedThreadStart(ThreadMethod));
            _converterThread.Start(outPath);
        }

        private void ThreadMethod(Object obj) {

            string outPath = (string)obj;

            FDConverter.ConvertImage(path, outPath, channel, channelType, new FPProgressCallBack(ProgressCallback));

            EndOfConversion();
        }

    }
}
