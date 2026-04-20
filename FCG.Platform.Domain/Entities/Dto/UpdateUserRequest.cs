namespace FCG.Platform.Domain.Entities.Dto
{
    public class UpdateUserRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
    }
}