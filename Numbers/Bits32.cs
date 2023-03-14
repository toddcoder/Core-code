using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;

namespace Core.Numbers;

public class Bits32<TEnum> : IEnumerable<TEnum> where TEnum : struct, IConvertible
{
   protected static TEnum enumOf(int value) => (TEnum)Enum.ToObject(typeof(TEnum), value);

   public static bool GetBit(TEnum bits, TEnum bit) => (bits.ToInt32(null) & bit.ToInt32(null)) != 0;

   public static TEnum SetBit(TEnum bits, TEnum bit, bool value)
   {
      var bitsValue = bits.ToInt32(null);
      var bitValue = bit.ToInt32(null);

      return enumOf(value ? bitsValue | bitValue : bitsValue & ~bitValue);
   }

   public static TEnum ReverseBit(TEnum bits, TEnum bit) => enumOf(bits.ToInt32(null) ^ bit.ToInt32(null));

   public static Bits32<TEnum> operator +(Bits32<TEnum> bits, TEnum value)
   {
      bits[value] = true;
      return bits;
   }

   public static Bits32<TEnum> operator -(Bits32<TEnum> bits, TEnum value)
   {
      bits[value] = false;
      return bits;
   }

   public static implicit operator Bits32<TEnum>(TEnum bits) => new(bits);

   public static implicit operator TEnum(Bits32<TEnum> bits) => bits.Value;

   protected TEnum bits;

   public Bits32(TEnum bits) => this.bits = bits;

   public bool this[TEnum bit]
   {
      get => GetBit(bits, bit);
      set => bits = SetBit(bits, bit, value);
   }

   public TEnum Value
   {
      get => bits;
      set => bits = value;
   }

   public void Reverse(TEnum bit) => bits = ReverseBit(bits, bit);

   public IEnumerator<TEnum> GetEnumerator() => Enumerable().GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public IEnumerable<TEnum> Enumerable() => Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Where(item => GetBit(bits, item));

   public override string ToString() => Enumerable().ToString(" | ");
}