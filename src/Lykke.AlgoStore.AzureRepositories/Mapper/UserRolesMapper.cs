using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class UserRolesMapper
    {
        public static UserRoleData ToModel(this UserRoleEntity entity)
        {
            var result = new UserRoleData()
            {
                Id = entity.PartitionKey,
                Name = entity.RowKey,
                CanBeDeleted = entity.CanBeDeleted,
                CanBeModified = entity.CanBeModified
            };

            return result;
        }

        public static List<UserRoleData> ToModel(this List<UserRoleEntity> entities)
        {
            var result = new List<UserRoleData>();

            foreach (var entity in entities)
            {
                var data = new UserRoleData()
                {
                    Id = entity.PartitionKey,
                    Name = entity.RowKey,
                    CanBeDeleted = entity.CanBeDeleted,
                    CanBeModified = entity.CanBeModified
                };

                result.Add(data);
            }

            return result;
        }

        public static UserRoleEntity ToEntity(this UserRoleData data)
        {
            var result = new UserRoleEntity()
            {
                PartitionKey = data.Id,
                RowKey = data.Name,
                ETag = "*",
                CanBeDeleted = data.CanBeDeleted,
                CanBeModified = data.CanBeModified
            };

            return result;
        }
    }
}
