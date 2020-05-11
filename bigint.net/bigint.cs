using System;
//BigInt.NET v1.0.0
//Made by Jessie Lesbian <jessielesbian@protonmail.com> https://www.reddit.com/u/jessielesbian
//This software is proudly made by LGBT programmers!
namespace jessielesbian.bigint
{
	/// <summary>
	/// Whole integer.
	/// </summary>
	public sealed class BigNumber : ICloneable, IComparable
	{
		private readonly int[] ints;
		/// <summary>
		/// The sign of the <see cref="BigNumber"/>
		/// </summary>
		public readonly bool negative = false;
		/// <summary>
		/// Creates a <see cref="BigNumber"/> with value = 0.
		/// </summary>
		public BigNumber()
		{
			ints = new int[] { 0 };
		}
		/// <summary>
		/// Cast <see cref="int"/> <paramref name="i"/> into <see cref="BigNumber"/>.
		/// </summary>
		/// <param name="i">The value of the new <see cref="BigNumber"/></param>
		public BigNumber(int i)
		{
			negative = i < 0;
			i = Math.Abs(i);
			if(i == int.MaxValue) {
				ints = new int[] { 0, 1 };
			} else {
				ints = new int[] { i };
			}
		}
		/// <summary>
		/// Raise a <see cref="BigNumber"/>'s value to the nth power.
		/// </summary>
		/// <param name="x">base</param>
		/// <param name="y">power</param>
		/// <returns>x to the power of y</returns>
		public static BigNumber operator ^(BigNumber x, BigNumber y) {
			if(y.negative)
			{
				throw new ArgumentOutOfRangeException();
			}
			string binary = y.ToBinaryRepresentationInternal();
			int length = binary.Length;
			BigNumber p = x;
			BigNumber q = 1;
			for(int i = 0; i < length; i++)
			{
				if(binary[i] == '1')
				{
					q *= p;
				}
				p *= p;
			}
			return q;
		}
		/// <summary>
		/// Increments the <see cref="BigNumber"/> by 1.
		/// </summary>
		public static BigNumber operator ++(BigNumber x) {
			return x + 1;
		}
		/// <summary>
		/// Decrements the <see cref="BigNumber"/> by 1.
		/// </summary>
		public static BigNumber operator --(BigNumber x)
		{
			return x - 1;
		}
		/// <summary>
		/// Creates a <see cref="BigNumber"/> from a <see cref="string"/> representation in base 10.
		/// </summary>
		/// <param name="number">The string representation of the <see cref="BigNumber"/> in base 10.</param>
		public BigNumber(string number)
		{
			if(number.StartsWith("-"))
			{
				number = number.Substring(1);
				negative = true;
			}
			number = ReverseString(number);
			BigNumber p = 1;
			BigNumber q = 0;
			int length = number.Length;
			for(int i = 0; i < length; i++)
			{
				BigNumber digit = Convert.ToInt32(number[i].ToString()) * p;
				q += digit;
				p *= 10;
			}
			ints = q.ints;
		}
		private static string ReverseString(string str)
		{
			int length = str.Length;
			string str2 = "";
			for(int i = 0; i < length; i++)
			{
				str2 = str[i].ToString() + str2;
			}
			return str2;
		}
		/// <summary>
		/// Cast <see cref="int"/> <paramref name="i"/> into <see cref="BigNumber"/>.
		/// </summary>
		/// <param name="i">The value of the new <see cref="BigNumber"/></param>
		public static implicit operator BigNumber(int i) {
			return new BigNumber(i);
		}
		/// <summary>
		/// Creates a <see cref="BigNumber"/> from a <see cref="string"/> representation in base 10.
		/// </summary>
		/// <param name="number">The string representation of the <see cref="BigNumber"/> in base 10.</param>
		public static implicit operator BigNumber(string str)
		{
			return new BigNumber(str);
		}
		private BigNumber(int[] ints, bool clone = true, bool negative = false)
		{
			int length = ints.Length;
			int originalLength = length;
			while(ints[length - 1] == 0)
			{
				if(length == 1)
				{
					if(ints[0] == 0)
					{
						negative = false;
					}
					this.ints = ints;
					this.negative = negative;
					return;
				}
				length--;
			}
			if(length != originalLength)
			{
				int[] n = new int[length];
				for(int i = 0; i < length; i++)
				{
					n[i] = ints[i];
				}
				this.ints = n;
			}
			else
			{
				if(clone)
				{
					this.ints = (int[])ints.Clone();
				}
				else
				{
					this.ints = ints;
				}
			}
			this.negative = negative;
		}
		/// <summary>
		/// Gets the absolute value of a <see cref="BigNumber"/>.
		/// </summary>
		/// <returns>The absolute value of the <see cref="BigNumber"/></returns>
		public BigNumber Abs()
		{
			return new BigNumber(ints, false, false);
		}
		/// <summary>
		/// Returns the sum of <paramref name="x"/> and <paramref name="y"/>
		/// </summary>
		/// <param name="x">Adden</param>
		/// <param name="y">Adden</param>
		/// <returns>The sum of <paramref name="x"/> and <paramref name="y"/></returns>
		public static BigNumber operator +(BigNumber x, BigNumber y) {
			if(x.negative)
			{
				return y - x.Abs();
			}
			else if(y.negative)
			{
				return x - y.Abs();
			}
			else
			{
				int[] xi = x.ints;
				int[] yi = y.ints;
				int xl = xi.Length;
				int yl = yi.Length;
				int min = Math.Min(xl, yl);
				int max = Math.Max(xl, yl);
				int[] result = new int[max];
				bool carry = false;
				bool bigger = max == xl;
				for(int i = 0; i < max; i++)
				{
					if(i >= min)
					{
						if(bigger)
						{
							uint total = (uint)xi[i];
							if(carry)
							{
								total++;
							}
							result[i] = (int)(total % int.MaxValue);
							carry = total == int.MaxValue;
						}
						else
						{
							uint total = (uint)yi[i];
							if(carry)
							{
								total++;
							}
							result[i] = (int)(total % int.MaxValue);
							carry = total == int.MaxValue;
						}
					}
					else
					{
						uint total = (uint)xi[i] + (uint)yi[i];
						if(carry)
						{
							total += 1;
						}
						carry = total >= int.MaxValue;
						result[i] = (int)(total % int.MaxValue);
					}
				}
				if(carry)
				{
					int[] final = new int[max + 1];
					result.CopyTo(final, 0);
					final[max] = 1;
					result = final;
				}
				return new BigNumber(result, false);
			}
		}
		/// <summary>
		/// Negate a <see cref="BigNumber"/>.
		/// </summary>
		/// <returns>The negative absolute value of the current <see cref="BigNumber"/></returns>
		public BigNumber Neg()
		{
			return new BigNumber(ints, false, true);
		}
		/// <summary>
		/// Gets the additive inverse of <see cref="BigNumber"/>.
		/// </summary>
		/// <returns>The additive inverse if <see cref="BigNumber"/></returns>
		public BigNumber InvSign()
		{
			return new BigNumber(ints, false, !negative);
		}
		/// <summary>
		/// Subtracts <paramref name="y"/> from <paramref name="x"/>.
		/// </summary>
		/// <param name="x">minuend</param>
		/// <param name="y">subtrahend</param>
		/// <returns>The diffrence between the minuend and the subtrahend.</returns>
		public static BigNumber operator -(BigNumber x, BigNumber y)
		{
			if(y.negative)
			{
				return x + y.Abs();
			}
			else if(x.negative)
			{
				return (x.Abs() + y).InvSign();
			}
			else
			{
				int[] xi = x.ints;
				int[] yi = y.ints;
				int xl = xi.Length;
				int yl = yi.Length;
				int min = Math.Min(xl, yl);
				int max = Math.Max(xl, yl);
				int[] result = new int[max];
				bool carry = false;
				bool bigger = max == xl;
				for(int i = 0; i < max; i++)
				{
					if(i >= min)
					{
						if(bigger)
						{
							int total = xi[i];
							if(carry)
							{
								total--;
							}
							if(Math.Abs(total) != total)
							{
								total = total + int.MaxValue;
								carry = true;
							}
							else
							{
								carry = false;
							}
							result[i] = total;
						}
						else
						{
							return (y - x).InvSign();
						}
					}
					else
					{
						int cx = xi[i];
						int cy = yi[i];
						int res = cx - cy;
						if(carry)
						{
							res--;
						}
						if(Math.Abs(res) != res)
						{
							res = res + int.MaxValue;
							carry = true;
						}
						else
						{
							carry = false;
						}
						result[i] = res % int.MaxValue;
					}
				}
				if(carry)
				{
					int maxm1 = max - 1;
					result[maxm1] = int.MaxValue - result[maxm1];
				}
				return new BigNumber(result, false, carry);
			}
		}
		/// <summary>
		/// Returns the product of <paramref name="x"/> and <paramref name="y"/>.
		/// </summary>
		/// <param name="x">Multiplier</param>
		/// <param name="y">Multiplier</param>
		/// <returns>The product of <paramref name="x"/> and <paramref name="y"/></returns>
		public static BigNumber operator *(BigNumber x, BigNumber y) {
			string binary = y.ToBinaryRepresentationInternal();
			int length = binary.Length;
			bool negative = x.negative ^ y.negative;
			x = x.Abs();
			y = y.Abs();
			BigNumber p = x;
			BigNumber q = new BigNumber(0);
			for(int i = 0; i < length; i++)
			{
				if(binary[i] == '1')
				{
					q += p;
				}
				p += p;
			}
			if(negative)
			{
				q = q.Neg();
			}
			return q;
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="x"/> is greater than <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <returns><see langword="true"/> if x is greater than <paramref name="y"/>, otherwise returns <see langword="false"/></returns>
		public static bool operator >(BigNumber x, BigNumber y)
		{
			return (y - x).negative;
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="x"/> is less than <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <returns><see langword="true"/> if x is less than <paramref name="y"/>, otherwise returns <see langword="false"/></returns>
		public static bool operator <(BigNumber x, BigNumber y)
		{
			return (x - y).negative;
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="x"/> is greater than or equal to <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <returns><see langword="true"/> if x is greater than or equal to <paramref name="y"/>, otherwise returns <see langword="false"/></returns>
		public static bool operator >=(BigNumber x, BigNumber y)
		{
			return !(x < y);
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="x"/> is less than or equal to <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <returns><see langword="true"/> if x is less than or equal to <paramref name="y"/>, otherwise returns <see langword="false"/></returns>
		public static bool operator <=(BigNumber x, BigNumber y)
		{
			return !(x > y);
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="x"/> is equal to <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <returns><see langword="true"/> if x is equal to <paramref name="y"/>, otherwise returns <see langword="false"/></returns>
		public static bool operator ==(BigNumber x, BigNumber y)
		{
			return !(x != y);
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="x"/> is not equal to <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <returns><see langword="true"/> if x is not equal to <paramref name="y"/>, otherwise returns <see langword="false"/></returns>
		public static bool operator !=(BigNumber x, BigNumber y) {
			return (x > y) || (y > x);
		}
		/// <summary>
		/// Returns the remainder of <paramref name="x"/> divided by <paramref name="y"/>.
		/// </summary>
		/// <param name="x">Dividen</param>
		/// <param name="y">Divisor</param>
		/// <returns>The remainder of <paramref name="x"/> divided by <paramref name="y"/></returns>
		public static BigNumber operator %(BigNumber x, BigNumber y) {
			if(y < 1)
			{
				throw new DivideByZeroException();
			}
			x = x - ((x / y) * y);
			if(x > y)
			{
				x = x % y;
			}
			return x;
		}
		/// <summary>
		/// Returns 1 if the <see cref="BigNumber"/> is positive, 0 it the <see cref="BigNumber"/> is zero or -1 if the <see cref="BigNumber"/> is negative.
		/// </summary>
		/// <returns>1 if the <see cref="BigNumber"/> is positive, 0 it the <see cref="BigNumber"/> is zero or -1 if the <see cref="BigNumber"/> is negative</returns>
		public BigNumber Sign()
		{
			return CompareTo((BigNumber)0);
		}
		/// <summary>
		/// Returns the quotient of <paramref name="x"/> divided by <paramref name="y"/>.
		/// </summary>
		/// <param name="x">Dividen</param>
		/// <param name="y">Divisor</param>
		/// <returns>The quotient of <paramref name="x"/> divided by <paramref name="y"/></returns>
		public static BigNumber operator /(BigNumber x, BigNumber y) {
			if(y == 0)
			{
				throw new DivideByZeroException();
			}
			bool negative = x.negative ^ y.negative;
			x = x.Abs();
			y = y.Abs();
			BigNumber upperBound = x;
			BigNumber lowerBound = 0;
			BigNumber middle = 0;
			while(upperBound > lowerBound)
			{
				middle = (upperBound + lowerBound).Divide2();
				BigNumber multiplication = y * middle;
				if(multiplication < x)
				{
					lowerBound = middle + 1;
				}
				else if(multiplication > x)
				{
					upperBound = middle - 1;
				}
				else
				{
					return middle;
				}
			}
			while((y * middle) < x)
			{
				middle += 1;
			}
			while((y * middle) > x)
			{
				middle -= 1;
			}
			if(negative)
			{
				middle = middle.Neg();
			}
			return middle;
		}
		private static BigNumber FromBinaryInternal(string binary, bool negative = false)
		{
			int length = binary.Length;
			BigNumber p = 1;
			BigNumber q = new BigNumber(new int[] { 0 }, false, negative);
			for(int i = 0; i < length; i++)
			{
				if(binary[i] == '1')
				{
					q += p;
				}
				p = p + p;
			}
			return q;
		}
		/// <summary>
		/// Returns the <see cref="string"/> representation of value of the <see cref="BigNumber"/> in base 10.
		/// </summary>
		/// <returns>The <see cref="string"/> representation of value of the <see cref="BigNumber"/> in base 10</returns>
		public override string ToString()
		{
			string str = "";
			BigNumber bigNumber = Abs();
			if(bigNumber < 10)
			{
				if(negative)
				{
					return "-" + bigNumber.ints[0].ToString();
				}
				else
				{
					return bigNumber.ints[0].ToString();
				}
			}
			else
			{
				while(bigNumber > 0)
				{
					str += (bigNumber % 10).ToString();
					bigNumber /= 10;
				}
				if(negative)
				{
					str += "-";
				}
				return ReverseString(str);
			}
		}
		private string ToBinaryRepresentationInternal()
		{
			string bin = "";
			BigNumber bigNumber = Abs();
			if(bigNumber == 0)
			{
				bin = "0";
			}
			else if(bigNumber == 1)
			{
				bin = "1";
			}
			else
			{
				while(bigNumber > 0)
				{
					BigNumber prev = bigNumber;
					bigNumber = bigNumber.Divide2();
					BigNumber remainder = prev - (bigNumber + bigNumber);
					bin += remainder.ints[0].ToString();
				}
			}
			return bin;
		}
		/// <summary>
		/// Divides the <see cref="BigNumber"/> by 2.
		/// </summary>
		/// <returns>The <see cref="BigNumber"/> divided by by 2</returns>
		public BigNumber Divide2()
		{
			BigNumber bigNumber = Abs();
			BigNumber add = 0;
			BigNumber prevAdd = add;
			while((add + add) < bigNumber)
			{
				prevAdd = add;
				BigNumber more = 1;
				BigNumber prev = more;
				while((add + more + add + more) < bigNumber) {
					prev = more;
					more += more;
				}
				more = prev;
				add += more;
			}
			add = prevAdd;
			while((add + add) < bigNumber)
			{
				add += 1;
			}
			while((add + add) > bigNumber)
			{
				add -= 1;
			}
			if(negative)
			{
				add = add.Neg();
			}
			return add;
		}
		/// <summary>
		/// Returns a clone of the current <see cref="BigNumber"/>.
		/// </summary>
		/// <returns>A clone of the current <see cref="BigNumber"/></returns>
		public object Clone()
		{
			return new BigNumber(ints, true, negative);
		}
		/// <summary>
		/// Compares two <see cref="BigNumber"/>s.
		/// </summary>
		/// <param name="obj">The other <see cref="BigNumber"/</param>
		/// <returns>1 if the current <see cref="BigNumber"/> is greater than the other <see cref="BigNumber"/>, 0 if the current <see cref="BigNumber"/> is equal to the other <see cref="BigNumber"/> and -1 if the current <see cref="BigNumber"/> is less than the other <see cref="BigNumber"/></returns>
		public int CompareTo(object obj)
		{
			BigNumber bigNumber = (BigNumber)obj;
			if(this > bigNumber)
			{
				return 1;
			}
			else if(this < bigNumber)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}
		/// <summary>
		/// Returns <see langword="true"/> if x is not equal to <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <param name="obj">The other <see cref="BigNumber"/></param>
		/// <returns><see langword="true"/> if x is not equal to <paramref name="y"/>, otherwise returns <see langword="false"/></returns>
		public override bool Equals(object obj)
		{
			return this == (BigNumber)obj;
		}
		public override int GetHashCode()
		{
			Random random = new Random(0);
			foreach(int i in ints)
			{
				random = new Random(random.Next() - i);
			}
			return random.Next();
		}
		/// <summary>
		/// Returns the <see cref="string"/> representation of value of the <see cref="BigNumber"/> in binary.
		/// </summary>
		/// <returns>the <see cref="string"/> representation of value of the <see cref="BigNumber"/> in binary</returns>
		public string ToBinaryRepresentaion()
		{
			if(negative)
			{
				return "-" + ReverseString(ToBinaryRepresentationInternal());
			}
			else
			{
				return ReverseString(ToBinaryRepresentationInternal());
			}
		}
		/// <summary>
		/// Create a <see cref="BigNumber"/> from a <see cref="byte"/> array containing it's Base256 representation.
		/// </summary>
		/// <param name="bytes"></param>
		/// <param name="negative"></param>
		public BigNumber(byte[] bytes, bool negative = false)
		{
			BigNumber p = 1;
			BigNumber q = 0;
			int length = bytes.Length;
			for(int i = 0; i < length; i++)
			{
				BigNumber digit = p * bytes[length - i - 1];
				q += digit;
				p *= 256;
			}
			ints = q.ints;
			this.negative = negative;
		}
		/// <summary>
		/// Base256 encode a <see cref="BigNumber"/> into a <see cref="byte"/> array.
		/// </summary>
		/// <returns>A <see cref="byte"/> array containing the Base256 representation of the current <see cref="BigNumber"/></returns>
		public byte[] ToByteArray()
		{
			int length = ints.Length;
			byte[] bytes = new byte[length * 4];
			int realLength = 0;
			BigNumber bigNumber = Abs();
			while(bigNumber > 0)
			{
				byte remainder = (byte) (bigNumber % 256).ints[0];
				bigNumber /= 256;
				bytes[realLength++] = remainder;
			}
			byte[] shorter = new byte[realLength];
			for(int i = 0; i < realLength; i++)
			{
				shorter[realLength - i - 1] = bytes[i];
			}
			bytes = shorter;
			return bytes;
		}
		internal string X()
		{
			return ToBinaryRepresentationInternal();
		}
	}
	/// <summary>
	/// Utility functions that you may find usefull.
	/// </summary>
	public static class Utils
	{
		/// <summary>
		/// Calculates the Greatest Common Divisor of <paramref name="x"/> and <paramref name="y"/>.
		/// </summary>
		/// <returns>The Greatest Common Divisor of x and y.</returns>
		public static BigNumber GCD(BigNumber x, BigNumber y)
		{
			if(x == y)
			{
				return x;
			}
			else
			{
				if(x.negative && y.negative)
				{
					return GCD(x.Abs(), y.Abs()) * -1;
				}
				else
				{
					x = x.Abs();
					y = y.Abs();
					if(x == 0)
					{
						return 1;
					}
					else if(y == 0)
					{
						return 1;
					}
					else
					{
						if(x < y)
						{
							BigNumber temp = x;
							x = y;
							y = temp;
						}
						BigNumber z = y;
						BigNumber prev = z;
						while(z != 0)
						{
							prev = z;
							z = x % z;
						}
						if(y % prev != 0)
						{
							return GCD(prev, y);
						}
						else
						{
							return prev;
						}
					}
				}
			}
		}
		/// <summary>
		/// Check if <paramref name="bigNumber"/> is prime
		/// </summary>
		/// <returns>True if the number is prime, otherwise return false.</returns>
		public static bool IsPrime(this BigNumber bigNumber)
		{
			if(bigNumber < 2)
			{
				return false;
			}
			else if(bigNumber == 2)
			{
				return true;
			}
			else if(bigNumber == 3)
			{
				return true;
			}
			else if(bigNumber % 2 == 0)
			{
				return false;
			}
			else
			{
				BigNumber phi = bigNumber - 1;
				IntegerModuloP alpha = new IntegerModuloP(2, bigNumber);
				alpha ^= phi;
				IntegerModuloP beta = new IntegerModuloP(3, bigNumber);
				beta ^= phi;
				return (alpha.value == 1) && (beta.value == 1);
			}
		}
		/// <summary>
		/// Gets the next prime number.
		/// </summary>
		public static BigNumber NextPrime(this BigNumber bigNumber)
		{
			if(bigNumber < 2)
			{
				return 2;
			}
			else
			{
				bigNumber++;
				if(bigNumber % 2 == 0)
				{
					bigNumber++;
				}
				while(bigNumber.IsPrime() == false)
				{
					bigNumber += 2;
				}
				return bigNumber;
			}
		}
		/// <summary>
		/// Gets the previous prime number.
		/// </summary>
		public static BigNumber PrevPrime(this BigNumber bigNumber)
		{
			if(bigNumber < 4)
			{
				return 2;
			}
			else
			{
				bigNumber--;
				if(bigNumber % 2 == 0)
				{
					bigNumber--;
				}
				while(bigNumber.IsPrime() == false)
				{
					bigNumber -= 2;
				}
				return bigNumber;
			}
		}
	}
	/// <summary>
	/// Integer over modulo P, have many applications in cryptography.
	/// </summary>
	public sealed class IntegerModuloP : ICloneable
	{
		/// <summary>
		/// The value of the <see cref="IntegerModuloP"/>.
		/// </summary>
		public readonly BigNumber value;
		/// <summary>
		/// The modulo of the <see cref="IntegerModuloP"/>.
		/// </summary>
		public readonly BigNumber modulo;
		/// <summary>
		/// Creates a new <see cref="IntegerModuloP"/>.
		/// </summary>
		public IntegerModuloP(BigNumber value, BigNumber modulo)
		{
			this.value = value % modulo;
			this.modulo = modulo;
		}
		/// <summary>
		/// Returns the remainder of the sum of <paramref name="x"/> and <paramref name="y"/> divided by the modulo.
		/// </summary>
		/// <param name="x">Adden</param>
		/// <param name="y">Adden</param>
		/// <returns>The remainder of the sum of <paramref name="x"/> and <paramref name="y"/> divided by the modulo</returns>
		public static IntegerModuloP operator +(IntegerModuloP x, BigNumber y)
		{
			return x + new IntegerModuloP(y, x.modulo);
		}
		/// <summary>
		/// Returns the remainder of the diffrence of <paramref name="x"/> and <paramref name="y"/> divided by the modulo.
		/// </summary>
		/// <param name="x">minuend</param>
		/// <param name="y">subtrahend</param>
		/// <returns>The remainder of the diffrence of <paramref name="x"/> and <paramref name="y"/> divided by the modulo</returns>
		public static IntegerModuloP operator -(IntegerModuloP x, BigNumber y)
		{
			return x - new IntegerModuloP(y, x.modulo);
		}
		/// <summary>
		/// Returns the remainder of the product of <paramref name="x"/> and <paramref name="y"/> divided by the modulo.
		/// </summary>
		/// <param name="x">Multiplier</param>
		/// <param name="y">Multiplier</param>
		/// <returns>The remainder of the product of <paramref name="x"/> and <paramref name="y"/> divided by the modulo</returns>
		public static IntegerModuloP operator *(IntegerModuloP x, BigNumber y)
		{
			return x * new IntegerModuloP(y, x.modulo);
		}
		/// <summary>
		/// Returns the remainder of the quotient of <paramref name="x"/> and <paramref name="y"/> divided by the modulo.
		/// </summary>
		/// <param name="x">Multiplier</param>
		/// <param name="y">Multiplier</param>
		/// <returns>The remainder of the quotient of <paramref name="x"/> and <paramref name="y"/> divided by the modulo</returns>
		public static IntegerModuloP operator /(IntegerModuloP x, BigNumber y)
		{
			return x / new IntegerModuloP(y, x.modulo);
		}
		/// <summary>
		/// Returns the remainder of the sum of <paramref name="x"/> and <paramref name="y"/> divided by the modulo.
		/// </summary>
		/// <param name="x">Adden</param>
		/// <param name="y">Adden</param>
		/// <returns>The remainder of the sum of <paramref name="x"/> and <paramref name="y"/> divided by the modulo</returns>
		public static IntegerModuloP operator +(IntegerModuloP x, IntegerModuloP y)
		{
			if(x.modulo != y.modulo)
			{
				throw new InvalidOperationException();
			}
			else
			{
				return new IntegerModuloP(x.value + y.value, x.modulo);
			}
		}
		/// <summary>
		/// Returns the remainder of the diffrence of <paramref name="x"/> and <paramref name="y"/> divided by the modulo.
		/// </summary>
		/// <param name="x">minuend</param>
		/// <param name="y">subtrahend</param>
		/// <returns>The remainder of the diffrence of <paramref name="x"/> and <paramref name="y"/> divided by the modulo</returns>
		public static IntegerModuloP operator -(IntegerModuloP x, IntegerModuloP y)
		{
			if(x.modulo != y.modulo)
			{
				throw new InvalidOperationException();
			}
			else
			{
				return new IntegerModuloP(x.value - y.value, x.modulo);
			}
		}
		/// <summary>
		/// Returns the remainder of the product of <paramref name="x"/> and <paramref name="y"/> divided by the modulo.
		/// </summary>
		/// <param name="x">Multiplier</param>
		/// <param name="y">Multiplier</param>
		/// <returns>The remainder of the product of <paramref name="x"/> and <paramref name="y"/> divided by the modulo</returns>
		public static IntegerModuloP operator *(IntegerModuloP x, IntegerModuloP y)
		{
			if(x.modulo != y.modulo)
			{
				throw new InvalidOperationException();
			}
			else
			{
				return new IntegerModuloP(x.value * y.value, x.modulo);
			}
		}
		/// <summary>
		/// Returns the remainder of the quotient of <paramref name="x"/> and <paramref name="y"/> divided by the modulo or 0 if the modulo is composite and a quotient can't be found.
		/// </summary>
		/// <param name="x">Multiplier</param>
		/// <param name="y">Multiplier</param>
		/// <returns>The remainder of the quotient of <paramref name="x"/> and <paramref name="y"/> divided by the modulo</returns>
		public static IntegerModuloP operator /(IntegerModuloP x, IntegerModuloP y) {
			if(x.modulo != y.modulo)
			{
				throw new InvalidOperationException();
			}
			if(Utils.GCD(y.value, x.modulo) == 1)
			{
				return x * ModInv(y.value, x.modulo);
			}
			else if(Utils.GCD(x.value, x.modulo) == 1)
			{
				return x * ModInv(x.modulo, x.modulo);
			}
			else
			{
				return new IntegerModuloP(1, x.modulo);
			}
		}
		private static BigNumber ModInv(BigNumber a, BigNumber m)
		{
			//ported from here: https://www.geeksforgeeks.org/multiplicative-inverse-under-modulo-m/
			try
			{
				BigNumber m0 = m;
				BigNumber y = 0, x = 1;
				if(m == 1)
				{
					return 0;
				}
				else
				{
					while(a > 1)
					{
						// q is quotient 
						BigNumber q = a / m;
						BigNumber t = m;

						// m is remainder now, process same as 
						// Euclid's algo 
						m = a % m;
						a = t;
						t = y;

						// Update y and x 
						y = x - q * y;
						x = t;
					}

					// Make x positive 
					if(x < 0)
					{
						x += m0;
					}
					return x;
				}
			} catch
			{
				return 0;
			}
		}
		/// <summary>
		/// Get the modular multiplicative inverse of the current <see cref="IntegerModuloP"/>.
		/// </summary>
		/// <returns>The modular multiplicative inverse of the current <see cref="IntegerModuloP"/></returns>
		public IntegerModuloP GetMultiplicativeInverse()
		{
			return new IntegerModuloP(ModInv(value, modulo), modulo);
		}
		/// <summary>
		/// Returns the remainder of <paramref name="x"/> to the power of <paramref name="y"/> divided by the modulo.
		/// </summary>
		/// <param name="x">Multiplier</param>
		/// <param name="y">Multiplier</param>
		/// <returns>The remainder of <paramref name="x"/> to the power of <paramref name="y"/> divided by the modulo</returns>
		public static IntegerModuloP operator ^(IntegerModuloP x, BigNumber y)
		{
			string binary = y.X();
			int length = binary.Length;
			IntegerModuloP p = x;
			IntegerModuloP q = new IntegerModuloP(1, x.modulo);
			for(int i = 0; i < length; i++)
			{
				if(binary[i] == '1')
				{
					q *= p;
				}
				p *= p;
			}
			return q;
		}
		/// <summary>
		/// Returns the remainder of the current <see cref="IntegerModuloP"/> incremented by 1 divided by the modulo.
		/// </summary>
		/// <param name="x">The current <see cref="IntegerModuloP"/></param>
		/// <returns>The remainder of the current <see cref="IntegerModuloP"/> incremented by 1 divided by the modulo</returns>
		public static IntegerModuloP operator ++(IntegerModuloP x)
		{
			return x + 1;
		}
		/// <summary>
		/// Returns the remainder of the current <see cref="IntegerModuloP"/> decremented by 1 divided by the modulo.
		/// </summary>
		/// <param name="x">The current <see cref="IntegerModuloP"/></param>
		/// <returns>The remainder of the current <see cref="IntegerModuloP"/> decremented by 1 divided by the modulo</returns>
		public static IntegerModuloP operator --(IntegerModuloP x)
		{
			return x - 1;
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="x"/> is equal to <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <returns><see langword="true"/> if x is equal to <paramref name="y"/>, otherwise returns <see langword="false"/>"/></returns>
		public static bool operator ==(IntegerModuloP x, IntegerModuloP y)
		{
			return x.Equals(y);
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="x"/> is not equal to <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <returns><see langword="true"/> if x is not equal to <paramref name="y"/>, otherwise returns <see langword="false"/>"/></returns>
		public static bool operator !=(IntegerModuloP x, IntegerModuloP y)
		{
			return !x.Equals(y);
		}
		/// <summary>
		/// Returns a clone of the current <see cref="IntegerModuloP"/>.
		/// </summary>
		/// <returns>A clone of the current <see cref="IntegerModuloP"/></returns>
		public object Clone()
		{
			return new IntegerModuloP((BigNumber) value.Clone(), (BigNumber) modulo.Clone());
		}
		/// <summary>
		/// Returns the <see cref="string"/> representation of value of the <see cref="IntegerModuloP"/> in base 10.
		/// </summary>
		/// <returns>The <see cref="string"/> representation of value of the <see cref="IntegerModuloP"/> in base 10</returns>
		public override string ToString()
		{
			return value
.ToString();
		}
		/// <summary>
		/// Returns <see langword="true"/> if x is not equal to <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <param name="obj">The other <see cref="BigNumber"/> or <see cref="IntegerModuloP"/></param>
		/// <returns><see langword="true"/> if x is not equal to <paramref name="y"/>, otherwise returns <see langword="false"/></returns>
		public override bool Equals(object obj)
		{
			if(obj is BigNumber)
			{
				obj = new IntegerModuloP((BigNumber) obj, modulo);
			}
			IntegerModuloP integerModuloP = (IntegerModuloP)obj;
			return (integerModuloP.modulo == modulo) && (integerModuloP.value == value);
		}
		public override int GetHashCode()
		{
			ulong l = (ulong) value.GetHashCode();
			l *= int.MaxValue;
			l += (ulong) modulo.GetHashCode();
			return (int) l % 2147483629;
		}
	}
	/// <summary>
	/// LGBT programmers don't use the F-word.
	/// </summary>
	public sealed class Fraction : ICloneable, IComparable
	{
		/// <summary>
		/// The numerator part of the <see cref="Fraction"/>
		/// </summary>
		public readonly BigNumber numerator;
		/// <summary>
		/// The denominator part of the <see cref="Fraction"/>
		/// </summary>
		public readonly BigNumber denominator;
		/// <summary>
		/// Create a <see cref="Fraction"/> whose value is zero.
		/// </summary>
		public Fraction()
		{
			numerator = 0;
			denominator = 1;
		}
		/// <summary>
		/// Create a <see cref="Fraction"/> with the following numerator and denominator.
		/// </summary>
		/// <param name="numerator">The numerator of the fraction</param>
		/// <param name="denominator">The denominator of the fraction</param>
		public Fraction(BigNumber numerator, BigNumber denominator)
		{
			BigNumber common = Utils.GCD(numerator, denominator);
			numerator /= common;
			denominator /= common;
			if(denominator.negative)
			{
				numerator = numerator.Neg();
				denominator = denominator.Abs();
			}
			if(numerator == 0)
			{
				denominator = 1;
			}
			if(denominator == 0)
			{
				numerator = 1;
			}
			this.numerator = numerator;
			this.denominator = denominator;
		}
		/// <summary>
		/// Gets the string representation of the <see cref="Fraction"/> in the form numerator/denominator
		/// </summary>
		public override string ToString()
		{
			return numerator.ToString() + "/" + denominator.ToString();
		}
		/// <summary>
		/// Returns a clone of the current <see cref="Fraction"/>.
		/// </summary>
		/// <returns>The current <see cref="Fraction"/></returns>
		public object Clone()
		{
			return new Fraction((BigNumber) numerator.Clone(), (BigNumber) denominator.Clone());
		}
		/// <summary>
		/// Returns true if the <see cref="Fraction"/> is negative, otherwise return false.
		/// </summary>
		public bool IsNegative
		{
			get
			{
				return numerator.negative;
			}
		}
		/// <summary>
		/// Returns 1 if the current <see cref="Fraction"/> is greater than the other <see cref="Fraction"/>, 0 if the current fraction is equal to the other fraction or -1 if the current fraction is less than the other fraction.
		/// </summary>
		/// <param name="obj">The other <see cref="Fraction"/></param>
		/// <returns>1 if the current <see cref="Fraction"/> is greater than the other <see cref="Fraction"/>, 0 if the current fraction is equal to the other fraction or -1 if the current fraction is less than the other fraction</returns>
		public int CompareTo(object obj)
		{
			if(obj is BigNumber) {
				obj = new Fraction((BigNumber) obj, denominator);
			}
			Fraction fraction = (Fraction) obj;
			Fraction subtraction = this - fraction;
			if(subtraction.numerator.negative)
			{
				return -1;
			}
			else if(subtraction.numerator == 0)
			{
				return 0;
			}
			else
			{
				return 1;
			}

		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="x"/> is greater than <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <returns><see langword="true"/> if <paramref name="x"/> is greater than <paramref name="y"/>, otherwise returns <see langword="false"/></returns>
		public static bool operator >(Fraction x, Fraction y)
		{
			return x.CompareTo(y) == 1;
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="x"/> is less than <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <returns><see langword="true"/> if <paramref name="x"/> is less than <paramref name="y"/>, otherwise returns <see langword="false"/></returns>
		public static bool operator <(Fraction x, Fraction y)
		{
			return x.CompareTo(y) == -1;
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="x"/> is greater than or equal to <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <returns><see langword="true"/> if <paramref name="x"/> is greater than or equal to <paramref name="y"/>, otherwise returns <see langword="false"/></returns>
		public static bool operator >=(Fraction x, Fraction y)
		{
			return x.CompareTo(y) != -1;
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="x"/> is less than or equal to <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <returns><see langword="true"/> if <paramref name="x"/> is less than or equal to <paramref name="y"/>, otherwise returns <see langword="false"/></returns>
		public static bool operator <=(Fraction x, Fraction y)
		{
			return x.CompareTo(y) != 1;
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="x"/> is equal to <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <returns><see langword="true"/> if <paramref name="x"/> is equal to <paramref name="y"/>, otherwise returns <see langword="false"/></returns>
		public static bool operator ==(Fraction x, Fraction y)
		{
			return x.CompareTo(y) == 0;
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="x"/> is not equal to <paramref name="y"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <returns><see langword="true"/> if <paramref name="x"/> is not equal to <paramref name="y"/>, otherwise returns <see langword="false"/></returns>
		public static bool operator !=(Fraction x, Fraction y)
		{
			return x.CompareTo(y) != 0;
		}
		/// <summary>
		/// Returns the sum of <paramref name="x"/> and <paramref name="y"/>
		/// </summary>
		/// <param name="x">Adden</param>
		/// <param name="y">Adden</param>
		public static Fraction operator +(Fraction x, Fraction y)
		{
			return new Fraction((x.numerator * y.denominator) + (y.numerator * x.denominator), x.denominator * y.denominator);
		}
		/// <summary>
		/// Subtracts <paramref name="y"/> from <paramref name="x"/>.
		/// </summary>
		/// <param name="x">minuend</param>
		/// <param name="y">subtrahend</param>
		/// <returns>The diffrence between the minuend and the subtrahend.</returns>
		public static Fraction operator -(Fraction x, Fraction y)
		{
			return new Fraction((x.numerator * y.denominator) - (y.numerator * x.denominator), x.denominator * y.denominator);
		}
		/// <summary>
		/// Returns the product of <paramref name="x"/> and <paramref name="y"/>.
		/// </summary>
		/// <param name="x">Multiplier</param>
		/// <param name="y">Multiplier</param>
		/// <returns>The product of <paramref name="x"/> and <paramref name="y"/></returns>
		public static Fraction operator *(Fraction x, Fraction y)
		{
			return new Fraction(x.numerator * y.numerator, x.denominator * y.denominator);
		}
		/// <summary>
		/// Returns the quotient of <paramref name="x"/> divided by <paramref name="y"/>.
		/// </summary>
		/// <param name="x">Dividen</param>
		/// <param name="y">Divisor</param>
		/// <returns>The quotient of <paramref name="x"/> divided by <paramref name="y"/></returns>
		public static Fraction operator /(Fraction x, Fraction y)
		{
			return new Fraction(x.numerator * y.denominator, x.denominator * y.numerator);
		}
		/// <summary>
		/// Returns the remainder of <paramref name="x"/> divided by <paramref name="y"/>.
		/// </summary>
		/// <param name="x">Dividen</param>
		/// <param name="y">Divisor</param>
		/// <returns>The remainder of <paramref name="x"/> divided by <paramref name="y"/></returns>
		public static Fraction operator %(Fraction x, Fraction y)
		{
			Fraction upperBound = x.Abs();
			bool negative = x.IsNegative;
			Fraction lowerBound = new Fraction();
			Fraction middle = 0;
			while(upperBound > lowerBound)
			{
				middle = (upperBound + lowerBound) / 2;
				Fraction multiplication = middle * y;
				if(multiplication < x)
				{
					lowerBound = middle + 1;
				}
				else if(multiplication > x)
				{
					upperBound = middle - 1;
				}
				else
				{
					return middle;
				}
			}
			while((y * middle) < x)
			{
				middle += 1;
			}
			while((y * middle) > x)
			{
				middle -= 1;
			}
			if(negative)
			{
				middle = middle.Neg();
			}
			return x - (middle * y);
		}
		/// <summary>
		/// Casts <see cref="BigNumber"/> into <see cref="Fraction"/>
		/// </summary>
		public static implicit operator Fraction(BigNumber bigNumber) {
			return new Fraction(bigNumber, 1);
		}
		/// <summary>
		/// Casts <see cref="int"/> into <see cref="Fraction"/>
		/// </summary>
		public static implicit operator Fraction(int i)
		{
			return new Fraction(i, 1);
		}
		/// <summary>
		/// Returns the absolute value of the current <see cref="Fraction"/>.
		/// </summary>
		/// <returns>The absolute value of the current <see cref="Fraction"/></returns>
		public Fraction Abs()
		{
			return new Fraction(numerator.Abs(), denominator);
		}
		/// <summary>
		/// Returns the negative absolute value of the current <see cref="Fraction"/>.
		/// </summary>
		/// <returns>The negative absolute value of the current <see cref="Fraction"/></returns>
		public Fraction Neg()
		{
			return new Fraction(numerator.Neg(), denominator);
		}
		/// <summary>
		/// Returns the current <see cref="Fraction"/> with inverted sign.
		/// </summary>
		/// <returns>The current <see cref="Fraction"/> with inverted sign</returns>
		public Fraction InvSign()
		{
			return new Fraction(numerator.InvSign(), denominator);
		}
		/// <summary>
		/// Returns the multiplicative inverse of the current <see cref="Fraction"/>.
		/// </summary>
		/// <returns>The multiplicative inverse of the current <see cref="Fraction"/></returns>
		public Fraction Invert()
		{
			return new Fraction(denominator, numerator);
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="obj"/> is equal to the current <see cref="Fraction"/>, otherwise returns <see langword="false"/>.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is equal to the current <see cref="Fraction"/>, otherwise returns <see langword="false"/></returns>
		public override bool Equals(object obj)
		{
			Fraction fraction = (Fraction)obj;
			return this == fraction;
		}
		public override int GetHashCode()
		{
			ulong l = (ulong)numerator.GetHashCode();
			l *= int.MaxValue;
			l += (ulong)denominator.GetHashCode();
			return (int)l % 2147483629;
		}
	}
}
