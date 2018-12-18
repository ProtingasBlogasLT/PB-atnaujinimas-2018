using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver.V1;
using PB.WebAPI.Models;

namespace PB.WebAPI.Repositories
{
    public abstract class RepoBase<TModel> : IRepo<TModel>
        where TModel : IDbModel, new()
    {
        protected IDriver Driver { get; }

        public RepoBase(IDriver driver)
        {
            Driver = driver;
        }

        protected TModel RecordToModel(IRecord record, string name)
        {
            if (record == null)
            {
                return default(TModel);
            }
            var node = record[name].As<INode>();
            var model = node.ConvertTo<TModel>();
            return model;
        }

        public abstract Task CreateAsync(TModel article);
        public abstract Task<TModel> ReadAsync(long id);
        public abstract Task<IList<TModel>> ReadListAsync();
        public abstract Task UpdateAsync(TModel model);
        public abstract Task DeleteAsync(long model);
    }
}
