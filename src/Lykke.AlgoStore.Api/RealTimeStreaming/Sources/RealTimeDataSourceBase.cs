using System;
using System.Threading;
using Lykke.AlgoStore.Api.RealTimeStreaming.Filters;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.Sources
{
    public abstract class RealTimeDataSourceBase<T> : IObservable<T>
    {
        protected DataFilter Filter;
        public readonly CancellationTokenSource TokenSource = new CancellationTokenSource();
        public abstract IDisposable Subscribe(IObserver<T> observer);

        public void SupplyDataFilter(DataFilter filter)
        {
            Filter = filter;
        }

    }
}
