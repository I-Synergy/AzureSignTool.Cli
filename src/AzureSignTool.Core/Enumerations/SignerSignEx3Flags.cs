using System.Diagnostics.CodeAnalysis;

namespace AzureSignTool.Core.Enumerations;

[SuppressMessage("Minor Code Smell", "S2344:Enumeration type names should not have \"Flags\" or \"Enum\" suffixes", Justification = "Name mirrors the native SIGNER_SIGN_EX3 flags.")]
[SuppressMessage("Critical Code Smell", "S2346:Flags enumerations zero-valued members should be named \"None\"", Justification = "NONE mirrors the native Win32 constant.")]
[type: Flags]
internal enum SignerSignEx3Flags : uint
{
    NONE = 0x0,
    SPC_EXC_PE_PAGE_HASHES_FLAG = 0x010,
    SPC_INC_PE_IMPORT_ADDR_TABLE_FLAG = 0x020,
    SPC_INC_PE_DEBUG_INFO_FLAG = 0x040,
    SPC_INC_PE_RESOURCES_FLAG = 0x080,
    SPC_INC_PE_PAGE_HASHES_FLAG = 0x100,
    SIGN_CALLBACK_UNDOCUMENTED = 0X400,
    SIG_APPEND = 0x1000
}
