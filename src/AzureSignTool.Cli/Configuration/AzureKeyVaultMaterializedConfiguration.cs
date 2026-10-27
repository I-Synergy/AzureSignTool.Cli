using Azure.Core;
using System.Security.Cryptography.X509Certificates;

namespace AzureSignTool.Cli.Configuration;

public sealed record AzureKeyVaultMaterializedConfiguration(TokenCredential TokenCredential, X509Certificate2 PublicCertificate, Uri KeyId);

