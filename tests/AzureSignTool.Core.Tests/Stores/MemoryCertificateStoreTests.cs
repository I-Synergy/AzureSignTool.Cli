using AzureSignTool.Core.Stores;
using System.Security.Cryptography.X509Certificates;

namespace AzureSignTool.Core.Tests.Stores;

[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class MemoryCertificateStoreTests
{
    [TestMethod]
    public void ShouldCreateAndDisposeAMemoryCertificateStore()
    {
        var store = MemoryCertificateStore.Create();
        Assert.AreNotEqual(IntPtr.Zero, store.Handle);
        Assert.IsEmpty(store.Certificates);
        store.Close();
    }

    [TestMethod]
    public void MultipleCloseOrDisposeCallsShouldNotError()
    {
        var store = MemoryCertificateStore.Create();
        store.Close();
        store.Close();
    }

    [TestMethod]
    public void ShouldAddCertificate()
    {
        using (var store = MemoryCertificateStore.Create())
        {
            using (var cert = X509CertificateLoader.LoadCertificateFromFile("certificates/test/test_cert.cer"))
            {
                Assert.IsEmpty(store.Certificates);
                store.Add(cert);
                Assert.IsNotEmpty(store.Certificates);
            }
        }
    }
}
