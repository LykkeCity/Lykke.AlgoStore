using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.Api.Models
{
    public class AlgoCommentModel
    {
        public string CommentId { get; set; }
        public string AlgoId { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? EditedOn { get; set; }
    }
}
