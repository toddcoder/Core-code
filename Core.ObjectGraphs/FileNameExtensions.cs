using System;
using Core.Collections;
using Core.Computers;
using static System.Activator;
using static Core.ObjectGraphs.ObjectGraph;

namespace Core.ObjectGraphs
{
   public static class FileNameExtensions
   {
      public static object GetObject(this FileName fileName, Type objectType, params object[] args)
      {
         var obj = CreateInstance(objectType, args);
         var graph = FromFile(fileName);
         graph.Fill(ref obj);

         return obj;
      }

      public static T GetObject<T>(this FileName fileName, params object[] args)
      {
         var type = typeof(T);
         var instance = (T)CreateInstance(type, args);
         var graph = FromFile(fileName);
         object obj = instance;
         graph.Fill(ref obj);

         return (T)obj;
      }

      public static void SetObject(this FileName fileName, object obj, Predicate<string> exclude, StringHash signatures)
      {
         fileName.Text = Serialize(obj, exclude, signatures).ToString();
      }

      public static void SetObject(this FileName fileName, object obj) => fileName.SetObject(obj, sig => false);

      public static void SetObject(this FileName fileName, object obj, Predicate<string> exclude) => fileName.SetObject(obj, exclude, "");

      public static void SetObject(this FileName fileName, object obj, StringHash signatures)
      {
         fileName.SetObject(obj, sig => false, signatures);
      }
   }
}