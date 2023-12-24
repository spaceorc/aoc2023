using System;
using System.Numerics;

namespace aoc.Lib
{
	public struct Rational : IComparable
    {
        public static readonly Rational Zero = new(BigInteger.Zero, BigInteger.One);

		public readonly BigInteger Numerator;
		public readonly BigInteger Denomerator;
		private readonly bool reduced;

        public double ToDouble()
        {
            return this;
        }

		public static Rational Parse(string s)
		{
			var parts = s.Split('/');
			if (parts.Length == 1)
				return new Rational(BigInteger.Parse(parts[0]), BigInteger.One);
			if (parts.Length != 2) throw new FormatException(s);
			return new Rational(BigInteger.Parse(parts[0]), BigInteger.Parse(parts[1]));
		}

		public Rational(BigInteger numerator, BigInteger denomerator)
			: this(numerator, denomerator, false)
		{
		}

        public Rational(BigInteger value)
			: this(value, BigInteger.One, true)
		{
		}

		private Rational(BigInteger numerator, BigInteger denomerator, bool reduced)
		{
            if (denomerator == BigInteger.Zero)
                throw new ArgumentException();
			Numerator = numerator;
			Denomerator = denomerator;
			this.reduced = reduced;
		}

		public static BigInteger Lcm(BigInteger a, BigInteger b)
		{
			return a * b / BigInteger.GreatestCommonDivisor(a, b);
		}

		public Rational Reduce()
		{
			if(reduced)
				return this;
			if(Numerator == BigInteger.Zero)
				return new Rational(BigInteger.Zero, BigInteger.One, true);
			var gcd = BigInteger.GreatestCommonDivisor(Numerator, Denomerator);
			var n = Numerator / gcd;
			var d = Denomerator / gcd;
			return d < 0 ? new Rational(-n, -d, true) : new Rational(n, d, true);
		}

        public readonly Rational Abs() => IsNegative ? -this : this;

		public readonly int ToInt()
		{
			return (int)BigInteger.Divide(Numerator, Denomerator);
		}

		public long ToLong()
		{
            return (long)BigInteger.Divide(Numerator, Denomerator);
		}

		public bool IsInt()
		{
			Reduce();
			return Denomerator == BigInteger.One;
		}

		public readonly bool IsPositive => Sign > 0;
        public readonly bool IsNegative => Sign < 0;
        public readonly bool IsZero => Numerator.IsZero;
        public readonly int Sign => Numerator.Sign * Denomerator.Sign;

		public override string ToString()
		{
			if (Numerator.IsZero) return "0";
			if (Denomerator.IsOne) return Numerator.ToString();
			return $"{Numerator}/{Denomerator}";
		}

		public override bool Equals(object? obj)
		{
			if(!(obj is Rational)) return false;
			var r1 = Reduce();
			var r2 = ((Rational) obj).Reduce();
			return r1.Numerator == r2.Numerator && r1.Denomerator == r2.Denomerator;
		}

		public override int GetHashCode()
		{
			var r = Reduce();
			unchecked
			{
				return (r.Numerator.GetHashCode() * 397) ^ r.Denomerator.GetHashCode();
			}
		}

		public static Rational operator +(Rational r1, Rational r2)
		{
			var nominator = r1.Numerator*r2.Denomerator + r2.Numerator*r1.Denomerator;
			return new Rational(
				nominator,
				nominator == BigInteger.Zero ? BigInteger.One : r1.Denomerator * r2.Denomerator
				).Reduce();
		}
		public static Rational operator -(Rational r1, Rational r2)
		{
			var nominator = r1.Numerator * r2.Denomerator - r2.Numerator * r1.Denomerator;
			return new Rational(
				nominator,
				nominator == BigInteger.Zero ? BigInteger.One : r1.Denomerator * r2.Denomerator
				).Reduce();
		}

		public static Rational operator *(Rational a, Rational b)
		{
			return new Rational(a.Numerator * b.Numerator, a.Denomerator * b.Denomerator).Reduce();
		}

		public static Rational operator /(Rational a, Rational b)
		{
			return new Rational(a.Numerator * b.Denomerator, a.Denomerator * b.Numerator).Reduce();
		}


		public static Rational operator +(Rational r1, int n2)
		{
			return r1 + new Rational(n2, BigInteger.One, true);
		}

		public static Rational operator -(Rational r)
		{
			return new Rational(-r.Numerator, r.Denomerator, r.reduced);
		}
		public static implicit operator Rational(int r)
		{
			return new Rational(r, BigInteger.One, true);
		}

        public static implicit operator Rational(long r)
		{
			return new Rational(r, BigInteger.One, true);
		}

		public static implicit operator Rational(string s)
		{
			return Rational.Parse(s);
		}

		public static Rational operator +(int n2, Rational r1)
		{
			return r1 + n2;
		}

		public static implicit operator double(Rational r1)
		{
		    r1 = r1.Reduce();
            //TODO Всё исправить!
            if (double.IsInfinity((double)r1.Numerator) || double.IsInfinity((double)r1.Denomerator))
                return (double)(r1.Numerator / r1.Denomerator);
            return (double) r1.Numerator / (double) r1.Denomerator;
		}

		public static implicit operator float(Rational r1)
		{
		    r1 = r1.Reduce();
			return (float) ((double) r1.Numerator / (double) r1.Denomerator);
		}

		public int CompareTo(object? obj)
		{
			if(!(obj is Rational)) throw new Exception();
			var r = (Rational) obj;
			return (Numerator * r.Denomerator).CompareTo(Denomerator * r.Numerator);
		}

		public static bool operator ==(Rational r1, Rational r2)
		{
			return r1.Equals(r2);
		}

		public static bool operator !=(Rational r1, Rational r2)
		{
			return !r1.Equals(r2);
		}

		public static bool operator <=(Rational r1, Rational r2)
		{
			return r1.CompareTo(r2) < 1;
		}

		public static bool operator >=(Rational r1, Rational r2)
		{
			return r2 <= r1;
		}

		public static bool operator <(Rational r1, Rational r2)
		{
			return r1.CompareTo(r2) < 0;
		}

		public static bool operator >(Rational r1, Rational r2)
		{
			return r2 < r1;
		}

		public static Rational Max(Rational a, Rational b)
		{
			return a > b ? a : b;
		}
		
		public static Rational Max(params Rational[] values)
		{
			return values.Max();
		}
	}
}
