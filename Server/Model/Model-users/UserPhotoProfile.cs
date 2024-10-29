namespace Server.Model.Model_users
{
    public class UserPhotoProfile
    {
        public int UppId { get; set; }

        public int? UserId { get; set; }

        public byte[]? Photo { get; set; }
    }
}
