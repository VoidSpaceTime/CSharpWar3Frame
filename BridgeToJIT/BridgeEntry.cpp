#include <windows.h>

using namespace System;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Runtime::Loader;

extern "C" IMAGE_DOS_HEADER __ImageBase;

#pragma managed(push, off)
static bool TryGetBridgeDirectory(wchar_t* buffer, DWORD length)
{
    DWORD written = 0;
    DWORD i = 0;

    if (buffer == NULL || length == 0)
    {
        return false;
    }

    written = GetModuleFileNameW(reinterpret_cast<HMODULE>(&__ImageBase), buffer, length);
    if (written == 0 || written >= length)
    {
        return false;
    }

    for (i = written; i > 0; --i)
    {
        if (buffer[i - 1] == L'\\' || buffer[i - 1] == L'/')
        {
            buffer[i - 1] = L'\0';
            return true;
        }
    }

    return false;
}
#pragma managed(pop)

ref class PayloadLoadContext sealed : AssemblyLoadContext
{
private:
    AssemblyDependencyResolver^ _resolver;

public:
    PayloadLoadContext(String^ componentPath)
        : AssemblyLoadContext("ProjectPayload", false)
    {
        _resolver = gcnew AssemblyDependencyResolver(componentPath);
    }

protected:
    virtual Assembly^ Load(AssemblyName^ assemblyName) override
    {
        String^ assemblyPath = _resolver->ResolveAssemblyToPath(assemblyName);
        if (String::IsNullOrEmpty(assemblyPath))
        {
            return nullptr;
        }

        return LoadFromAssemblyPath(assemblyPath);
    }

    virtual IntPtr LoadUnmanagedDll(String^ unmanagedDllName) override
    {
        String^ dllPath = _resolver->ResolveUnmanagedDllToPath(unmanagedDllName);
        if (String::IsNullOrEmpty(dllPath))
        {
            return IntPtr::Zero;
        }

        return LoadUnmanagedDllFromPath(dllPath);
    }
};

static String^ GetBridgeDirectory()
{
    wchar_t pathBuffer[MAX_PATH] = {};
    if (!TryGetBridgeDirectory(pathBuffer, MAX_PATH))
    {
        return nullptr;
    }

    return gcnew String(pathBuffer);
}

extern "C" __declspec(dllexport) int main()
{
    try
    {
        String^ bridgeDirectory = GetBridgeDirectory();
        if (String::IsNullOrEmpty(bridgeDirectory))
        {
            return -1;
        }

        String^ payloadPath = Path::Combine(bridgeDirectory, "project.dll");
        if (!File::Exists(payloadPath))
        {
            return 0;
        }

        auto loadContext = gcnew PayloadLoadContext(payloadPath);
        Assembly^ payloadAssembly = loadContext->LoadFromAssemblyPath(payloadPath);
        Type^ entryType = payloadAssembly->GetType("War3Frame.Game", false);
        if (entryType == nullptr)
        {
            return -12;
        }

        MethodInfo^ method = entryType->GetMethod("BridgeMain", BindingFlags::Public | BindingFlags::Static);
        if (method == nullptr)
        {
            return -11;
        }

        Object^ result = method->Invoke(nullptr, nullptr);
        if (result == nullptr)
        {
            return 0;
        }

        return safe_cast<int>(result);
    }
    catch (Exception^)
    {
        return -100;
    }
}
