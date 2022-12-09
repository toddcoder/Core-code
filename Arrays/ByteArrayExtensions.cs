using System;

namespace Core.Arrays;

public static class ByteArrayExtensions
{
   public static string ToBase64(this byte[] source) => Convert.ToBase64String(source);
}