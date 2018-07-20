using System;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.Api.Models
{
    public class TeamCityWebHookResponseModel
    {
        [JsonProperty("buildId")]
        public int BuildId { get; set; }

        [JsonProperty("build_start_time")]
        public DateTime BuildStartTime { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        //REMARK: This property exist ONLY for these TC build events: Successful, Fixed, Failed, Broken
        //and does not exist for these TC build events: Started, Changes Loaded, Interrupted, Almost Completed, Responsibility Changed
        [JsonProperty("build_finish_time")]
        public DateTime? BuildFinishTime { get; set; }

        [JsonProperty("build_event")]
        public string BuildEvent { get; set; }

        [JsonProperty("build_name")]
        public string BuildName { get; set; }

        [JsonProperty("build_status_url")]
        public string BuildStatusUrl { get; set; }

        [JsonProperty("build_number")]
        public int BuildNumber { get; set; }

        [JsonProperty("triggered_by")]
        public string TriggeredBy { get; set; }

        [JsonProperty("build_result")]
        public string BuildResult { get; set; }

        [JsonProperty("build_result_previous")]
        public string BuildResultPrevious { get; set; }

        [JsonProperty("build_result_delta")]
        public string BuildResultDelta { get; set; }
    }
}
