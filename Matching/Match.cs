using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Arrays;
using Core.Strings;

namespace Core.Matching
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

      public void Deconstruct(out string firstGroup, out string secondGroup)
      {
         firstGroup = FirstGroup;
         secondGroup = SecondGroup;
      }

      public void Deconstruct(out string firstGroup, out string secondGroup, out string thirdGroup, out string fourthGroup)
      {
         firstGroup = FirstGroup;
         secondGroup = SecondGroup;
         thirdGroup = ThirdGroup;
         fourthGroup = FourthGroup;
      }

      public void Deconstruct(out string firstGroup, out string secondGroup, out string thirdGroup, out string fourthGroup, out string fifthGroup)
      {
         firstGroup = FirstGroup;
         secondGroup = SecondGroup;
         thirdGroup = ThirdGroup;
         fourthGroup = FourthGroup;
         fifthGroup = FifthGroup;
      }

      public IEnumerable<Slice> Slices()
      {
         foreach (var group in Groups)
         {
            yield return group.Slice;
         }
      }
   }
}