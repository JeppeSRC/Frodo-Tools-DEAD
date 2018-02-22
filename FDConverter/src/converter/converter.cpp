#include "converter.h"
#include "../lib/lz4.h"

#include <stdio.h>
#include <string>
#include <Windows.h>

#define STB_IMAGE_IMPLEMENTATION
#include "../lib/stb_image.h"

namespace FD {

std::string get_file_name(const char* path) {
	uint32 slashIndex = 0;
	uint_t pathLen = strlen(path);

	for (uint32 i = 0; i < pathLen; i++) {
		if (path[i] == '/' || path[i] == '\\') {
			slashIndex = i;
		}
	}

	if (path[slashIndex] == '/' || path[slashIndex] == '\\') slashIndex++;

	return std::string(path + slashIndex);
}

byte get_channel_size(TextureChannelType type) {
	switch (type) {
		case TextureChannelType::Uint8:
			return sizeof(byte);
	}

	return 0;
}

byte get_num_channels(TextureChannel channel) {
	switch (channel) {
		case TextureChannel::R:
			return 1;
		case TextureChannel::RG:
			return 2;
		case TextureChannel::RGBA:
			return 4;
	}

	return 0;
}

void ConvertImage(const char* path, const char* outPath, TextureChannel channel, TextureChannelType type, FPProgressCallBack callback) {
	FILE* file = fopen(path, "rb");

	if (file == nullptr) {
		printf("Failed to open file \"%s\"", path);
		return;
	}

	std::string tmp(std::string(outPath) + get_file_name(path));

	FILE* newFile = fopen(tmp.c_str(), "wb");

	if (newFile == nullptr) {
		printf("Failed to create destination file \"%s\"\n", tmp.c_str());
		return;
	}

	int32 width = 0;
	int32 height = 0;
	int32 c = 0;

	byte* imageData = stbi_load_from_file(file, &width, &height, &c, 4);

	fclose(file);

	byte numChannels = get_num_channels(channel);
	byte channelSize = get_channel_size(type);

	Header header;

	header.version = FD_HEADER_VERSION;
	header.type = FD_HEADER_TYPE_TEXTURE;
	header.size = sizeof(Header) + sizeof(TextureHeader) + (width * height * numChannels * channelSize);

	TextureHeader texHeader;

	texHeader.version = FD_TEXTURE_HEADER_VERSION;
	texHeader.pixelLayout = channel;
	texHeader.pixelChannelSize = type;
	texHeader.width = width;
	texHeader.height = height;

	fwrite(&header, sizeof(Header), 1, newFile);
	fwrite(&texHeader, sizeof(TextureHeader), 1, newFile);

	uint32 pixelsSize = width * height * numChannels * channelSize;

	byte* pixels = new byte[pixelsSize];

	if (type == TextureChannelType::Uint8) {
		for (uint32 i = 0; i < (uint32)(width * height); i++) {
			for (byte c = 0; c < numChannels; c++) {
				pixels[i * numChannels + c] = imageData[i * 4 + c];
			}

			int32 a = ((int32)(((float32)i / (width * height)) * 100.0f));

			if (a % 10 == 0) {
				callback(a);
			}
		}
	}

	free(imageData);

	uint32 bufferSize = LZ4_compressBound(pixelsSize);

	byte* data = new byte[bufferSize];

	bufferSize = LZ4_compress_default((const char*)pixels, (char*)data, pixelsSize, bufferSize);

	bufferSize = LZ4_compress_default((const char*)data, (char*)pixels, bufferSize, LZ4_compressBound(bufferSize));
	
	delete[] data;

	fwrite(pixels, bufferSize, 1, newFile);
	fclose(newFile);

	delete[] pixels;

	printf("Compression Ratio: \"%s\" -> %u%%\n", tmp.c_str(), (uint32)((((float32)bufferSize) / pixelsSize) * 100.0f));

	callback(100);
}

}

