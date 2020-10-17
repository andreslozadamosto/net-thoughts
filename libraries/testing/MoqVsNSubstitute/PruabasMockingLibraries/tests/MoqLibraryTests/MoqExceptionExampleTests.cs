using AutoFixture;
using FluentAssertions;
using Moq;
using SourceLibraryToTests.interfaces;
using System;
using Xunit;

namespace MoqLibraryTests
{
    public class MoqExceptionExampleTests
    {
        private readonly Fixture Fixture;

        public MoqExceptionExampleTests()
        {
            Fixture = new Fixture();
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public void ExceptionTests()
        {
            var mock = new Mock<IRepository>();
            mock.Setup(x => x.Save()).Throws<Exception>();
            mock.Object.Invoking(x => x.Save())
                .Should().Throw<Exception>();

            mock = new Mock<IRepository>();
            mock.Setup(x => x.Save()).Throws(new Exception("msj"));
            mock.Object.Invoking(x => x.Save())
                .Should().Throw<Exception>().WithMessage("msj");
        }
    }
}
