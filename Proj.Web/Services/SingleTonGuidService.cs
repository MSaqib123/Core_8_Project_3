namespace Proj.Web.Services
{
    public class SingleTonGuidService: ISingleTonGuidService
    {
        private readonly Guid Id;
        public SingleTonGuidService()
        {
            Id = Guid.NewGuid();
        }

        public string GetGuild()
        {
            return Id.ToString();
        }
    }
}
