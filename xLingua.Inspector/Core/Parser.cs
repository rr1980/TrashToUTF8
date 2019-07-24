using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using xLingua.Entities;

namespace xLingua.Inspector.Core
{
    public class Parser<T> where T : class, IEntity
    {
        Expression<Func<T, long>> idProp;
        Expression<Func<T, string>> columnProp;
        char[] searchChars;
        BaseResolver<T> resolver = null;
        bool save = false;

        public Parser(Expression<Func<T, long>> idProp, Expression<Func<T, string>> columnProp, char[] searchChars, BaseResolver<T> resolver = null, bool save = false)
        {
            this.idProp = idProp;
            this.columnProp = columnProp;
            this.searchChars = searchChars;
            this.resolver = resolver;
            this.save = save;
        }

        public List<T> Parse()
        {
            List<T> results = null;

            Console.WriteLine("Start... " + typeof(T).Name + " - " + GetName(columnProp));

            using (var _context = new DataDbContext())
            {
                var _entityType = _context.Model.FindEntityType(typeof(T));
                var _tableName = _entityType.Relational().TableName;
                var _column = GetColumnName(_entityType, columnProp);

                string _where = GetWhere(_column, searchChars);

                var _command = $"SELECT * FROM {_tableName} WHERE {_where}";

#pragma warning disable EF1000 // Possible SQL injection vulnerability.
                results = _context.Set<T>().FromSql(_command).AsNoTracking().ToList();
#pragma warning restore EF1000 // Possible SQL injection vulnerability.

                if (resolver != null)
                {
                    foreach (var entity in results)
                    {
                        resolver.PreResolve(entity, idProp, columnProp);

                        if (save)
                        {
                            _context.Update(entity);
                        }
                    }

                    resolver.Close();
                    //resolver.Dispose();
                }

                if (save)
                {
                    _context.SaveChanges();
                }
            }

            Console.WriteLine("Fertig... " + typeof(T).Name + " - " + GetName(columnProp));

            return results;
        }

        private string GetColumnName(IEntityType entityType, Expression<Func<T, string>> column)
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

        private string GetWhere(string column, char[] searchChars)
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

        private string GetName<TSource, TField>(Expression<Func<TSource, TField>> Field)
        {
            return (Field.Body as MemberExpression ?? ((UnaryExpression)Field.Body).Operand as MemberExpression).Member.Name;
        }
    }
}