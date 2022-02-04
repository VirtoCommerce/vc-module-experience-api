using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using GraphQL.Language.AST;
using VirtoCommerce.ExperienceApiModule.Core.Schemas.ScalarTypes;
using Xunit;

namespace VirtoCommerce.ExperienceApiModule.Tests.Schemas.ScalarTypes
{
    public class AnyValueGraphTypeTests
    {
        private const float DecimalMinAsFloat = DecimalMaxAsFloat * -1f;
        private const float DecimalMaxAsFloat = 7.922816E+28f;
        private const double DecimalMinAsDouble = DecimalMaxAsDouble * -1d;
        private const double DecimalMaxAsDouble = 7.922816251426433E+28d;
        private static readonly DateTime DateTimeUtc = new(2022, 2, 3, 1, 2, 3, DateTimeKind.Utc);
        private static readonly string DateTimeIso8601UtcString = DateTimeUtc.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", DateTimeFormatInfo.InvariantInfo);

        private readonly AnyValueGraphType _anyValueGraphType = new();

        #region Arrange

        public static IEnumerable<object[]> CanParseLiteralValidData => ParseLiteralValidData.Select(x => new [] { x.First() }).ToArray();

        public static IEnumerable<object[]> ParseLiteralValidData => new List<object[]>
        {
            //                 IValue                  Value                         Expected result     Expected result type
            // Int
            new object[] { new IntValue(               int.MinValue),                int.MinValue,       typeof(int)     },
            new object[] { new IntValue(               0),                           0,                  typeof(int)     },
            new object[] { new IntValue(               int.MaxValue),                int.MaxValue,       typeof(int)     },
            // Long
            new object[] { new LongValue(              long.MinValue),      (decimal)long.MinValue,      typeof(decimal) },
            new object[] { new LongValue(              0L),                          0,                  typeof(int)     },
            new object[] { new LongValue(              long.MaxValue),      (decimal)long.MaxValue,      typeof(decimal) },
            // BigInt
            new object[] { new BigIntValue((BigInteger)decimal.MinValue),            decimal.MinValue,   typeof(decimal) },
            new object[] { new BigIntValue(            0),                           0,                  typeof(int)     },
            new object[] { new BigIntValue((BigInteger)decimal.MaxValue),            decimal.MaxValue,   typeof(decimal) },
            // Float
            new object[] { new FloatValue(             DecimalMinAsDouble), (decimal)DecimalMinAsDouble, typeof(decimal) },
            new object[] { new FloatValue(             0d),                          0m,                 typeof(decimal) },
            new object[] { new FloatValue(             DecimalMaxAsDouble), (decimal)DecimalMaxAsDouble, typeof(decimal) },
            // Decimal
            new object[] { new DecimalValue(           decimal.MinValue),            decimal.MinValue,   typeof(decimal) },
            new object[] { new DecimalValue(           0m),                          0m,                 typeof(decimal) },
            new object[] { new DecimalValue(           decimal.MaxValue),            decimal.MaxValue,   typeof(decimal) },
            // Boolean
            new object[] { new BooleanValue(           false),                       false,              typeof(bool)    },
            new object[] { new BooleanValue(           true),                        true,               typeof(bool)    },
            // DateTime
            new object[] { new StringValue(            DateTimeIso8601UtcString),    DateTimeUtc,        typeof(DateTime) },
            // String
            new object[] { new StringValue(            string.Empty),                string.Empty,       typeof(string)  },
            new object[] { new StringValue(            "test"),                      "test",             typeof(string)  },
            // Null
            new object[] { new NullValue(),                                          null,               null            },
        };

        public static IEnumerable<object[]> CanParseLiteralInvalidData => ParseLiteralInvalidData.Select(x => new[] { x.First() }).ToArray();

        public static IEnumerable<object[]> ParseLiteralInvalidData => new List<object[]>
        {
            //                 IValue                  Value                                            Expected exception type
            new object[] { new ListValue(           new[] { new StringValue("test") }),                 typeof(InvalidOperationException) },
            new object[] { new ObjectValue(new[] { new ObjectField("test", new StringValue("test")) }), typeof(InvalidOperationException) },
            new object[] { new BigIntValue((BigInteger)decimal.MinValue - BigInteger.One),              typeof(OverflowException)         },
            new object[] { new BigIntValue((BigInteger)decimal.MaxValue + BigInteger.One),              typeof(OverflowException)         },
            new object[] { new FloatValue(     (double)decimal.MinValue),                               typeof(OverflowException)         },
            new object[] { new FloatValue(     (double)decimal.MaxValue),                               typeof(OverflowException)         }
        };

        public static IEnumerable<object[]> CanParseValueValidData => ParseValueValidData.Select(x => new[] { x.First() }).ToArray();

