using NUnit.Framework;
using System.Threading.Tasks;
using FluentAssertions;

namespace Briefcase.ActiveObject.Test.Simple
{
    [TestFixture]
    class GreeterTests
    {
        private const string Brontofuzik = "Brontofuzik";

        private readonly Greeter passive;
        private readonly GreeterActive active;

        public GreeterTests()
        {
            passive = new Greeter(1000);
            active = new GreeterActive(passive);
        }

        [Test]
        public async Task CallFunction1()
        {
            var passiveR = passive.Hello(Brontofuzik);
            var activeR = await active.HelloAsync1(Brontofuzik);
            activeR.Should().Be(passiveR);
        }

        [Test]
        public async Task CallFunction2()
        {
            var passiveR = passive.Hello(Brontofuzik);
            var activeR = await active.HelloAsync2(Brontofuzik);
            activeR.Should().Be(passiveR);
        }

        [Test]
        public async Task CallAction()
        {
            await active.IncAsync();
            passive.Counter.Should().Be(1);
        }
    }
}
