using Lykke.AlgoStore.Core.Utils;
using Newtonsoft.Json;
using System;

namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoCommentModel
    {
        public string CommentId { get; set; }
        public string AlgoId { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        [JsonConverter(typeof(DefaultDateTimeConverter))]
        public DateTime CreatedOn { get; set; }
        [JsonConverter(typeof(DefaultDateTimeConverter))]
        public DateTime? EditedOn { get; set; }
    }
}
