using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SNS.Apps.KPC.Admin.Services
{
    //public enum ClauseOperator
    //{
    //    Like = ".Contains",
    //    Equal = "=",
    //}

    public sealed class ClauseOperator
    {
        private readonly String name;
        private readonly int value;

        public static readonly ClauseOperator Like = new ClauseOperator(1, ".Contains");
        public static readonly ClauseOperator Equal = new ClauseOperator(2, "=");
        private ClauseOperator(int value, String name)
        {
            this.name = name;
            this.value = value;
        }

        public override String ToString()
        {
            return name;
        }
    }


    public class EntityCondition
    {
        public string ClauseKey { get; set; }
        public ClauseOperator ClauseOperator { get; set; }
        public string ClauseValue { get; set; }

        public EntityCondition()
        {
        }

        public EntityCondition(string clauseKey, ClauseOperator clauseOperator, string clauseValue)
        {
            this.ClauseKey = clauseKey;
            this.ClauseOperator = clauseOperator;
            this.ClauseValue = clauseValue;
        }

        public static string GetExpression(string clauseKey, ClauseOperator clauseOperator, string clauseValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}{1}(@{2})", clauseKey, clauseOperator, clauseValue);
            return sb.ToString();

        }
    }


}