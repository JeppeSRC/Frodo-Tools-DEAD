#pragma once

#include "formats.h"

namespace FD {

typedef void(__stdcall* FPProgressCallBack)(int);

void __stdcall ConvertImage(const char* path, const char* outPath, TextureChannel channel, TextureChannelType type, FPProgressCallBack callback);

}