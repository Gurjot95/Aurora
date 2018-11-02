#pragma once

#include "CgSDK.x64_2015Global.h"
#include <cstdint>

#ifdef __cplusplus
extern "C"
{
#endif

	enum CorsairAccessMode		// contains list of available SDK access modes
	{
		CAM_ExclusiveLightingControl = 0
	};

	enum CorsairError			// contains shared list of all errors which could happen during calling of Corsair* functions
	{
		CE_Success,						// if previously called function completed successfully
		CE_ServerNotFound,				// CUE is not running or was shut down or third-party control is disabled in CUE settings(runtime error)
		CE_NoControl,					// if some other client has or took over exclusive control (runtime error)
		CE_ProtocolHandshakeMissing,	// if developer did not perform protocol handshake(developer error)
		CE_IncompatibleProtocol,		// if developer is calling the function that is not supported by the server(either because protocol has broken by server or client or because the function is new and server is too old. Check CorsairProtocolDetails for details) (developer error)
		CE_InvalidArguments,			// if developer supplied invalid arguments to the function(for specifics look at function descriptions). (developer error)
	};

	struct CorsairProtocolDetails // contains information about SDK and CUE versions
	{
		const char* sdkVersion;			// null - terminated string containing version of SDK(like “1.0.0.1”). Always contains valid value even if there was no CUE found
		const char* serverVersion;		// null - terminated string containing version of CUE(like “1.0.0.1”) or NULL if CUE was not found.
		int sdkProtocolVersion;			// integer number that specifies version of protocol that is implemented by current SDK. Numbering starts from 1. Always contains valid value even if there was no CUE found
		int serverProtocolVersion;		// integer number that specifies version of protocol that is implemented by CUE. Numbering starts from 1. If CUE was not found then this value will be 0
		bool breakingChanges;			// boolean value that specifies if there were breaking changes between version of protocol implemented by server and client
	};

	//  requestes control using specified access mode. By default client has shared control over lighting so there is no need to call CorsairRequestControl unless client requires exclusive control
	CORSAIR_GAME_SDK bool CgSdkRequestControl(CorsairAccessMode accessMode);

	// checks file and protocol version of CUE to understand which of SDK functions can be used with this version of CUE
	CORSAIR_GAME_SDK CorsairProtocolDetails CgSdkPerformProtocolHandshake();

	// returns last error that occured while using any of Corsair* functions
	CORSAIR_GAME_SDK CorsairError CgSdkGetLastError();

	//releases previously requested control for specified access mode
	CORSAIR_GAME_SDK bool CgSdkReleaseControl(CorsairAccessMode accessMode);


	CORSAIR_GAME_SDK bool CgSdkSetGame();

	CORSAIR_GAME_SDK bool CgSdkSetState();

	CORSAIR_GAME_SDK bool CgSdkSetStateWithKey();

	CORSAIR_GAME_SDK bool CgSdkSetEvent();

	CORSAIR_GAME_SDK bool CgSdkSetEventWithKey();

	CORSAIR_GAME_SDK bool CgSdkSetProgressBarValue();

	CORSAIR_GAME_SDK bool CgSdkShowProgressBar();

	CORSAIR_GAME_SDK bool CgSdkHideProgressBar();

	CORSAIR_GAME_SDK bool CgSdkClearState();

	CORSAIR_GAME_SDK bool CgSdkClearStateWithKey();

	CORSAIR_GAME_SDK bool CgSdkClearAllStates();

	CORSAIR_GAME_SDK bool CgSdkClearAllEvents();

#ifdef __cplusplus
} //exten "C"
#endif