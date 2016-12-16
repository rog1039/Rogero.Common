using FluentAssertions;
using Rogero.Common.ExtensionMethods;
using Xunit;

namespace Rogero.Common.Tests
{
    public class TypeExtensionMethod_ImplementsInterface
    {
        [Fact()]
        [Trait("Category", "Instant")]
        public void Test()
        {
            var cImplementsIA1 = typeof(C).ImplementsInterface<IA1>();
            cImplementsIA1.Should().BeTrue();

            var cImplementsIA = typeof(C).ImplementsInterface<IA>();
            cImplementsIA.Should().BeTrue();
        }
    }

    class A : IA1 { }
    class B { }
    class C : A { }
    interface IA { }
    interface IA1 : IA { }
}
