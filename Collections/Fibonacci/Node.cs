using System;
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections.Fibonacci;

public class Node<T, TKey> : IEquatable<Node<T, TKey>> where T : IEquatable<T> where TKey : IComparable<TKey>
{
   public Node(T data, TKey key)
   {
      Data = data;
      Key = key;

      Right = this;
      Left = this;
      Child = nil;
      Parent = nil;
   }

   public T Data { get; }

   internal Maybe<Node<T, TKey>> Child { get; set; }

   internal Maybe<Node<T, TKey>> Left { get; set; }

   internal Maybe<Node<T, TKey>> Parent { get; set; }

   internal Maybe<Node<T, TKey>> Right { get; set; }

   internal bool Mark { get; set; }

   public TKey Key { get; set; }

   internal int Degree { get; set; }

   public bool Equals(Node<T, TKey> other) => Data.Equals(other.Data);

   public override bool Equals(object obj) => obj is Node<T, TKey> other && Equals(other);

   public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(Data);

   public static bool operator ==(Node<T, TKey> left, Node<T, TKey> right) => Equals(left, right);

   public static bool operator !=(Node<T, TKey> left, Node<T, TKey> right) => !Equals(left, right);
}