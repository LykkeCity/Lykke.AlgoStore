﻿using System;
using Autofac;
using Common.Log;

namespace Lykke.AlgoStore.Client
{
    public static class AutofacExtension
    {
        public static void RegisterLykkeServiceClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterInstance(new LykkeServiceClient(serviceUrl, log)).As<ILykkeServiceClient>().SingleInstance();
        }
    }
}
