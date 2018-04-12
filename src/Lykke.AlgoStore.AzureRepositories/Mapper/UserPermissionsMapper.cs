﻿using Lykke.AlgoStore.AzureRepositories.Entities;
using Lykke.AlgoStore.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.AzureRepositories.Mapper
{
    public static class UserPermissionsMapper
    {
        public static UserPermissionData ToModel(this UserPermissionEntity entity)
        {
            var result = new UserPermissionData()
            {
                Id = entity.PartitionKey,
                Name = entity.RowKey,
                DisplayName = entity.DisplayName
            };

            return result;
        }

        public static List<UserPermissionData> ToModel(this List<UserPermissionEntity> entities)
        {
            var result = new List<UserPermissionData>();

            foreach (var entity in entities)
            {
                var data = new UserPermissionData()
                {
                    Id = entity.PartitionKey,
                    Name = entity.RowKey,
                    DisplayName = entity.DisplayName
                };

                result.Add(data);
            }

            return result;
        }

        public static UserPermissionEntity ToEntity(this UserPermissionData data)
        {
            var result = new UserPermissionEntity()
            {
                PartitionKey = data.Id,
                RowKey = data.Name,
                DisplayName = data.DisplayName,
                ETag = "*"
            };

            return result;
        }
    }
}
