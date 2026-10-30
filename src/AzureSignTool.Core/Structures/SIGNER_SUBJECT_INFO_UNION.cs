using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Structures;

[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Name mirrors the native Win32 structure.")]
[SuppressMessage("Major Vulnerability", "S6640:Using unsafe code blocks is security-sensitive", Justification = "Required to marshal the native Win32 structure.")]
[type: StructLayout(LayoutKind.Explicit)]
internal readonly unsafe struct SIGNER_SUBJECT_INFO_UNION
{
    [FieldOffset(0)]
    public readonly SIGNER_FILE_INFO* file;

    public SIGNER_SUBJECT_INFO_UNION(SIGNER_FILE_INFO* file)
    {
        this.file = file;
    }
}
