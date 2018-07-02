using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace dF.Commons.Services.BL
{
    public static class ExpressionKeys
    {
        public static IDictionary<string, object> Unwrap<T>(Expression<Func<T, bool>>[] expressionKeys) where T : class
        {
            try
            {
                if (expressionKeys.Count() == 0)
                    return null;

                var keyValues = new Dictionary<string, object>();

                foreach (var key in expressionKeys)
                {
                    var context = new Dictionary<string, object>();
                    if (ExtractKeyValues(key as Expression, context))
                    {
                        if (context.ContainsKey(keyConst) && context.ContainsKey(valueConst))
                            keyValues.Add(context[keyConst] as string, context[valueConst]);
                    }
                    else
                        return null;
                }

                return keyValues;
            }
            catch (Exception)
            {
                return null;
            }
        }

        const string paramNameConst = "p";
        const string keyConst = "k";
        const string valueConst = "v";

        /// <see cref="https://devio.wordpress.com/2011/08/05/converting-linq-expressions-to-t-sql/"/>
        /// <seealso cref="https://stackoverflow.com/questions/2616638/access-the-value-of-a-member-expression?rq=1"/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        static bool ExtractKeyValues(Expression expression, IDictionary<string, object> context)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    var l = expression as LambdaExpression;
                    if (l.Parameters.Count == 1 && !context.ContainsKey(paramNameConst))
                    {
                        context.Add(paramNameConst, l.Parameters[0].Name);
                        return ExtractKeyValues(l.Body, context);
                    }
                    else
                        return false;
                case ExpressionType.Equal:
                    var equal = expression as BinaryExpression;

                    if (ExtractKeyValues(equal.Left, context) && ExtractKeyValues(equal.Right, context))
                        return true;

                    return false;
                case ExpressionType.MemberAccess:
                    var memberAccess = expression as MemberExpression;

                    switch (memberAccess.Expression.NodeType)
                    {
                        case ExpressionType.Parameter:
                            if (memberAccess.Member.MemberType == MemberTypes.Property && context.ContainsKey(paramNameConst) && (memberAccess.Expression as ParameterExpression).Name == context[paramNameConst] as string)
                            {
                                context.Add(keyConst, memberAccess.Member.Name);
                                return true;
                            }

                            return false;
                        case ExpressionType.Constant:
                            var constantExpressionInMember = memberAccess.Expression as ConstantExpression;

                            object constValue = null;

                            if (memberAccess.Member.MemberType == MemberTypes.Property)
                                constValue = ((PropertyInfo)memberAccess.Member)?.GetValue(constantExpressionInMember.Value);
                            else if (memberAccess.Member.MemberType == MemberTypes.Field)
                                constValue = ((FieldInfo)memberAccess.Member)?.GetValue(constantExpressionInMember.Value);

                            if (constValue != null)
                            {
                                context.Add(valueConst, constValue);
                                return true;
                            }

                            return false;
                        case ExpressionType.MemberAccess:
                            if (ExtractKeyValues(memberAccess.Expression, context))
                            {
                                var memberValue = ((PropertyInfo)memberAccess.Member).GetValue(context[valueConst], null);

                                if (memberValue != null)
                                {
                                    context[valueConst] = memberValue;
                                    return true;
                                }
                            }

                            return false;
                    }

                    return false;
                case ExpressionType.Constant:
                    var constantExpression = expression as ConstantExpression;
                    if (constantExpression.Type.IsPrimitive)
                    {
                        context.Add(valueConst, constantExpression.Value);
                        return true;
                    }

                    return false;
                case ExpressionType.Convert:
                    var convertExpression = expression as UnaryExpression;

                    if (ExtractKeyValues(convertExpression.Operand, context))
                        return true;

                    return false;
            }

            return false;
        }

        public static bool Remove<T>(ref Expression<Func<T, bool>>[] expressionKeys, IDictionary<string, object> keyValues, string keyToRemove) where T : class
        {
            if (string.IsNullOrWhiteSpace(keyToRemove) || expressionKeys.Count() != keyValues.Count)
                return false;

            var i = keyValues.Keys.ToList().IndexOf(keyToRemove);

            if (keyValues.ElementAt(i).Key == keyToRemove)
            {
                var newExpressionKeys = expressionKeys.ToList();
                newExpressionKeys.RemoveAt(i);
                expressionKeys = newExpressionKeys.ToArray();

                keyValues.Remove(keyToRemove);

                return true;
            }

            return false;
        }

        public static bool Add<T>(ref Expression<Func<T, bool>>[] expressionKeys, Expression<Func<T, bool>> newExpressionKey, IDictionary<string, object> keyValues, KeyValuePair<string, object> newKeyValue) where T : class
        {
            if (expressionKeys.Count() != keyValues.Count)
                return false;

            var newExpressionKeys = expressionKeys.ToList();
            newExpressionKeys.Add(newExpressionKey);
            expressionKeys = newExpressionKeys.ToArray();

            keyValues.Add(newKeyValue);

            return true;
        }
    }
}
