//Created by StatProgramAttrClassCreator at 6/28/2021 12:52:58

using SystemToolsShared;

namespace UsbCopy;

public sealed class StatProgramAttr
{
    public static void SetAttr()
    {
        ProgramAttributes.Instance.SetAttribute("AppName", "Usb Copy");
    }
}