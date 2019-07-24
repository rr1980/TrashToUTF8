using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace xLingua.Inspector.Core
{
    public abstract class BaseResolver<T> where T : class
    {
        private string _logPath_bad;
        private string _logPath_good;
        private StreamWriter _sw_bad;
        private StreamWriter _sw_good;



        public BaseResolver(string logPath, Expression<Func<T, string>> columnProp)
        {
            if (!string.IsNullOrEmpty(logPath))
            {
                //this._logPath = Path.Combine(logPath, DateTime.Now.ToString().Replace(':', '_').Replace('.', '_').Replace(' ', '_') + string.Format("_Bad_{0}_{1}.txt", typeof(T).Name, GetName(columnProp)));
                this._logPath_bad = Path.Combine(logPath, DateTime.Now.ToShortDateString().Replace('.', '_') + string.Format("_Bad_{0}_{1}.txt", typeof(T).Name, GetName(columnProp)));
                this._logPath_good = Path.Combine(logPath, DateTime.Now.ToShortDateString().Replace('.', '_') + string.Format("_Good_{0}_{1}.txt", typeof(T).Name, GetName(columnProp)));

                if (File.Exists(this._logPath_bad))
                {
                    File.Delete(this._logPath_bad);
                }

                if (File.Exists(this._logPath_good))
                {
                    File.Delete(this._logPath_good);
                }

            }
        }

        private string GetName<TSource, TField>(Expression<Func<TSource, TField>> Field)
        {
            return (Field.Body as MemberExpression ?? ((UnaryExpression)Field.Body).Operand as MemberExpression).Member.Name;
        }

        protected void Print(string msg = "")
        {
            Console.WriteLine(msg);
        }

        protected void LogBad(string msg = "")
        {
            Console.WriteLine(msg);

            if (!string.IsNullOrEmpty(_logPath_bad))
            {
                if (_sw_bad == null)
                {
                    _sw_bad = new StreamWriter(_logPath_bad);
                }

                _sw_bad.WriteLine(msg);
            }
        }

        internal void Close()
        {
            if (_sw_bad != null)
            {
                _sw_bad.Close();
            }

            if (_sw_good != null)
            {
                _sw_good.Close();
            }
        }

        protected void LogGood(string msg = "")
        {
            Console.WriteLine(msg);

            if (!string.IsNullOrEmpty(_logPath_good))
            {
                if (_sw_good == null)
                {
                    _sw_good = new StreamWriter(_logPath_good);
                }

                _sw_good.WriteLine(msg);
            }
        }

        public void PreResolve(T entitiy, Expression<Func<T, long>> idProp, Expression<Func<T, string>> columnProp)
        {
            Func<T, long> idGetter;
            Func<T, string> valueGetter;
            Action<T, string> valueSetter;

            CreateGetter(idProp, out idGetter);
            CreateGetterSetter(columnProp, out valueGetter, out valueSetter);

            var result = Resolve(idGetter(entitiy), valueGetter(entitiy));

            valueSetter(entitiy, result);
        }

        public abstract string Resolve(long id, string dirtyValue);

        private static bool CreateGetterSetter<V>(Expression<Func<T, V>> getterExpression, out Func<T, V> getter, out Action<T, V> setter)
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

        private static bool CreateGetter<V>(Expression<Func<T, V>> getterExpression, out Func<T, V> getter)
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