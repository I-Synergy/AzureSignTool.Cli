using AzureSignTool.Core.Enumerations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Structures;

[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Name mirrors the native Win32 structure.")]
[SuppressMessage("Major Vulnerability", "S6640:Using unsafe code blocks is security-sensitive", Justification = "Required to marshal the native Win32 structure.")]
[type: StructLayout(LayoutKind.Sequential)]
internal unsafe struct SIGNER_SIGN_EX3_PARAMS
{
    public SignerSignEx3Flags dwFlags;
    public SIGNER_SUBJECT_INFO* pSubjectInfo;
    public SIGNER_CERT* pSignerCert;
    public SIGNER_SIGNATURE_INFO* pSignatureInfo;
    public IntPtr pProviderInfo;
    public SignerSignTimeStampFlags dwTimestampFlags;
    public byte* pszTimestampAlgorithmOid;
    public char* pwszHttpTimeStamp;
    public IntPtr psRequest;
    public SIGN_INFO* pSignCallBack;
    public IntPtr* ppSignerContext;
    public IntPtr pCryptoPolicy;
    public IntPtr pReserved;
}
