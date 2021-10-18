using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Arrays;
using Core.Objects;
using Core.Strings;

namespace Core.Matching
{
   public class Match : Group, IEnumerable<Group>
   {
      protected LateLazy<MatchResult> matchResult;

      public Match()
      {
         matchResult = new LateLazy<MatchResult>(errorMessage: "Match result has not been set");
      }

      public MatchResult MatchResult
      {
         get => matchResult.Value;
         set => matchResult.ActivateWith(() => value);
      }

      public Group[] Groups { get; set; }

      public int MatchCount(int groupIndex) => Groups[groupIndex].Length;

      public string this[int index]
      {
         get => matchResult.Value[Which, index];
         set => matchResult.Value[Which, index] = value;
      }

      public override string Text
      {
         get => base.Text;
         set
         {
            base.Text = value;
            if (matchResult.HasActivator)
            {
               matchResult.Value[Which] = value;
            }
         }
      }

      public string ZerothGroup
      {
         get => Groups.Of(0, Empty).Text;
         set
         {
            Groups.Of(0, Empty).Text = value;
            matchResult.Value[Which, 0] = value;
         }
      }

      public string FirstGroup
      {
         get => Groups.Of(1, Empty).Text;
         set
         {
            Groups.Of(1, Empty).Text = value;
            matchResult.Value[Which, 1] = value;
         }
      }

      public string SecondGroup
      {
         get => Groups.Of(2, Empty).Text;
         set
         {
            Groups.Of(2, Empty).Text = value;
            matchResult.Value[Which, 2] = value;
         }
      }

      public string ThirdGroup
      {
         get => Groups.Of(3, Empty).Text;
         set
         {
            Groups.Of(3, Empty).Text = value;
            matchResult.Value[Which, 3] = value;
         }
      }

      public string FourthGroup
      {
         get => Groups.Of(4, Empty).Text;
         set
         {
            Groups.Of(4, Empty).Text = value;
            matchResult.Value[Which, 4] = value;
         }
      }

      public string FifthGroup
      {
         get => Groups.Of(5, Empty).Text;
         set
         {
            Groups.Of(5, Empty).Text = value;
            matchResult.Value[Which, 5] = value;
         }
      }

      public string SixthGroup
      {
         get => Groups.Of(6, Empty).Text;
         set
         {
            Groups.Of(6, Empty).Text = value;
            matchResult.Value[Which, 6] = value;
         }
      }

      public string SeventhGroup
      {
         get => Groups.Of(7, Empty).Text;
         set
         {
            Groups.Of(7, Empty).Text = value;
            matchResult.Value[Which, 7] = value;
         }
      }

      public string EighthGroup
      {
         get => Groups.Of(8, Empty).Text;
         set
         {
            Groups.Of(8, Empty).Text = value;
            matchResult.Value[Which, 8] = value;
         }
      }

      public string NinthGroup
      {
         get => Groups.Of(9, Empty).Text;
         set
         {
            Groups.Of(9, Empty).Text = value;
            matchResult.Value[Which, 9] = value;
         }
      }

      public string TenthGroup
      {
         get => Groups.Of(10, Empty).Text;
         set
         {
            Groups.Of(10, Empty).Text = value;
            matchResult.Value[Which, 10] = value;
         }
      }

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