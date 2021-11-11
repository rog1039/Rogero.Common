using FluentAssertions;
using Rogero.Common.ExtensionMethods;
using Xunit;
using Xunit.Abstractions;

namespace Rogero.Common.Tests;

public class MemberIsNullableTests : UnitTestBaseWithConsoleRedirection
{
    [Fact()]
    [Trait("Category", "Instant")]
    public void TestProperties()
    {
        var myClassType           = typeof(MyClass);
        var myClassStringNotNull  = myClassType.GetProperty(nameof(MyClass.StringNotNull));
        var myClassStringNull     = myClassType.GetProperty(nameof(MyClass.StringNull));
        var myClassObjectNotNull  = myClassType.GetProperty(nameof(MyClass.ObjectNotNull));
        var myClassObjectNull     = myClassType.GetProperty(nameof(MyClass.ObjectNullable));
        var myClassIntNotNullable = myClassType.GetField(nameof(MyClass.IntNotNullable));
        var myClassIntNullable    = myClassType.GetField(nameof(MyClass.IntNullable));

        myClassObjectNotNull.IsMemberNullable().Should().BeFalse();
        myClassObjectNull.IsMemberNullable().Should().BeTrue();
        myClassStringNotNull.IsMemberNullable().Should().BeFalse();
        myClassStringNull.IsMemberNullable().Should().BeTrue();
        myClassIntNotNullable.IsMemberNullable().Should().BeFalse();
        myClassIntNullable.IsMemberNullable().Should().BeTrue();
    }

    [Fact()]
    [Trait("Category", "Instant")]
    public void TestNullableInt()
    {
        var myClassType           = typeof(MyClass);
        var myClassIntNotNullable = myClassType.GetField(nameof(MyClass.IntNotNullable));
        var myClassIntNullable    = myClassType.GetField(nameof(MyClass.IntNullable));
            
        myClassIntNotNullable.IsMemberNullable().Should().BeFalse();
        myClassIntNullable.IsMemberNullable().Should().BeTrue();
    } 


    [Fact()]
    [Trait("Category", "Instant")]
    public void TestMethodReturnTypes()
    {
        var myClassType       = typeof(MyClass);
        var methodNotNullable = myClassType.GetMethod("GetNotNullable");
        var methodNullable    = myClassType.GetMethod("GetNullable");

        methodNullable.ReturnIsNullable().Should().BeTrue();
        methodNotNullable.ReturnIsNullable().Should().BeFalse();
    } 


    public MemberIsNullableTests(ITestOutputHelper outputHelperHelper) : base(outputHelperHelper) { }
}

public class MyClass
{
    public string  StringNotNull  { get; set; }
    public string? StringNull     { get; set; }
    public object  ObjectNotNull  { get; set; }
    public object? ObjectNullable { get; set; }

    public object  GetNotNullable() => null;
    public object? GetNullable()    => null;

    public int  IntNotNullable;
    public int? IntNullable;
}

public class GenericType<T>
{
    public T? TNullable    { get; set; }
    public T  TNotNullable { get; set; }
}