using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDconverter {

    public enum TextureChannel {
        R,
        RG,
        RGB,
        RGBA,
    };

    public enum TextureChannelType {
        Uint8,
        Uint16,
        Uint32,
        Float32
    };

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

    }
}
