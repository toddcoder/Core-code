using System;
using Core.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
	class Person : IEquatable<Person>
	{
		string name;
		int age;
		Equatable<Person> equatable;

		public Person(string name, int age)
		{
			this.name = name;
			this.age = age;

			equatable = new Equatable<Person>(this, "name", "age");
		}

		public string Name => name;

		public int Age => age;

		public bool Equals(Person other) => equatable.Equals(other);

		public override bool Equals(object obj) => equatable.Equals(obj);

		public override int GetHashCode() => equatable.GetHashCode();
	}

   [TestClass]
   public class ObjectTests
   {
      [TestMethod]
      public void EquatableTest()
      {
	      var person1 = new Person("Digby J. Egghead", 50);
	      var person2 = new Person("Mortimer Snerd", 100);
			Assert.IsFalse(person1.Equals(person2));
			Console.WriteLine($"person1.GetHashCode() == {person1.GetHashCode()}");
			Console.WriteLine($"person2.GetHashCode() == {person2.GetHashCode()}");
      }
   }
}
