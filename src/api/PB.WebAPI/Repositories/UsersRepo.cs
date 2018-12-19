using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Neo4j.Driver.V1;
using PB.WebAPI.Models;

namespace PB.WebAPI.Repositories
{
    public class UsersRepo : RepoBase<User>, IUsersRepo
    {
        private const string UserRecord = "user";

        public UsersRepo([NotNull]IDriver driver) : base(driver)
        {
        }

        public override async Task CreateAsync(User user)
        {
            using (var session = Driver.Session())
            {
                var result = await session.RunAsync(@"
CREATE (user:User) 
SET user.username = $Username,
user.hashedPassword = $HashedPassword,
user.firstName = $FirstName,
user.LastName = $LastName
RETURN id(user)", user);
                var record = await result.SingleAsync();
            }
        }

        public override async Task<User> ReadAsync(long id)
        {
            using (var session = Driver.Session())
            {
                var result = await session.RunAsync("MATCH(user:User) WHERE id(user) = $id RETURN user", new { id });
                var record = await result.SingleAsync();
                var user = RecordToModel(record, UserRecord);
                return user;
            }
        }

        public async Task<User> ReadAsync(string username)
        {
            using (var session = Driver.Session())
            {
                var result = await session.RunAsync("MATCH (user:User) WHERE user.username = $username RETURN user", new { username });
                var records = await result.ToListAsync();
                var record = records.SingleOrDefault();
                var user = RecordToModel(record, UserRecord);
                return user;
            }
        }

        public override async Task<IList<User>> ReadListAsync()
        {
            using (var session = Driver.Session())
            {
                var result = await session.RunAsync("MATCH (user:User) RETURN user");
                var list = await result.ToListAsync(r => RecordToModel(r, UserRecord));
                return list;
            }
        }

        public override async Task UpdateAsync(User user)
        {
            using (var session = Driver.Session())
            {
                var result = await session.RunAsync(@"
MATCH (user:User)
WHERE user.id = $Id
SET user.username = $Username,
user.hashedPassword = $HashedPassword,
user.firstName = $FirstName,
user.LastName = $LastName
RETURN id(user)", user);
                var record = await result.SingleAsync();
            }
        }

        public override async Task DeleteAsync(long id)
        {
            using (var session = Driver.Session())
            {
                var result = await session.RunAsync("MATCH(user:User) WHERE id(user) = $id DELETE user", new { id });
                await result.ConsumeAsync();
            }
        }
    }
}
