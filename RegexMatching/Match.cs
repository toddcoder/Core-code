﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Arrays;

namespace Core.RegexMatching
{
   public class Match : Group, IEnumerable<Group>
   {
      public Group[] Groups { get; set; }

      public int MatchCount(int groupIndex) => Groups[groupIndex].Length;

      public string FirstGroup => Groups.Of(1, Empty).Text;

      public string SecondGroup => Groups.Of(2, Empty).Text;

      public string ThirdGroup => Groups.Of(3, Empty).Text;

      public string FourthGroup => Groups.Of(4, Empty).Text;

      public string FifthGroup => Groups.Of(5, Empty).Text;

      public (string firstGroup, string secondGroup) Groups2() => (FirstGroup, SecondGroup);

      public (string firstGroup, string secondGroup, string thirdGroup) Groups3() => (FirstGroup, SecondGroup, ThirdGroup);

      public IEnumerator<Group> GetEnumerator()
      {
         foreach (var group in Groups.Skip(1))
         {
            yield return group;
         }
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   }
}