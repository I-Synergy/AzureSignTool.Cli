using AzureSignTool.Core.Enumerations;
using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Structures;

[type: StructLayout(LayoutKind.Sequential)]
internal readonly struct SIGNER_CERT_STORE_INFO(IntPtr pSigningCert, SignerCertStoreInfoFlags dwCertPolicy, IntPtr hCertStore)
{
    public readonly uint cbSize = (uint)Marshal.SizeOf<SIGNER_CERT_STORE_INFO>();
    public readonly IntPtr pSigningCert = pSigningCert;
    public readonly SignerCertStoreInfoFlags dwCertPolicy = dwCertPolicy;
    public readonly IntPtr hCertStore = hCertStore;
}
