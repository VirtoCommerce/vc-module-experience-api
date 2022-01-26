using System;
using System.Numerics;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas.ScalarTypes
{
    public class AnyValueGraphType : ScalarGraphType
    {
        private readonly StringGraphType _stringGraphType = new();
        private readonly IntGraphType _intGraphType = new();
        private readonly DecimalGraphType _decimalGraphType = new();
        private readonly DateTimeGraphType _dateTimeGraphType = new();
        private readonly BooleanGraphType _booleanGraphType = new();

        public override bool CanParseLiteral(IValue value)
        {
            return value switch
            {
                IntValue or LongValue or BigIntValue => _intGraphType.CanParseLiteral(value) || _decimalGraphType.CanParseLiteral(value),
                DecimalValue => _decimalGraphType.CanParseLiteral(value),
                BooleanValue => _booleanGraphType.CanParseLiteral(value),
                NullValue => true,
                _ => base.CanParseLiteral(value)
            };
        }

        public override object ParseLiteral(IValue value)
        {
            return value switch
            {
                IntValue or LongValue or BigIntValue => _intGraphType.CanParseLiteral(value) ? _intGraphType.ParseLiteral(value) : _decimalGraphType.ParseLiteral(value),
                DecimalValue => _decimalGraphType.ParseLiteral(value),
                BooleanValue => _booleanGraphType.ParseLiteral(value),
                StringValue => _dateTimeGraphType.CanParseLiteral(value) ? _dateTimeGraphType.ParseLiteral(value) : _stringGraphType.ParseLiteral(value),
                NullValue => null,
                _ => ThrowLiteralConversionError(value)
            };
        }

        public override bool CanParseValue(object value)
        {
            return value is sbyte or byte or short or ushort or int or uint or long or ulong or BigInteger or float or double or decimal or bool or DateTime or string or null;
        }

        public override object ParseValue(object value)
        {
            return value switch
            {
                sbyte or byte or short or ushort or int or uint or long or ulong or BigInteger =>
                    _intGraphType.CanParseValue(value) ? _intGraphType.ParseValue(value) : _decimalGraphType.ParseValue(value),
                float or double or decimal => _decimalGraphType.ParseValue(value),
                bool => _booleanGraphType.ParseValue(value),
                DateTime => _dateTimeGraphType.ParseValue(value),
                string => _stringGraphType.ParseValue(value),
                null => null,
                _ => ThrowValueConversionError(value)
            };
        }
    }
}
