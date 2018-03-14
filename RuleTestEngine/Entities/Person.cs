using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleTestEngine.Entities
{
    public class Person
    {
        public string Name { get; set; }       
        public string City { get; set; }
        public double Salary { get; set; }        
        public List<string> skus { get; set; }
    }
}
