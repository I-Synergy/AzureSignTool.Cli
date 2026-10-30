using System.Diagnostics.CodeAnalysis;

namespace AzureSignTool.Core.Enumerations;

[SuppressMessage("Minor Code Smell", "S2344:Enumeration type names should not have \"Flags\" or \"Enum\" suffixes", Justification = "Name mirrors the native SIGNER_TIMESTAMP flags.")]
internal enum SignerSignTimeStampFlags : uint
{
    SIGNER_TIMESTAMP_AUTHENTICODE = 1,
    SIGNER_TIMESTAMP_RFC3161 = 2,
}
