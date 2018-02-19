#include "convertercli.h"
#include <converter/converter.h>

#include <msclr/marshal_cppstd.h>

using namespace System;
using namespace System::Runtime::InteropServices;


typedef void(__stdcall *FP)(const char*, const char*, FD::TextureChannel, FD::TextureChannelType, FDconverterCLI::FPProgressCallBack^);

namespace FDconverterCLI {

bool FDConverter::ConvertImage(System::String^ path, System::String^ outPath, TextureChannel channel, TextureChannelType type, FPProgressCallBack^ callback) {
		
	FP fp = (FP)FD::ConvertImage;

	fp(system_string_to_string(path).c_str(), system_string_to_string(outPath).c_str(), (FD::TextureChannel)channel, (FD::TextureChannelType)type, callback);
	
	return false;
}

std::string system_string_to_string(System::String^ string) {
	return msclr::interop::marshal_as<std::string>(string);
}

System::String^ string_to_system_string(std::string string) {
	return gcnew System::String(string.c_str());
}

}