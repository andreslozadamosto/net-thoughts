using AutoFixture;
using Moq;
using SourceLibraryToTests.interfaces;
using SourceLibraryToTests.Models;
using System.Linq;
using Xunit;

namespace MoqLibraryTests
{
    public class MoqVerifyCheckExampleTests
    {
        private readonly Fixture Fixture;

        public MoqVerifyCheckExampleTests()
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

            var mock = new Mock<IRepository>();
            mock.Setup(x => x.Users).Returns(users1);
            var users = mock.Object.Users;
            mock.VerifyGet(x => x.Users);

            mock = new Mock<IRepository>();
            mock.Object.Users = users2;
            mock.VerifySet(x => x.Users = users2);


            // methods
            mock = new Mock<IRepository>();
            mock.Setup(x => x.Search(It.IsAny<string>())).Returns(users1);
            mock.Object.Search("adas");
            mock.Verify(x => x.Search(It.IsAny<string>()));

            mock = new Mock<IRepository>();
            mock.Setup(x => x.Search(It.IsAny<string>())).Returns(users1);
            mock.Object.Search("aa");
            mock.Object.Search("bb");
            mock.Object.Search("bb");
            mock.Verify(x => x.Search("aa"));
            mock.Verify(x => x.Search("aaa"), Times.Never());
            mock.Verify(x => x.Search("aa"), Times.Once);
            mock.Verify(x => x.Search("bb"), Times.AtLeastOnce);
            mock.Verify(x => x.Search("bb"), Times.AtMost(2));
            mock.Verify(x => x.Search("bb"), Times.Exactly(2));
        }
    }
}
