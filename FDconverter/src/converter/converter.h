#pragma once


namespace FD {

enum class TextureChannel {
	R,
	RG,
	RGB,
	RGBA,
};

enum class TextureChannelType {
	Uint8,
	Uint16,
	Uint32,
	Float32
};

typedef void(__stdcall* FPProgressCallBack)(int);

void __stdcall ConvertImage(const char* path, const char* outPath, TextureChannel channel, TextureChannelType type, FPProgressCallBack callback);

}