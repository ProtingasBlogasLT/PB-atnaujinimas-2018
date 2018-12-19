using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Neo4j.Driver.V1;
using PB.WebAPI.Models;

namespace PB.WebAPI.Repositories
{
    public class ArticlesRepo : RepoBase<Article>, IArticlesRepo
    {
        private const string ArticleRecord = "article";

        public ArticlesRepo([NotNull]IDriver driver) : base(driver)
        {
        }

        public override async Task CreateAsync(Article article)
        {
            using (var session = Driver.Session())
            {
                var result = await session.RunAsync("CREATE (article:Article) SET article.name = $Name, article.text = $Text RETURN id(article)", article);
                var record = await result.SingleAsync();
            }
        }

        public override async Task<Article> ReadAsync(long id)
        {
            using (var session = Driver.Session())
            {
                var result = await session.RunAsync("MATCH(article) WHERE id(article) = $id RETURN article", new { id });
                var record = await result.SingleAsync();
                var article = RecordToModel(record, ArticleRecord);
                return article;
            }
        }

        public override async Task<IList<Article>> ReadListAsync()
        {
            using (var session = Driver.Session())
            {
                var result = await session.RunAsync("MATCH (article:Article) RETURN article");
                var list = await result.ToListAsync(r => RecordToModel(r, ArticleRecord));
                return list;
            }
        }

        public override async Task UpdateAsync(Article article)
        {
            using (var session = Driver.Session())
            {
                var result = await session.RunAsync("MATCH (article:Article) WHERE id(article) = $Id SET article.name = $Name, article.text = $Text", article);
                var record = await result.SingleAsync();
            }
        }

        public override async Task DeleteAsync(long id)
        {
            using (var session = Driver.Session())
            {
                var result = await session.RunAsync("MATCH(article:Article) WHERE id(article) = $id DELETE article", new { id });
                var record = await result.SingleAsync();
                var article = RecordToModel(record, ArticleRecord);
            }
        }
    }
}
