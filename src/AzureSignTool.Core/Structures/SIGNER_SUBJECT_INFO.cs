using AzureSignTool.Core.Enumerations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Structures;

[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Name mirrors the native Win32 structure.")]
[SuppressMessage("Major Vulnerability", "S6640:Using unsafe code blocks is security-sensitive", Justification = "Required to marshal the native Win32 structure.")]
[type: StructLayout(LayoutKind.Sequential)]
internal readonly unsafe struct SIGNER_SUBJECT_INFO
{
    public readonly uint cbSize;
    public readonly uint* pdwIndex;
    public readonly SignerSubjectInfoUnionChoice dwSubjectChoice;
    public readonly SIGNER_SUBJECT_INFO_UNION unionInfo;

    public SIGNER_SUBJECT_INFO(uint* pdwIndex, SignerSubjectInfoUnionChoice dwSubjectChoice, SIGNER_SUBJECT_INFO_UNION unionInfo)
    {
        cbSize = (uint)Marshal.SizeOf<SIGNER_SUBJECT_INFO>();
        this.pdwIndex = pdwIndex;
        this.dwSubjectChoice = dwSubjectChoice;
        this.unionInfo = unionInfo;
    }
}
