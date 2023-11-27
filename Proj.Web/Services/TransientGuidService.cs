
namespace Proj.Web.Services
{
    public class TransientGuidService:ITransientGuidService
    {
        private readonly Guid Id;
        public TransientGuidService()
        {
            Id = Guid.NewGuid();
        }

        public string GetGuild()
        {
            return Id.ToString();
        }
    }
}
