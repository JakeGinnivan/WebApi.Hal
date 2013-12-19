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
        const string _UriReservedSymbols = ":/?#[]@!$&'()*+,;=";
        const string _UriUnreservedSymbols = "-._~";

        static Dictionary<char, OperatorInfo> _Operators = new Dictionary<char, OperatorInfo>() {
            {'\0', new OperatorInfo {Default = true, First = "", Seperator = ',', Named = false, IfEmpty = "",AllowReserved = false}},
            {'+', new OperatorInfo {Default = false, First = "", Seperator = ',', Named = false, IfEmpty = "",AllowReserved = true}},
            {'.', new OperatorInfo {Default = false, First = ".", Seperator = '.', Named = false, IfEmpty = "",AllowReserved = false}},
            {'/', new OperatorInfo {Default = false, First = "/", Seperator = '/', Named = false, IfEmpty = "",AllowReserved = false}},
            {';', new OperatorInfo {Default = false, First = ";", Seperator = ';', Named = true, IfEmpty = "",AllowReserved = false}},
            {'?', new OperatorInfo {Default = false, First = "?", Seperator = '&', Named = true, IfEmpty = "=",AllowReserved = false}},
            {'&', new OperatorInfo {Default = false, First = "&", Seperator = '&', Named = true, IfEmpty = "=",AllowReserved = false}},
            {'#', new OperatorInfo {Default = false, First = "#", Seperator = ',', Named = false, IfEmpty = "",AllowReserved = true}}
        };

        readonly string _template;
        readonly Dictionary<string, object> _Parameters = new Dictionary<string, object>();

        enum States
        {
            CopyingLiterals,
            ParsingExpression
        }

        bool _ErrorDetected = false;
        StringBuilder _Result;
        List<string> _ParameterNames;

        public UriTemplate(string template)
        {
            _template = template;
        }


        public void SetParameter(string name, object value)
        {
            _Parameters[name] = value;
        }

        public void SetParameter(string name, string value)
        {
            _Parameters[name] = value;
        }

        public void SetParameter(string name, IEnumerable<string> value)
        {
            _Parameters[name] = value;
        }

        public void SetParameter(string name, IDictionary<string, string> value)
        {
            _Parameters[name] = value;
        }


        public IEnumerable<string> GetParameterNames()
        {
            var parameterNames = new List<string>();
            _ParameterNames = parameterNames;
            Resolve();
            _ParameterNames = null;
            return parameterNames;
        }

        public string Resolve()
        {
            var currentState = States.CopyingLiterals;
            _Result = new StringBuilder();
            StringBuilder currentExpression = null;
            foreach (var character in _template.ToCharArray())
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
                            throw new ArgumentException("Malformed template, unexpected } : " + _Result.ToString());
                        }
                        else
                        {
                            _Result.Append(character);
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
                            currentExpression.Append(character);
                        }

                        break;
                }
            }
            if (currentState == States.ParsingExpression)
            {
                _Result.Append("{");
                _Result.Append(currentExpression.ToString());

                throw new ArgumentException("Malformed template, missing } : " + _Result.ToString());
            }

            if (_ErrorDetected)
            {
                throw new ArgumentException("Malformed template : " + _Result.ToString());
            }
            return _Result.ToString();
        }

        void ProcessExpression(StringBuilder currentExpression)
        {

            if (currentExpression.Length == 0)
            {
                _ErrorDetected = true;
                _Result.Append("{}");
                return;
            }

            OperatorInfo op = GetOperator(currentExpression[0]);

            var firstChar = op.Default ? 0 : 1;


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
                        var success = ProcessVariable(varSpec);
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
                            _ErrorDetected = true;
                        }
                        break;
                }
            }
            ProcessVariable(varSpec);

        }

        bool ProcessVariable(VarSpec varSpec)
        {
            var varname = varSpec.VarName.ToString();
            if (_ParameterNames != null) _ParameterNames.Add(varname);

            if (!_Parameters.ContainsKey(varname)
                || _Parameters[varname] == null
                || (_Parameters[varname] is IList && ((IList)_Parameters[varname]).Count == 0)
                || (_Parameters[varname] is IDictionary && ((IDictionary)_Parameters[varname]).Count == 0))
                return false;

            if (varSpec.First)
            {
                _Result.Append(varSpec.OperatorInfo.First);
            }
            else
            {
                _Result.Append(varSpec.OperatorInfo.Seperator);
            }

            object value = _Parameters[varname];

            // Handle Strings
            if (value is string)
            {
                var stringValue = (string)value;
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
                        AppendName(varname, varSpec.OperatorInfo, list.Count() == 0);
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
                            AppendName(varname, varSpec.OperatorInfo, dictionary.Count() == 0);
                        }
                        AppendDictionary(varSpec.OperatorInfo, varSpec.Explode, dictionary);
                    }

                }

            }
            return true;
        }


        void AppendDictionary(OperatorInfo op, bool explode, IDictionary<string, string> dictionary)
        {
            foreach (string key in dictionary.Keys)
            {
                _Result.Append(key);
                if (explode) _Result.Append('=');
                else _Result.Append(',');
                AppendValue(dictionary[key], 0, op.AllowReserved);

                if (explode)
                {
                    _Result.Append(op.Seperator);
                }
                else
                {
                    _Result.Append(',');
                }
            }
            if (dictionary.Count() > 0)
            {
                _Result.Remove(_Result.Length - 1, 1);
            }
        }

        void AppendList(OperatorInfo op, bool explode, string variable, IEnumerable<string> list)
        {
            foreach (string item in list)
            {
                if (op.Named && explode)
                {
                    _Result.Append(variable);
                    _Result.Append("=");
                }
                AppendValue(item, 0, op.AllowReserved);

                _Result.Append(explode ? op.Seperator : ',');
            }
            if (list.Count() > 0)
            {
                _Result.Remove(_Result.Length - 1, 1);
            }
        }

        void AppendValue(string value, int prefixLength, bool allowReserved)
        {

            if (prefixLength != 0)
            {
                if (prefixLength < value.Length)
                {
                    value = value.Substring(0, prefixLength);
                }
            }

            _Result.Append(Encode(value, allowReserved));

        }

        void AppendName(string variable, OperatorInfo op, bool valueIsEmpty)
        {
            _Result.Append(variable);
            if (valueIsEmpty)
            {
                _Result.Append(op.IfEmpty);
            }
            else
            {
                _Result.Append("=");
            }
        }


        bool IsVarNameChar(char c)
        {
            return ((c >= 'A' && c <= 'z') //Alpha
                    || (c >= '0' && c <= '9') // Digit
                    || c == '_'
                    || c == '%'
                    || c == '.');
        }

        static string Encode(string p, bool allowReserved)
        {

            var result = new StringBuilder();
            foreach (char c in p)
            {
                if ((c >= 'A' && c <= 'z') //Alpha
                    || (c >= '0' && c <= '9') // Digit
                    || _UriUnreservedSymbols.IndexOf(c) != -1
                    // Unreserved symbols  - These should never be percent encoded
                    || (allowReserved && _UriReservedSymbols.IndexOf(c) != -1))
                // Reserved symbols - should be included if requested (+)
                {
                    result.Append(c);
                }
                else
                {
#if PCL
                         result.Append(HexEscape(c));  
#else
                    var s = c.ToString();

                    var chars = s.Normalize(NormalizationForm.FormC).ToCharArray();
                    foreach (var ch in chars)
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
            esc[1] = HexDigits[(((int)c & 240) >> 4)];
            esc[2] = HexDigits[((int)c & 15)];
            return new string(esc);
        }

        static readonly char[] HexDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        static OperatorInfo GetOperator(char operatorIndicator)
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
                    op = _Operators[operatorIndicator];
                    break;

                default:
                    op = _Operators['\0'];
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
            readonly OperatorInfo _operatorInfo;
            public StringBuilder VarName = new StringBuilder();
            public bool Explode = false;
            public int PrefixLength = 0;
            public bool First = true;

            public VarSpec(OperatorInfo operatorInfo)
            {
                _operatorInfo = operatorInfo;
            }

            public OperatorInfo OperatorInfo
            {
                get { return _operatorInfo; }
            }
        }
    }
}
