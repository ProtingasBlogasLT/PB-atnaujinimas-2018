using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using NUnit.Framework;
using PB.WebAPI.Controllers;
using PB.WebAPI.Models;
using PB.WebAPI.Repositories;

namespace PB.WebAPI.Tests.Controllers
{
    [TestFixture]
    class ArticlesControllerTes
    {
        private IList<Article> Articles { get; set; }
        private Mock<IArticlesRepo> RepoMock { get; set; }
        private ArticlesController Controller { get; set; }

        [SetUp]
        public void SetUp()
        {
            Articles = new[] { new Article { Id = 1 }, new Article { Id = 2 } };
            RepoMock = new Mock<IArticlesRepo>();
            Controller = new ArticlesController(RepoMock.Object);
        }

        private T GetResultAndAssert<T>(ActionResult<T> result)
            where T : class
        {
            var innerResult = result.Result;
            var okResult = innerResult as OkObjectResult;
            Assert.That(okResult != null);
            var resultValue = okResult.Value as T;
            Assert.That(resultValue != null);
            return resultValue;
        }

        [Test]
        public void Get_ReadsList()
        {
            RepoMock.Setup(r => r.ReadListAsync()).Returns(Task.FromResult(Articles));

            var result = Controller.Get().Result;

            RepoMock.Verify(r => r.ReadListAsync(), Times.Once);
            var resultArticles = GetResultAndAssert(result);
            Assert.That(resultArticles.SequenceEqual(Articles));
        }

        [TestCase(1)]
        [TestCase(2)]
        public void Get_Reads(long id)
        {
            var article = Articles.Single(a => a.Id == id);
            RepoMock.Setup(r => r.ReadAsync(id)).Returns(Task.FromResult(article));

            var result = Controller.Get(id).Result;

            RepoMock.Verify(r => r.ReadAsync(id), Times.Once);
            var resultArticle = GetResultAndAssert(result);
            Assert.That(resultArticle == article);
        }

        [Test]
        public void Post_Creates()
        {
            var article = Articles[0];
            RepoMock.Setup(r => r.CreateAsync(article)).Returns(Task.CompletedTask);

            var result = Controller.Post(article).Result;

            Assert.That(result is OkResult);
            RepoMock.Verify(r => r.CreateAsync(article), Times.Once);
        }

        [Test]
        public void Put_Updates()
        {
            var article = Articles[0];
            RepoMock.Setup(r => r.UpdateAsync(article)).Returns(Task.CompletedTask);

            var result = Controller.Put(article.Id, article).Result;

            Assert.That(result is OkResult);
            RepoMock.Verify(r => r.UpdateAsync(article), Times.Once);
        }

        [Test]
        public void Delete_Deletes()
        {
            var article = Articles[0];
            RepoMock.Setup(r => r.DeleteAsync(article.Id)).Returns(Task.CompletedTask);

            var result = Controller.Delete(article.Id).Result;

            Assert.That(result is OkResult);
            RepoMock.Verify(r => r.DeleteAsync(article.Id), Times.Once);
        }
    }
}
