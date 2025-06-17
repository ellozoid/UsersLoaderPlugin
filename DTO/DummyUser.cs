using Newtonsoft.Json;

namespace UsersLoaderPlugin.DTO
{
    public class DummyUser
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}
