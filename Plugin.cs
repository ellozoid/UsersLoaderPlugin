using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;
using PhoneApp.Domain.Attributes;
using PhoneApp.Domain.DTO;
using PhoneApp.Domain.Interfaces;
using UsersLoaderPlugin.DTO;

namespace UsersLoaderPlugin
{
    [Author(Name = "Bogdan G")]
    public class Plugin : IPluggable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> args)
        {
            logger.Info("Loading users from API");
            var result = args?.Cast<EmployeesDTO>().ToList() ?? new List<EmployeesDTO>();
            try
            {
                string baseUrl = ConfigurationManager.AppSettings["UsersApiUrl"] ?? "https://dummyjson.com/users";
                int limit = 10;
                int.TryParse(ConfigurationManager.AppSettings["UsersLimit"], out limit);

                var fetchedUsers = new List<EmployeesDTO>();
                using (var client = new HttpClient())
                {
                    int skip = 0;
                    const int batchSize = 5;

                    while (fetchedUsers.Count < limit)
                    {
                        int take = Math.Min(batchSize, limit - fetchedUsers.Count);
                        var json = client
                          .GetStringAsync($"{baseUrl}?limit={take}&skip={skip}")
                          .GetAwaiter()
                          .GetResult();

                        var response = JsonConvert.DeserializeObject<DummyUsersResponse>(json);

                        foreach (var user in response.Users)
                        {
                            EmployeesDTO dto = CreateEmployee(user);
                            fetchedUsers.Add(dto);

                            if (fetchedUsers.Count >= limit)
                            {
                                break;
                            }
                        }

                        if (response.Users.Count < take)
                        {
                            break;
                        }

                        skip += take;
                    }
                }
                result.AddRange(fetchedUsers);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Trace(ex.StackTrace);
                return result.Cast<DataTransferObject>();
            }

            logger.Info($"Loaded {result.Count} users");
            return result.Cast<DataTransferObject>();
        }

        private static EmployeesDTO CreateEmployee(DummyUser user)
        {
            var dto = new EmployeesDTO { Name = $"{user.FirstName} {user.LastName}" };
            dto.AddPhone(user.Phone);
            return dto;
        }
    }
}
