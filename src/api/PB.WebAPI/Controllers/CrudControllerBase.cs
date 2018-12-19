using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PB.WebAPI.Models;
using PB.WebAPI.Repositories;

namespace PB.WebAPI.Controllers
{
    public abstract class CrudControllerBase<TModel> : ControllerBase
        where TModel : IDbModel, new()
    {
        protected IRepo<TModel> Repo{ get; }

        public CrudControllerBase(IRepo<TModel> repo)
        {
            Repo = repo;
        }

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TModel>>> Get()
        {
            var models = await Repo.ReadListAsync();
            return Ok(models);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TModel>> Get(long id)
        {
            var model = await Repo.ReadAsync(id);
            return Ok(model);
        }

        [HttpPost]
        public virtual async Task<ActionResult> Post([FromBody] TModel value)
        {
            await Repo.CreateAsync(value);
            return Ok();
        }

        [HttpPut("{id}")]
        public virtual async Task<ActionResult> Put(long id, [FromBody] TModel value)
        {
            value.Id = id;
            await Repo.UpdateAsync(value);
            return Ok();
        }

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult> Delete(long id)
        {
            await Repo.DeleteAsync(id);
            return Ok();
        }
    }
}
