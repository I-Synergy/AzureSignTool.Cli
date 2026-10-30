using System.Diagnostics.CodeAnalysis;

namespace AzureSignTool.Core.Enumerations;

[SuppressMessage("Minor Code Smell", "S2344:Enumeration type names should not have \"Flags\" or \"Enum\" suffixes", Justification = "Name mirrors the native SIGNER_CERT_STORE_INFO flags.")]
[type: Flags]
internal enum SignerCertStoreInfoFlags
{

    SIGNER_CERT_POLICY_CHAIN = 0x02,
    SIGNER_CERT_POLICY_CHAIN_NO_ROOT = 0x08,
    SIGNER_CERT_POLICY_STORE = 0x01
}
