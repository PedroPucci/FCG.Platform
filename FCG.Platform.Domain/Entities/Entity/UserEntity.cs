using FCG.Platform.Domain.Entities.General;

namespace FCG.Platform.Domain.Entities.Entity
{
    public class UserEntity : BaseEntity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }        
    }
}