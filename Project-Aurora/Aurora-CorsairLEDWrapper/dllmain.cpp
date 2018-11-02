// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include "CgSDK.x64_2015.h"
#include <stdio.h>
#include <string>
#include <iomanip>
#include <fstream>

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

void write_text_to_log_file(const std::string &text)
{

	std::ofstream out("output.txt", std::ios_base::app);
	out << text;
	out.close();

}

CORSAIR_GAME_SDK bool CgSdkRequestControl(CorsairAccessMode accessMode)
{
	write_text_to_log_file("\nRequest");
	return true;
}

// checks file and protocol version of CUE to understand which of SDK functions can be used with this version of CUE
CORSAIR_GAME_SDK CorsairProtocolDetails CgSdkPerformProtocolHandshake()
{
	CorsairProtocolDetails details;
	details.sdkVersion = "3.0.165";
	details.sdkProtocolVersion = 1;
	details.serverProtocolVersion = 1;
	details.serverVersion = "3.7.99";
	details.breakingChanges = false;
	write_text_to_log_file("\nhanshake");
	return details;
}

// returns last error that occured while using any of Corsair* functions
CORSAIR_GAME_SDK CorsairError CgSdkGetLastError()
{
	write_text_to_log_file("error");
	return CE_InvalidArguments;
}

//releases previously requested control for specified access mode
CORSAIR_GAME_SDK bool CgSdkReleaseControl(CorsairAccessMode accessMode)
{
	write_text_to_log_file("\nrelease");
	return true;
}


CORSAIR_GAME_SDK bool CgSdkSetGame(char *gamename)
{
	write_text_to_log_file("\nSet Game: "); 
	write_text_to_log_file(gamename);
	return true;
}

CORSAIR_GAME_SDK bool CgSdkSetState(char *gamename)
{
	write_text_to_log_file("\nSet State"); write_text_to_log_file(gamename); return true;
}

CORSAIR_GAME_SDK bool CgSdkSetStateWithKey()
{
	write_text_to_log_file("\nstatekey"); return true;
}

CORSAIR_GAME_SDK bool CgSdkSetEvent(char *gamename)
{
	write_text_to_log_file("\nsetevent"); write_text_to_log_file(gamename); return true;
}

CORSAIR_GAME_SDK bool CgSdkSetEventWithKey()
{
	write_text_to_log_file("\neventkey"); return true;
}

CORSAIR_GAME_SDK bool CgSdkSetProgressBarValue()
{
	write_text_to_log_file("\nsetProgress"); return true;
}

CORSAIR_GAME_SDK bool CgSdkShowProgressBar()
{
	write_text_to_log_file("\nshowProgress"); return true;
}

CORSAIR_GAME_SDK bool CgSdkHideProgressBar()
{
	write_text_to_log_file("\ntest"); return true;
}

CORSAIR_GAME_SDK bool CgSdkClearState()
{
	write_text_to_log_file("\nclearstate"); return true;
}

CORSAIR_GAME_SDK bool CgSdkClearStateWithKey()
{
	write_text_to_log_file("\nclearstatekey"); return true;
}

CORSAIR_GAME_SDK bool CgSdkClearAllStates()
{
	write_text_to_log_file("\nclearall"); return true;
}

CORSAIR_GAME_SDK bool CgSdkClearAllEvents()
{
	write_text_to_log_file("\nclearallevents"); return true;
}

