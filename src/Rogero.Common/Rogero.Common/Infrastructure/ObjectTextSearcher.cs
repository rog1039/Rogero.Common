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
                    if(itemMatches) yield return item;
                }

                yield break;
            }

            var itemProps = GetProperties(typeof(T));
            foreach (var obj in items)
            {
                if (Search(itemProps, obj, searchText)) yield return obj;
            }
        }

        public static bool Search(object obj, string searchText)
        {
            var objectProperties = GetProperties(obj.GetType());
            return Search(objectProperties, obj, searchText);
        }

        public static bool Search(PropertyInfo[] properties, object obj, string searchText)
        {
            if(string.IsNullOrWhiteSpace(searchText)) return true;
            
            var terms = searchText.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var term in terms)
            {
                var match = Matches(properties, obj, term, 0);
                if (!match) return false;
            }
            return true;
        }

        private static bool Matches(PropertyInfo[] properties, object item, string searchText, int depth = 0)
        {
            if (item == null) return false;
            if (string.IsNullOrWhiteSpace(searchText)) return true;
            if (depth > 3) return false;

            var performingNegationSearch = searchText[0] == '-';
            searchText = performingNegationSearch ? searchText.RemoveLeft(1) : searchText;
            var resultTransform = performingNegationSearch
                ? (Func<bool, bool>) InvertBoolTransform
                : DoNothingBoolTransform;

            foreach (var propertyInfo in properties)
            {
                var type = propertyInfo.PropertyType;
                if (type == typeof(string))
                {
                    var value = (string)propertyInfo.GetValue(item);
                    var result = value != null && value.InsensitiveContains(searchText);
                    if (result) return resultTransform(true);
                }
                else if (type == typeof(int))
                {
                    var value = ((int)propertyInfo.GetValue(item));
                    var result = value.ToString().InsensitiveContains(searchText);
                    if (result) return resultTransform(true);
                }
                else if (type == typeof(decimal))
                {
                    var value = ((decimal)propertyInfo.GetValue(item));
                    var result = value.ToString().InsensitiveContains(searchText);
                    if (result) return resultTransform(true);
                }
                else if (type == typeof(double))
                {
                    var value = ((double)propertyInfo.GetValue(item));
                    var result = value.ToString().InsensitiveContains(searchText);
                    if (result) return resultTransform(true);
                }
                else if (type == typeof(DateTime))
                {
                    var value = ((DateTime) propertyInfo.GetValue(item));
                    var result = value.ToString("d").InsensitiveContains(searchText);
                    if (result) return resultTransform(true);
                }
                else if (type.ImplementsInterface<IEnumerable>())
                {
                    var value = (IEnumerable) propertyInfo.GetValue((item));
                    foreach (var child in value)
                    {
                        var childProperties = GetProperties(child.GetType());
                        var childMatchResult = Matches(childProperties, child, searchText, depth + 1);
                        if (childMatchResult) return resultTransform(true);
                    }
                }
                else
                {
                    var val = propertyInfo.GetValue(item);
                    var props = type.GetProperties();
                    var result = Matches(props, val, searchText, depth + 1);
                    if(result) return resultTransform(true);
                }
            }
            return resultTransform(false);
        }

        private static bool DoNothingBoolTransform(bool b) => b;
        private static bool InvertBoolTransform(bool b) => !b;

        private static PropertyInfo[] GetProperties(Type objectType)
        {
            var properties = PropertyInfoMap.GetOrAdd(objectType, t => t.GetProperties());
            return properties;
        }
    }

    public class SearchTerm
    {
        public bool NegativeSearch { get; }
        public string TermText { get; }

        private SearchTerm(bool negativeSearch, string termText)
        {
            NegativeSearch = negativeSearch;
            TermText = termText;
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
                ? new SearchTerm(true, rawTerm.Remove(0, 1))
                : new SearchTerm(false, rawTerm);
        }

        public bool MatchesAgainst(string text)
        {
            var textContainsThis = text.InsensitiveContains(TermText);

            return NegativeSearch ? !textContainsThis : textContainsThis;
        }
    }
}
