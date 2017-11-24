namespace Lykke.AlgoStore.Core.Domain.Entities
{
    public class AlgoTemplateData
    {
        public string TemplateId { get; set; }
        public string LanguageId { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Version { get; set; }
        public string Branch { get; set; }
        public string Build { get; set; }
    }
}
