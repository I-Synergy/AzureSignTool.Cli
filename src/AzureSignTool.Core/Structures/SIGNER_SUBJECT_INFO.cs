using AzureSignTool.Core.Enumerations;
using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Structures;

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
