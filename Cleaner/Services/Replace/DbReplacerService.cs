using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using Cleaner.Core;
using Cleaner.Core.DB;
using Cleaner.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Cleaner.Core.DB.Entities;
using System.Linq.Expressions;
using System.Reflection;

namespace Cleaner.Services.Replace
{
    public class DbReplacerService : IDbReplacerService
    {
        private readonly ILogger<DbReplacerService> _logger;
        private readonly AppSettings _appSettings;
        private readonly DataDbContext _dataDbContext;

        public DbReplacerService(ILogger<DbReplacerService> logger, IOptions<AppSettings> appSettings, DataDbContext dataDbContext)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _dataDbContext = dataDbContext;

            _logger.LogDebug("DbReplacerService init...");
        }


        public void Stop()
        {
            _logger.LogDebug("DbReplacerService stop...");
        }

        public async Task Replace<T>(Expression<Func<T, long>> idSelector, Expression<Func<T, string>> valueSelector, char[] searchChars, char[] blackChars, bool save = false) where T : class
        {
            Func<T, long> idGetter;
            Action<T, long> idSetter;
            Func<T, string> valueGetter;
            Action<T, string> valueSetter;

            CreateGetterSetter<T, string>(valueSelector, out valueGetter, out valueSetter);
            CreateGetterSetter<T, long>(idSelector, out idGetter, out idSetter);

            Regex regex = new Regex("(.+)(ini)(.+)");

            var entities = await _dataDbContext.Set<T>().ToListAsync();

            List<T> results = new List<T>();
            foreach (var item in searchChars)
            {
                var r = entities.Where(x => !string.IsNullOrEmpty(valueGetter(x).Trim()) && valueGetter(x).Trim().Contains(item));
                if (r != null && r.Any())
                {
                    results.AddRange(r);
                }
            }

            var count = results.Count();

            try
            {
                foreach (var item in results)
                {
                    var result = Clear(idGetter(item), valueGetter(item).Trim(), searchChars, blackChars);
                    if (result.Ok)
                    {
                        valueSetter(item, result.Text.Trim());

                        if (save)
                        {
                            _dataDbContext.Update(item);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            if (save)
            {
                _dataDbContext.SaveChanges();
            }

            Console.WriteLine("FERTIG!");

            await Task.CompletedTask;

        }

        #region private
        private ClearResult Clear(long id, string dirtyWord, char[] searchChars, char[] blackChars)
        {
            var oldWord = dirtyWord.Trim();

            while (CheckSearchChars(searchChars, dirtyWord.Trim()).Found)
            {
                var newWord = Convert(dirtyWord.Trim()).Trim();

                if (CheckBlackChars(blackChars, newWord.Trim()))
                {
                    Console.WriteLine(string.Format("NO  {0,-10} {1,50} = {2}", id, oldWord, newWord));
                    return new ClearResult
                    {
                        Text = "'" + oldWord + "'",
                        Ok = false
                    };
                }
                else
                {
                    dirtyWord = newWord;
                }
            }

            Console.WriteLine(string.Format("YES {0,-10} {1,50} = {2}", id, oldWord, dirtyWord));

            return new ClearResult
            {
                Text = "'" + dirtyWord + "'",
                Ok = true
            };
        }

        private CheckResult CheckSearchChars(char[] searchChars, string row)
        {
            foreach (var item in searchChars)
            {
                if (row.Contains(item))
                {
                    return new CheckResult
                    {
                        Found = true,
                        FoundChar = item
                    };
                }
            }

            return new CheckResult
            {
                Found = false,
            };
        }

        private bool CheckBlackChars(char[] blackChars, string row)
        {
            foreach (var item in blackChars)
            {
                if (row.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        public string Convert(string sourceText)
        {
            byte[] asciiBytes = Encoding.GetEncoding("windows-1252").GetBytes(sourceText);
            char[] asciiChars = Encoding.UTF8.GetChars(asciiBytes);

            return new string(asciiChars);
        }

        private bool CreateGetterSetter<T, V>(Expression<Func<T, V>> getterExpression, out Func<T, V> getter, out Action<T, V> setter)
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

            // Create the setter function
            if (memberExpression.Member.MemberType == MemberTypes.Field)
            {
                // The getter retrieves a field. 
                var field = memberExpression.Member as FieldInfo;
                if (field == null)
                    throw new ArgumentException("Could not get field info for member " + memberExpression.Member.ToString());

                // This expression represents the field on the parameter.
                var fieldExpr = Expression.Field(paramT, field);

                // This expression is what the field will be set to.
                Expression rightExpr;
                if (paramV.Type == field.FieldType)
                    rightExpr = paramV;
                else
                    rightExpr = Expression.Convert(paramV, field.FieldType);

                // This is the assignment expression (sets the field to the parameter value).
                var assign = Expression.Assign(fieldExpr, rightExpr);

                // Compile the expressions into a lambda.
                setter = Expression.Lambda<Action<T, V>>(assign, paramT, paramV).Compile();
            }
            else if (memberExpression.Member.MemberType == MemberTypes.Property)
            {
                // The getter retrieves a property.
                var property = memberExpression.Member as PropertyInfo;
                if (property == null)
                    throw new ArgumentException("Could not get property info for member " + memberExpression.Member.ToString());

                // This expression represents what the property will be set to.
                Expression paramSet;
                if (paramV.Type == property.PropertyType)
                    paramSet = paramV;
                else
                    paramSet = Expression.Convert(paramV, property.PropertyType);

                // Handle read-only properties (have no setter).
                if (!property.CanWrite)
                {
                    setter = (t, v) => Debug.Write("Cannot set read-only property: " + property.ToString());
                    return false;
                }

                // This expression represents calling the property setter.
                var call = Expression.Call(paramT, property.SetMethod, paramSet);

                // Compile the expressions into a lambda.
                setter = Expression.Lambda<Action<T, V>>(call, paramT, paramV).Compile();
            }
            else
            {
                // The member was not a field or a property.
                setter = (t, v) => Debug.Write("Setter invoked for invalid expression");
                return false;
            }
            return true;
        }
        #endregion
    }
}
