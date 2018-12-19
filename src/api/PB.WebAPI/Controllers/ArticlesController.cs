using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PB.WebAPI.Models;
using PB.WebAPI.Repositories;

namespace PB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ArticlesController : CrudControllerBase<Article>
    {
        public ArticlesController(IArticlesRepo articlesRepo) : base(articlesRepo)
        {
        }
    }
}
