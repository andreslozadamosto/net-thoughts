using AutoFixture;
using NSubstitute;
using SourceLibraryToTests.interfaces;
using SourceLibraryToTests.Models;
using System.Linq;
using Xunit;

namespace NSubstituteLibraryTests
{
    public class NSubstituteVerifyCheckExampleTests
    {
        private readonly Fixture Fixture;

        public NSubstituteVerifyCheckExampleTests()
        {
            Fixture = new Fixture();
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void VerifyTests()
        {
            // properties
            var userList = Fixture
                .Build<UserModel>()
                .With(x => x.Address, Fixture.Create<Address>())
                .CreateMany(10).ToList<IUserModel>();

            var users1 = userList.Take(2).ToList();
            var users2 = userList.Skip(1).Take(3).ToList();
            var users3 = userList.Skip(2).Take(2).ToList();

            var mock = Substitute.For<IRepository>();
            mock.Users.Returns(users1);
            var users = mock.Users;
            var a = mock.Received().Users;

            mock = Substitute.For<IRepository>();
            mock.Users = users2;
            mock.Received().Users = users2;


            // methods
            mock = Substitute.For<IRepository>();
            mock.Search(Arg.Any<string>()).Returns(users1);
            mock.Search("adas");
            mock.Received().Search(Arg.Any<string>());

            mock = Substitute.For<IRepository>();
            mock.Search(Arg.Any<string>()).Returns(users1);
            mock.Search("aa");
            mock.Search("bb");
            mock.Search("bb");
            mock.Received().Search("aa");
            mock.DidNotReceive().Search("aaa");
            mock.Received(2).Search("bb");
        }
    }
}
