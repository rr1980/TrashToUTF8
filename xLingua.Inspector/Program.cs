using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;

namespace xLingua.Inspector
{
    class Program
    {
        public static char[] SearchChars = new char[] {
            'Â',
            'Ã',
            '«',
            '‘',
            '¹',
            '“',
            'Ã',
            'Ð',
            'Å',
            '©',
            'º',
            '‡',
            '™',
            '…',
            'Å',
            '¾',
            '†',
            '»',
            '°',
            'Ñ',
            //'â',
        };

        public static char[] BlackChars = new char[] {
            'ￅ',
            '�',
            '¬',
            '±',
        };

        static void Main(string[] args)
        {
            try
            {

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Console.OutputEncoding = Encoding.UTF8;

                Search<Statistic>(x=>x.Id, x => x.Keyword);

               


            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Debug.WriteLine("ERROR: " + ex.Message + Environment.NewLine + JsonConvert.SerializeObject(ex, Formatting.Indented));
            }




            Console.ReadLine();
        }
        private static string GetName<TSource, TField>(Expression<Func<TSource, TField>> Field)
        {
            return (Field.Body as MemberExpression ?? ((UnaryExpression)Field.Body).Operand as MemberExpression).Member.Name;
        }

        private static void Search<T>(Expression<Func<T, long>> idProp, Expression<Func<T, string>> columnProp) where T: class
        {
            Func<T, long> idGetter;
            Func<T, string> valueGetter;

            CreateGetter(idProp, out idGetter);
            CreateGetter(columnProp, out valueGetter);

            var name = typeof(T);
            using (var _context = new DataDbContext())
            {

                var _entityType = _context.Model.FindEntityType(typeof(T));
                var _tableName = _entityType.Relational().TableName;
                var _column = GetColumnName(_entityType, columnProp);

                string _where = GetWhere(_column);

                var _command = $"SELECT * FROM {_tableName} WHERE {_where}";
                
                var _results = _context.Set<T>().FromSql(_command).AsNoTracking().ToList();

                foreach (var result in _results)
                {
                    Console.WriteLine(string.Format("{0,-10} {1,50} {2,-50}", idGetter(result), valueGetter(result), Convert(valueGetter(result))));
                }
            }
        }

        private static string GetColumnName<T>(IEntityType entityType, Expression<Func<T, string>> column) where T : class
        {
            List<string> results = new List<string>();

            var _propName = GetName(column);
            var _stringColumns = entityType.GetProperties().Where(x => x.Relational().ColumnType == "longtext");
            var _column = _stringColumns.FirstOrDefault(x => x.Name == _propName);

            if (_column == null)
            {
                throw new Exception("konnte Spalte nicht auflösen!");
            }

            return _column.Relational().ColumnName;
        }

        private static string GetWheres(List<string> columns)
        {
            var result = "";

            foreach (var c in columns)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += " or ";
                }

                result += "(" + GetWhere(c) + ")";
            }

            return result;
        }

        private static string GetWhere(string column)
        {
            var result = "";

            foreach (var item in SearchChars)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += " or ";
                }

                result += $"{column} like BINARY '%{item}%'";
            }

            //return "CONTAINS(Word, 'a')";
            return result;
        }

        private static List<string> GetStringColumns(Microsoft.EntityFrameworkCore.Metadata.IEntityType entityType)
        {
            List<string> results = new List<string>();

            foreach (var property in entityType.GetProperties().Where(x => x.Relational().ColumnType == "longtext"))
            {
                results.Add(property.Relational().ColumnName);
                //var columnType = property.Relational().ColumnType;
            };

            return results;
        }

        private static string Convert(string sourceText)
        {
            byte[] asciiBytes = Encoding.GetEncoding("windows-1252").GetBytes(sourceText);
            char[] asciiChars = Encoding.UTF8.GetChars(asciiBytes);

            return new string(asciiChars);
        }


        private static bool CreateGetter<T, V>(Expression<Func<T, V>> getterExpression, out Func<T, V> getter)
        {
            if (getterExpression == null)
                throw new ArgumentNullException("getterExpression");

            var memberExpression = getterExpression.Body as MemberExpression;
            if (memberExpression == null || memberExpression.NodeType != ExpressionType.MemberAccess || memberExpression.Member == null)
                throw new ArgumentException("The expression must get a member (property or field).", "getterExpression");

            // The expression passed in is the getter, so just compile it.
            getter = getterExpression.Compile();

            // The setter function takes two parameters as input.
            var paramT = Expression.Parameter(typeof(T));
            var paramV = Expression.Parameter(typeof(V));


            return true;
        }

    }
}