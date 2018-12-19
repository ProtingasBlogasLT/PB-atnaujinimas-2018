using Neo4j.Driver.V1;
using Newtonsoft.Json;
using PB.WebAPI.Models;

namespace PB.WebAPI.ExtensionMethods
{
    public static class NodeExtensionMethods
    {
        public static T ConvertTo<T>(this INode node)
            where T : IDbModel, new()
        {
            var serialized = JsonConvert.SerializeObject(node.Properties);
            var deserialized = JsonConvert.DeserializeObject<T>(serialized);
            deserialized.Id = node.Id;
            return deserialized;
        }
    }
}
