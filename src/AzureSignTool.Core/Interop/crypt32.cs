using AzureSignTool.Core.Enumerations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Interop;

[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Name mirrors the native crypt32 DLL.")]
internal static partial class crypt32
{
    [LibraryImport(nameof(crypt32), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CertCloseStore
    (
        IntPtr hCertStore,
        CertCloreStoreFlags dwFlags
    );

    [LibraryImport(nameof(crypt32), SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr CertOpenStore
    (
        string lpszStoreProvider,
        CertEncodingType CertEncodingType,
        IntPtr hCryptProv,
        CertOpenStoreFlags dwFlags,
        IntPtr pvPara
    );
}
