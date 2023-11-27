namespace Proj.Web.Services
{
    public class ScopedGuidService: IScopedGuidService
    {
        private readonly Guid Id;
        public ScopedGuidService()
        {
            Id = Guid.NewGuid();
        }

        public string GetGuild()
        {
            return Id.ToString();
        }
    }
}
