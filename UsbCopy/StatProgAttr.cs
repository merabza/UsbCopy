using SystemToolsShared;

namespace UsbCopy;

public static class StatProgAttr
{
    public static void SetAttr()
    {
        ProgramAttributes.Instance.SetAttribute("AppName", "UsbCopy");
        ProgramAttributes.Instance.SetAttribute("AppKey", "DFD53CBF-F62F-4795-80DA-F4AD7ECC4AD3");
    }
}