using AutoFixture;
using FluentAssertions;
using NSubstitute;
using SourceLibraryToTests.interfaces;
using System;
using Xunit;

namespace NSubstituteLibraryTests
{
    public class NSubstituteExceptionTests
    {
        private readonly Fixture Fixture;

        public NSubstituteExceptionTests()
        {
            Fixture = new Fixture();
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void ExceptionTests()
        {
            var mock = Substitute.For<IRepository>();
            mock.ActiveUsers().Returns(x => { throw new Exception(); });
            mock.Invoking(x => x.ActiveUsers())
                .Should().Throw<Exception>();

            mock = Substitute.For<IRepository>();
            mock.When(x => x.Save())
                .Do(x => { throw new Exception("msj"); });
            mock.Invoking(x => x.Save())
                .Should().Throw<Exception>().WithMessage("msj");
        }
    }
}
