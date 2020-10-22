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
        public void LinqToMockPropertiesTest()
        {
            var id = 32;
            var username = "Andres";
            var street = "Streeettt";


            // Simple & hierarchy/recursive properties
            var userModel = Mock.Of<IUserModel>(user =>
                user.Id == id &&
                user.Username == username &&
                user.Address.Street == street
            );

            userModel.Should()
                .Match<IUserModel>(user => user.Id == id)
                .And.Match<IUserModel>(user => user.Username == username)
                .And.Match<IUserModel>(user => user.Address.Street == street);
        }

        [Fact]
        public void LinqToMockMethodsTests()
        {
            var userModel = Mock.Of<IUserModel>();
            var userList = Fixture
                .Build<UserModel>()
                .With(x => x.Address, Fixture.Create<Address>())
                .CreateMany(10).ToList<IUserModel>();

            // Methods with any param matching
            var repository = Mock.Of<IRepository>(x =>
                x.Add(It.IsAny<IUserModel>()) == true &&
                x.Add(userModel) == false &&
                x.ActiveUsers() == userList
            );


            repository.Should()
                .Match<IRepository>(x => x.Add((new Mock<IUserModel>()).Object) == true)
                .And.Match<IRepository>(x => x.Add(userModel) == false)
                .And.Match<IRepository>(x => x.ActiveUsers() == userList);
        }

        [Fact]
        public void LinqToMockVerifyTests()
        {
            var userModel = Mock.Of<IUserModel>();
            var userList = Fixture
                .Build<UserModel>()
                .With(x => x.Address, Fixture.Create<Address>())
                .CreateMany(10).ToList<IUserModel>();

            var repository = Mock.Of<IRepository>(x =>
                x.Add(It.IsAny<IUserModel>()) == true &&
                x.Add(userModel) == false &&
                x.ActiveUsers() == userList
            );

            var userAdded = repository.Add(Mock.Of<IUserModel>());

            var mock = Mock.Get(repository);

            mock.Verify(user => user.Add(It.IsAny<IUserModel>()), Times.Once);
        }
    }
}
