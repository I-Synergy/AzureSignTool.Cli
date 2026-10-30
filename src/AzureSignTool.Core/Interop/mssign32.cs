using AzureSignTool.Core.Enumerations;
using AzureSignTool.Core.Structures;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Interop;

[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Name mirrors the native mssign32 DLL.")]
[SuppressMessage("Major Vulnerability", "S6640:Using unsafe code blocks is security-sensitive", Justification = "Required to marshal the native SignerSignEx3 signature.")]
internal static partial class mssign32
{
    [LibraryImport(nameof(mssign32), EntryPoint = "SignerSignEx3")]
    public static unsafe partial int SignerSignEx3
    (
        SignerSignEx3Flags dwFlags,
        SIGNER_SUBJECT_INFO* pSubjectInfo,
        SIGNER_CERT* pSignerCert,
        SIGNER_SIGNATURE_INFO* pSignatureInfo,
        IntPtr pProviderInfo,
        SignerSignTimeStampFlags dwTimestampFlags,
        byte* pszTimestampAlgorithmOid,
        char* pwszHttpTimeStamp,
        IntPtr psRequest,
        void* pSipData,
        IntPtr* ppSignerContext,
        IntPtr pCryptoPolicy,
        SIGN_INFO* pSignInfo,
        IntPtr pReserved
    );

    [LibraryImport(nameof(mssign32))]
    public static partial int SignerFreeSignerContext(
        IntPtr pSignerContext
    );
}
