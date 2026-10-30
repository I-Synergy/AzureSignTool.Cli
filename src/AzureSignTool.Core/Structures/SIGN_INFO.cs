using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Structures;

[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Name mirrors the native Win32 structure.")]
[type: StructLayout(LayoutKind.Sequential)]
internal readonly struct SIGN_INFO
{
    public readonly uint cbSize;

    public readonly IntPtr callback;

    public readonly IntPtr pvOpaque;

    public SIGN_INFO(IntPtr callback)
    {
        cbSize = (uint)Marshal.SizeOf<SIGN_INFO>();
        this.callback = callback;
        pvOpaque = default;
    }
}
