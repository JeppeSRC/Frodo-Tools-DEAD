#pragma once

#include "../types.h"

#define FD_HEADER_TYPE_TEXTURE 0x01
#define FD_HEADER_TYPE_MODEL   0x02

#define FD_HEADER_VERSION 0x0001

#define FD_TEXTURE_HEADER_VERSION 0x0001

struct Header {
	uint16 version;
	byte type;
	uint64 size;
};

struct TextureHeader {
	uint16 version;
	byte pixelLayout;
	byte pixelSize;
	uint16 width;
	uint16 height;
};
