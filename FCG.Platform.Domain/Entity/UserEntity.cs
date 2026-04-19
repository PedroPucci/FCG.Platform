using FCG.Platform.Domain.General;

namespace FCG.Platform.Domain.Entity
{
    public class UserEntity : BaseEntity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }        
    }
}