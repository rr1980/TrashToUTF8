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
using System.IO;

namespace Cleaner.Services.Replace
{
    public class DbReplacerService : IDbReplacerService
    {
        private readonly ILogger<DbReplacerService> _logger;
        private readonly AppSettings _appSettings;
        private readonly DataDbContext _dataDbContext;
        private readonly DataOldDbContext _dataOldDbContext;

        public DbReplacerService(ILogger<DbReplacerService> logger, IOptions<AppSettings> appSettings, DataDbContext dataDbContext, DataOldDbContext dataOldDbContext)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _dataDbContext = dataDbContext;
            _dataOldDbContext = dataOldDbContext;

            _logger.LogDebug("DbReplacerService init...");
        }


        public void Stop()
        {
            _logger.LogDebug("DbReplacerService stop...");
        }

        public async Task ReplaceHugos<T>(Expression<Func<T, long>> idSelector, Expression<Func<T, string>> valueSelector, Expression<Func<T, string>> langSelector, char[] searchChars, string[] includes, Expression<Func<T, bool>> searchParameter = null, string name = null) where T : class, IEntity
        {
            Func<T, long> idGetter;
            Func<T, string> langGetter;


            Func<T, string> valueGetter;
            Action<T, string> valueSetter;

            CreateGetterSetter(valueSelector, out valueGetter, out valueSetter);
            CreateGetter(idSelector, out idGetter);
            CreateGetter(langSelector, out langGetter);

            var _name = string.IsNullOrEmpty(name) ? typeof(T).Name : name;

            //_dataDbContext.Database.OpenConnection();
            List<T> entities = null;
            if (searchParameter != null)
            {
                entities = await _dataDbContext.Set<T>().Where(searchParameter).ToListAsync();
            }
            else
            {
                entities = await _dataDbContext.Set<T>().ToListAsync();
            }

            if (entities == null)
            {
                return;
            }

            var path = Path.Combine(@"D:\Projekte\TrashToUTF8\Cleaner\Results", "Repaired_" + _name + ".txt");

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Console.WriteLine("Suche in: " + _name);

            List<T> results = new List<T>();


            foreach (var entity in entities)
            {
                string value = null;
                try
                {
                    value = valueGetter(entity);
                }
                catch (Exception)
                {
                    continue;
                }

                foreach (var item in searchChars)
                {
                    if (value != null && !string.IsNullOrEmpty(value) && value.Contains(item))
                    {
                        results.Add(entity);
                    }
                }
            }

            var count = results.Count();

            var ids = results.Select(x => x.Id);

            var olds = await _dataOldDbContext.Set<T>().Where(x => ids.Contains(idGetter(x))).ToListAsync();

            using (StreamWriter outputFile = new StreamWriter(path))
            {
                var counter = 0;

                foreach (var item in results)
                {
                    counter++;

                    var old = olds.FirstOrDefault(x => idGetter(x) == idGetter(item));
                    if(old == null)
                    {
                        continue;
                    }

                    var percent = CalculateSimilarity(valueGetter(item), valueGetter(old));
                    Console.WriteLine(string.Format("{0}  {1,-10} {2,50} = {3, -50} {4}%", counter + "/" + results.Count(), idGetter(item), valueGetter(item), valueGetter(old), percent));

                    if (percent < 50)
                    {
                        ConsoleKeyInfo keyInput;

                        Console.WriteLine("LeftArrow for NO / RightArrow for YES");
                        do
                        {
                            keyInput = Console.ReadKey();
                        } while (keyInput.Key != ConsoleKey.RightArrow && keyInput.Key != ConsoleKey.LeftArrow);

                        if (keyInput.Key == ConsoleKey.RightArrow)
                        {
                            outputFile.WriteLine(string.Format("{0,-10} {1,-10} {2,50} {3,-50} {4}%", idGetter(item), langGetter(item), valueGetter(item), valueGetter(old), percent));

                            valueSetter(item, valueGetter(old));
                            _dataDbContext.Update(item);


                            Console.WriteLine("Übernehmen");
                        }
                        else if (keyInput.Key == ConsoleKey.LeftArrow)
                        {
                            Console.WriteLine("NOT");
                        }
                    }
                    else
                    {
                        outputFile.WriteLine(string.Format("{0,-10} {1,-10} {2,50} {3,-50} {4}%", idGetter(item), langGetter(item), valueGetter(item), valueGetter(old), percent));

                        valueSetter(item, valueGetter(old));
                        _dataDbContext.Update(item);

                        //outputFile.WriteLine(string.Format("{0,-10} {1,-10} {2,50} {2,-50}", idGetter(item), langGetter(item), valueGetter(item), valueGetter(old)));
                    }

                    //outputFile.WriteLine(string.Format("{0,-10} {1,-10} {2,-50}", idGetter(item), langGetter(item), valueGetter(item)));

                }

                await _dataDbContext.SaveChangesAsync();
                //outputFile.WriteLine(Environment.NewLine + "Gefunden: " + count.ToString("N0"));
            }


            //_dataDbContext.Database.CloseConnection();

            Console.WriteLine("FERTIG!");

            await Task.CompletedTask;

        }

