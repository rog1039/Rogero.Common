using FluentAssertions;
using Rogero.Common.ExtensionMethods;
using Rogero.Common.Infrastructure;
using Xunit;

namespace Rogero.Common.Tests;

public class ObjectTextSearchTests
{
    [Fact()]
    [Trait("Category", "Instant")]
    public void SearchSimpleObject()
    {
        var objects = new ObjectToSearch(){Name = "A B", Quantity = 125}.MakeList();
        objects.Add(new ObjectToSearch() {Name = "A"});
        var results = ObjectTextSearcher.FindMatches(objects, "A");
        results.Should().HaveCount(2);

        var results2 = ObjectTextSearcher.FindMatches(objects, "C");
        results2.Should().HaveCount(0);

        var results3 = ObjectTextSearcher.FindMatches(objects, "25");
        results3.Should().HaveCount(1);

        var results4 = ObjectTextSearcher.FindMatches(objects, "B");
        results4.Should().HaveCount(1);

        var results5 = ObjectTextSearcher.FindMatches(objects, "A -B");
        results5.Should().HaveCount(1);

        var results6 = ObjectTextSearcher.FindMatches(objects, "a -ab");
        results6.Should().HaveCount(2);
    }

    [Fact()]
    [Trait("Category", "Instant")]
    public void NestedObjectSearch()
    {
        var obj = new NestedObject()
        {
            Prop1 = new Holder<string>() { Value = "A B" }
        };
        var obj2 = new NestedObject()
        {
            Prop1 = new Holder<string>() { Value = "B C" }
        };
        var list = new List<NestedObject>{obj, obj2};


        var result = ObjectTextSearcher.FindMatches(list, "a");
        result.Should().HaveCount(1);

        var result2 = ObjectTextSearcher.FindMatches(list, "b");
        result2.Should().HaveCount(2);

        var result3 = ObjectTextSearcher.FindMatches(list, "d");
        result3.Should().HaveCount(0);
    }
       
    [Fact()]
    [Trait("Category", "Instant")]
    public void SearchListOfItems()
    {
        var obj = new HeaderObject()
        {
            Name = "Header",
            LineObjects = new List<LineObject>()
            {
                new LineObject()
                {
                    Name = "Line1"
                },
                new LineObject()
                {
                    Name = "Line2"
                }
            }
        };

        var result = ObjectTextSearcher.Search(obj, "ine");
        result.Should().BeTrue();
    }
}

public class ObjectToSearch
{
    public string  Name     { get; set; }
    public decimal Quantity { get; set; }
}

public class NestedObject
{
    public Holder<string> Prop1 { get; set; }
}

public class Holder<T>
{
    public T Value { get; set; }
}

public class HeaderObject
{
    public string            Name        { get; set; }
    public IList<LineObject> LineObjects { get; set; }
}

public class LineObject
{
    public string Name { get; set; }
}