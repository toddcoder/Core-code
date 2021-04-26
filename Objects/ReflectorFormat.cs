﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Assertions;
using Core.Collections;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static System.Reflection.BindingFlags;
using static System.Reflection.MemberTypes;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Objects
{
   public class ReflectorFormat
   {
      protected class Pair
      {
         public Pair(ReflectorReplacement replacement, IGetter getter)
         {
            Replacement = replacement;
            Getter = getter;
         }

         public ReflectorReplacement Replacement { get; }

         public IGetter Getter { get; }

         public void Replace(object obj, Slicer slicer) => Replacement.Replace(obj, Getter, slicer);
      }

      protected class MemberData
      {
         public MemberData(Hash<string, Pair> pairs, string source)
         {
            Pairs = pairs;
            Source = source;
         }

         public Hash<string, Pair> Pairs { get; }

         public string Source { get; }
      }

      protected class Replacements
      {
         public Replacements(IEnumerable<ReflectorReplacement> replacements, string source)
         {
            ReflectorReplacements = replacements.ToArray();
            Source = source;
         }

         public ReflectorReplacement[] ReflectorReplacements { get; }

         public string Source { get; }
      }

      public static IResult<ReflectorFormat> GetReflector(object obj) =>
         from nonNullObject in obj.Must().Not.BeNull().OrFailure()
         from type in tryTo(nonNullObject.GetType)
         select new ReflectorFormat(nonNullObject, type);

      protected static IResult<Replacements> getReplacements(string source)
      {
         return source.MatchAll(@"-(< '\') '{' /(-['}']+) '}'").FlatMap(matches =>
            {
               var replacements = getReplacements(matches);
               return new Replacements(replacements, source).Success();
            },
            () => "Couldn't find any replacements".Failure<Replacements>(),
            failure<Replacements>);
      }

      protected static IEnumerable<ReflectorReplacement> getReplacements(Matcher.Match[] matches)
      {
         return matches.Select(match => new ReflectorReplacement(match.Index, match.Length, match.Groups[1]));
      }

      protected static IResult<MemberData> getMembers(Type type, string template)
      {
         var members = new Hash<string, Pair>();
         const MemberTypes memberTypes = Field | Property;
         const BindingFlags bindingFlags = BindingFlags.Instance | GetField | GetProperty | NonPublic | Public;

         var replacements = getReplacements(template);
         return replacements.Map(r =>
         {
            foreach (var reflectorReplacement in r.ReflectorReplacements)
            {
               var memberInfos = type.GetMember(reflectorReplacement.MemberName, memberTypes, bindingFlags);
               if (memberInfos.Length != 0)
               {
                  var chosen = none<IGetter>();
                  foreach (var info in memberInfos)
                  {
                     if (info is FieldInfo fieldInfo)
                     {
                        chosen = new FieldGetter(fieldInfo).Some<IGetter>();
                        break;
                     }

                     if (info is PropertyInfo propertyInfo)
                     {
                        chosen = new PropertyGetter(propertyInfo).Some<IGetter>();
                        break;
                     }
                  }

                  if (chosen.If(out var ch))
                  {
                     members[reflectorReplacement.MemberName] = new Pair(reflectorReplacement, ch);
                  }
                  else
                  {
                     return failedFind(type, reflectorReplacement.MemberName);
                  }
               }
               else
               {
                  return failedFind(type, reflectorReplacement.MemberName);
               }
            }

            return new MemberData(members, r.Source).Success();
         });
      }

      protected static IResult<MemberData> failedFind(Type type, string memberName)
      {
         return $"Member {memberName} in type {type} couldn't be found".Failure<MemberData>();
      }

      protected object obj;
      protected Type type;

      protected ReflectorFormat(object obj, Type type)
      {
         this.obj = obj;
         this.type = type;
      }

      public IResult<string> Format(string template) =>
         tryTo(() => from memberData in getMembers(type, template)
            from formatted in getText(memberData)
            select formatted.Substitute(@"'\{'", "{"));

      protected IResult<string> getText(MemberData memberData) => tryTo(() =>
      {
         var slicer = new Slicer(memberData.Source);

         foreach (var item in memberData.Pairs)
         {
            item.Value.Replace(obj, slicer);
         }

         return slicer.ToString();
      });

      protected IResult<object> getValue(MemberInfo info) => info switch
      {
         FieldInfo fieldInfo => fieldInfo.GetValue(obj).Success(),
         PropertyInfo propertyInfo => propertyInfo.GetValue(obj).Success(),
         _ => $"Couldn't invoke member {info.Name}".Failure<object>()
      };
   }
}