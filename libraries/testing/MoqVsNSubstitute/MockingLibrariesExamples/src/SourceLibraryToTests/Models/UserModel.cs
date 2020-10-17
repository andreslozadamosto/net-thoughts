using SourceLibraryToTests.interfaces;

namespace SourceLibraryToTests.Models
{
    public class UserModel : IUserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public IAddress Address { get; set; }
    }
}