        double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 100;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            return Math.Round((1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length))) * 100);
        }

        int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }

        //public async Task FindHugos<T>(Expression<Func<T, long>> idSelector, Expression<Func<T, string>> valueSelector, Expression<Func<T, string>> langSelector, char[] searchChars, Expression<Func<T, object>>[] includes, Expression<Func<T, bool>> searchParameter = null, string name = null) where T : class
        public async Task FindHugos<T>(Expression<Func<T, long>> idSelector, Expression<Func<T, string>> valueSelector, Expression<Func<T, string>> langSelector, char[] searchChars, string[] includes, Expression<Func<T, bool>> searchParameter = null, string name = null) where T : class
        {
            Func<T, long> idGetter;
            Func<T, string> valueGetter;
            Func<T, string> langGetter;

            CreateGetter(valueSelector, out valueGetter);
            CreateGetter(idSelector, out idGetter);
            CreateGetter(langSelector, out langGetter);

            var _name = string.IsNullOrEmpty(name) ? typeof(T).Name : name;

            _dataDbContext.Database.OpenConnection();
            List<T> entities = null;
            if (searchParameter != null)
            {
                entities = await _dataDbContext.Set<T>().Where(searchParameter).ToListAsync();

                //foreach (var item in includes)
                //{
                //    _entities.Include(item);
                //}

                //entities = await _entities.Where(searchParameter).ToListAsync();
            }
            else
            {
                entities = await _dataDbContext.Set<T>().ToListAsync();

                //foreach (var item in includes)
                //{
                //    _entities.Include(item);
                //}

                //entities = await _entities.ToListAsync();
            }

            if (entities == null)
            {
                return;
            }

            var path = Path.Combine(@"D:\Projekte\TrashToUTF8\Cleaner\Results", "Bad_" + _name + ".txt");

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Console.WriteLine("Suche in: " + _name);

            List<T> results = new List<T>();


            foreach (var entity in entities)
            {
                string value = null;
                try
                {
                    value = valueGetter(entity);
                }
                catch (Exception)
                {
                    continue;
                }

                foreach (var item in searchChars)
                {
                    if (value != null && !string.IsNullOrEmpty(value) && value.Contains(item))
                    {
                        results.Add(entity);
                    }
                }
            }

            //foreach (var item in searchChars)
            //{
            //foreach (var entity in entities)
            //{
            //    var value = valueGetter(entity);
            //    if(value != null && !string.IsNullOrEmpty(value) && value.Contains(item))
            //    {
            //        results.Add(entity);
            //    }
            //}

            //var r = entities.Where(x => valueGetter(x) != null && !string.IsNullOrEmpty(valueGetter(x).Trim()) && valueGetter(x).Trim().Contains(item));
            //if (r != null && r.Any())
            //{
            //    results.AddRange(r);
            //}
            //}

            var count = results.Count();

            Console.WriteLine();

            using (StreamWriter outputFile = new StreamWriter(path))
            {
                foreach (var item in results)
                {
                    outputFile.WriteLine(string.Format("{0,-10} {1,-10} {2,-50}", idGetter(item), langGetter(item), valueGetter(item)));
                }

                outputFile.WriteLine(Environment.NewLine + "Gefunden: " + count.ToString("N0"));
            }


            _dataDbContext.Database.CloseConnection();

            Console.WriteLine("FERTIG!");

            await Task.CompletedTask;

        }

        public async Task Replace<T>(Expression<Func<T, long>> idSelector, Expression<Func<T, string>> valueSelector, char[] searchChars, char[] blackChars, Expression<Func<T, bool>> searchParameter = null, bool save = false) where T : class
        {
            Func<T, long> idGetter;
            Action<T, long> idSetter;
            Func<T, string> valueGetter;
            Action<T, string> valueSetter;

            CreateGetterSetter(valueSelector, out valueGetter, out valueSetter);
            CreateGetterSetter(idSelector, out idGetter, out idSetter);

            List<T> entities = null;
            if (searchParameter != null)
            {
                entities = await _dataDbContext.Set<T>().Where(searchParameter).ToListAsync();
            }
            else
            {
                entities = await _dataDbContext.Set<T>().ToListAsync();
            }

            if (entities == null)
            {
                return;
            }

            List<T> results = new List<T>();
            foreach (var item in searchChars)
            {
                var r = entities.Where(x => valueGetter(x) != null && !string.IsNullOrEmpty(valueGetter(x).Trim()) && valueGetter(x).Trim().Contains(item));
                if (r != null && r.Any())
                {
                    results.AddRange(r);
                }
            }

            var count = results.Count();

            int clearCounter = 0;
            int impossibleCounter = 0;

            //try
            //{
            foreach (var item in results)
            {
                try
                {
                    var result = Clear(idGetter(item), valueGetter(item).Trim(), searchChars, blackChars);
                    if (result.Type == ClearResultType.Fixed)
                    {
                        clearCounter++;

                        if (save)
                        {
                            valueSetter(item, result.Text.Trim());
                            _dataDbContext.Update(item);
                        }
                    }
                    else if (result.Type == ClearResultType.Impossible)
                    {
                        impossibleCounter++;

                        if (save)
                        {
                            valueSetter(item, result.Text.Trim());
                            _dataDbContext.Update(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            Console.WriteLine("Cleared: " + clearCounter);
            Console.WriteLine("Impossible: " + impossibleCounter);

            if (save)
            {
                try
                {
                    await _dataDbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
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

                var cbc = CheckBlackChars(blackChars, newWord.Trim());
                if (cbc.Found)
                {
                    Console.WriteLine(string.Format("NO  {0,-10} {1,50} = {2, -50} {3}", id, oldWord, newWord, cbc.FoundChar));
                    return new ClearResult
                    {
                        Text = newWord,
                        Type = ClearResultType.Impossible
                    };
                }
                else
                {
                    dirtyWord = newWord;
                }
            }

            if (oldWord.Trim() != dirtyWord.Trim())
            {
                Console.WriteLine(string.Format("YES {0,-10} {1,50} = {2,-50} {3}", id, oldWord, dirtyWord, CheckSearchChars(searchChars, oldWord.Trim()).FoundChar));
                return new ClearResult
                {
                    Text = dirtyWord,
                    Type = ClearResultType.Fixed
                };
            }

            return new ClearResult
            {
                Text = dirtyWord,
                Type = ClearResultType.Unnecessary
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

        private CheckResult CheckBlackChars(char[] blackChars, string row)
        {
            foreach (var item in blackChars)
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

        private bool CreateGetter<T, V>(Expression<Func<T, V>> getterExpression, out Func<T, V> getter)
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
        #endregion
    }
}
