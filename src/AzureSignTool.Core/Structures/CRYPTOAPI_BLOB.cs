using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Structures;

[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Name mirrors the native Win32 structure.")]
[type: StructLayout(LayoutKind.Sequential)]
internal struct CRYPTOAPI_BLOB
{
    public uint cbData;
    public IntPtr pbData;
}
