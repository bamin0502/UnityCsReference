// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets.Syntax
{
    [VisibleToOtherModules("UnityEditor.UIBuilderModule")]
    internal class Expression
    {
        public ExpressionType type;
        public ExpressionMultiplier multiplier;
        public DataType dataType;

        // Only set for combinator
        public ExpressionCombinator combinator;
        public Expression[] subExpressions;

        public string keyword;

        public Expression(ExpressionType type)
        {
            this.type = type;
            this.combinator = ExpressionCombinator.None;
            this.multiplier = new ExpressionMultiplier(ExpressionMultiplierType.None);
            this.subExpressions = null;
            this.keyword = null;
        }
    }

    [VisibleToOtherModules("UnityEditor.UIBuilderModule")]
    internal enum ExpressionType
    {
        Unknown,
        Data, // <type>
        Keyword, // any string not inside <>
        Combinator // any combinator
    }

    [VisibleToOtherModules("UnityEditor.UIBuilderModule")]
    internal enum DataType
    {
        None,
        Number, // <number>
        Integer, // <integer>
        Length, // <length>
        Percentage, // <percentage>
        Color, // <color>
        Resource, // <resource>
        Url, // <url>
        Time, // <time>
        FilterFunction, // <filter-function>
        Angle, //<angle>
        CustomIdent // <custom-ident>
    }

    // In order of precedence
    internal enum ExpressionCombinator
    {
        None,
        Or, // |
        OrOr, // ||
        AndAnd, // &&
        Juxtaposition, // ' '
        Group // [ ]
    }

    internal enum ExpressionMultiplierType
    {
        None,
        ZeroOrMore, // *
        OneOrMore,  // +
        ZeroOrOne, // ?
        Ranges, // {A,B}
        OneOrMoreComma, // #
        GroupAtLeastOne // !
    }

    internal struct ExpressionMultiplier
    {
        // Assume that 100 is the max number of value that a property can have.
        // No properties get close to having that amount.
        public const int Infinity = 100;

        private ExpressionMultiplierType m_Type;

        public ExpressionMultiplierType type
        {
            get { return m_Type; }
            set { SetType(value);}
        }

        public int min;
        public int max;

        public ExpressionMultiplier(ExpressionMultiplierType type = ExpressionMultiplierType.None)
        {
            m_Type = type;
            min = max = 1;
            SetType(type);
        }

        private void SetType(ExpressionMultiplierType value)
        {
            m_Type = value;
            switch (value)
            {
                case ExpressionMultiplierType.ZeroOrMore:
                    min = 0;
                    max = Infinity;
                    break;
                case ExpressionMultiplierType.ZeroOrOne:
                    min = 0;
                    max = 1;
                    break;
                case ExpressionMultiplierType.OneOrMore:
                case ExpressionMultiplierType.OneOrMoreComma:
                case ExpressionMultiplierType.GroupAtLeastOne:
                    min = 1;
                    max = Infinity;
                    break;
                default:
                    min = max = 1;
                    break;
            }
        }
    }
}
