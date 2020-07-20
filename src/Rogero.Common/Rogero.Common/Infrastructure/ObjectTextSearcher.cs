using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnsureThat;
using Rogero.Common.ExtensionMethods;

namespace Rogero.Common
{
    public static class ObjectTextSearcherExtensionMethods
    {
        public static IEnumerable<T> Search<T>(this IEnumerable<T> items, string searchText)
        {
            return ObjectTextSearcher.FindMatches(items, searchText);
        }
    }

    public class ObjectTextSearcher
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyInfoMap =
            new ConcurrentDictionary<Type, PropertyInfo[]>();

        public static IEnumerable<T> FindMatches<T>(IEnumerable<T> items, string searchText)
        {
            if (typeof(T) == typeof(string))
            {
                var searchTerms = SearchTerm.ExtractAllTerms(searchText);
                foreach (var item in items)
                {
                    var itemMatches = true;
                    foreach (var searchTerm in searchTerms)
                    {
                        var searchMatch = searchTerm.MatchesAgainst(item as string);
                        if (!searchMatch)
                        {
                            itemMatches = false;
                            break;
                        }
                    }

                    if (itemMatches) yield return item;
                }

                yield break;
            }

            var itemProps = GetProperties(typeof(T));
            foreach (var obj in items)
            {
                if (Search(itemProps, obj, searchText)) yield return obj;
            }
        }

        public static bool Search(object obj, string searchText, IList<Func<object, string, bool?>> additionalSearchTypeFuncs = null)
        {
            var objectProperties = GetProperties(obj.GetType());
            return Search(objectProperties, obj, searchText, additionalSearchTypeFuncs);
        }


        public static bool Search(PropertyInfo[] properties, object obj, string searchText, IList<Func<object, string, bool?>> additionalSearchTypeFuncs = null)
        {
            if (string.IsNullOrWhiteSpace(searchText)) return true;

            var terms = searchText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var term in terms)
            {
            var match = Matches(properties, obj, term, 0, additionalSearchTypeFuncs:additionalSearchTypeFuncs);
                if (!match) return false;
            }

            return true;
        }
        
        private static readonly List<Func<object, string, bool?>> EmptySearchFuncs = new List<Func<object, string, bool?>>();

        private static bool Matches(PropertyInfo[] properties, object item, string searchText, int depth = 0, int maxDepth = 3, 
                                    IList<Func<object, string, bool?>> additionalSearchTypeFuncs = null)
        {
            additionalSearchTypeFuncs ??= EmptySearchFuncs;

            if (item == null) return false;
            if (string.IsNullOrWhiteSpace(searchText)) return true;
            if (depth > maxDepth) return false;

            var performingNegationSearch = searchText[0] == '-';
            searchText = performingNegationSearch ? searchText.RemoveLeft(1) : searchText;
            var resultTransform = performingNegationSearch
                ? (Func<bool, bool>) InvertBoolTransform
                : DoNothingBoolTransform;

            foreach (var propertyInfo in properties)
            {
                var type      = propertyInfo.PropertyType;
                var propValue = propertyInfo.GetValue(item);

                if (additionalSearchTypeFuncs.Count > 0)
                {
                    foreach (var additionalSearchTypeFunc in additionalSearchTypeFuncs)
                    {
                        var result = additionalSearchTypeFunc(propValue, searchText);
                        if (result.HasValue) return resultTransform(result.Value);
                    }
                }

                if (type == typeof(string))
                {
                    var value  = (string) propValue;
                    var result = value != null && value.InsensitiveContains(searchText);
                    if (result) return resultTransform(true);
                }
                else if (type == typeof(int))
                {
                    var value  = (int) propValue;
                    var result = value.ToString().InsensitiveContains(searchText);
                    if (result) return resultTransform(true);
                }
                else if (type == typeof(decimal))
                {
                    var value  = (decimal) propValue;
                    var result = value.ToString().InsensitiveContains(searchText);
                    if (result) return resultTransform(true);
                }
                else if (type == typeof(double))
                {
                    var value  = (double) propValue;
                    var result = value.ToString().InsensitiveContains(searchText);
                    if (result) return resultTransform(true);
                }
                else if (type == typeof(DateTime))
                {
                    var value  = (DateTime) propValue;
                    var result = value.ToString("d").InsensitiveContains(searchText);
                    if (result) return resultTransform(true);
                }
                else if (type.ImplementsInterface<IEnumerable>())
                {
                    var value = (IEnumerable) propValue;
                    foreach (var child in value)
                    {
                        var childProperties  = GetProperties(child.GetType());
                        var childMatchResult = Matches(childProperties, child, searchText, depth + 1);
                        if (childMatchResult) return resultTransform(true);
                    }
                }
                else
                {
                    try
                    {
                        var props  = type.GetProperties();
                        var result = Matches(props, propValue, searchText, depth + 1);
                        if (result) return resultTransform(true);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

            return resultTransform(false);
        }

        private static bool DoNothingBoolTransform(bool b) => b;
        private static bool InvertBoolTransform(bool    b) => !b;

        private static PropertyInfo[] GetProperties(Type objectType)
        {
            var properties = PropertyInfoMap.GetOrAdd(objectType, t => t.GetProperties());
            return properties;
        }
    }

    public class SearchTerm
    {
        public bool   NegativeSearch { get; }
        public string TermText       { get; }

        private SearchTerm(bool negativeSearch, string termText)
        {
            NegativeSearch = negativeSearch;
            TermText       = termText;
        }

        public static IList<SearchTerm> ExtractAllTerms(string searchText)
        {
            var searchTerms = searchText
                .Split(' ')
                .Select(ExtractOneTerm)
                .ToList();
            return searchTerms;
        }

        public static SearchTerm ExtractOneTerm(string rawTerm)
        {
            Ensure.String.IsNotNullOrWhiteSpace(rawTerm);
            rawTerm = rawTerm.Trim();

            var isTermNegated = rawTerm.Length > 1 && rawTerm[0] == '-';

            return isTermNegated
                ? new SearchTerm(true,  rawTerm.Remove(0, 1))
                : new SearchTerm(false, rawTerm);
        }

        public bool MatchesAgainst(string text)
        {
            var textContainsThis = text.InsensitiveContains(TermText);

            return NegativeSearch ? !textContainsThis : textContainsThis;
        }
    }
}