        public static IEnumerable<object[]> ParseValueValidData => new List<object[]>
        {
            //                         Value                        Expected result     Expected result type
            // byte & sbyte
            new object[] {       (byte)0,                           0,                  typeof(int)     },
            new object[] {      (sbyte)0,                           0,                  typeof(int)     },
            // short & ushort
            new object[] {      (short)0,                           0,                  typeof(int)     },
            new object[] {     (ushort)0,                           0,                  typeof(int)     },
            // int
            new object[] {             int.MinValue,                int.MinValue,       typeof(int)     },
            new object[] {             0,                           0,                  typeof(int)     },
            new object[] {             int.MaxValue,                int.MaxValue,       typeof(int)     },
            // uint
            new object[] {             uint.MinValue,               0,                  typeof(int)     },
            new object[] {             uint.MaxValue,      (decimal)uint.MaxValue,      typeof(decimal) },
            // long
            new object[] {             long.MinValue,      (decimal)long.MinValue,      typeof(decimal) },
            new object[] {             0L,                          0,                  typeof(int)     },
            new object[] {             long.MaxValue,      (decimal)long.MaxValue,      typeof(decimal) },
            // ulong
            new object[] {             ulong.MinValue,              0,                  typeof(int)     },
            new object[] {             ulong.MaxValue,     (decimal)ulong.MaxValue,     typeof(decimal) },
            // BigInt
            new object[] { (BigInteger)decimal.MinValue,            decimal.MinValue,   typeof(decimal) },
            new object[] {             0,                           0,                  typeof(int)     },
            new object[] { (BigInteger)decimal.MaxValue,            decimal.MaxValue,   typeof(decimal) },
            // float
            new object[] {             DecimalMinAsFloat,  (decimal)DecimalMinAsFloat,  typeof(decimal) },
            new object[] {             0f,                          0m,                 typeof(decimal) },
            new object[] {             DecimalMaxAsFloat,  (decimal)DecimalMaxAsFloat,  typeof(decimal) },
            // double
            new object[] {             DecimalMinAsDouble, (decimal)DecimalMinAsDouble, typeof(decimal) },
            new object[] {             0d,                          0m,                 typeof(decimal) },
            new object[] {             DecimalMaxAsDouble, (decimal)DecimalMaxAsDouble, typeof(decimal) },
            // decimal
            new object[] {             decimal.MinValue,            decimal.MinValue,   typeof(decimal) },
            new object[] {             0m,                          0m,                 typeof(decimal) },
            new object[] {             decimal.MaxValue,            decimal.MaxValue,   typeof(decimal) },
            // bool
            new object[] {             false,                       false,              typeof(bool)    },
            new object[] {             true,                        true,               typeof(bool)    },
            // DateTime
            new object[] {             DateTimeUtc,                 DateTimeUtc,        typeof(DateTime)},
            // string
            new object[] {             string.Empty,                string.Empty,       typeof(string)  },
            new object[] {             "test",                      "test",             typeof(string)  },
            // null
            new object[] {             null,                        null,               null            },
        };

        public static IEnumerable<object[]> CanParseValueInvalidData => ParseValueInvalidData.Select(x => new[] { x.First() }).ToArray();

        public static IEnumerable<object[]> ParseValueInvalidData => new List<object[]>
        {
            //                         Value                              Expected exception type
            new object[] {         new object(),                          typeof(InvalidOperationException) },
            new object[] { (BigInteger)decimal.MinValue - BigInteger.One, typeof(OverflowException)         },
            new object[] { (BigInteger)decimal.MaxValue + BigInteger.One, typeof(OverflowException)         },
            new object[] {     (double)decimal.MinValue,                  typeof(OverflowException)         },
            new object[] {     (double)decimal.MaxValue,                  typeof(OverflowException)         }
        };

        #endregion

        [MemberData(nameof(CanParseLiteralValidData))]
        [Theory]
        public void CanParseLiteral_ValidValues_ReturnsTrue(IValue value)
        {
            // Act
            var result = _anyValueGraphType.CanParseLiteral(value);

            // Assert
            Assert.True(result);
        }

        [MemberData(nameof(CanParseLiteralInvalidData))]
        [Theory]
        public void CanParseLiteral_InvalidValues_ReturnsFalse(IValue value)
        {
            // Act
            var result = _anyValueGraphType.CanParseLiteral(value);

            // Assert
            Assert.False(result);
        }

        [MemberData(nameof(ParseLiteralValidData))]
        [Theory]
        public void ParseLiteral_ValidValues_Parsed(IValue value, object expectedResult, Type expectedResultType)
        {
            // Act
            var result = _anyValueGraphType.ParseLiteral(value);

            // Assert
            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedResultType, result?.GetType());
        }

        [MemberData(nameof(ParseLiteralInvalidData))]
        [Theory]
        public void ParseLiteral_InvalidValues_ThrowException(IValue value, Type exceptionType)
        {
            // Act
            var action = () => _anyValueGraphType.ParseLiteral(value);

            // Assert
            Assert.Throws(exceptionType, action);
        }

        [MemberData(nameof(CanParseValueValidData))]
        [Theory]
        public void CanParseValue_ValidValues_ReturnsTrue(object value)
        {
            // Act
            var result = _anyValueGraphType.CanParseValue(value);

            // Assert
            Assert.True(result);
        }

        [MemberData(nameof(CanParseValueInvalidData))]
        [Theory]
        public void CanParseValue_InvalidValues_ReturnsFalse(object value)
        {
            // Act
            var result = _anyValueGraphType.CanParseValue(value);

            // Assert
            Assert.False(result);
        }

        [MemberData(nameof(ParseValueValidData))]
        [Theory]
        public void ParseValue_ValidValues_Parsed(object value, object expectedResult, Type expectedResultType)
        {
            // Act
            var result = _anyValueGraphType.ParseValue(value);

            // Assert
            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedResultType, result?.GetType());
        }

        [MemberData(nameof(ParseValueInvalidData))]
        [Theory]
        public void ParseValue_InvalidValues_ThrowException(object value, Type exceptionType)
        {
            // Act
            var action = () => _anyValueGraphType.ParseValue(value);

            // Assert
            Assert.Throws(exceptionType, action);
        }
    }
}
