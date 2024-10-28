namespace Server.Model.Model_users
{
    public class ProfileUser
    {
        public int UserId { get; set; }

        public string LastName { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string? Patronymic { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public int GenderId { get; set; }

        public int LocationId { get; set; }
    }
}
