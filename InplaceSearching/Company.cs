

using MongoDB.Bson;
using Realms;

namespace InplaceSearching
{
    public class Company: RealmObject
    {

        [PrimaryKey]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public string Name { get; set; }
    }
}
