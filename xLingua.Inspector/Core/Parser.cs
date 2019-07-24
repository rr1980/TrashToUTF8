using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using xLingua.Entities;

namespace xLingua.Inspector.Core
{
    public static class Parser
    {
        public static List<T> Parse<T>(Expression<Func<T, long>> idProp, Expression<Func<T, string>> columnProp, char[] searchChars, BaseResolver resolver = null) where T : class
        {
            List<T> results = null;

            using (var _context = new DataDbContext())
            {
                var _entityType = _context.Model.FindEntityType(typeof(T));
                var _tableName = _entityType.Relational().TableName;
                var _column = GetColumnName(_entityType, columnProp);

                string _where = GetWhere(_column, searchChars);

                var _command = $"SELECT * FROM {_tableName} WHERE {_where}";

                results = _context.Set<T>().FromSql(_command).AsNoTracking().ToList();

                if (resolver != null)
                {
                    foreach (var entity in results)
                    {
                       resolver.PreResolve<T>(entity, idProp, columnProp);
                    }
                }
            }

            return results;
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

        private static string GetWhere(string column, char[] searchChars)
        {
            var result = "";

            foreach (var item in searchChars)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += " or ";
                }

                result += $"{column} like BINARY '%{item}%'";
            }

            return result;
        }

        private static string GetName<TSource, TField>(Expression<Func<TSource, TField>> Field)
        {
            return (Field.Body as MemberExpression ?? ((UnaryExpression)Field.Body).Operand as MemberExpression).Member.Name;
        }
    }
}