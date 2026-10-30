using AzureSignTool.Core.Enumerations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Structures;

[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Name mirrors the native Win32 structure.")]
[type: StructLayout(LayoutKind.Sequential)]
internal readonly struct SIGNER_CERT
{
    public readonly uint cbSize;
    public readonly SignerCertChoice dwCertChoice;
    public readonly SIGNER_CERT_UNION union;
    public readonly IntPtr hwnd;

    public SIGNER_CERT(SignerCertChoice dwCertChoice, SIGNER_CERT_UNION union)
    {
        this.dwCertChoice = dwCertChoice;
        this.union = union;
        hwnd = default;
        cbSize = (uint)Marshal.SizeOf<SIGNER_CERT>();
    }
}
