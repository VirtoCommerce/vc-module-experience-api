using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GraphQL.Language.AST;
using VirtoCommerce.ExperienceApiModule.Core.Schemas.ScalarTypes;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.Tests.Schemas.ScalarTypes
{
    public class AnyValueGraphTypeTests
    {
        private const double DecimalMinAsDouble = DecimalMaxAsDouble * -1d;
        private const double DecimalMaxAsDouble = 7.922816251426433E+28d;

        private readonly AnyValueGraphType _anyValueGraphType = new();

        public static IEnumerable<object[]> CanParseLiteralValidData => ParseLiteralValidData.Select(x => new [] { x.First() }).ToArray();

        public static IEnumerable<object[]> ParseLiteralValidData => new List<object[]>
        {
            //                 IValue                  Value                         Expected result         Expected result type
            new object[] { new IntValue(               int.MinValue),                int.MinValue,           typeof(int)     },
            new object[] { new IntValue(               0),                           0,                      typeof(int)     },
            new object[] { new IntValue(               int.MaxValue),                int.MaxValue,           typeof(int)     },
            new object[] { new LongValue(              long.MinValue),      (decimal)long.MinValue,          typeof(decimal) },
            new object[] { new LongValue(              0L),                          0,                      typeof(int)     },
            new object[] { new LongValue(              long.MaxValue),      (decimal)long.MaxValue,          typeof(decimal) },
            new object[] { new BigIntValue((BigInteger)decimal.MinValue),            decimal.MinValue,       typeof(decimal) },
            new object[] { new BigIntValue(            0),                           0,                      typeof(int)     },
            new object[] { new BigIntValue((BigInteger)decimal.MaxValue),            decimal.MaxValue,       typeof(decimal) },
            new object[] { new FloatValue(             DecimalMinAsDouble), (decimal)DecimalMinAsDouble,     typeof(decimal) },
            new object[] { new FloatValue(             0d),                          0m,                     typeof(decimal) },
            new object[] { new FloatValue(             DecimalMaxAsDouble), (decimal)DecimalMaxAsDouble,     typeof(decimal) },
            new object[] { new DecimalValue(           decimal.MinValue),            decimal.MinValue,       typeof(decimal) },
            new object[] { new DecimalValue(           0m),                          0m,                     typeof(decimal) },
            new object[] { new DecimalValue(           decimal.MaxValue),            decimal.MaxValue,       typeof(decimal) },
            new object[] { new BooleanValue(           false),                       false,                  typeof(bool)    },
            new object[] { new BooleanValue(           true),                        true,                   typeof(bool)    },
            new object[] { new NullValue(),                                          null,                   null            },
            new object[] { new StringValue(            string.Empty),                string.Empty,           typeof(string)  },
            new object[] { new StringValue(            "test"),                      "test",                 typeof(string)  },
        };

        public static IEnumerable<object[]> CanParseLiteralInvalidData => ParseLiteralInvalidData.Select(x => new[] { x.First() }).ToArray();

        public static IEnumerable<object[]> ParseLiteralInvalidData => new List<object[]>
        {
            //                 IValue                  Value                                            Expected exception type
            new object[] { new ListValue(           new[] { new StringValue("test") }),                 typeof(InvalidOperationException) },
            new object[] { new ObjectValue(new[] { new ObjectField("test", new StringValue("test")) }), typeof(InvalidOperationException) },
            new object[] { new BigIntValue((BigInteger)decimal.MinValue - BigInteger.One),              typeof(OverflowException) },
            new object[] { new BigIntValue((BigInteger)decimal.MaxValue + BigInteger.One),              typeof(OverflowException) },
            new object[] { new FloatValue(     (double)decimal.MinValue),                               typeof(OverflowException) },
            new object[] { new FloatValue(     (double)decimal.MaxValue),                               typeof(OverflowException) }
        };

        [MemberData(nameof(CanParseLiteralValidData))]
        [Theory]
        public void CanParseLiteral_ValidValues_ReturnsTrue(IValue value)
        {
            Assert.True(_anyValueGraphType.CanParseLiteral(value));
        }

        [MemberData(nameof(CanParseLiteralInvalidData))]
        [Theory]
        public void CanParseLiteral_InvalidValues_ReturnsFalse(IValue value)
        {
            Assert.False(_anyValueGraphType.CanParseLiteral(value));
        }

        [MemberData(nameof(ParseLiteralValidData))]
        [Theory]
        public void ParseLiteral_ValidValues_Parsed(IValue value, object expectedResult, Type expectedResultType)
        {
            var result = _anyValueGraphType.ParseLiteral(value);
            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedResultType, result?.GetType());
        }

        [MemberData(nameof(ParseLiteralInvalidData))]
        [Theory]
        public void ParseLiteral_InvalidValues_ThrowException(IValue value, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => _anyValueGraphType.ParseLiteral(value));
        }
    }
}
