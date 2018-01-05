using System;
using System.Linq;
using System.Text;
using AzureStorage.Blob;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Repositories;
using NUnit.Framework;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class AlgoBlobRepositoryTests
    {
        private static readonly string BlobKey = "TestKey";
        private static readonly byte[] BlobBytes = Encoding.Unicode.GetBytes(BlobKey);

        [Test, Explicit("Should run manually only. Manipulate data in Table Storage")]
        public void Blob_Large_Binary_Save_Test()
        {
            var repo = Given_Algo_RealBlob_Starage_Repository();
            var largeByteArray = Give_Large_Byte_Array();
            When_Invoke_Save_BinaryFile(repo, BlobKey, largeByteArray);
            Then_BinaryFile_ShouldBe(repo, BlobKey, largeByteArray);
            Then_DeleteBinary(repo, BlobKey);
        }

        [Test]
        public void BlobBinary_Save_Test()
        {
            var repo = Given_AlgoBinary_InMemory_Storage_Repository();
            When_Invoke_Save_BinaryFile(repo, BlobKey, BlobBytes);
            Then_BinaryFile_ShouldBe(repo, BlobKey, BlobBytes);
            And_BinaryFileToString_ShouldBe(repo, BlobKey);
        }

        [Test]
        public void BlobBinary_Exists_Test()
        {
            var repo = Given_AlgoBinary_InMemory_Storage_Repository();
            When_Invoke_Save_BinaryFile(repo, BlobKey, BlobBytes);
            Then_BinaryFile_ShouldBe(repo, BlobKey, BlobBytes);
        }

        [Test]
        public void BlobBinary_Delete_Test()
        {
            var repo = Given_AlgoBinary_InMemory_Storage_Repository();
            When_Invoke_Save_BinaryFile(repo, BlobKey, BlobBytes);
            And_TryDelete_BinaryFile(repo, BlobKey);
            Then_BinaryFile_ShouldNotExist(repo, BlobKey);
        }

        [Test]
        public void BlobString_Save_Test()
        {
            string data = "Test string";
            var repo = Given_AlgoBinary_InMemory_Storage_Repository();
            When_Invoke_Save_String(repo, BlobKey, data);
            Then_String_ShouldBe(repo, BlobKey, data);
        }

        #region Private Methods

        private static void Then_DeleteBinary(IAlgoBlobRepository repo, string blobKey)
        {
            repo.DeleteBlobAsync(blobKey).Wait();
            Assert.False(repo.BlobExists(blobKey).Result);
        }

        private static byte[] Give_Large_Byte_Array()
        {
            Random rnd = new Random();
            Byte[] b = new Byte[100000000];
            rnd.NextBytes(b);
            return b;
        }

        private static void Then_BinaryFile_ShouldNotExist(IAlgoBlobRepository repo, string blobKey)
        {
            var exists = repo.BlobExists(blobKey).Result;
            Assert.False(exists);
        }

        private static IAlgoBlobRepository Given_AlgoBinary_InMemory_Storage_Repository()
        {
            return new AlgoBlobRepository(new AzureBlobInMemory());
        }

        private static IAlgoBlobRepository Given_Algo_RealBlob_Starage_Repository()
        {
            return new AlgoBlobRepository(new AzureBlobInMemory());
        }

        private static void When_Invoke_Save_BinaryFile(IAlgoBlobRepository repository, string key, byte[] bytes)
        {
            repository.SaveBlobAsync(key, bytes).Wait();
        }

        private static void And_TryDelete_BinaryFile(IAlgoBlobRepository repository, string key)
        {
            repository.DeleteBlobAsync(key).Wait();
        }

        private static void Then_BinaryFile_ShouldBe(IAlgoBlobRepository repository, string key, byte[] bytes)
        {
            var saved = repository.GetBlobAsync(key).Result;
            Assert.True(saved.SequenceEqual(bytes));
        }

        private static void And_BinaryFileToString_ShouldBe(IAlgoBlobRepository repository, string key)
        {
            var saved = repository.GetBlobAsync(key).Result;
            Assert.True(Encoding.Unicode.GetString(saved) == key);
        }

        private static void When_Invoke_Save_String(IAlgoBlobRepository repository, string key, string data)
        {
            repository.SaveBlobAsync(key, data).Wait();
        }
        private static void Then_String_ShouldBe(IAlgoBlobRepository repository, string key, string data)
        {
            var saved = repository.GetBlobStringAsync(key).Result;
            Assert.AreEqual(data, saved);
        }

        #endregion
    }
}
