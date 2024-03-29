﻿#nullable enable
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace Rogero.Common.ExtensionMethods;

public static class StringExtensions
{
   public static string AppendLine(this string input, string newText)
   {
      return input + Environment.NewLine + newText;
   }

   public static string ValueOr(this string input, string otherText)
   {
      return input.IsNullOrWhitespace()
         ? otherText
         : input;
   }

   public static string Bracketize(this string input)
   {
      return "[" + input + "]";
   }

   public static string Bracify(this string input)
   {
      return "{" + input + "}";
   }

   public static string Parenthesize(this string input)
   {
      return "(" + input + ")";
   }


   public static string Repeat(this string input, int repeatCount)
   {
      StringBuilder sb = new StringBuilder();

      for (int i = 0; i < repeatCount; i++)
      {
         sb.Append(input);
      }

      return sb.ToString();
   }

   public static bool InsensitiveEquals(this string? s1, string s2)
   {
      var oneIsNullOtherIsnt = (s1 == null && s2 != null) || (s1 != null && s2 == null);
      if (oneIsNullOtherIsnt)
         return false;

      var bothAreNull = s1 == null && s2 == null;
      if (bothAreNull)
         return true;

      return s1.Length == s2.Length && s1.IndexOf(s2, StringComparison.InvariantCultureIgnoreCase) == 0;
   }

   public static bool InsensitiveContains(this string? input, string? part)
   {
      if (part == null || input == null) return false;
      return input.Contains(part, StringComparison.OrdinalIgnoreCase);
      //return CultureInfo.InvariantCulture.CompareInfo.IndexOf(input, part, CompareOptions.IgnoreCase) >= 0;
   }

   public static string RemoveAllWhitespace(this string s)
   {
      var result = s.Replace("\r\n", "");
      result = result.Replace("\r", "");
      result = result.Replace("\n", "");
      result = result.Replace("\t", "");

      return result;
   }

   public static string RemoveLineBreaks(this string s, string lineBreakReplace = "/")
   {
      if(s.IsNullOrWhitespace())return String.Empty;
      
      var result = s.Replace("\r\n", lineBreakReplace);
      result = result.Replace("\r", lineBreakReplace);
      result = result.Replace("\n", lineBreakReplace);
      result = result.Replace("\t", lineBreakReplace);

      return result;
   }

   public static string RemoveLeft(this string s, int numberOfCharacters)
   {
      if (numberOfCharacters > s.Length)
         throw new ArgumentException("Number of characters to remove cannot be larger than length of string");
      return s.Substring(numberOfCharacters, s.Length - numberOfCharacters);
   }

   public static void ToConsoleWriteLine(this string s, string title = null)
   {
      if (title.IsNotNullOrWhitespace()) Console.WriteLine(title);
      Console.WriteLine(s);
   }

   public static void ToDebugWriteLine(this string s)
   {
      Debug.WriteLine(s);
   }

   public static bool IsNullOrWhitespace([NotNullWhen(false)] this string? s)
   {
      return string.IsNullOrWhiteSpace(s);
   }

   public static bool IsNotNullOrWhitespace([NotNullWhen(true)] this string? s)
   {
      return !string.IsNullOrWhiteSpace(s);
   }

   public static string Left(this string s, int count)
   {
      var charCount = Math.Min(s.Length, count);
      return s.Substring(0, charCount);
   }

   public static string Right(this string s, int count)
   {
      var charCount = Math.Min(s.Length, count);
      return s.Substring(s.Length - charCount, charCount);
   }

   public static string StringJoin(this IEnumerable<string> list, string separator)
   {
      return string.Join(separator, list.Where(x => x.IsNotNullOrWhitespace()));
   }

   private static string SeparatorCommaNewLine = ", " + Environment.NewLine;

   public static string StringJoinCommaNewLine(this IEnumerable<string> list)
   {
      return string.Join(SeparatorCommaNewLine, list);
   }

   public static string EscapeString(this string s)
   {
      return s
         .Replace("\\", "\\\\")  //escape backslashes
         .Replace("\"", "\\\""); //escape quotations;
   }

