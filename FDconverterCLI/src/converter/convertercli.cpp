#include "convertercli.h"
#include <converter/converter.h>

using namespace System;
using namespace System::Runtime::InteropServices;


typedef void(__stdcall *FP)(const char*, const char*, FD::TextureChannel, FD::TextureChannelType, FDconverterCLI::FPProgressCallBack^);

namespace FDconverterCLI {


bool FDConverter::ConvertImage(System::String^ path, System::String^ outPath, TextureChannel channel, TextureChannelType type, FPProgressCallBack^ callback) {
		
	FP fp = (FP)FD::ConvertImage;

	fp(0, 0, FD::TextureChannel::R, FD::TextureChannelType::Uint8, callback);
	
	return false;
}

}