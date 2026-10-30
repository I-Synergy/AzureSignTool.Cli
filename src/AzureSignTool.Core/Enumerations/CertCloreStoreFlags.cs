using System.Diagnostics.CodeAnalysis;

namespace AzureSignTool.Core.Enumerations;

[SuppressMessage("Minor Code Smell", "S2344:Enumeration type names should not have \"Flags\" or \"Enum\" suffixes", Justification = "Name mirrors the native CERT_CLOSE_STORE flags.")]
[SuppressMessage("Critical Code Smell", "S2346:Flags enumerations zero-valued members should be named \"None\"", Justification = "NONE mirrors the native Win32 constant.")]
[type: Flags]
internal enum CertCloreStoreFlags : uint
{
    NONE = 0,
    CERT_CLOSE_STORE_FORCE_FLAG = 0x00000001,
    CERT_CLOSE_STORE_CHECK_FLAG = 0x00000002,
}
