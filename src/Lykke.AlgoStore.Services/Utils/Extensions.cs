using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.Service.CandlesHistory.Client.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Rest;
using Candle = Lykke.AlgoStore.Algo.Candle;

namespace Lykke.AlgoStore.Services.Utils
{
    public static class Extensions
    {
        public static T ParseHttpResponse<T>(this HttpOperationResponse<object> httpOperation, ModelStateDictionary errorsDictionary)
        {
            using (var response = httpOperation)
            {
                if (response.Body is ErrorResponse || !response.Response.IsSuccessStatusCode)
                {
                    var errors = response.Body as ErrorResponse;
                    foreach (var error in errors?.ErrorMessages ?? new ConcurrentDictionary<string, IList<string>>())
                    {
                        foreach (var message in error.Value)
                        {
                            errorsDictionary.AddModelError(error.Key, message);
                        }
                    }

                    return default(T);
                }

                return (T)response.Body;
            }
        }

        public static Candle ToAlgoCandle(this Lykke.Service.CandlesHistory.Client.Models.Candle serviceCandle)
        {
            return new Candle
            {
                 DateTime = serviceCandle.DateTime,
                 Close = serviceCandle.Close,
                 High = serviceCandle.High,
                 Low = serviceCandle.Low,
                 Open = serviceCandle.Open
            };
        }

        public static CandleChartingUpdate ToCandleChartingUpdate(this Candle algoCandle)
        {
            return new CandleChartingUpdate
            {
                DateTime = algoCandle.DateTime,
                Close = algoCandle.Close,
                High = algoCandle.High,
                Low = algoCandle.Low,
                Open = algoCandle.Open
            };
        }
    }
}
