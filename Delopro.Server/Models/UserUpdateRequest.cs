namespace Delopro.Server.Models
{
    public class UserUpdateRequest
    {
        public int UserId { get; set; }
        public DateTime? DeletionDate { get; set; }
        public int? Status { get; set; }
        public int[]? Roles { get; set; }
    }
}
