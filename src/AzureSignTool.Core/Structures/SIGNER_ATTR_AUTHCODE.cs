using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Structures;

[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Name mirrors the native Win32 structure.")]
[SuppressMessage("Major Vulnerability", "S6640:Using unsafe code blocks is security-sensitive", Justification = "Required to marshal the native Win32 structure.")]
[type: StructLayout(LayoutKind.Sequential)]
internal readonly unsafe struct SIGNER_ATTR_AUTHCODE
{
    public readonly uint cbSize;
    public readonly uint fCommercial;
    public readonly uint fIndividual;

    public readonly char* pwszName;
    public readonly char* pwszInfo;

    public SIGNER_ATTR_AUTHCODE(char* pwszName, char* pwszInfo)
    {
        cbSize = (uint)Marshal.SizeOf<SIGNER_ATTR_AUTHCODE>();
        fCommercial = 0;
        fIndividual = 0;
        this.pwszName = pwszName;
        this.pwszInfo = pwszInfo;
    }
}
