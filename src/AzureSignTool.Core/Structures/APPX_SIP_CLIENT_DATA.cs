using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Structures;

[type: StructLayout(LayoutKind.Sequential)]
internal struct APPX_SIP_CLIENT_DATA
{
    public unsafe SIGNER_SIGN_EX3_PARAMS* pSignerParams;
    public IntPtr pAppxSipState;

}