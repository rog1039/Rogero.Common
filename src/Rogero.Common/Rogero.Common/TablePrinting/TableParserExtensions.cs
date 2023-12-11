using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Rogero.Common.ExtensionMethods;

public static class TableParserExtensions
{
   public static void PrintStringTable<T>(this IEnumerable<T>               values,
                                          string                            tableTitle     = null,
                                          int                               sampleCount    = Int32.MaxValue,
                                          List<Expression<Func<T, object>>> includeColumns = null,
                                          List<Expression<Func<T, object>>> excludeColumns = null
   )
   {
      Console.WriteLine(ToStringTable(values, tableTitle, sampleCount, includeColumns, excludeColumns));
   }

   public static string ToStringTable<T>(this IEnumerable<T>                values,
                                         string                             tableTitle     = null,
                                         int                                sampleCount    = Int32.MaxValue,
                                         IList<Expression<Func<T, object>>> includeColumns = null,
                                         IList<Expression<Func<T, object>>> excludeColumns = null
   )
   {
      var sb = new StringBuilder();
      if (tableTitle.IsNotNullOrWhitespace())
      {
         sb.AppendLine(new string('=', 350));
         var datasourceRowCount = values.Count();
         var tableHeader        = $"{tableTitle} |";
         var sampleSection = datasourceRowCount > sampleCount
            ? $" Sampling {sampleCount}/{datasourceRowCount} rows"
            : $" Showing all {datasourceRowCount} rows";
         sb.AppendLine(tableHeader + sampleSection);
      }

      sb.AppendLine(values.Take(sampleCount).ToStringTable(useTForProperties: false, includeColumns, excludeColumns));

      return sb.ToString();
   }

   public static string ToStringTable<T>(this IEnumerable<T>                values,
                                         bool                               useTForProperties = false,
                                         IList<Expression<Func<T, object>>> includeColumns    = null,
                                         IList<Expression<Func<T, object>>> excludeColumns    = null
   )
   {
      if (values == null || !values.Any()) return String.Empty;
      if (typeof(T) == typeof(string))
      {
         return string.Join(Environment.NewLine, values);
      }

      var objectProperties = GetObjectProperties<T>(values, useTForProperties, includeColumns, excludeColumns);
      var columnHeaders    = new string[objectProperties.Length];
      var valueSelectors   = new Func<T, object>[objectProperties.Length];

      for (int i = 0; i < objectProperties.Count(); i++)
      {
         var propertyInfo  = objectProperties[i];
         var columnHeader  = propertyInfo.Name;
         var valueSelector = new Func<T, object>(input => propertyInfo.GetValue(input, null));

         columnHeaders[i]  = columnHeader;
         valueSelectors[i] = valueSelector;
      }

      return ToStringTable(values, columnHeaders, valueSelectors);
   }

