using FCG.Platform.Domain.Entities.General;

namespace FCG.Platform.Domain.Entities.Entity
{
    public class UserGameEntity : BaseEntity
    {
        public int UserId { get; set; }
        public int GameId { get; set; }
        public UserEntity? User { get; set; }
        public GameEntity? Game { get; set; }
    }
}