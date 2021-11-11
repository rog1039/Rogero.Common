using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace Rogero.Common.ExtensionMethods;

public static class RegexExtensions
{
    private static readonly ConcurrentDictionary<RegexId, Regex> RegexCache = new();
            

    private record RegexId(string RegexPattern, RegexOptions RegexOptions);

    private static Regex GetRegex(string regexPattern, RegexOptions regexOptions = RegexOptions.None)
    {
        var regexId = new RegexId(regexPattern, regexOptions);
        if (RegexCache.TryGetValue(regexId, out var regex))
        {
            return regex;
        }

        var newRegex = new Regex(regexPattern, regexOptions);
        if (RegexCache.TryAdd(regexId, newRegex))
        {
            //Here and in the return below, we don't care if the TryAdd succeeds.
            //At the end of the day we just want a regex, so if the add fails because
            //it already exists, then we still just want a regex and it doesn't matter
            //if we return the newly made one or the version from the dictionary.
        }

        return newRegex;
    }
        
    public static GroupCollection GetMatches(this string input, string regexPattern, RegexOptions regexOptions = RegexOptions.None)
    {
        var regex   = GetRegex(regexPattern, regexOptions);
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