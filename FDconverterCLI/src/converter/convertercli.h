#pragma once
#include <string>
#include <converter/converter.h>

namespace FDconverterCLI {

public enum class TextureChannel {
	R,
	RG,
	RGB,
	RGBA,

	NUM
};

public enum class TextureChannelType {
	Uint8,
	/*Uint16,
	Uint32,
	Float32*/

	NUM
};


public delegate void FPProgressCallBack(int);

public ref class FDConverter {
public:
	static bool ConvertImage(System::String^ path, System::String^ outPath, TextureChannel channel, TextureChannelType type, FPProgressCallBack^ callback);

};

std::string system_string_to_string(System::String^ string);
System::String^ string_to_system_string(std::string& string);

}