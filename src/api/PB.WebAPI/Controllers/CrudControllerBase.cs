using System;
using System.Collections.Generic;
using System.Linq;
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
        public virtual async Task<ActionResult<IEnumerable<Article>>> Get()
        {
            var models = await Repo.ReadListAsync();
            return Ok(models);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public virtual async Task<ActionResult<Article>> Get(long id)
        {
            var model = await Repo.ReadAsync(id);
            return Ok(model);
        }

        // POST api/values
        [HttpPost]
        public virtual async Task<ActionResult> Post([FromBody] TModel value)
        {
            await Repo.CreateAsync(value);
            return Ok();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public virtual async Task<ActionResult> Put(long id, [FromBody] TModel value)
        {
            value.Id = id;
            await Repo.UpdateAsync(value);
            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public virtual async Task<ActionResult> Delete(long id)
        {
            await Repo.DeleteAsync(id);
            return Ok();
        }
    }
}
