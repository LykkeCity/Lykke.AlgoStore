using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class UserRoleMatchMapper
    {
        public static UserRoleMatchData ToModel(this UserRoleMatchEntity entity)
        {
            var result = new UserRoleMatchData()
            {
                ClientId = entity.PartitionKey,
                RoleId = entity.RowKey
            };

            return result;
        }

        public static List<UserRoleMatchData> ToModel(this List<UserRoleMatchEntity> entities)
        {
            var result = new List<UserRoleMatchData>();

            foreach (var entity in entities)
            {
                var data = new UserRoleMatchData()
                {
                    ClientId = entity.PartitionKey,
                    RoleId = entity.RowKey
                };

                result.Add(data);
            }

            return result;
        }

        public static UserRoleMatchEntity ToEntity(this UserRoleMatchData data)
        {
            var result = new UserRoleMatchEntity()
            {
                PartitionKey = data.ClientId,
                RowKey = data.RoleId,
                ETag = "*"
            };

            return result;
        }
    }
}
