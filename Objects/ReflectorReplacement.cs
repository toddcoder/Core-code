using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Objects;

public class ReflectorReplacement
{
   protected int index;
   protected int length;
   protected string memberName;
   protected Maybe<IFormatter> _formatter;

   public ReflectorReplacement(int index, int length, Group group)
   {
      this.index = index;
      this.length = length;

      var _result = group.Text.Matches("^ /(/w+) /s* (/['$,:'] /s* /(.*))? $; f");
      if (_result is (true, var (mn, prefix, format)))
      {
         memberName = mn;
         _formatter = prefix switch
         {
            "," or ":" => some<StandardFormatter, IFormatter>(new StandardFormatter(prefix + format)),
            "$" => some<NewFormatter, IFormatter>(new NewFormatter(format)),
            _ => nil
         };
      }
   }

   public string MemberName => memberName;

   public void Replace(object obj, IGetter getter, Slicer slicer)
   {
      var value = getter.GetValue(obj);
      slicer[index, length] = _formatter.Map(f => f.Format(value)) | (() => value.ToNonNullString());
   }
}