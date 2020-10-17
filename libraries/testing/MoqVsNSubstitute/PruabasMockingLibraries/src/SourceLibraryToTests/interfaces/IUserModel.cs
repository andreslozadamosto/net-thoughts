namespace SourceLibraryToTests.interfaces
{
    public interface IUserModel
    {
        int Id { get; set; }

        string Username { get; set; }

        IAddress Address { get; set; }
    }
}
