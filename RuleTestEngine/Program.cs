using RuleTestEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RuleTestEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> _values = new List<string> { "abc-666","abc-222" };

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

            List<string> _value = new List<string> { "abc-666","abc-222" };
            List<Filter> filter = new List<Filter>()
            {
                new Filter { PropertyName = "City" , Operation = Op .Equals, Value = "Peje"  },
                new Filter { PropertyName = "Salary" , Operation = Op .GreaterThan, Value = 9000.0 },
                new Filter { PropertyName="Skus", Operation = Op.Contains, Value=_value}

            };

            

            var deleg = ExpressionBuilder.GetExpression<Person>(filter).Compile();



            var filteredCollection = persons.Where(deleg).ToList();
        }

        // Author: Cole Francis, Architect
        /// The pre-compiled rules type
        /// 
        ////public class PrecompiledRules
        ////{
        ////    ///
        ////    /// A method used to precompile rules for a provided type
        ////    /// 
        ////    public static List<Func<T, bool>> CompileRule<T>(List<T> targetEntity, List<Filter <List<string>>> rules)
        ////    {
        ////        var compiledRules = new List<Func<T, bool>>();

        ////        // Loop through the rules and compile them against the properties of the supplied shallow object 
        ////        rules.ForEach(rule =>
        ////        {
        ////            var genericType = Expression.Parameter(typeof(T));
        ////            var key = MemberExpression.Property(genericType, rule.ComparisonPredicate);
        ////            var propertyType = typeof(T).GetProperty(rule.ComparisonPredicate).PropertyType;
        ////            var value = Expression.Constant(Convert.ChangeType(rule.ComparisonValue, propertyType));
        ////            var binaryExpression = Expression.MakeBinary(rule.ComparisonOperator, key, value);

        ////            compiledRules.Add(Expression.Lambda<Func<T, bool>>(binaryExpression, genericType).Compile());
        ////        });

        ////        // Return the compiled rules to the caller
        ////        return compiledRules;
        ////    }
        ////}


    }
}
