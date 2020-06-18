using System;
using System.Globalization;

namespace VirtoCommerce.XPurchase.Models.Common
{
    public sealed class Money : IComparable<Money>, IEquatable<Money>, IComparable, IConvertible<Money>, ICloneable
    {
        public Money()
        {
        }

        public Money(Currency currency)
            : this(0m, currency)
        {
        }

        public Money(double amount, Currency currency)
            : this((decimal)amount, currency)
        {
        }

        public Money(decimal amount, Currency currency)
        {
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            InternalAmount = amount;
        }

        /// <summary>
        /// Accesses the internal representation of the value of the Money.
        /// </summary>
        /// <returns>A decimal with the internal amount stored for this Money.</returns>
        public decimal InternalAmount { get; }

        /// <summary>
        /// Rounds the amount to the number of significant decimal digits
        /// of the associated currency using MidpointRounding.AwayFromZero.
        /// </summary>
        /// <returns>A decimal with the amount rounded to the significant number of decimal digits.</returns>
        public decimal Amount => decimal.Round(InternalAmount, DecimalDigits, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Truncates the amount to the number of significant decimal digits
        /// of the associated currency.
        /// </summary>
        /// <returns>A decimal with the amount truncated to the significant number of decimal digits.</returns>
        public decimal TruncatedAmount
        {
            get
            {
                var roundingBase = (decimal)Math.Pow(10, DecimalDigits);
                return (long)Math.Truncate(InternalAmount * roundingBase) / roundingBase;
            }
        }

        public string FormattedAmount => ToString(true, true);

        public string FormattedAmountWithoutPoint => ToString(false, true);

        public string FormattedAmountWithoutCurrency => ToString(false, true);

        public string FormattedAmountWithoutPointAndCurrency => ToString(false, false);

        public Currency Currency { get; }

        /// <summary>
        /// Gets the number of decimal digits for the associated currency.
        /// </summary>
        /// <returns>An int containing the number of decimal digits.</returns>
        public int DecimalDigits => Currency.NumberFormat.CurrencyDecimalDigits;

        public static bool operator ==(Money first, Money second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (first is null || second is null)
            {
                return false;
            }

            return first.Equals(second);
        }

        public static bool operator !=(Money first, Money second) => !(first == second);

        public static bool operator >(Money first, Money second)
            => first.InternalAmount > second.ConvertTo(first.Currency).InternalAmount &&
            second.InternalAmount < first.ConvertTo(second.Currency).InternalAmount;

        public static bool operator >=(Money first, Money second)
            => first.InternalAmount >= second.ConvertTo(first.Currency).InternalAmount &&
            second.InternalAmount <= first.ConvertTo(second.Currency).InternalAmount;

        public static bool operator <=(Money first, Money second)
            => first.InternalAmount <= second.ConvertTo(first.Currency).InternalAmount &&
            second.InternalAmount >= first.ConvertTo(second.Currency).InternalAmount;

        public static bool operator <(Money first, Money second)
            => first.InternalAmount < second.ConvertTo(first.Currency).InternalAmount &&
            second.InternalAmount > first.ConvertTo(second.Currency).InternalAmount;

        public static Money operator +(Money first, Money second)
            => new Money(first.InternalAmount + second.ConvertTo(first.Currency).InternalAmount, first.Currency);

        public static Money operator -(Money first, Money second)
            => new Money(first.InternalAmount - second.ConvertTo(first.Currency).InternalAmount, first.Currency);

        public static Money operator *(Money first, Money second)
            => new Money(first.InternalAmount * second.ConvertTo(first.Currency).InternalAmount, first.Currency);

        public static Money operator /(Money first, Money second)
            => new Money(first.InternalAmount / second.ConvertTo(first.Currency).InternalAmount, first.Currency);

        public static bool operator ==(Money money, long value) => !(money is null) && money.InternalAmount == value;

        public static bool operator !=(Money money, long value) => !(money == value);

        public static bool operator ==(Money money, decimal value) => !(money is null) && money.InternalAmount == value;

        public static bool operator !=(Money money, decimal value) => !(money == value);

        public static bool operator ==(Money money, double value) => !(money is null) && money.InternalAmount == (decimal)value;

        public static bool operator !=(Money money, double value) => !(money == value);

        public static Money operator +(Money money, long value) => money + (decimal)value;

        public static Money operator +(Money money, double value) => money + (decimal)value;

        public static Money operator +(Money money, decimal value)
        {
            if (money == null)
            {
                throw new ArgumentNullException(nameof(money));
            }

            return new Money(money.InternalAmount + value, money.Currency);
        }

        public static Money operator -(Money money, long value) => money - (decimal)value;

        public static Money operator -(Money money, double value) => money - (decimal)value;

        public static Money operator -(Money money, decimal value)
        {
            if (money == null)
            {
                throw new ArgumentNullException(nameof(money));
            }

            return new Money(money.InternalAmount - value, money.Currency);
        }

        public static Money operator *(Money money, long value) => money * (decimal)value;

        public static Money operator *(Money money, double value) => money * (decimal)value;

        public static Money operator *(Money money, decimal value)
        {
            if (money == null)
            {
                throw new ArgumentNullException(nameof(money));
            }

            return new Money(money.InternalAmount * value, money.Currency);
        }

        public static Money operator /(Money money, long value) => money / (decimal)value;

        public static Money operator /(Money money, double value) => money / (decimal)value;

        public static Money operator /(Money money, decimal value)
        {
            if (money == null)
            {
                throw new ArgumentNullException(nameof(money));
            }

            return new Money(money.InternalAmount / value, money.Currency);
        }

        public override int GetHashCode() => Amount.GetHashCode() ^ Currency.Code.GetHashCode();

        public override bool Equals(object obj) => obj is Money && Equals((Money)obj);

        public bool Equals(Money other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Currency.Equals(other.Currency) && InternalAmount == other.InternalAmount;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (!(obj is Money))
            {
                throw new ArgumentException("Argument must be Money");
            }

            return CompareTo((Money)obj);
        }

        public int CompareTo(Money other)
        {
            if (this < other)
            {
                return -1;
            }

            if (this > other)
            {
                return 1;
            }

            return 0;
        }

        public override string ToString() => ToString(true, true);

        public string ToString(bool showDecimalDigits, bool showCurrencySymbol)
        {
            string result = null;

            if (Currency != null && !string.IsNullOrEmpty(Currency.CustomFormatting))
            {
                result = Amount.ToString(Currency.CustomFormatting, Currency.NumberFormat);
            }

            if (result == null)
            {
                var format = showDecimalDigits ? "C" : "C0";

                var numberFormat = Currency != null && Currency.NumberFormat != null
                    ? Currency.NumberFormat
                    : CultureInfo.InvariantCulture.NumberFormat;

                if (!showCurrencySymbol)
                {
                    numberFormat = (NumberFormatInfo)numberFormat.Clone();
                    numberFormat.CurrencySymbol = string.Empty;
                }

                result = Amount.ToString(format, numberFormat);
            }

            return result;
        }

        /// <summary>
        /// Evenly distributes the amount over n parts, resolving remainders that occur due to rounding
        /// errors, thereby guaranteeing the post-condition: result->sum(r|r.amount) = this.amount and
        /// x elements in result are greater than at least one of the other elements, where x = amount mod n.
        /// </summary>
        /// <param name="n">Number of parts over which the amount is to be distributed.</param>
        /// <returns>Array with distributed Money amounts.</returns>
        public Money[] Allocate(int n)
        {
            var cents = Math.Pow(10, DecimalDigits);
            var lowResult = (long)Math.Truncate((double)InternalAmount / n * cents) / cents;
            var highResult = lowResult + 1.0d / cents;
            var remainder = (int)((double)InternalAmount * cents % n);

            var results = new Money[n];

            for (var i = 0; i < remainder; i++)
            {
                results[i] = new Money((decimal)highResult, Currency);
            }

            for (var i = remainder; i < n; i++)
            {
                results[i] = new Money((decimal)lowResult, Currency);
            }

            return results;
        }

        public Money ConvertTo(Currency currency) => Currency == currency
            ? this
            : new Money(InternalAmount * Currency.ExchangeRate / currency.ExchangeRate, currency);

        public object Clone() => MemberwiseClone() as Money;
    }
}
