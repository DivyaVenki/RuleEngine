using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuleTestEngine.Entities
{
	public class ExpressionBuilder
	{

		private static MethodInfo containsMethod = typeof(List<string>).GetMethod("Contains", new Type[] { typeof(string) });
		private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
		private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
		private static MethodInfo anyMethod = typeof(ExtensibleMethods).GetMethod("AnyMatch", BindingFlags.Public | BindingFlags.Static);

		private static readonly MethodInfo changeTypeMethod = typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(object), typeof(Type) });

		public static Expression<Func<T, bool>> GetExpression<T>(IList<Filter> filters, T t)
		{
			if (filters.Count == 0)
				return null;

			ParameterExpression param = Expression.Parameter(typeof(T), "t");
			Expression exp = null;

			if (filters.Count == 1)
				exp = GetExpression<T>(param, filters[0], t);
			else if (filters.Count == 2)
				exp = GetExpression<T>(param, filters[0], filters[1], t);
			else
			{
				while (filters.Count > 0)
				{
					var f1 = filters[0];
					var f2 = filters[1];

					if (exp == null)
						exp = GetExpression<T>(param, filters[0], filters[1], t);
					else
						exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0], filters[1], t));

					filters.Remove(f1);
					filters.Remove(f2);

					if (filters.Count == 1)
					{
						exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0], t));
						filters.RemoveAt(0);
					}
				}
			}

			return Expression.Lambda<Func<T, bool>>(exp, param);
		}

		private static Expression Convert(Expression from, Type type)
		{
			return Expression.Convert(from, type);
		}

		private static Expression GetExpression<T>(ParameterExpression param, Filter filter, T t)
		{
			MemberExpression member = Expression.Property(param, filter.PropertyName);
			if (filter.Operation == Op.Contains)
			{
				var lamda = Expression.Lambda<Func<T, List<string>>>(member, param);
				var cc = lamda.Compile().Invoke(t);
			}
			ConstantExpression constant = Expression.Constant(filter.Value);


			switch (filter.Operation)
			{
				case Op.Equals:
					return Expression.Equal(member, constant);

				case Op.GreaterThan:
					return Expression.GreaterThan(member, constant);

				case Op.GreaterThanOrEqual:
					return Expression.GreaterThanOrEqual(member, constant);

				case Op.LessThan:
					return Expression.LessThan(member, constant);

				case Op.LessThanOrEqual:
					return Expression.LessThanOrEqual(member, constant);

				case Op.Contains:
					ConstantExpression dbListConstantExpression = Expression.Constant(Expression.Lambda<Func<T, List<string>>>(member, param).Compile().Invoke(t));
					var containsExpression = GetContainsExpression(dbListConstantExpression);
					ParameterExpression source = Expression.Parameter(typeof(List<string>), "v");
					var anyExpression = Expression.Call(anyMethod, source, Expression.Constant(containsExpression.Compile()));
					var lambda = Expression.Lambda<Func<List<string>, bool>>(anyExpression, source);
					bool status = lambda.Compile().Invoke(filter.Value);
					return Expression.Constant(status);
				//return Expression.Call(member, containsMethod, constant);

				case Op.StartsWith:
					return Expression.Call(member, startsWithMethod, constant);

				case Op.EndsWith:
					return Expression.Call(member, endsWithMethod, constant);
			}

			return null;
		}

		private static BinaryExpression GetExpression<T>(ParameterExpression param, Filter filter1, Filter filter2, T t)
		{
			Expression bin1 = GetExpression<T>(param, filter1, t);
			Expression bin2 = GetExpression<T>(param, filter2, t);

			return Expression.And(bin1, bin2);
		}

		static Expression<Func<string, bool>> GetContainsExpression(ConstantExpression constant)
		{
			MethodInfo containsMethod = typeof(List<string>).GetMethod("Contains");
			var uiStringParam = Expression.Parameter(typeof(string), "str");//str=>
			Expression expContains = Expression.Call(constant, containsMethod, uiStringParam);//str=> lst.Contains(str)
			var lamdaContains = Expression.Lambda<Func<string, bool>>(expContains, uiStringParam);

			return lamdaContains;
		}

		private static object GetValue<T>(MemberExpression member)
		{
			var objectMember = Expression.Convert(member, typeof(T));

			var getterLambda = Expression.Lambda<Func<T>>(objectMember);

			var getter = getterLambda.Compile();

			return getter();
		}
	}


}