   public static string SmartCombine(this string separator, params string[] pieces)
   {
      var isFirst              = true;
      var sb                   = new StringBuilder();
      var hasTrailingSeparator = false;
      foreach (var piece in pieces.Where(x => x.IsNotNullOrWhitespace()))
      {
         if (isFirst)
         {
            sb.Append(piece);
            isFirst = false;
         }
         else
         {
            var startsWithSeparator = piece.StartsWith(separator);
            switch (hasTrailingSeparator)
            {
               case true when startsWithSeparator:
                  sb.Append(piece[1..]);
                  break;
               case true when !startsWithSeparator:
               case false when startsWithSeparator:
                  sb.Append(piece);
                  break;
               case false when !startsWithSeparator:
                  sb.Append(separator);
                  sb.Append(piece);
                  break;
            }
         }

         hasTrailingSeparator = piece.EndsWith(separator);
      }

      return sb.ToString();
   }

   public static string IfNullOrWhitespaceThen(this string? s, string fallbackValue)
   {
      return s.IsNullOrWhitespace()
         ? fallbackValue
         : s;
   }

   public static string ToCamelCase(string name)
   {
      return $"{name[..1].ToLower()}{name.Substring(1, name.Length - 1)}";
   }

   public static string NamespaceToPath(this string text)
   {
      return text.Replace(".", "/");
   }

   public static void ToFile(this string text, string filePath)
   {
      File.WriteAllText(filePath, text);
   }

   public static bool ContainsAny(this string text, IEnumerable<string> parts)
   {
      var contains = parts.Any(part => text.Contains(part));
      return contains;
   }

   public static string ToSingleLine(this string text)
   {
      text = text.Replace("\r\n", "\\r\\n");
      text = text.Replace("\r",   "\\r");
      text = text.Replace("\n",   "\\n");
      return text;
   }

   public static bool StartsWithInsensitive(this string text, string startsWith)
   {
      return text.StartsWith(startsWith, StringComparison.CurrentCultureIgnoreCase);
   }

   /// <summary>
   /// Splits on comma, new lines, or pipes.
   /// </summary>
   /// <param name="text"></param>
   /// <param name="splitChar"></param>
   /// <returns></returns>
   public static string[] SplitOnDefault(this string text)
   {
      char[] splitChars = { ',', '\r', '\n', '|' };
      return text.Split(splitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
   }

   public static string[] SplitOn(this string text, params char[] splitChar)
   {
      return text.Split(splitChar, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
   }

   /// <summary>
   /// Takes a number like 1,200,300 and returns 1 200 300
   /// </summary>
   /// <param name="lot"></param>
   /// <returns></returns>
   public static string Format7DigitNumber(this string lot)
   {
      if (lot.IsNullOrWhitespace()) return String.Empty;

      var lotFormatted = Regex.Replace(lot, @"(\d{1})(\d{3})(\d{3})", "$1 $2 $3");

      if (lotFormatted.IsNullOrWhitespace()) return lot;

      return lotFormatted;
   }

   public static bool LessThan(this string s1, string s2)
   {
      var result = String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
      return result == -1;
   }

   public static bool LessThanOrEqual(this string s1, string s2)
   {
      var result = String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
      return result is -1 or 0;
   }

   public static bool GreaterThan(this string s1, string s2)
   {
      var result = String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
      return result == 1;
   }

   public static bool GreaterThanOrEqual(this string s1, string s2)
   {
      var result = String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
      return result is 1 or 0;
   }

   public static string IndentWith(this string s, string indentText, int textCount)
   {
      var indentTextFinal = indentText.Repeat(textCount);
      return
         indentTextFinal +
         s.Replace(Environment.NewLine, Environment.NewLine + indentTextFinal);
   }

   public static string IndentWithSpaces(this string s, int spacesCount)
   {
      return s.IndentWith(" ", spacesCount);
   }

   public static string IndentWithTabs(this string s, int tabCount)
   {
      return s.IndentWith("\t", tabCount);
   }
}