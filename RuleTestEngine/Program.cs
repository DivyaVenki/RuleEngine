using RuleTestEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace RuleTestEngine
{
	class Program
	{
		static void Main1(string[] args)
		{
			List<string> lstDb = new List<string> { "abc-666", "abc-222" };
			List<string> lstUI = new List<string> { "abc-6661", "abc-222" };
			var containsEx = GetContainsExpression(lstDb);
			MethodInfo anyMethod = typeof(ExtensibleMethods).GetMethod("AnyMatch", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
			ParameterExpression source = Expression.Parameter(typeof(List<string>), "v");
			var bEXP = Expression.Call(anyMethod, source, Expression.Constant(containsEx.Compile()));
			var lambda = Expression.Lambda<Func<List<string>, bool>>(bEXP, source);
			bool status = lambda.Compile().Invoke(lstUI);
		}


		static void Main(string[] args)
		{
			List<string> _values = new List<string> { "abc-666", "abc-222" };

			List<Person> persons = new List<Person>
			{
				new  Person  { Name = "Flamur" ,  City = "Prishtine" ,   Salary = 12000.0, skus= _values },
				new  Person  { Name = "Blerta" ,  City = "Mitrovice" ,   Salary = 9000.0 , skus= _values},
				new  Person  { Name = "Berat" ,   City = "Peje" ,        Salary = 10000.0 , skus =_values},
				new  Person  { Name = "Laura" ,   City = "Mitrovice" ,   Salary = 25000.0, skus= _values},
				new  Person  { Name = "Olti" ,    City = "Prishtine" ,   Salary = 8000.0, skus=_values},
				new  Person  { Name = "Xhenis" ,  City = "Gjakove" ,     Salary = 7000.0 , skus= _values},
				new  Person  { Name = "Fatos" ,   City = "Peje" ,        Salary = 6000.0, skus= _values},
			};

			List<string> lstUI1 = new List<string> { "abc-666", "abc-222" };
			List<string> lstUI2 = new List<string> { "abc-666", "abc-2221" };
			List<string> lstUI3 = new List<string> { "abc-6661", "abc-222" };
			List<string> lstUI4 = new List<string> { "abc-6661", "abc-2221" };

			List<Filter> filter = new List<Filter>()
			{
				new Filter { PropertyName = "City" , Operation = Op .Equals, Value = "Peje"  },
				new Filter { PropertyName = "Salary" , Operation = Op .GreaterThan, Value = 9000.0 },

				new Filter { PropertyName="Skus", Operation = Op.Contains, Value=lstUI1},
				new Filter { PropertyName="Skus", Operation = Op.Contains, Value=lstUI2},
				new Filter { PropertyName="Skus", Operation = Op.Contains, Value=lstUI3},
				//new Filter { PropertyName="Skus", Operation = Op.Contains, Value=lstUI4},
			};

			var filterClone = filter.Select(v => new Filter() { PropertyName = v.PropertyName, Operation = v.Operation, Value = v.Value }).ToList();

			List<Person> lstSuccessRules = new List<Person>();
			foreach (var p in persons)
			{
				if (filter != null && filter.Count == 0)
				{
					filter = filterClone.Select(v => new Filter() { PropertyName = v.PropertyName, Operation = v.Operation, Value = v.Value }).ToList();
				}
				var deleg = ExpressionBuilder.GetExpression<Person>(filter, p).Compile();
				var lstSinglePerson = new List<Person> { p };
				var filteredCollection = lstSinglePerson.Where(deleg).ToList();
				lstSuccessRules.AddRange(filteredCollection);
			}
		}

		static Expression<Func<string, bool>> GetContainsExpression(List<string> lstDb)
		{
			MethodInfo containsMethod = typeof(List<string>).GetMethod("Contains");
			var uiStringParam = Expression.Parameter(typeof(string), "str");//str=>
			var dbListParam = Expression.Constant(lstDb);//passing list actual data
			Expression expContains = Expression.Call(dbListParam, containsMethod, uiStringParam);//str=> lst.Contains(str)
			var lamdaContains = Expression.Lambda<Func<string, bool>>(expContains, uiStringParam);
			return lamdaContains;
		}
	}
}
