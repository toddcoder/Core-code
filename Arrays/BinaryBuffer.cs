using System;
using static System.Math;

namespace Core.Arrays
{
   public class BinaryBuffer
   {
      protected byte[] bytes;

      public BinaryBuffer(int length) => bytes = new byte[length];

      public BinaryBuffer Next { get; set; }

      public void Add(byte[] bytesToAdd) => Array.Copy(bytesToAdd, bytes, Min(bytesToAdd.Length, bytes.Length));

      public void CopyTo(byte[] bytes, int index) => this.bytes.CopyTo(bytes, index);
   }
}