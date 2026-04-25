// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"

// 缓存当前 BridgeToJIT.dll 的模块句柄。
// 注意：这里只做被动记录，不在 DllMain 中执行任何托管启动逻辑。
HMODULE g_bridgeModule = nullptr;

BOOL APIENTRY DllMain(HMODULE hModule,
                      DWORD ul_reason_for_call,
                      LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        g_bridgeModule = hModule;
        break;

    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }

    return TRUE;
}

