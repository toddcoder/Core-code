using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Computers;
using Core.Dates;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using static Core.ObjectGraphs.Configurations.Json.JSONDeserializerFunctions;

namespace Core.ObjectGraphs.Configurations.Json
{
   public class JSONBuilder
   {
      protected JSONObject root;
      protected Stack<IContainer> containers;
      protected IContainer currentContainer;

      public JSONBuilder()
      {
         root = new JSONObject();
         containers = new Stack<IContainer>();
         currentContainer = root;
      }

      public void Add(JSONItem item) => currentContainer.Add(item);

      public void Add(string name, string @string) => Add(new JSONString(name, @string));

      public void Add(string name, int integer) => Add(new JSONInteger(name, integer));

      public void Add(string name, double @double) => Add(new JSONDouble(name, @double));

      public void Add(string name, bool boolean) => Add(new JSONBoolean(name, boolean));

      public void Add(string name, byte[] byteArray) => Add(new JSONByteArray(name, byteArray));

      public void Add(string name, FileName fileName) => Add(new JSONFileName(name, fileName));

      public void Add(string name, FolderName folderName) => Add(new JSONFolderName(name, folderName));

      public void Add(string name, Enum enumeration) => Add(new JSONEnum(name, enumeration));

      public void Add(string name, Guid guid) => Add(new JSONGuid(name, guid));

      public void Add(string name, TimeSpan timeSpan) => Add(new JSONTimeSpan(name, timeSpan));

      public void Add(string name) => Add(new JSONNull(name));

      public void Add(string name, object @object)
      {
         if (@object.IsNull())
         {
            Add(name);
         }
         else
         {
            Add(new JSONAny(name, @object));
         }
      }

      public void BeginObject(string name = "") => beginContainer(new JSONObject(name));

      public void BeginArray(string name = "") => beginContainer(new JSONArray(name));

      protected void beginContainer(IContainer container)
      {
         currentContainer.Add((JSONBase)container);
         containers.Push(currentContainer);
         currentContainer = container;
      }

      public void End()
      {
         if (containers.Count > 0)
         {
            currentContainer = containers.Pop();
         }
      }

      public string Generate(string indentString = "   ")
      {
         while (containers.Count > 0)
         {
            End();
         }

         var writer = new JSONWriter(indentString);
         root.Generate(writer);
         return writer.ToString();
      }

      public override string ToString() => Generate();

      public JSONObject Root => root;
   }

   public interface IContainer
   {
      void Add(JSONBase item);
   }

   public abstract class JSONBase
   {
      protected string name;

      public JSONBase(string name) => this.name = name;

      public JSONBase() : this("") { }

      public string Name => name;

      public abstract void Generate(JSONWriter writer);

      protected void writeName(JSONWriter writer)
      {
         if (name.IsNotEmpty())
         {
            writer.WriteName(name);
         }
      }

      protected IResult<T> getFailure<T>() => $"Couldn't cast to type {nameof(T)}".Failure<T>();

      public virtual IResult<JSONObject> Object() => getFailure<JSONObject>();

      public virtual IResult<T> Object<T>(Func<JSONObject, T> map) => getFailure<T>();

