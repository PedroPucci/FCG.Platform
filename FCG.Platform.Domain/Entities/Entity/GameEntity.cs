using FCG.Platform.Domain.Entities.General;

namespace FCG.Platform.Domain.Entities.Entity
{
    public class GameEntity : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}