using System.Collections.Generic;
using Newtonsoft.Json;

namespace UsersLoaderPlugin.DTO
{
    public class DummyUsersResponse
    {
        [JsonProperty("users")]
        public IReadOnlyList<DummyUser> Users { get; set; }
    }
}
