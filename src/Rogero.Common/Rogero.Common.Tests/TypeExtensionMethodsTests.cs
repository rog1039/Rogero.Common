using FluentAssertions;
using Xunit;

namespace Rogero.Common.Tests;

public class TypeExtensionMethodsTests
{
    [Fact()]
    [Trait("Category", "Instant")]
    public void InterfaceBaseTypeTest()
    {
        var type = typeof(ITest);
        type.BaseType.Should().BeNull();
    } 
}

interface ITest{}