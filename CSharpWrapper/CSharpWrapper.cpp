#using <mscorlib.dll>

#include <msclr/marshal_cppstd.h>
#include <iostream>

using namespace System;
using namespace System::Reflection;
using namespace System::IO;
using namespace msclr::interop;

extern "C" __declspec(dllexport)
int main() {
	//return War3Frame::Program::MainCLR();

// 确保开启 /clr 并且运行时能在输出目录找到 War3Frame.dll
    String^ baseDir = AppDomain::CurrentDomain->BaseDirectory;
    String^ dllPath = Path::Combine(baseDir, "War3Frame.dll");

    if (!File::Exists(dllPath))
    {
        std::cerr << "未找到 War3Frame.dll，路径: " << marshal_as<std::string>(dllPath) << std::endl;
        return -1;
    }

    // 运行时加载程序集，避免在编译期引入其所有元数据，防止命名空间冲突（C2757）
    Assembly^ asmObj = Assembly::LoadFrom(dllPath);
    if (asmObj == nullptr)
    {
        std::cerr << "Assembly::LoadFrom 返回 null" << std::endl;
        return -1;
    }

    Type^ progType = asmObj->GetType("War3Frame.Program");
    if (progType == nullptr)
    {
        std::cerr << "未能找到类型 War3Frame.Program" << std::endl;
        return -1;
    }

    MethodInfo^ mi = progType->GetMethod("MainCLR", BindingFlags::Public | BindingFlags::Static);
    if (mi == nullptr)
    {
        std::cerr << "未能找到静态方法 MainCLR" << std::endl;
        return -1;
    }

    // 调用并返回结果（假定返回 int）
    Object^ ret = mi->Invoke(nullptr, nullptr);
    return safe_cast<int>(ret);

}