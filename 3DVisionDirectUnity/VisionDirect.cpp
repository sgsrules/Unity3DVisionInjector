#include <windows.h>
#include <d3d11.h>
#include <d3dcompiler.h>
#include "nvapi.h"
#include "nvapi_lite_stereo.h"
#include "Unity/IUnityGraphics.h"
#include "Unity/IUnityInterface.h"
#include "Unity/IUnityGraphicsD3D11.h"

static StereoHandle				g_StereoHandle;
static ID3D11Device*			g_pd3dDevice;
static IUnityInterfaces*		s_UnityInterfaces = NULL;

// Manually create stereo handle.  Needs to be called before any other NvAPI calls.
extern "C" INT UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API InitializeStereo()
{
	if (!s_UnityInterfaces)return -1;
	IUnityGraphicsD3D11* d3d = s_UnityInterfaces->Get<IUnityGraphicsD3D11>();
	if (!d3d)return -1;
	g_pd3dDevice = d3d->GetDevice();
	if (!g_pd3dDevice)return -1;
	NvAPI_Status status = NvAPI_Stereo_CreateHandleFromIUnknown(g_pd3dDevice, &g_StereoHandle);
	return status;
}

bool initialized= false;
void Initialize()
{
	if (initialized)return;
	if (!s_UnityInterfaces)return ;
	IUnityGraphicsD3D11* d3d = s_UnityInterfaces->Get<IUnityGraphicsD3D11>();
	if (!d3d)return ;
	g_pd3dDevice = d3d->GetDevice();
	if (!g_pd3dDevice)return;
	NvAPI_Status status = NvAPI_Stereo_CreateHandleFromIUnknown(g_pd3dDevice, &g_StereoHandle);
	if (status > -1)initialized = true;
}

extern "C" INT UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API  SetSeparation(float separation)
{
	Initialize();
	if (initialized)
	{
		float pConvergence;
		float pSeparationPercentage;
		float pEyeSeparation;

		NvAPI_Status	status = NvAPI_Stereo_SetSeparation(g_StereoHandle, separation);
		return status;
	}
	return -1;


}

extern "C" INT UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API  SetConvergence(float convergence)
{
	Initialize();
	if (initialized)
	{
		NvAPI_Status	status = NvAPI_Stereo_SetConvergence(g_StereoHandle, convergence);
		return status;
	}
	return -1;

}

extern "C" FLOAT UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetSeparation()
{
	Initialize();
	if (initialized)
	{
		float pConvergence;
		float pSeparationPercentage;
		float pEyeSeparation;

		NvAPI_Status	status = NvAPI_Stereo_GetConvergence(g_StereoHandle, &pConvergence);
		status = NvAPI_Stereo_GetSeparation(g_StereoHandle, &pSeparationPercentage);
		status = NvAPI_Stereo_GetEyeSeparation(g_StereoHandle, &pEyeSeparation);

		float separation = pEyeSeparation * pSeparationPercentage / 100;

		return separation;
	}
	return -1;

}
extern "C" FLOAT UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetConvergence()
{
	Initialize();
	if (initialized)
	{
		float pConvergence;
		float pSeparationPercentage;
		float pEyeSeparation;

		NvAPI_Status	status = NvAPI_Stereo_GetConvergence(g_StereoHandle, &pConvergence);
		status = NvAPI_Stereo_GetSeparation(g_StereoHandle, &pSeparationPercentage);
		status = NvAPI_Stereo_GetEyeSeparation(g_StereoHandle, &pEyeSeparation);

		float convergence = pEyeSeparation * pSeparationPercentage / 100 * pConvergence;

		return convergence;
	}
	return -1;

}


/*

extern "C" INT UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API ActivateLeftEye()
{
	Initialize();
	if (initialized)
	{
		NvAPI_Status status = NvAPI_Stereo_SetActiveEye(g_StereoHandle, NVAPI_STEREO_EYE_LEFT);
			return status;
	}
		return -1;

}

extern "C" INT UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API ActivateRightEye()
{
	Initialize();
	if (initialized)
	{
		NvAPI_Status status = NvAPI_Stereo_SetActiveEye(g_StereoHandle, NVAPI_STEREO_EYE_RIGHT);
			return status;
	}
	return -1;

}*/



static void UNITY_INTERFACE_API ActivateLeftEye(int eventID)
{
	Initialize();
	if (initialized)
	{
		NvAPI_Status status = NvAPI_Stereo_SetActiveEye(g_StereoHandle, NVAPI_STEREO_EYE_LEFT);
		//	return status;
	}
	//	return -1;

}

static void UNITY_INTERFACE_API  ActivateRightEye(int eventID)
{
	Initialize();
	if (initialized)
	{
		NvAPI_Status status = NvAPI_Stereo_SetActiveEye(g_StereoHandle, NVAPI_STEREO_EYE_RIGHT);
		//	return status;
	}
	//return -1;

}

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetLeftEye()
{
	return ActivateLeftEye;
}

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API  SetRightEye()
{
	return ActivateRightEye;
}


//Automatically called by Unity after plugin is loaded
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
{
	s_UnityInterfaces = unityInterfaces; // get reference to Unity interface
}

// TODO: Resource cleanup
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
{
	
}



