using System;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Api.RealTimeStreaming.Sources
{
    public abstract class RealTimeDataSource<T>
    {
        public abstract void Subscribe(Func<T, Task> handler);
        public abstract void Unsubscribe(Func<T, Task> handler);
    }
}
