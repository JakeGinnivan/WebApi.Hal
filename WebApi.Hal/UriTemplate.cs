using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApi.Hal
{
    /// <summary>
    /// this is https://github.com/tavis-software/UriTemplates
    /// an RFC6570-compliant level-4 UriTemplate handler
    /// </summary>
    public class UriTemplate
    {
        private const string UriReservedSymbols = ":/?#[]@!$&'()*+,;=";
        private const string UriUnreservedSymbols = "-._~";

        private static readonly Dictionary<char, OperatorInfo> operators = new Dictionary<char, OperatorInfo>
        {
            {'\0', new OperatorInfo {Default = true, First = "", Seperator = ',', Named = false, IfEmpty = "",AllowReserved = false}},
            {'+', new OperatorInfo {Default = false, First = "", Seperator = ',', Named = false, IfEmpty = "",AllowReserved = true}},
            {'.', new OperatorInfo {Default = false, First = ".", Seperator = '.', Named = false, IfEmpty = "",AllowReserved = false}},
            {'/', new OperatorInfo {Default = false, First = "/", Seperator = '/', Named = false, IfEmpty = "",AllowReserved = false}},
            {';', new OperatorInfo {Default = false, First = ";", Seperator = ';', Named = true, IfEmpty = "",AllowReserved = false}},
            {'?', new OperatorInfo {Default = false, First = "?", Seperator = '&', Named = true, IfEmpty = "=",AllowReserved = false}},
            {'&', new OperatorInfo {Default = false, First = "&", Seperator = '&', Named = true, IfEmpty = "=",AllowReserved = false}},
            {'#', new OperatorInfo {Default = false, First = "#", Seperator = ',', Named = false, IfEmpty = "",AllowReserved = true}}
        };

        private readonly string template;
        private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

        private enum States
        {
            CopyingLiterals,
            ParsingExpression
        }

        private bool errorDetected;
        private StringBuilder result;
        private List<string> parameterNames;

        public UriTemplate(string template)
        {
            this.template = template;
        }


        public void SetParameter(string name, object value)
        {
            parameters[name] = value;
        }

        public void SetParameter(string name, string value)
        {
            parameters[name] = value;
        }

        public void SetParameter(string name, IEnumerable<string> value)
        {
            parameters[name] = value;
        }

        public void SetParameter(string name, IDictionary<string, string> value)
        {
            parameters[name] = value;
        }


        public IEnumerable<string> GetParameterNames()
        {
            var names = new List<string>();
            parameterNames = names;
            Resolve();
            parameterNames = null;
            return names;
        }

        public string Resolve()
        {
            var currentState = States.CopyingLiterals;
            result = new StringBuilder();
            StringBuilder currentExpression = null;
            foreach (char character in template)
            {
                switch (currentState)
                {
                    case States.CopyingLiterals:
                        if (character == '{')
                        {
                            currentState = States.ParsingExpression;
                            currentExpression = new StringBuilder();
                        }
                        else if (character == '}')
                        {
                            throw new ArgumentException("Malformed template, unexpected } : " + result);
                        }
                        else
                        {
                            result.Append(character);
                        }
                        break;
                    case States.ParsingExpression:
                        if (character == '}')
                        {
                            ProcessExpression(currentExpression);

                            currentState = States.CopyingLiterals;
                        }
                        else
                        {
                            currentExpression?.Append(character);
                        }

                        break;
                }
            }
            if (currentState == States.ParsingExpression)
            {
                result.Append("{");
                result.Append(currentExpression);

                throw new ArgumentException("Malformed template, missing } : " + result);
            }

            if (errorDetected)
            {
                throw new ArgumentException("Malformed template : " + result);
            }
            return result.ToString();
        }

        private void ProcessExpression(StringBuilder currentExpression)
        {

            if (currentExpression.Length == 0)
            {
                errorDetected = true;
                result.Append("{}");
                return;
            }

            OperatorInfo op = GetOperator(currentExpression[0]);

            int firstChar = op.Default ? 0 : 1;


            var varSpec = new VarSpec(op);
            for (int i = firstChar; i < currentExpression.Length; i++)
            {
                char currentChar = currentExpression[i];
                switch (currentChar)
                {
                    case '*':
                        varSpec.Explode = true;
                        break;
                    case ':': // Parse Prefix Modifier
                        var prefixText = new StringBuilder();
                        currentChar = currentExpression[++i];
                        while (currentChar >= '0' && currentChar <= '9' && i < currentExpression.Length)
                        {
                            prefixText.Append(currentChar);
                            i++;
                            if (i < currentExpression.Length) currentChar = currentExpression[i];
                        }
                        varSpec.PrefixLength = int.Parse(prefixText.ToString());
                        i--;
                        break;
                    case ',':
                        bool success = ProcessVariable(varSpec);
                        bool isFirst = varSpec.First;
                        // Reset for new variable
                        varSpec = new VarSpec(op);
                        if (success || !isFirst) varSpec.First = false;

                        break;


                    default:
                        if (IsVarNameChar(currentChar))
                        {
                            varSpec.VarName.Append(currentChar);
                        }
                        else
                        {
                            errorDetected = true;
                        }
                        break;
                }
            }
            ProcessVariable(varSpec);

        }

        private bool ProcessVariable(VarSpec varSpec)
        {
            string varname = varSpec.VarName.ToString();
            parameterNames?.Add(varname);

            if (!parameters.ContainsKey(varname)
                || parameters[varname] == null
                || (parameters[varname] is IList && ((IList)parameters[varname]).Count == 0)
                || (parameters[varname] is IDictionary && ((IDictionary)parameters[varname]).Count == 0))
                return false;

            if (varSpec.First)
            {
                result.Append(varSpec.OperatorInfo.First);
            }
            else
            {
                result.Append(varSpec.OperatorInfo.Seperator);
            }

            object value = parameters[varname];

            // Handle Strings
            var stringValue = value as string;
            if (stringValue != null)
            {
                if (varSpec.OperatorInfo.Named)
                {
                    AppendName(varname, varSpec.OperatorInfo, string.IsNullOrEmpty(stringValue));
                }
                AppendValue(stringValue, varSpec.PrefixLength, varSpec.OperatorInfo.AllowReserved);
            }
            else
            {
                // Handle Lists
                var list = value as IEnumerable<string>;
                if (list != null)
                {
                    if (varSpec.OperatorInfo.Named && !varSpec.Explode) // exploding will prefix with list name
                    {
                        AppendName(varname, varSpec.OperatorInfo, !list.Any());
                    }

                    AppendList(varSpec.OperatorInfo, varSpec.Explode, varname, list);
                }
                else
                {

                    // Handle associative arrays
                    var dictionary = value as IDictionary<string, string>;
                    if (dictionary != null)
                    {
                        if (varSpec.OperatorInfo.Named && !varSpec.Explode) // exploding will prefix with list name
                        {
                            AppendName(varname, varSpec.OperatorInfo, !dictionary.Any());
                        }
                        AppendDictionary(varSpec.OperatorInfo, varSpec.Explode, dictionary);
                    }

                }

            }
            return true;
        }


        private void AppendDictionary(OperatorInfo op, bool explode, IDictionary<string, string> dictionary)
        {
            foreach (string key in dictionary.Keys)
            {
                result.Append(key);
                result.Append(explode ? '=' : ',');
                AppendValue(dictionary[key], 0, op.AllowReserved);

                result.Append(explode ? op.Seperator : ',');
            }
            if (dictionary.Any())
            {
                result.Remove(result.Length - 1, 1);
            }
        }

        private void AppendList(OperatorInfo op, bool explode, string variable, IEnumerable<string> list)
        {
            foreach (string item in list)
            {
                if (op.Named && explode)
                {
                    result.Append(variable);
                    result.Append("=");
                }
                AppendValue(item, 0, op.AllowReserved);

                result.Append(explode ? op.Seperator : ',');
            }
            if (list.Any())
            {
                result.Remove(result.Length - 1, 1);
            }
        }

        private void AppendValue(string value, int prefixLength, bool allowReserved)
        {

            if (prefixLength != 0)
            {
                if (prefixLength < value.Length)
                {
                    value = value.Substring(0, prefixLength);
                }
            }

            result.Append(Encode(value, allowReserved));

        }

        private void AppendName(string variable, OperatorInfo op, bool valueIsEmpty)
        {
            result.Append(variable);
            result.Append(valueIsEmpty ? op.IfEmpty : "=");
        }


        private bool IsVarNameChar(char c)
        {
            return (c >= 'A' && c <= 'z') //Alpha
                   || (c >= '0' && c <= '9') // Digit
                   || c == '_'
                   || c == '%'
                   || c == '.';
        }

        private static string Encode(string p, bool allowReserved)
        {

            var result = new StringBuilder();
            foreach (char c in p)
            {
                if ((c >= 'A' && c <= 'z') //Alpha
                    || (c >= '0' && c <= '9') // Digit
                    || UriUnreservedSymbols.IndexOf(c) != -1
                    // Unreserved symbols  - These should never be percent encoded
                    || (allowReserved && UriReservedSymbols.IndexOf(c) != -1))
                // Reserved symbols - should be included if requested (+)
                {
                    result.Append(c);
                }
                else
                {
#if PCL
                         result.Append(HexEscape(c));  
#else
                    string s = c.ToString();

                    char[] chars = s.Normalize(NormalizationForm.FormC).ToCharArray();
                    foreach (char ch in chars)
                    {
                        result.Append(HexEscape(ch));
                    }
#endif

                }
            }

            return result.ToString();


        }

        public static string HexEscape(char c)
        {
            var esc = new char[3];
            esc[0] = '%';
            esc[1] = hexDigits[(c & 240) >> 4];
            esc[2] = hexDigits[c & 15];
            return new string(esc);
        }

        private static readonly char[] hexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        private static OperatorInfo GetOperator(char operatorIndicator)
        {
            OperatorInfo op;
            switch (operatorIndicator)
            {

                case '+':
                case ';':
                case '/':
                case '#':
                case '&':
                case '?':
                case '.':
                    op = operators[operatorIndicator];
                    break;

                default:
                    op = operators['\0'];
                    break;
            }
            return op;
        }


        public class OperatorInfo
        {
            public bool Default { get; set; }
            public string First { get; set; }
            public char Seperator { get; set; }
            public bool Named { get; set; }
            public string IfEmpty { get; set; }
            public bool AllowReserved { get; set; }

        }

        public class VarSpec
        {
            public StringBuilder VarName = new StringBuilder();
            public bool Explode;
            public int PrefixLength;
            public bool First = true;

            public VarSpec(OperatorInfo operatorInfo)
            {
                OperatorInfo = operatorInfo;
            }

            public OperatorInfo OperatorInfo { get; }
        }
    }

}
