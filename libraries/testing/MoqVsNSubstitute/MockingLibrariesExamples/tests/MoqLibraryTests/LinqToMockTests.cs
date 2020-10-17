using AutoFixture;
using FluentAssertions;
using Moq;
using SourceLibraryToTests.interfaces;
using SourceLibraryToTests.Models;
using System.Linq;
using Xunit;

namespace MoqLibraryTests
{
    public class LinqToMockTests
    {
        private readonly Fixture Fixture;

        public LinqToMockTests()
        {
            Fixture = new Fixture();
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void LinqToMockTest()
        {
            var userModel = new Mock<IUserModel>();
            var userList = Fixture
                .Build<UserModel>()
                .With(x => x.Address, Fixture.Create<Address>())
                .CreateMany(10).ToList<IUserModel>();

            var repository = Mock.Of<IRepository>(x =>
                x.Add(It.IsAny<IUserModel>()) == true &&
                x.Add(userModel.Object) == false &&
                x.ActiveUsers() == userList &&
                x.Users == userList
            );


            repository.Should()
                .Match<IRepository>(x => x.Add((new Mock<IUserModel>()).Object) == true)
                .And.Match<IRepository>(x => x.Add(userModel.Object) == false)
                .And.Match<IRepository>(x => x.ActiveUsers() == userList)
                .And.Match<IRepository>(x => x.Users == userList);
        }
    }
}
