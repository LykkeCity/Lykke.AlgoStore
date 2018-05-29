using System.Runtime.Serialization;

namespace Lykke.AlgoStore.Api.Models
{
    public class CreateAlgoModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Author { get; set; }
        public string Content { get; set; } //Base64 encoded

        [IgnoreDataMember]
        public string DecodedContent => System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(Content));
    }
}
