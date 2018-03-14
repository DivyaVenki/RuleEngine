using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleTestEngine.Entities
{
    public class Filter
    {
        public string PropertyName { get; set; }
        public Op Operation { get; set; }
        public dynamic Value { get; set; }
    }
    public enum Op
    {
        Equals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith
    }
}