      public virtual T Object<T>(Func<JSONObject, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IMaybe<JSONObject> IfObject() => none<JSONObject>();

      public virtual IResult<JSONArray> Array() => getFailure<JSONArray>();

      public virtual IResult<T> Array<T>(Func<JSONArray, T> map) => getFailure<T>();

      public virtual T Array<T>(Func<JSONArray, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IMaybe<JSONArray> IfArray() => none<JSONArray>();

      public virtual IResult<JSONString> String() => getFailure<JSONString>();

      public virtual IResult<T> String<T>(Func<JSONString, T> map) => getFailure<T>();

      public virtual T String<T>(Func<JSONString, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<string> AsString() => getFailure<string>();

      public virtual IResult<string> AsString(Func<string> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JSONString> IfString() => none<JSONString>();

      public virtual IResult<JSONInteger> Integer() => getFailure<JSONInteger>();

      public virtual IResult<T> Integer<T>(Func<JSONInteger, T> map) => getFailure<T>();

      public virtual T Integer<T>(Func<JSONInteger, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<int> AsInteger() => getFailure<int>();

      public virtual IResult<int> AsInteger(Func<int> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JSONInteger> IfInteger() => none<JSONInteger>();

      public virtual IResult<JSONDouble> Double() => getFailure<JSONDouble>();

      public virtual IResult<T> Double<T>(Func<JSONDouble, T> map) => getFailure<T>();

      public virtual T Double<T>(Func<JSONDouble, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<double> AsDouble() => getFailure<double>();

      public virtual IResult<double> AsDouble(Func<double> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JSONDouble> IfDouble() => none<JSONDouble>();

      public virtual IResult<JSONBoolean> Boolean() => getFailure<JSONBoolean>();

      public virtual IResult<T> Boolean<T>(Func<JSONBoolean, T> map) => getFailure<T>();

      public virtual T Boolean<T>(Func<JSONBoolean, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<bool> AsBoolean() => getFailure<bool>();

      public virtual IResult<bool> AsBoolean(Func<bool> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JSONBoolean> IfBoolean() => none<JSONBoolean>();

      public virtual IResult<JSONByteArray> ByteArray() => getFailure<JSONByteArray>();

      public virtual IResult<T> ByteArray<T>(Func<JSONByteArray, T> map) => getFailure<T>();

      public virtual T ByteArray<T>(Func<JSONByteArray, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<byte[]> AsByteArray() => getFailure<byte[]>();

      public virtual IResult<byte[]> AsByteArray(Func<byte[]> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JSONByteArray> IfByteArray() => none<JSONByteArray>();

      public virtual IResult<JSONFileName> FileName() => getFailure<JSONFileName>();

      public virtual IResult<T> FileName<T>(Func<JSONFileName, T> map) => getFailure<T>();

      public virtual T FileName<T>(Func<JSONFileName, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<FileName> AsFileName() => getFailure<FileName>();

      public virtual IResult<FileName> AsFileName(Func<FileName> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JSONFileName> IfFileName() => none<JSONFileName>();

      public virtual IResult<JSONFolderName> FolderName() => getFailure<JSONFolderName>();

      public virtual IResult<T> FolderName<T>(Func<JSONFolderName, T> map) => getFailure<T>();

      public virtual T FolderName<T>(Func<JSONFolderName, T> ifSuccess, Func<Exception, T> ifFailure)
      {
         return getFailure<T>().Recover(ifFailure);
      }

      public virtual IResult<FolderName> AsFolderName() => getFailure<FolderName>();

      public virtual IResult<FolderName> AsFolderName(Func<FolderName> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JSONFolderName> IfFolderName() => none<JSONFolderName>();

      public virtual IResult<JSONEnum> Enum() => getFailure<JSONEnum>();

      public virtual IResult<T> Enum<T>(Func<JSONEnum, T> map) => getFailure<T>();

      public virtual T Enum<T>(Func<JSONEnum, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<Enum> AsEnum() => getFailure<Enum>();

      public virtual IResult<Enum> AsEnum(Func<Enum> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JSONEnum> IfEnum() => none<JSONEnum>();

      public virtual IResult<JSONGuid> Guid() => getFailure<JSONGuid>();

      public virtual IResult<T> Guid<T>(Func<JSONGuid, T> map) => getFailure<T>();

      public virtual T Guid<T>(Func<JSONGuid, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<Guid> AsGuid() => getFailure<Guid>();

      public virtual IResult<Guid> AsGuid(Func<Guid> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JSONGuid> IfGuid() => none<JSONGuid>();

      public virtual IResult<JSONTimeSpan> TimeSpan() => getFailure<JSONTimeSpan>();

      public virtual IResult<T> TimeSpan<T>(Func<JSONTimeSpan, T> map) => getFailure<T>();

      public virtual T TimeSpan<T>(Func<JSONTimeSpan, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<TimeSpan> AsTimeSpan() => getFailure<TimeSpan>();

      public virtual IResult<TimeSpan> AsTimeSpan(Func<TimeSpan> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JSONTimeSpan> IfTimeSpan() => none<JSONTimeSpan>();

      public virtual IResult<JSONNull> Null() => getFailure<JSONNull>();

      public virtual IMaybe<JSONNull> IfNull() => none<JSONNull>();

      public virtual IResult<JSONAny> Any() => getFailure<JSONAny>();

      public virtual IResult<T> Any<T>(Func<JSONAny, T> map) => getFailure<T>();

      public virtual T Any<T>(Func<JSONAny, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<object> AsAny() => getFailure<object>();

      public virtual IMaybe<JSONAny> IfAny() => none<JSONAny>();

      public virtual IResult<object> AsAny(Func<object> defaultValue) => tryTo(defaultValue);

      public abstract bool IsContainer { get; }
   }

   public class JSONObject : JSONBase, IContainer, IEnumerable<JSONBase>, IHash<string, JSONBase>
   {
      protected Hash<string, JSONBase> content;

      public JSONObject(string name) : base(name) => content = new AutoHash<string, JSONBase>(new JSONNothing());

      public JSONObject() : this("") { }

      public JSONBase this[string name]
      {
         get => content[name];
         set => content[name] = value;
      }

      public bool ContainsKey(string key) => content.ContainsKey(key);

      public IResult<Hash<string, JSONBase>> AnyHash() => content.Success();

      public override void Generate(JSONWriter writer)
      {
         writeName(writer);
         writer.BeginObject();
         foreach (var item in content)
         {
            item.Value.Generate(writer);
         }

         writer.EndObject();
      }

      public void Add(JSONBase item) => content[item.Name] = item;

      public IEnumerator<JSONBase> GetEnumerator() => content.ValueArray().AsEnumerator<JSONBase>();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public override IResult<JSONObject> Object() => this.Success();

      public override IResult<T> Object<T>(Func<JSONObject, T> map) => tryTo(() => map(this));

      public override T Object<T>(Func<JSONObject, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IMaybe<JSONObject> IfObject() => this.Some();

      public override bool IsContainer => true;

      public Hash<string, JSONBase> ToHash() => content;

      public IResult<T> Deserialize<T>() => JSONDeserializer<T>.Deserialize(this);

      public int Count => content.Count;

      public IResult<Hash<string, TValue>> ToHash<TValue>(Func<JSONBase, IResult<TValue>> valueFunc)
      {
         var hash = new Hash<string, TValue>();

         foreach (var (key, jsonBase) in content)
         {
            var value = valueFunc(jsonBase);
            if (value.If(out var v, out var exception))
            {
               hash[key] = v;
            }
            else
            {
               return failure<Hash<string, TValue>>(exception);
            }
         }

         return hash.Success();
      }

      public IResult<Unit> Fill(object obj) => FillObject(obj, this);
   }

   public class JSONArray : JSONBase, IContainer, IEnumerable<JSONBase>, IHash<int, JSONBase>
   {
      protected List<JSONBase> content;

      public JSONArray(string name) : base(name) => content = new List<JSONBase>();

      public JSONArray() : this("") { }

      public void Add(JSONBase item) => content.Add(item);

      public JSONBase this[int index]
      {
         get => content[index];
         set => content[index] = value;
      }

      public bool ContainsKey(int key) => key.Between(0).Until(content.Count);

      public IResult<Hash<int, JSONBase>> AnyHash()
      {
         return content.Select((j, i) => (key: i, value: j)).ToHash(i => i.key, i => i.value).Success();
      }

      public override void Generate(JSONWriter writer)
      {
         writeName(writer);
         if (content.Count == 0)
         {
            writer.Write("[]");
            return;
         }

         writer.BeginArray();
         foreach (var item in content)
         {
            item.Generate(writer);
         }

         writer.EndArray();
      }

      public IEnumerator<JSONBase> GetEnumerator() => content.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public JSONBase[] ToArray() => content.ToArray();

      public override IResult<JSONArray> Array() => this.Success();

      public override IResult<T> Array<T>(Func<JSONArray, T> map) => tryTo(() => map(this));

      public override T Array<T>(Func<JSONArray, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IMaybe<JSONArray> IfArray() => this.Some();

      public override bool IsContainer => true;

      public int Count => content.Count;
   }

   public abstract class JSONItem : JSONBase
   {
      public override void Generate(JSONWriter writer)
      {
         writer.WriteName(name);
         GenerateValue(writer);
      }

      public abstract void GenerateValue(JSONWriter writer);

      public override bool IsContainer => false;
   }

   public class JSONString : JSONItem
   {
      protected string @string;

      public JSONString(string name, string @string)
      {
         this.name = name;
         this.@string = @string;
      }

      public override void GenerateValue(JSONWriter writer) => writer.WriteQuoted(@string);

      public override string ToString() => @string;

      public override IResult<JSONString> String() => this.Success();

      public override IResult<T> String<T>(Func<JSONString, T> map) => tryTo(() => map(this));

      public override T String<T>(Func<JSONString, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<string> AsString() => @string.Success();

      public override IResult<string> AsString(Func<string> defaultValue) => @string.Success();

      public override IMaybe<JSONString> IfString() => this.Some();

      public override IResult<JSONFileName> FileName()
      {
         return
            from fileName in Computers.FileName.Try.FromString(@string)
            from jFileName in tryTo(() => new JSONFileName(name, fileName))
            select jFileName;
      }

      public override IResult<T> FileName<T>(Func<JSONFileName, T> map) => FileName().Map(map);

      public override T FileName<T>(Func<JSONFileName, T> ifSuccess, Func<Exception, T> ifFailure)
      {
         return FileName().FlatMap(ifSuccess, ifFailure);
      }

      public override IResult<FileName> AsFileName() => FileName().Map(f => f.AsFileName());

      public override IResult<FileName> AsFileName(Func<FileName> defaultValue) => AsFileName().Or(defaultValue());

      public override IResult<JSONFolderName> FolderName()
      {
         return
            from folderName in Computers.FolderName.Try.FromString(@string)
            from jFolderName in tryTo(() => new JSONFolderName(name, folderName))
            select jFolderName;
      }

      public override IResult<T> FolderName<T>(Func<JSONFolderName, T> map) => FolderName().Map(map);

      public override T FolderName<T>(Func<JSONFolderName, T> ifSuccess, Func<Exception, T> ifFailure)
      {
         return FolderName().FlatMap(ifSuccess, ifFailure);
      }

      public override IResult<FolderName> AsFolderName() => FolderName().Map(f => f.AsFolderName());

      public override IResult<FolderName> AsFolderName(Func<FolderName> defaultValue) => AsFolderName().Or(defaultValue());

      public override IResult<JSONGuid> Guid() =>
         from guid in @string.Guid()
         from jGuid in tryTo(() => new JSONGuid(name, guid))
         select jGuid;

      public override IResult<T> Guid<T>(Func<JSONGuid, T> map) => Guid().Map(map);

      public override T Guid<T>(Func<JSONGuid, T> ifSuccess, Func<Exception, T> ifFailure) => Guid().FlatMap(ifSuccess, ifFailure);

      public override IResult<Guid> AsGuid() => Guid().Map(g => g.AsGuid());

      public override IResult<Guid> AsGuid(Func<Guid> defaultValue) => AsGuid().Or(defaultValue());

      public override IResult<JSONTimeSpan> TimeSpan() =>
         from timeSpan in @string.TimeSpan()
         from jTimeSpan in tryTo(() => new JSONTimeSpan(name, timeSpan))
         select jTimeSpan;

      public override IResult<T> TimeSpan<T>(Func<JSONTimeSpan, T> map) => TimeSpan().Map(map);

      public override T TimeSpan<T>(Func<JSONTimeSpan, T> ifSuccess, Func<Exception, T> ifFailure) => TimeSpan().FlatMap(ifSuccess, ifFailure);

      public override IResult<TimeSpan> AsTimeSpan() => TimeSpan().Map(ts => ts.AsTimeSpan());

      public override IResult<TimeSpan> AsTimeSpan(Func<TimeSpan> defaultValue) => AsTimeSpan().Or(defaultValue);
   }

   public class JSONInteger : JSONItem
   {
      protected int integer;

      public JSONInteger(string name, int integer)
      {
         this.name = name;
         this.integer = integer;
      }

      public override void GenerateValue(JSONWriter writer) => writer.Write(integer);

      public int ToInteger() => integer;

      public override IResult<JSONInteger> Integer() => this.Success();

      public override IResult<T> Integer<T>(Func<JSONInteger, T> map) => tryTo(() => map(this));

      public override T Integer<T>(Func<JSONInteger, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<int> AsInteger() => integer.Success();

      public override IResult<int> AsInteger(Func<int> defaultValue) => integer.Success();

      public override IMaybe<JSONInteger> IfInteger() => this.Some();
   }

   public class JSONDouble : JSONItem
   {
      protected double @double;

      public JSONDouble(string name, double @double)
      {
         this.name = name;
         this.@double = @double;
      }

      public override void GenerateValue(JSONWriter writer) => writer.Write(@double);

      public double ToDouble() => @double;

      public override IResult<JSONDouble> Double() => this.Success();

      public override IResult<T> Double<T>(Func<JSONDouble, T> map) => tryTo(() => map(this));

      public override T Double<T>(Func<JSONDouble, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<double> AsDouble() => @double.Success();

      public override IResult<double> AsDouble(Func<double> defaultValue) => @double.Success();

      public override IMaybe<JSONDouble> IfDouble() => this.Some();
   }

   public class JSONBoolean : JSONItem
   {
      bool boolean;

      public JSONBoolean(string name, bool boolean)
      {
         this.name = name;
         this.boolean = boolean;
      }

      public override void GenerateValue(JSONWriter writer) => writer.Write(boolean);

      public bool ToBoolean() => boolean;

      public override IResult<JSONBoolean> Boolean() => this.Success();

      public override IResult<T> Boolean<T>(Func<JSONBoolean, T> map) => tryTo(() => map(this));

      public override T Boolean<T>(Func<JSONBoolean, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<bool> AsBoolean() => boolean.Success();

      public override IResult<bool> AsBoolean(Func<bool> defaultValue) => boolean.Success();

      public override IMaybe<JSONBoolean> IfBoolean() => this.Some();
   }

   public class JSONByteArray : JSONItem
   {
      byte[] byteArray;

      public JSONByteArray(string name, byte[] byteArray)
      {
         this.name = name;
         this.byteArray = byteArray;
      }

      public override void GenerateValue(JSONWriter writer) => writer.Write(byteArray);

      public byte[] ToByteArray() => byteArray;

      public override IResult<JSONByteArray> ByteArray() => this.Success();

      public override IResult<T> ByteArray<T>(Func<JSONByteArray, T> map) => tryTo(() => map(this));

      public override T ByteArray<T>(Func<JSONByteArray, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<byte[]> AsByteArray() => byteArray.Success();

      public override IResult<byte[]> AsByteArray(Func<byte[]> defaultValue) => byteArray.Success();

      public override IMaybe<JSONByteArray> IfByteArray() => this.Some();
   }

   public class JSONFileName : JSONItem
   {
      FileName fileName;

      public JSONFileName(string name, FileName fileName)
      {
         this.name = name;
         this.fileName = fileName;
      }

      public override void GenerateValue(JSONWriter writer) => writer.Write(fileName);

      public FileName ToFileName() => fileName;

      public override IResult<JSONFileName> FileName() => this.Success();

      public override IResult<T> FileName<T>(Func<JSONFileName, T> map) => tryTo(() => map(this));

      public override T FileName<T>(Func<JSONFileName, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<FileName> AsFileName() => fileName.Success();

      public override IResult<FileName> AsFileName(Func<FileName> defaultValue) => fileName.Success();

      public override IMaybe<JSONFileName> IfFileName() => this.Some();
   }

   public class JSONFolderName : JSONItem
   {
      FolderName folderName;

      public JSONFolderName(string name, FolderName folderName)
      {
         this.name = name;
         this.folderName = folderName;
      }

      public override void GenerateValue(JSONWriter writer) => writer.Write(folderName);

      public FolderName ToFolderName() => folderName;

      public override IResult<JSONFolderName> FolderName() => this.Success();

      public override IResult<T> FolderName<T>(Func<JSONFolderName, T> map) => tryTo(() => map(this));

      public override T FolderName<T>(Func<JSONFolderName, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<FolderName> AsFolderName() => folderName.Success();

      public override IResult<FolderName> AsFolderName(Func<FolderName> defaultValue) => folderName.Success();

      public override IMaybe<JSONFolderName> IfFolderName() => this.Some();
   }

   public class JSONEnum : JSONItem
   {
      Enum enumeration;

      public JSONEnum(string name, Enum enumeration)
      {
         this.name = name;
         this.enumeration = enumeration;
      }

      public override void GenerateValue(JSONWriter writer) => writer.Write(enumeration);

      public Enum ToEnum() => enumeration;

      public override IResult<JSONEnum> Enum() => this.Success();

      public override IResult<T> Enum<T>(Func<JSONEnum, T> map) => tryTo(() => map(this));

      public override T Enum<T>(Func<JSONEnum, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<Enum> AsEnum() => enumeration.Success();

      public override IResult<Enum> AsEnum(Func<Enum> defaultValue) => enumeration.Success();

      public override IMaybe<JSONEnum> IfEnum() => this.Some();
   }

   public class JSONGuid : JSONItem
   {
      protected Guid guid;

      public JSONGuid(string name, Guid guid)
      {
         this.name = name;
         this.guid = guid;
      }

      public override void GenerateValue(JSONWriter writer) => writer.Write(guid.ToString().ToUpper());

      public Guid ToGuid() => guid;

      public override IResult<JSONGuid> Guid() => this.Success();

      public override IResult<T> Guid<T>(Func<JSONGuid, T> map) => tryTo(() => map(this));

      public override T Guid<T>(Func<JSONGuid, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<Guid> AsGuid() => guid.Success();

      public override IResult<Guid> AsGuid(Func<Guid> defaultValue) => guid.Success();

      public override IMaybe<JSONGuid> IfGuid() => this.Some();
   }

   public class JSONTimeSpan : JSONItem
   {
      protected TimeSpan timeSpan;

      public JSONTimeSpan(string name, TimeSpan timeSpan)
      {
         this.name = name;
         this.timeSpan = timeSpan;
      }

      public override void GenerateValue(JSONWriter writer) => writer.Write(timeSpan.ToLongString(true));

      public override IResult<JSONTimeSpan> TimeSpan() => this.Success();

      public override IResult<T> TimeSpan<T>(Func<JSONTimeSpan, T> map) => tryTo(() => map(this));

      public override T TimeSpan<T>(Func<JSONTimeSpan, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<TimeSpan> AsTimeSpan() => timeSpan.Success();

      public override IResult<TimeSpan> AsTimeSpan(Func<TimeSpan> defaultValue) => timeSpan.Success();

      public override IMaybe<JSONTimeSpan> IfTimeSpan() => this.Some();

      public TimeSpan ToTimeSpan() => timeSpan;
   }

   public class JSONNull : JSONItem
   {
      public JSONNull(string name) => this.name = name;

      public override void GenerateValue(JSONWriter writer) => writer.Write("null");

      public override IResult<JSONNull> Null() => this.Success();

      public override IMaybe<JSONNull> IfNull() => this.Some();
   }

   public class JSONAny : JSONItem
   {
      protected object @object;

      public JSONAny(string name, object @object)
      {
         this.name = name;
         this.@object = @object;
      }

      public override void GenerateValue(JSONWriter writer) => writer.Write(@object);

      public object ToObject() => @object;

      public override IResult<JSONAny> Any() => this.Success();

      public override IResult<T> Any<T>(Func<JSONAny, T> map) => tryTo(() => map(this));

      public override T Any<T>(Func<JSONAny, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<object> AsAny() => @object.Success();

      public override IResult<object> AsAny(Func<object> defaultValue) => @object.Success();

      public override IMaybe<JSONAny> IfAny() => this.Some();
   }

   public class JSONNothing : JSONBase
   {
      public override void Generate(JSONWriter writer) { }

      public override bool IsContainer => false;
   }
}