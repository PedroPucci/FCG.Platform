namespace FCG.Platform.Domain.Entities.Entity
{
    public class UserGameEntity
    {
        public string? UserId { get; set; }
        public int GameId { get; set; }
        public int Id { get; set; }

        public UserEntity? User { get; set; }
        public GameEntity? Game { get; set; }
    }
}