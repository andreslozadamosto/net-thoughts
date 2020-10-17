using AutoFixture;
using FluentAssertions;
using NSubstitute;
using SourceLibraryToTests.interfaces;
using System;
using Xunit;

namespace NSubstituteLibraryTests
{
    public class NSubstituteExceptionExample
    {
        private readonly Fixture Fixture;

        public NSubstituteExceptionExample()
        {
            Fixture = new Fixture();
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void ExceptionTests()
        {
            var mock = Substitute.For<IRepository>();
            mock.When(x => x.Save()).Do(x => { throw new Exception("msj"); });
            mock.Invoking(x => x.Save())
                .Should().Throw<Exception>().WithMessage("msj");

            mock = Substitute.For<IRepository>();
            mock.Search(Arg.Any<string>()).Returns(x => { throw new Exception("msj"); });
            mock.Invoking(x => x.Search("ads"))
                .Should().Throw<Exception>().WithMessage("msj");
        }
    }
}
