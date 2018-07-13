using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class TeamCityWebHookResponse
    {
        public DateTime BuildStartTime { get; set; }

        public DateTime Timestamp { get; set; }

        //REMARK: This property exist ONLY for these TC build events: Successful, Fixed, Failed, Broken
        //and does not exist for these TC build events: Started, Changes Loaded, Interrupted, Almost Completed, Responsibility Changed
        public DateTime? BuildFinishTime { get; set; }

        public string BuildEvent { get; set; }

        public string BuildName { get; set; }

        public string BuildStatusUrl { get; set; }

        public int BuildNumber { get; set; }

        public string TriggeredBy { get; set; }

        public string BuildResult { get; set; }

        public string BuildResultPrevious { get; set; }

        public string BuildResultDelta { get; set; }
    }
}
