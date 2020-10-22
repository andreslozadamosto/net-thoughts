using AutoFixture;
using FluentAssertions;
using Moq;
using SourceLibraryToTests.interfaces;
using SourceLibraryToTests.Models;
using System.Linq;
using Xunit;

namespace MoqLibraryTests
{
    public class MoqSimpleExamplesTests
    {

        private readonly Fixture Fixture;

        public MoqSimpleExamplesTests()
        {
            Fixture = new Fixture();
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void PropertiesTests()
        {
            // Simple Properties
            var mock = new Mock<IRepository>();
            mock.Setup(foo => foo.Users).Returns(Fixture
                .Build<UserModel>()
                .With(x => x.Address, Fixture.Create<Address>())
                .CreateMany(2).ToList<IUserModel>());

            // Hierarchy/recursive properties
            var street = Fixture.Create<string>();
            var mockUser = new Mock<IUserModel>();
            mockUser.Setup(foo => foo.Address.Street).Returns(street);


            // Asserts
            mock.Object.Users.Should().HaveCount(2);
            mockUser.Object.Address.Street.Should().Be(street);
        }

        [Fact]
        public void MethodsWithArgumentMatchingTests()
        {
            var userList = Fixture
                .Build<UserModel>()
                .With(x => x.Address, Fixture.Create<Address>())
                .CreateMany(10).ToList<IUserModel>();
            var user1 = new Mock<IUserModel>();
            var user2 = new Mock<IUserModel>();
            var user3 = new Mock<IUserModel>();

            // Matching without parameters
            var mock = new Mock<IRepository>();
            mock.Setup(x => x.ActiveUsers()).Returns(userList);
            mock.Object.ActiveUsers().Should().BeEquivalentTo(userList);

            // Matching by value
            mock = new Mock<IRepository>();
            mock.Setup(x => x.SearchById(1)).Returns(user1.Object);
            mock.Setup(x => x.SearchById(5)).Returns(user2.Object);
            mock.Object.SearchById(1).Should().Be(user1.Object);
            mock.Object.SearchById(5).Should().Be(user2.Object);
            mock.Object.SearchById(2).Should().BeNull();

            // Matching any value
            mock = new Mock<IRepository>();
            mock.Setup(x => x.SearchById(It.IsAny<int>())).Returns(user1.Object);
            mock.Object.SearchById(234).Should().Be(user1.Object);

            // Matching by inline custom matcher
            mock = new Mock<IRepository>();
            mock.Setup(x => x.SearchById(It.Is<int>(i => i < 10))).Returns(user2.Object);
            mock.Setup(x => x.SearchById(It.Is<int>(i => i >= 10))).Returns(user3.Object);
            mock.Object.SearchById(3).Should().Be(user2.Object);
            mock.Object.SearchById(30).Should().Be(user3.Object);

            // Matching by range
            mock = new Mock<IRepository>();
            mock.Setup(x => x.SearchById(It.IsInRange(0, 10, Range.Inclusive))).Returns(user3.Object);
            mock.Object.SearchById(3).Should().Be(user3.Object);
            mock.Object.SearchById(44).Should().BeNull();

            // Matching by enumerable
            mock = new Mock<IRepository>();
            mock.Setup(x => x.SearchById(It.IsIn(Enumerable.Range(1, 5)))).Returns(user3.Object);
            mock.Object.SearchById(4).Should().Be(user3.Object);
            mock.Object.SearchById(44).Should().BeNull();

            // Matching by enumerable not in
            mock = new Mock<IRepository>();
            mock.Setup(x => x.SearchById(It.IsNotIn(Enumerable.Range(1, 5)))).Returns(user3.Object);
            mock.Object.SearchById(44).Should().Be(user3.Object);
            mock.Object.SearchById(4).Should().BeNull();

            // Matching by value string
            mock = new Mock<IRepository>();
            mock.Setup(x => x.Search("hola")).Returns(userList);
            mock.Object.Search("hola").Should().HaveCount(userList.Count);
            mock.Object.Search("asda").Should().BeNull();

            // Matching by not null value
            mock = new Mock<IRepository>();
            mock.Setup(x => x.Search(It.IsNotNull<string>())).Returns(userList.Take(2).ToList());
            mock.Object.Search("holad").Should().HaveCount(2);

            // Matching by regex
            mock = new Mock<IRepository>();
            mock.Setup(x => x.Search(It.IsRegex("abc"))).Returns(userList.Skip(2).Take(3).ToList());
            mock.Object.Search("abc").Should().HaveCount(3);
            mock.Object.Search("asda").Should().BeNull();

            // Matching by custom matcher
            mock = new Mock<IRepository>();
            mock.Setup(x => x.Search(IsLarge())).Returns(userList.Skip(3).Take(4).ToList());
            mock.Object.Search("hola").Should().HaveCount(4);
            mock.Object.Search("as").Should().BeNull();

            // Matching by type
            mock = new Mock<IRepository>();
            mock.Setup(x => x.Add(It.IsAny<IUserModel>())).Returns(true);
            mock.Object.Add(new UserModel()).Should().BeTrue();

            // Matching for any argument
            // There is no option here
        }

        [Fact]
        public void MethodsWithParameterAccessTests()
        {
            // Accessing to parameter value
            var mock = new Mock<IRepository>();
            mock.Setup(x => x.Add(It.IsAny<IUserModel>())).Returns((IUserModel user) => user.Username == "Andres");
            mock.Object.Add(new UserModel()
            {
                Username = "Andres"
            }).Should().BeTrue();
            mock.Object.Add(new UserModel()
            {
                Username = "Lozada"
            }).Should().BeFalse();


            // Overload
            var userList = Fixture
                .Build<UserModel>()
                .With(x => x.Address, Fixture.Create<Address>())
                .CreateMany(10).ToList<IUserModel>();
            mock = new Mock<IRepository>();
            mock.Setup(x => x.Search(It.IsRegex("abc"))).Returns((string s) => userList.Skip(2).Take(3).ToList());
            mock.Setup(x => x.SearchById(It.IsAny<int>())).Returns((int i) => userList.Skip(1).Take(1).First());
            mock.Setup(x => x.SearchById(2)).Returns((int i) => userList.First());
            mock.Object.Search("abc").Should().HaveCount(3);
            mock.Object.SearchById(2).Should().Be(userList.First());
            mock.Object.SearchById(3).Should().Be(userList.Skip(1).Take(1).First());
        }

        [Fact]
        private void MethodsWithGenericsTests()
        {
            var mock = new Mock<IRepository>();
            mock.Setup(m => m.AddUser(It.IsAny<It.IsSubtype<IUserModel>>())).Returns(true);
            mock.Setup(m => m.AddUser(It.IsAny<UserModel2>())).Returns(false);

            mock.Object.AddUser(new UserModel()).Should().BeTrue();
            mock.Object.AddUser(new UserModel2()).Should().BeFalse();
        }

        [Fact]
        public void CallbacksTests()
        {
            // simple callback
            var userList = Fixture
                .Build<UserModel>()
                .With(x => x.Address, Fixture.Create<Address>())
                .CreateMany(10).ToList<IUserModel>();

            int parameterValue = 0;

            var mock = new Mock<IRepository>();
            mock.Setup(x => x.SearchById(1))
                .Callback((int x) => parameterValue = x)
                .Returns(userList.First());

            var user = mock.Object.SearchById(1);

            Assert.Equal(1, parameterValue);
            Assert.Equal(userList.First(), user);

            // callback bedore and ufter returns
            string name = string.Empty;
            mock = new Mock<IRepository>();
            mock.Setup(x => x.SearchById(2))
                .Callback<int>(x => parameterValue = x)
                .Returns(userList.First())
                .Callback<int>(x => name = "andres");

            user = mock.Object.SearchById(2);

            Assert.Equal(2, parameterValue);
            Assert.Equal("andres", name);
            Assert.Equal(userList.First(), user);

            // callback without return
            mock = new Mock<IRepository>();
            mock.Setup(x => x.Save()).Callback(() => name = "aa");
            mock.Object.Save();
            Assert.Equal("aa", name);
        }

        [Fact]
        public void MultipleReturnsTests()
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
            mock.SetupSequence(x => x.Users)
                .Returns(users1)
                .Returns(users2)
                .Returns(users3);

            Assert.Equal(users1, mock.Object.Users);
            Assert.Equal(users2, mock.Object.Users);
            Assert.Equal(users3, mock.Object.Users);

            // methods
            mock = new Mock<IRepository>();
            mock.SetupSequence(x => x.Search(It.IsAny<string>()))
                .Returns(users1)
                .Returns(users2)
                .Returns(users3);

            Assert.Equal(users1, mock.Object.Search("aa"));
            Assert.Equal(users2, mock.Object.Search("aaa"));
            Assert.Equal(users3, mock.Object.Search("aaaa"));
        }

        public string IsLarge()
        {
            return Moq.Match.Create<string>(s => !string.IsNullOrEmpty(s) && s.Length > 3);
        }
    }
}
