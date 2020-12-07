using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rogero.Common.ExtensionMethods
{
    public static class RegexExtensions
    {
        private static readonly ConcurrentDictionary<string, Regex> RegexCache =
            new ConcurrentDictionary<string, Regex>();

        private static Regex GetRegex(string regexPattern)
        {
            if (RegexCache.TryGetValue(regexPattern, out var regex))
            {
                return regex;
            }

            var newRegex = new Regex(regexPattern);
            if (RegexCache.TryAdd(regexPattern, newRegex))
            {
                //Here and in the return below, we don't care if the TryAdd succeeds.
                //At the end of the day we just want a regex, so if the add fails because
                //it already exists, then we still just want a regex and it doesn't matter
                //if we return the newly made one or the version from the dictionary.
            }

            return newRegex;
        }
        
        public static GroupCollection GetMatches(this string input, string regexPattern)
        {
            var regex = GetRegex(regexPattern);
            var matches = regex.Match(input);
            return matches.Groups;
        }

        public static IEnumerable<string> MatchRegex(this IEnumerable<string> inputStrings, string regex, string groupName)
        {
            return inputStrings.Select(input => GetMatches(input, regex)[groupName]).WhereMatchSuccessful();
        }
        
        public static IEnumerable<string> MatchRegex(this IEnumerable<string> inputStrings, Regex regex, string groupName)
        {
            return inputStrings.Select(input => regex.Match(input).Groups[groupName]).WhereMatchSuccessful();
        }

        public static IEnumerable<string> WhereMatchSuccessful(this IEnumerable<Group> groups)
        {
            foreach (var group in groups)
            {
                if (string.IsNullOrWhiteSpace(group.Value))
                {
                    continue;
                }
                yield return group.Value;
            }
        }
    }
}