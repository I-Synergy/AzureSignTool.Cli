using System.Runtime.InteropServices;

namespace AzureSignTool.Core.Structures;

[type: StructLayout(LayoutKind.Explicit)]
internal readonly unsafe struct SIGNER_SIGNATURE_INFO_UNION
{
    public SIGNER_SIGNATURE_INFO_UNION(SIGNER_ATTR_AUTHCODE* pAttrAuthcode)
    {
        this.pAttrAuthcode = pAttrAuthcode;
    }

    [field: FieldOffset(0)]
    public readonly SIGNER_ATTR_AUTHCODE* pAttrAuthcode;
}
