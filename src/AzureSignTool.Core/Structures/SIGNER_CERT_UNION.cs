using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Structures;

[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Name mirrors the native Win32 structure.")]
[SuppressMessage("Major Vulnerability", "S6640:Using unsafe code blocks is security-sensitive", Justification = "Required to marshal the native Win32 structure.")]
[type: StructLayout(LayoutKind.Explicit)]
internal readonly unsafe struct SIGNER_CERT_UNION
{
    public SIGNER_CERT_UNION(SIGNER_CERT_STORE_INFO* certStoreInfo)
    {
        pSpcChainInfo = certStoreInfo;
    }

    [field: FieldOffset(0)]
    public readonly SIGNER_CERT_STORE_INFO* pSpcChainInfo;
}
