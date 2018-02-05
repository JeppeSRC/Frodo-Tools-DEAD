#pragma once

#include <converter/converter.h>

namespace FDconverterCLI {

public enum class TextureChannel {
	R,
	RG,
	RGB,
	RGBA,
};

public enum class TextureChannelType {
	Uint8,
	Uint16,
	Uint32,
	Float32
};


public delegate void FPProgressCallBack(int);

public ref class FDConverter {
public:
	static bool ConvertImage(System::String^ path, System::String^ outPath, TextureChannel channel, TextureChannelType type, FPProgressCallBack^ callback);

};

}