namespace FCG.Platform.Domain.Entities.Dto.UserDto
{
    public class UserResponse
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool IsActive { get; set; }
    }
}