   private static PropertyInfo[] GetObjectProperties<T>(IEnumerable<T>                     values,
                                                        bool                               useTForProperties,
                                                        IList<Expression<Func<T, object>>> includeColumns = null,
                                                        IList<Expression<Func<T, object>>> excludeColumns = null
   )
   {
      if (includeColumns is not null && excludeColumns is not null)
         throw new InvalidOperationException(
            $@"
Cannot have both include and exclude columns. Only send one or the other.
Include columns: {includeColumns.Select(x => x.GetPropertyName()).StringJoin(", ")}
Exclude columns: {excludeColumns.Select(x => x.GetPropertyName()).StringJoin(", ")}");

      //Can't use the code below since it may very well be that T is object and the list contains subtypes of object
      //We must use it though if the values array has no elements.
      if (!values.Any() || useTForProperties)
      {
         var objectProperties = typeof(T).GetProperties();
         return FilterProperties(objectProperties);
      }

      //So instead, let's check the type of the first element
      var type = values.First().GetType();
      return FilterProperties(GetBasePropertiesFirst(type));

      PropertyInfo[] FilterProperties(PropertyInfo[] propertyInfos)
      {
         if (includeColumns is not null)
         {
            var includeNames = includeColumns.Select(x => x.GetPropertyName()).ToList();
            return propertyInfos.Where(x => includeNames.Contains(x.Name)).ToArray();
         }

         if (excludeColumns is not null)
         {
            var excludeNames = excludeColumns.Select(x => x.GetPropertyName()).ToList();
            return propertyInfos.Where(x => !excludeNames.Contains(x.Name)).ToArray();
         }

         return propertyInfos;
      }
   }

   // this is alternative for typeof(T).GetProperties()
   // that returns base class properties before inherited class properties
   private static PropertyInfo[] GetBasePropertiesFirst(Type type)
   {
      var orderList     = new List<Type>();
      var iteratingType = type;
      do
      {
         orderList.Insert(0, iteratingType);
         iteratingType = iteratingType.BaseType;
      } while (iteratingType != null);

      var props = type.GetProperties()
         .Where(prop => prop.CanRead)
         .Where(prop => !prop.GetMethod.IsStatic)
         .OrderBy(x => orderList.IndexOf(x.DeclaringType))
         .ToArray();

      return props;
   }

   public static string ToStringTable<T>(this IEnumerable<T>      values,
                                         string[]                 columnHeaders,
                                         params Func<T, object>[] valueSelectors)
   {
      return ToStringTable(values.ToArray(), columnHeaders, valueSelectors);
   }

   public static string ToStringTable<T>(this T[]                 values,
                                         string[]                 columnHeaders,
                                         params Func<T, object>[] valueSelectors)
   {
      Debug.Assert(columnHeaders.Length == valueSelectors.Length);

      var arrValues = new string[values.Length + 1, valueSelectors.Length];

      // Fill headers
      for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
      {
         arrValues[0, colIndex] = columnHeaders[colIndex];
      }

      // Fill table rows
      for (int rowIndex = 1; rowIndex < arrValues.GetLength(0); rowIndex++)
      {
         for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
         {
            object value = valueSelectors[colIndex].Invoke(values[rowIndex - 1]);

            switch (value)
            {
               case decimal dec:
               {
                  arrValues[rowIndex, colIndex] = dec.ToString("#,#0.#####");
                  break;
               }
               default:
               {
                  var val = value != null ? value.ToString() : "null";

                  val = val.Replace("System.Collections.Generic.List`1", "List<>");
                  if (val.StartsWith("List<>"))
                  {
                     var inner    = val.Substring(7, val.Length - 8);
                     var typeName = inner.SplitOn('.').Last();

                     if (value is ICollection coll)
                     {
                        val = $"List<{typeName}> ({coll.Count} items)";
                     }
                     else
                     {
                        val = $"List<{typeName}> ()";
                     }
                  }

                  arrValues[rowIndex, colIndex] = val;
                  break;
               }
            }
         }
      }

      return ToStringTable(arrValues);
   }

   public static string ToStringTable(this string[,] arrValues)
   {
      int[] maxColumnsWidth = GetMaxColumnsWidth(arrValues);
      var   headerSpliter   = new string('-', maxColumnsWidth.Sum(i => i + 3) - 1);

      var sb = new StringBuilder();
      for (int rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
      {
         for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
         {
            // Print cell
            string cell = arrValues[rowIndex, colIndex];
            cell = cell.PadRight(maxColumnsWidth[colIndex]);
            sb.Append(" | ");
            sb.Append(cell);
         }

         // Print end of line
         sb.Append(" | ");
         sb.AppendLine();

         // Print splitter
         if (rowIndex == 0)
         {
            sb.AppendFormat(" |{0}| ", headerSpliter);
            sb.AppendLine();
         }
      }

      return sb.ToString();
   }

   private static int[] GetMaxColumnsWidth(string[,] arrValues)
   {
      var maxColumnsWidth = new int[arrValues.GetLength(1)];
      for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
      {
         for (int rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
         {
            int newLength = arrValues[rowIndex, colIndex].Length;
            int oldLength = maxColumnsWidth[colIndex];

            if (newLength > oldLength)
            {
               maxColumnsWidth[colIndex] = newLength;
            }
         }
      }

      return maxColumnsWidth;
   }

   public static string ToStringTable<T>(this   IEnumerable<T>                values,
                                         params Expression<Func<T, object>>[] valueSelectors)
   {
      var headers   = valueSelectors.Select(func => ExpressionHelpers.GetPropertyInfo(func).Name).ToArray();
      var selectors = valueSelectors.Select(exp => exp.Compile()).ToArray();
      return ToStringTable(values, headers, selectors);
   }

   public static void PrintStringTable<T>(this   IEnumerable<T>                values,
                                          params Expression<Func<T, object>>[] valueSelectors)
   {
      Console.WriteLine(values.ToStringTable(valueSelectors));
   }
}

public static class ExpressionHelpers
{
   public static PropertyInfo GetPropertyInfo<T,TProp>(this Expression<Func<T, TProp>> expression)
   {
      if (expression.Body is UnaryExpression unaryExpression)
      {
         if (unaryExpression.Operand is MemberExpression memberExpression)
         {
            return memberExpression.Member as PropertyInfo ??
                   throw new InvalidOperationException("Unable to extract PropertyInfo from MemberExpression.");
         }
      }

      if ((expression.Body is MemberExpression body))
      {
         return body.Member as PropertyInfo ??
                throw new InvalidOperationException("Unable to extract PropertyInfo from MemberExpression");
      }

      throw new InvalidOperationException("Unable to extract PropertyInfo from expression.");
   }
   public static PropertyInfo GetPropertyInfo<T>(this Expression<Func<T, object>> expression)
   {
      if (expression.Body is UnaryExpression unaryExpression)
      {
         if (unaryExpression.Operand is MemberExpression memberExpression)
         {
            return memberExpression.Member as PropertyInfo ??
                   throw new InvalidOperationException("Unable to extract PropertyInfo from MemberExpression.");
         }
      }

      if ((expression.Body is MemberExpression body))
      {
         return body.Member as PropertyInfo ??
                throw new InvalidOperationException("Unable to extract PropertyInfo from MemberExpression");
      }

      throw new InvalidOperationException("Unable to extract PropertyInfo from expression.");
   }

   public static string GetPropertyName<T,TProp>(this Expression<Func<T, TProp>> expression)
   {
      var propertyInfo = GetPropertyInfo(expression);
      var propertyname = propertyInfo.Name;
      return propertyname;
   }
   
   public static string GetPropertyName<T>(this Expression<Func<T, object>> expression)
   {
      var propertyInfo = GetPropertyInfo(expression);
      var propertyname = propertyInfo.Name;
      return propertyname;
   }
}