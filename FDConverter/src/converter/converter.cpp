#include "converter.h"

#include <stdio.h>
#include <string>
#include <Windows.h>

namespace FD {

std::string get_file_name(const char* path) {

}

void ConvertImage(const char* path, const char* outPath, TextureChannel channel, TextureChannelType type, FPProgressCallBack callback) {
	FILE* file = fopen(path, "rb");

	std::string tmp(std::string(outPath) + get_file_name(path));

	FILE* newFile = fopen(tmp.c_str(), "wb");


}

}

