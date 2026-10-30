using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Structures;

[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Name mirrors the native Win32 structure.")]
[SuppressMessage("Major Vulnerability", "S6640:Using unsafe code blocks is security-sensitive", Justification = "Required to marshal the native Win32 structure.")]
[type: StructLayout(LayoutKind.Sequential)]
internal readonly unsafe struct SIGNER_FILE_INFO
{
    public readonly uint cbSize;
    public readonly char* pwszFileName;
    public readonly IntPtr hFile;

    public SIGNER_FILE_INFO(char* pwszFileName, IntPtr hFile)
    {
        cbSize = (uint)Marshal.SizeOf<SIGNER_FILE_INFO>();
        this.pwszFileName = pwszFileName;
        this.hFile = hFile;
    }
}
