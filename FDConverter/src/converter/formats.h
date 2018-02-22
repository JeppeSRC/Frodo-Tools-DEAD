#pragma once

#include "../types.h"


#define FD_HEADER_TYPE_TEXTURE 0x01
#define FD_HEADER_TYPE_MODEL   0x02

#define FD_HEADER_VERSION 0x0001

#define FD_TEXTURE_HEADER_VERSION 0x0001

namespace FD {

enum class TextureChannel : byte {
	R,
	RG,
	//RGB,
	RGBA,

	NUM
};

enum class TextureChannelType : byte {
	Uint8,
	/*Uint16,
	Uint32,
	Float32*/

	NUM
};

}

struct Header {
	uint16 version;
	byte type;
	uint64 size;
	uint32 compressedSize[2];
};

struct TextureHeader {
	uint16 version;
	FD::TextureChannel pixelLayout;
	FD::TextureChannelType pixelChannelSize;
	uint16 width;
	uint16 height;
};