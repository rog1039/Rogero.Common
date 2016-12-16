using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Rogero.Common.ExtensionMethods;
using Xunit;

namespace Rogero.Common.Tests
{
    public class ObjectTextSearchTests
    {
        [Fact()]
        [Trait("Category", "Instant")]
        public void SearchSimpleObject()
        {
            var objects = new ObjectToSearch(){Name = "hello world", Quantity = 125}.MakeList();
            var results = ObjectTextSearcher.Search(objects, "el");
            results.Should().HaveCount(1);

            var results2 = ObjectTextSearcher.Search(objects, "e12312l");
            results2.Should().HaveCount(0);

            var results3 = ObjectTextSearcher.Search(objects, "25");
            results3.Should().HaveCount(1);
        }
    }

    public class ObjectToSearch
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
    }
}
