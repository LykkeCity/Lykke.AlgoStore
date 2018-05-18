using AutoFixture;
using Common.Log;
using Lykke.AlgoStore.AzureRepositories.Repositories;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Services;
using Lykke.AlgoStore.Tests.Infrastructure;
using Lykke.Service.PersonalData.Client;
using Lykke.Service.PersonalData.Contract;
using Lykke.Service.PersonalData.Settings;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Tests.Unit
{
    [TestFixture]
    public class AlgoStoreCommentsServiceTests
    {

        private readonly string AlgoId = Guid.NewGuid().ToString();
        private readonly string ClientId = Guid.NewGuid().ToString();

        [Test]
        public void GetAlgoComments()
        {
            var commentsRepo = Given_Correct_AlgoCommentsRepository();
            var commentsService = Given_Correct_AlgoStoreCommentsService(commentsRepo, null);

            var result = When_Invoke_GetAlgoCommentsAsync(commentsService, AlgoId, ClientId, out Exception ex);
            Then_Exception_Should_BeNull(ex);
            Then_Collection_Should_NotBeEmpty(result);

        }

        [Test]
        public void GetCommentById()
        {
            var commentsRepo = Given_Correct_AlgoCommentsRepository();
            var commentsService = Given_Correct_AlgoStoreCommentsService(commentsRepo, null);
            var commentId = Guid.NewGuid().ToString();

            var result = When_Invoke_GetCommentByIdAsync(commentsService, AlgoId, commentId, ClientId, out Exception ex);
            Then_Exception_Should_BeNull(ex);
            Then_Object_Should_NotBeNull(result);
        }

        [Test]
        public void SaveComment()
        {
            var commentsRepo = Given_Correct_AlgoCommentsRepository();
            var commentsService = Given_Correct_AlgoStoreCommentsService(commentsRepo, null);
            var data = Given_AlgoCommentDataObject();
            

            var result = When_Invoke_SaveCommentAsync(commentsService, data, out Exception ex);
            Then_Exception_Should_BeNull(ex);
            Then_Object_Should_NotBeNull(result);
        }

        private static IAlgoCommentsRepository Given_Correct_AlgoCommentsRepository()
        {
            var fixture = new Fixture();
            var result = new Mock<IAlgoCommentsRepository>();

            result.Setup(repo => repo.GetCommentsForAlgoAsync(It.IsAny<string>())).Returns((string algoId) =>
            {
                var comments = new List<AlgoCommentData>();
                comments.AddRange(fixture.Build<AlgoCommentData>().With(b => b.AlgoId, algoId).With(b => b.Author, null).CreateMany<AlgoCommentData>());
                return Task.FromResult(comments);
            });

            result.Setup(repo => repo.GetCommentByIdAsync(It.IsAny<string>(), It.IsAny<string>())).Returns((string algoId, string commentId) =>
            {
                var comment = fixture.Build<AlgoCommentData>()
                .With(data => data.CommentId, commentId)
                .With(data => data.AlgoId, algoId)
                .With(data => data.Author, null).Create();

                return Task.FromResult(comment);
            });

            result.Setup(repo => repo.SaveCommentAsync(It.IsAny<AlgoCommentData>())).Returns((AlgoCommentData data) => 
            {
                data.Author = null;
                return Task.FromResult(data);
            });

            result.Setup(repo => repo.DeleteCommentAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            return result.Object;
        }

        private static AlgoStoreCommentsService Given_Correct_AlgoStoreCommentsService(IAlgoCommentsRepository algoCommentsRepository,
            IPersonalDataService personalDataService)
        {
            return new AlgoStoreCommentsService(algoCommentsRepository, personalDataService, new LogMock());
        }
 
        private static AlgoCommentData Given_AlgoCommentDataObject()
        {
            var fixture = new Fixture();
            return fixture.Create<AlgoCommentData>();
        }

        private static List<AlgoCommentData> When_Invoke_GetAlgoCommentsAsync(IAlgoStoreCommentsService service, string algoId, string clientId, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetAlgoCommentsAsync(algoId, clientId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static AlgoCommentData When_Invoke_GetCommentByIdAsync(IAlgoStoreCommentsService service, string algoId, string commentId, string clientId, out Exception exception)
        {
            exception = null;
            try
            {
                return service.GetCommentByIdAsync(algoId, commentId, clientId).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static AlgoCommentData When_Invoke_SaveCommentAsync(IAlgoStoreCommentsService service, AlgoCommentData data, out Exception exception)
        {
            exception = null;
            try
            {
                return service.SaveCommentAsync(data).Result;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }

        private static void Then_Exception_Should_BeNull(Exception ex)
        {
            Assert.IsNull(ex);
        }

        private static void Then_Collection_Should_NotBeEmpty(List<AlgoCommentData> collection)
        {
            Assert.NotNull(collection);
            Assert.NotZero(collection.Count);
        }

        private static void Then_Object_Should_NotBeNull(AlgoCommentData data)
        {
            Assert.NotNull(data);
        }
    }
}
