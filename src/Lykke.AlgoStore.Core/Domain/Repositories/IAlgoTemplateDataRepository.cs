using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Core.Domain.Entities;

namespace Lykke.AlgoStore.Core.Domain.Repositories
{
    public interface IAlgoTemplateDataRepository
    {
        Task<List<AlgoTemplateData>> GetTemplatesByLanguage(string languageId);
    }
}
