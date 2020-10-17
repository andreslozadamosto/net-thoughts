using AutoFixture;
using FluentAssertions;
using NSubstitute;
using SourceLibraryToTests.interfaces;
using SourceLibraryToTests.Models;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace NSubstituteLibraryTests
{
    public class NSubstituteSimpleExampleTests
    {
        private readonly Fixture Fixture;

        public NSubstituteSimpleExampleTests()
        {
            Fixture = new Fixture();
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void PropertiesTests()
        {
            // Simple Properties
            var mock = Substitute.For<IRepository>();
            mock.Users.Returns(Fixture
                .Build<UserModel>()
                .With(x => x.Address, Fixture.Create<Address>())
                .CreateMany(2).ToList<IUserModel>());

            // Hierarchy properties
            var street = Fixture.Create<string>();
            var mockUser = Substitute.For<IUserModel>();
            mockUser.Address.Street.Returns(street);

            // Asserts
            mock.Users.Should().HaveCount(2);
            mockUser.Address.Street.Should().Be(street);
        }

        [Fact]
        public void MethodsWithArgumentMatchingTests()
        {
            var userList = Fixture
                .Build<UserModel>()
                .With(x => x.Address, Fixture.Create<Address>())
                .CreateMany(10).ToList<IUserModel>();
            var user1 = userList.First();
            var user2 = userList.Skip(1).Take(1).First();
            var user3 = userList.Skip(2).Take(1).First();

            // Matching without parameters
            var mock = Substitute.For<IRepository>();
            mock.ActiveUsers().Returns(userList);
            mock.ActiveUsers().Should().BeEquivalentTo(userList);

            // Matching by value
            mock = Substitute.For<IRepository>();
            mock.SearchById(1).Returns(user1);
            mock.SearchById(5).Returns(user2);
            mock.SearchById(1).Should().Be(user1);
            mock.SearchById(5).Should().Be(user2);
            //mock.Search(2).Should().BeNull(); // There is no default value

            // Matching any value
            mock = Substitute.For<IRepository>();
            mock.SearchById(Arg.Any<int>()).Returns(user1);
            mock.SearchById(234).Should().Be(user1);

            // Matching by inline custom matcher
            mock = Substitute.For<IRepository>();
            mock.SearchById(Arg.Is<int>(i => i < 10)).Returns(user2);
            mock.SearchById(Arg.Is<int>(i => i >= 10)).Returns(user3);
            mock.SearchById(3).Should().Be(user2);
            mock.SearchById(30).Should().Be(user3);

            // Matching by range - Alternative Arg for InRange is not implemented
            mock = Substitute.For<IRepository>();
            mock.SearchById(Arg.Is<int>(x => Enumerable.Range(1, 10).Contains(x))).Returns(user3);
            mock.SearchById(3).Should().Be(user3);
            //mock.Search(44).Should().BeNull();  // There is no default value

            // Matching by enumerable - Alternative Arg for InList is not implemented
            mock = Substitute.For<IRepository>();
            mock.SearchById(Arg.Is<int>(x => Enumerable.Range(1, 5).Contains(x))).Returns(user3);
            mock.SearchById(4).Should().Be(user3);
            //mock.Search(44).Should().BeNull();

            // Matching by enumerable not in - Alternative Arg for NotIn is not implemented
            mock = Substitute.For<IRepository>();
            mock.SearchById(Arg.Is<int>(x => !Enumerable.Range(1, 5).Contains(x))).Returns(user3);
            mock.SearchById(44).Should().Be(user3);
            //mock.Search(4).Should().BeNull(); // There is no default value

            // Matching by value string
            mock = Substitute.For<IRepository>();
            mock.Search("hola").Returns(userList);
            mock.Search("hola").Should().HaveCount(userList.Count);
            //mock.Search("asda").Should().BeNull(); // There is no default value

            // Matching by not null value- Alternative Arg for Regex is not implemented
            mock = Substitute.For<IRepository>();
            mock.Search(Arg.Is<string>(x => x != null)).Returns(userList.Take(2).ToList());
            mock.Search("holad").Should().HaveCount(2);

            // Matching by regex
            mock = Substitute.For<IRepository>();
            mock.Search(Arg.Is<string>(x => (new Regex("abc")).IsMatch(x))).Returns(userList.Skip(2).Take(3).ToList());
            mock.Search("abc").Should().HaveCount(3);
            //mock.Search("asda").Should().BeNull(); // There is no default value

            // Matching by custom matcher
            mock = Substitute.For<IRepository>();
            mock.Search(Arg.Is<string>(x => IsLarge(x))).Returns(userList.Skip(3).Take(4).ToList());
            mock.Search("hola").Should().HaveCount(4);
            //mock.Search("as").Should().BeNull(); // There is no default value

            // Matching by type
            mock = Substitute.For<IRepository>();
            mock.Add(Arg.Any<IUserModel>()).Returns(true);
            mock.Add(new UserModel()).Should().BeTrue();

            // Matching for any argument - Defaults
            mock = Substitute.For<IRepository>();
            mock.Add(default).ReturnsForAnyArgs(true);
            mock.Add(new UserModel()).Should().BeTrue();
            mock.Add(null).Should().BeTrue();

        }

        [Fact]
        public void MethodsWithParameterAccessTests()
        {
            // Accessing to parameter value
            var mock = Substitute.For<IRepository>();
            mock.Add(Arg.Any<IUserModel>()).Returns(x => { return ((IUserModel)x[0]).Username == "Andres"; });
            mock.Add(new UserModel()
            {
                Username = "Andres"
            }).Should().BeTrue();
            mock.Add(new UserModel()
            {
                Username = "Lozada"
            }).Should().BeFalse();


            // Overload
            var userList = Fixture
                .Build<UserModel>()
                .With(x => x.Address, Fixture.Create<Address>())
                .CreateMany(10).ToList<IUserModel>();
            mock = Substitute.For<IRepository>();
            mock.Search("abc").Returns(userList.Skip(2).Take(3).ToList());
            mock.SearchById(Arg.Any<int>()).Returns(userList.Skip(1).Take(1).First());
            mock.SearchById(2).Returns(userList.First());
            mock.Search("abc").Should().HaveCount(3);
            mock.SearchById(2).Should().Be(userList.First());
            mock.SearchById(3).Should().Be(userList.Skip(1).Take(1).First());
        }

        [Fact]
        private void MethodsWithGenericsTests()
        {
            var mock = Substitute.For<IRepository>();
            mock.AddUser<IUserModel>(Arg.Any<IUserModel>()).Returns(true);
            mock.AddUser<IUserModel>(Arg.Any<UserModel2>()).Returns(false);

            mock.AddUser(new UserModel()).Should().BeTrue();
            mock.AddUser(new UserModel2()).Should().BeFalse();
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

            var mock = Substitute.For<IRepository>();
            mock.SearchById(1)
                .ReturnsForAnyArgs(x =>
                {
                    parameterValue = (int)x[0];
                    return userList.First();
                });

            var user = mock.SearchById(1);

            Assert.Equal(1, parameterValue);
            Assert.Equal(userList.First(), user);

            // callback bedore and ufter returns
            string name = string.Empty;
            mock = Substitute.For<IRepository>();
            mock.SearchById(2)
               .ReturnsForAnyArgs(x =>
               {
                   parameterValue = (int)x[0];
                   return userList.First();
               })
               .AndDoes(x => name = "andres");

            user = mock.SearchById(2);

            Assert.Equal(2, parameterValue);
            Assert.Equal("andres", name);
            Assert.Equal(userList.First(), user);


            // callback without return
            mock = Substitute.For<IRepository>();
            mock.When(x => x.Save())
                .Do(x => name = "aa");
            mock.Save();
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

            var mock = Substitute.For<IRepository>();
            mock.Users.Returns(users1, users2, users3);

            Assert.Equal(users1, mock.Users);
            Assert.Equal(users2, mock.Users);
            Assert.Equal(users3, mock.Users);

            // methods
            mock = Substitute.For<IRepository>();
            mock.Search(Arg.Any<string>()).Returns(users1, users2, users3);

            Assert.Equal(users1, mock.Search("aa"));
            Assert.Equal(users2, mock.Search("aaa"));
            Assert.Equal(users3, mock.Search("aaaa"));
        }

        public bool IsLarge(string s) => !string.IsNullOrEmpty(s) && s.Length > 3;
    }
}
