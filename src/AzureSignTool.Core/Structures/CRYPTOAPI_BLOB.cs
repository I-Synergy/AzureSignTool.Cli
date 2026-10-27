using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Structures;

[type: StructLayout(LayoutKind.Sequential)]
internal struct CRYPTOAPI_BLOB
{
    public uint cbData;
    public IntPtr pbData;
}
