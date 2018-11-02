#pragma once

#ifndef _LIB
#   if defined(_WIN32) || defined(WIN32)
#		if defined(CORSAIR_GAME_SDK_DLL) || defined(CGSDK_EXPORTS)
#           define CORSAIR_GAME_SDK __declspec(dllexport)
#       else
#           define CORSAIR_GAME_SDK __declspec(dllexport)
#       endif
#   else
#       define CORSAIR_GAME_SDK __attribute__((visibility("default")))
#   endif // WIN32
#else
#   define CORSAIR_GAME_SDK
#endif // !_LIB