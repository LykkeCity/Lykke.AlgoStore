using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class RolePermissionMatchMapper
    {
        public static RolePermissionMatchData ToModel(this RolePermissionMatchEntity entity)
        {
            var result = new RolePermissionMatchData()
            {
                RoleId = entity.PartitionKey,
                PermissionId = entity.RowKey
            };

            return result;
        }

        public static List<RolePermissionMatchData> ToModel(this List<RolePermissionMatchEntity> entities)
        {
            var result = new List<RolePermissionMatchData>();

            foreach (var entity in entities)
            {
                var data = new RolePermissionMatchData()
                {
                    RoleId = entity.PartitionKey,
                    PermissionId = entity.RowKey
                };

                result.Add(data);
            }

            return result;
        }

        public static RolePermissionMatchEntity ToEntity(this RolePermissionMatchData data)
        {
            var result = new RolePermissionMatchEntity()
            {
                PartitionKey = data.RoleId,
                RowKey = data.PermissionId,
                ETag = "*"
            };

            return result;
        }
    }
}
