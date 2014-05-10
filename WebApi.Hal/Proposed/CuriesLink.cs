using System;
using System.Linq;
using System.Text;

namespace WebApi.Hal.Proposed
{
    public class CuriesLink
    {
        const string CuriesRelExpression = "rel";

        public CuriesLink(string name, string href)
        {
            if (string.IsNullOrEmpty(name)) 
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(href)) 
                throw new ArgumentNullException("href");

            if (!IsValidCuriesHref(href))
                throw new ArgumentException("The provided href is not a valid uri template: " + href);

            Name = name;
            Href = href;
        }

        public string Name { get; private set; }
        public string Href { get; private set; }

        private static bool IsValidCuriesHref(string template)
        {
            if (string.IsNullOrEmpty(template))
                return false;

            var expression = new StringBuilder();
            var building = false;
            var foundRel = false;

            foreach (var c in template)
            {
                switch (c)
                {
                    case '{':
                        if (foundRel)
                            return false; // only a single "rel" expression is allowed in this template ...
                        building = true;
                        expression.Clear();
                        break;
                    case '}':
                        if (!IsValidCuriesHrefRelExpression(expression.ToString()))
                            return false; // only a single "rel" expression is allowed in this template ...
                        building = false;
                        foundRel = true;
                        break;
                    default:
                        if (building)
                            expression.Append(c);
                        break;
                }
            }

            return foundRel;
        }

        private static bool IsValidCuriesHrefRelExpression(string expression)
        {
            if (expression.Equals(CuriesRelExpression, StringComparison.OrdinalIgnoreCase))
                return true;

            var operators = new[] { '+', ';', '/', '#', '&', '?', '.' };
            var first = expression[0];

            if (operators.Any(o => o == first))
                return expression.Substring(1).Equals(CuriesRelExpression, StringComparison.OrdinalIgnoreCase);

            return false; // only a single "rel" expression is allowed in this template ...
        }
    }
}