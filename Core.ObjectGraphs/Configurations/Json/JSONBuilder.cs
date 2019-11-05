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
using static Core.ObjectGraphs.Configurations.Json.JsonDeserializerFunctions;

namespace Core.ObjectGraphs.Configurations.Json
{
   public class JsonBuilder
   {
      protected JsonObject root;
      protected Stack<IContainer> containers;
      protected IContainer currentContainer;

      public JsonBuilder()
      {
         root = new JsonObject();
         containers = new Stack<IContainer>();
         currentContainer = root;
      }

      public void Add(JsonItem item) => currentContainer.Add(item);

      public void Add(string name, string @string) => Add(new JsonString(name, @string));

      public void Add(string name, int integer) => Add(new JsonInteger(name, integer));

      public void Add(string name, double @double) => Add(new JsonDouble(name, @double));

      public void Add(string name, bool boolean) => Add(new JsonBoolean(name, boolean));

      public void Add(string name, byte[] byteArray) => Add(new JsonByteArray(name, byteArray));

      public void Add(string name, FileName fileName) => Add(new JsonFileName(name, fileName));

      public void Add(string name, FolderName folderName) => Add(new JsonFolderName(name, folderName));

      public void Add(string name, Enum enumeration) => Add(new JsonEnum(name, enumeration));

      public void Add(string name, Guid guid) => Add(new JsonGuid(name, guid));

      public void Add(string name, TimeSpan timeSpan) => Add(new JsonTimeSpan(name, timeSpan));

      public void Add(string name) => Add(new JsonNull(name));

      public void Add(string name, object @object)
      {
         if (@object.IsNull())
         {
            Add(name);
         }
         else
         {
            Add(new JsonAny(name, @object));
         }
      }

      public void BeginObject(string name = "") => beginContainer(new JsonObject(name));

      public void BeginArray(string name = "") => beginContainer(new JsonArray(name));

      protected void beginContainer(IContainer container)
      {
         containers.Push(currentContainer);
         currentContainer = container;
      }

      public void End()
      {
         if (containers.Count > 0)
         {
            var previousContainer = containers.Pop();
            previousContainer.Add((JsonBase)currentContainer);
            currentContainer = previousContainer;
         }
      }

      public string Generate(string indentString = "   ")
      {
         while (containers.Count > 0)
         {
            End();
         }

         var writer = new JsonWriter(indentString);
         root.Generate(writer);

         return writer.ToString();
      }

      public override string ToString() => Generate();

      public JsonObject Root => root;
   }

   public interface IContainer
   {
      void Add(JsonBase item);
   }

   public abstract class JsonBase
   {
      protected string name;

      public JsonBase(string name) => this.name = name;

      public JsonBase() : this("") { }

      public string Name => name;

      public abstract void Generate(JsonWriter writer);

      protected void writeName(JsonWriter writer)
      {
         if (name.IsNotEmpty())
         {
            writer.WriteName(name);
         }
      }

      protected IResult<T> getFailure<T>() => $"Couldn't cast to type {nameof(T)}".Failure<T>();

      public virtual IResult<JsonObject> Object() => getFailure<JsonObject>();

      public virtual IResult<T> Object<T>(Func<JsonObject, T> map) => getFailure<T>();

      public virtual T Object<T>(Func<JsonObject, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IMaybe<JsonObject> IfObject() => none<JsonObject>();

      public virtual IResult<JsonArray> Array() => getFailure<JsonArray>();

      public virtual IResult<T> Array<T>(Func<JsonArray, T> map) => getFailure<T>();

      public virtual T Array<T>(Func<JsonArray, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IMaybe<JsonArray> IfArray() => none<JsonArray>();

      public virtual IResult<JsonString> String() => getFailure<JsonString>();

      public virtual IResult<T> String<T>(Func<JsonString, T> map) => getFailure<T>();

      public virtual T String<T>(Func<JsonString, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<string> AsString() => getFailure<string>();

      public virtual IResult<string> AsString(Func<string> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JsonString> IfString() => none<JsonString>();

      public virtual IResult<JsonInteger> Integer() => getFailure<JsonInteger>();

      public virtual IResult<T> Integer<T>(Func<JsonInteger, T> map) => getFailure<T>();

      public virtual T Integer<T>(Func<JsonInteger, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<int> AsInteger() => getFailure<int>();

      public virtual IResult<int> AsInteger(Func<int> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JsonInteger> IfInteger() => none<JsonInteger>();

      public virtual IResult<JsonDouble> Double() => getFailure<JsonDouble>();

      public virtual IResult<T> Double<T>(Func<JsonDouble, T> map) => getFailure<T>();

      public virtual T Double<T>(Func<JsonDouble, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<double> AsDouble() => getFailure<double>();

      public virtual IResult<double> AsDouble(Func<double> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JsonDouble> IfDouble() => none<JsonDouble>();

      public virtual IResult<JsonBoolean> Boolean() => getFailure<JsonBoolean>();

      public virtual IResult<T> Boolean<T>(Func<JsonBoolean, T> map) => getFailure<T>();

      public virtual T Boolean<T>(Func<JsonBoolean, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<bool> AsBoolean() => getFailure<bool>();

      public virtual IResult<bool> AsBoolean(Func<bool> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JsonBoolean> IfBoolean() => none<JsonBoolean>();

      public virtual IResult<JsonByteArray> ByteArray() => getFailure<JsonByteArray>();

      public virtual IResult<T> ByteArray<T>(Func<JsonByteArray, T> map) => getFailure<T>();

      public virtual T ByteArray<T>(Func<JsonByteArray, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<byte[]> AsByteArray() => getFailure<byte[]>();

      public virtual IResult<byte[]> AsByteArray(Func<byte[]> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JsonByteArray> IfByteArray() => none<JsonByteArray>();

      public virtual IResult<JsonFileName> FileName() => getFailure<JsonFileName>();

      public virtual IResult<T> FileName<T>(Func<JsonFileName, T> map) => getFailure<T>();

      public virtual T FileName<T>(Func<JsonFileName, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<FileName> AsFileName() => getFailure<FileName>();

      public virtual IResult<FileName> AsFileName(Func<FileName> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JsonFileName> IfFileName() => none<JsonFileName>();

      public virtual IResult<JsonFolderName> FolderName() => getFailure<JsonFolderName>();

      public virtual IResult<T> FolderName<T>(Func<JsonFolderName, T> map) => getFailure<T>();

      public virtual T FolderName<T>(Func<JsonFolderName, T> ifSuccess, Func<Exception, T> ifFailure)
      {
         return getFailure<T>().Recover(ifFailure);
      }

      public virtual IResult<FolderName> AsFolderName() => getFailure<FolderName>();

      public virtual IResult<FolderName> AsFolderName(Func<FolderName> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JsonFolderName> IfFolderName() => none<JsonFolderName>();

      public virtual IResult<JsonEnum> Enum() => getFailure<JsonEnum>();

      public virtual IResult<T> Enum<T>(Func<JsonEnum, T> map) => getFailure<T>();

      public virtual T Enum<T>(Func<JsonEnum, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<Enum> AsEnum() => getFailure<Enum>();

      public virtual IResult<Enum> AsEnum(Func<Enum> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JsonEnum> IfEnum() => none<JsonEnum>();

      public virtual IResult<JsonGuid> Guid() => getFailure<JsonGuid>();

      public virtual IResult<T> Guid<T>(Func<JsonGuid, T> map) => getFailure<T>();

      public virtual T Guid<T>(Func<JsonGuid, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<Guid> AsGuid() => getFailure<Guid>();

      public virtual IResult<Guid> AsGuid(Func<Guid> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JsonGuid> IfGuid() => none<JsonGuid>();

      public virtual IResult<JsonTimeSpan> TimeSpan() => getFailure<JsonTimeSpan>();

      public virtual IResult<T> TimeSpan<T>(Func<JsonTimeSpan, T> map) => getFailure<T>();

      public virtual T TimeSpan<T>(Func<JsonTimeSpan, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<TimeSpan> AsTimeSpan() => getFailure<TimeSpan>();

      public virtual IResult<TimeSpan> AsTimeSpan(Func<TimeSpan> defaultValue) => tryTo(defaultValue);

      public virtual IMaybe<JsonTimeSpan> IfTimeSpan() => none<JsonTimeSpan>();

      public virtual IResult<JsonNull> Null() => getFailure<JsonNull>();

      public virtual IMaybe<JsonNull> IfNull() => none<JsonNull>();

      public virtual IResult<JsonAny> Any() => getFailure<JsonAny>();

      public virtual IResult<T> Any<T>(Func<JsonAny, T> map) => getFailure<T>();

      public virtual T Any<T>(Func<JsonAny, T> ifSuccess, Func<Exception, T> ifFailure) => getFailure<T>().Recover(ifFailure);

      public virtual IResult<object> AsAny() => getFailure<object>();

      public virtual IMaybe<JsonAny> IfAny() => none<JsonAny>();

      public virtual IResult<object> AsAny(Func<object> defaultValue) => tryTo(defaultValue);

      public abstract bool IsContainer { get; }
   }

   public class JsonObject : JsonBase, IContainer, IEnumerable<JsonBase>, IHash<string, JsonBase>
   {
      protected Hash<string, JsonBase> content;

      public JsonObject(string name) : base(name) => content = new AutoHash<string, JsonBase>(new JsonNothing());

      public JsonObject() : this("") { }

      public JsonBase this[string name]
      {
         get => content[name];
         set => content[name] = value;
      }

      public bool ContainsKey(string key) => content.ContainsKey(key);

      public IResult<Hash<string, JsonBase>> AnyHash() => content.Success();

      public override void Generate(JsonWriter writer)
      {
         writeName(writer);
         writer.BeginObject();
         foreach (var item in content)
         {
            item.Value.Generate(writer);
         }

         writer.EndObject();
      }

      public void Add(JsonBase item) => content[item.Name] = item;

      public IEnumerator<JsonBase> GetEnumerator() => content.ValueArray().AsEnumerator<JsonBase>();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public override IResult<JsonObject> Object() => this.Success();

      public override IResult<T> Object<T>(Func<JsonObject, T> map) => tryTo(() => map(this));

      public override T Object<T>(Func<JsonObject, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IMaybe<JsonObject> IfObject() => this.Some();

      public override bool IsContainer => true;

      public Hash<string, JsonBase> ToHash() => content;

      public IResult<T> Deserialize<T>() => JsonDeserializer<T>.Deserialize(this);

      public int Count => content.Count;

      public IResult<Hash<string, TValue>> ToHash<TValue>(Func<JsonBase, IResult<TValue>> valueFunc)
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

   public class JsonArray : JsonBase, IContainer, IEnumerable<JsonBase>, IHash<int, JsonBase>
   {
      protected List<JsonBase> content;

      public JsonArray(string name) : base(name) => content = new List<JsonBase>();

      public JsonArray() : this("") { }

      public void Add(JsonBase item) => content.Add(item);

      public JsonBase this[int index]
      {
         get => content[index];
         set => content[index] = value;
      }

      public bool ContainsKey(int key) => key.Between(0).Until(content.Count);

      public IResult<Hash<int, JsonBase>> AnyHash()
      {
         return content.Select((j, i) => (key: i, value: j)).ToHash(i => i.key, i => i.value).Success();
      }

      public override void Generate(JsonWriter writer)
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

      public IEnumerator<JsonBase> GetEnumerator() => content.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public JsonBase[] ToArray() => content.ToArray();

      public override IResult<JsonArray> Array() => this.Success();

      public override IResult<T> Array<T>(Func<JsonArray, T> map) => tryTo(() => map(this));

      public override T Array<T>(Func<JsonArray, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IMaybe<JsonArray> IfArray() => this.Some();

      public override bool IsContainer => true;

      public int Count => content.Count;
   }

   public abstract class JsonItem : JsonBase
   {
      public override void Generate(JsonWriter writer)
      {
         writer.WriteName(name);
         GenerateValue(writer);
      }

      public abstract void GenerateValue(JsonWriter writer);

      public override bool IsContainer => false;
   }

   public class JsonString : JsonItem
   {
      protected string @string;

      public JsonString(string name, string @string)
      {
         this.name = name;
         this.@string = @string;
      }

      public override void GenerateValue(JsonWriter writer) => writer.WriteQuoted(@string);

      public override string ToString() => @string;

      public override IResult<JsonString> String() => this.Success();

      public override IResult<T> String<T>(Func<JsonString, T> map) => tryTo(() => map(this));

      public override T String<T>(Func<JsonString, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<string> AsString() => @string.Success();

      public override IResult<string> AsString(Func<string> defaultValue) => @string.Success();

      public override IMaybe<JsonString> IfString() => this.Some();

      public override IResult<JsonFileName> FileName()
      {
         return
            from fileName in Computers.FileName.Try.FromString(@string)
            from jFileName in tryTo(() => new JsonFileName(name, fileName))
            select jFileName;
      }

      public override IResult<T> FileName<T>(Func<JsonFileName, T> map) => FileName().Map(map);

      public override T FileName<T>(Func<JsonFileName, T> ifSuccess, Func<Exception, T> ifFailure)
      {
         return FileName().FlatMap(ifSuccess, ifFailure);
      }

      public override IResult<FileName> AsFileName() => FileName().Map(f => f.AsFileName());

      public override IResult<FileName> AsFileName(Func<FileName> defaultValue) => AsFileName().Or(defaultValue());

      public override IResult<JsonFolderName> FolderName()
      {
         return
            from folderName in Computers.FolderName.Try.FromString(@string)
            from jFolderName in tryTo(() => new JsonFolderName(name, folderName))
            select jFolderName;
      }

      public override IResult<T> FolderName<T>(Func<JsonFolderName, T> map) => FolderName().Map(map);

      public override T FolderName<T>(Func<JsonFolderName, T> ifSuccess, Func<Exception, T> ifFailure)
      {
         return FolderName().FlatMap(ifSuccess, ifFailure);
      }

      public override IResult<FolderName> AsFolderName() => FolderName().Map(f => f.AsFolderName());

      public override IResult<FolderName> AsFolderName(Func<FolderName> defaultValue) => AsFolderName().Or(defaultValue());

      public override IResult<JsonGuid> Guid() =>
         from guid in @string.Guid()
         from jGuid in tryTo(() => new JsonGuid(name, guid))
         select jGuid;

      public override IResult<T> Guid<T>(Func<JsonGuid, T> map) => Guid().Map(map);

      public override T Guid<T>(Func<JsonGuid, T> ifSuccess, Func<Exception, T> ifFailure) => Guid().FlatMap(ifSuccess, ifFailure);

      public override IResult<Guid> AsGuid() => Guid().Map(g => g.AsGuid());

      public override IResult<Guid> AsGuid(Func<Guid> defaultValue) => AsGuid().Or(defaultValue());

      public override IResult<JsonTimeSpan> TimeSpan() =>
         from timeSpan in @string.TimeSpan()
         from jTimeSpan in tryTo(() => new JsonTimeSpan(name, timeSpan))
         select jTimeSpan;

      public override IResult<T> TimeSpan<T>(Func<JsonTimeSpan, T> map) => TimeSpan().Map(map);

      public override T TimeSpan<T>(Func<JsonTimeSpan, T> ifSuccess, Func<Exception, T> ifFailure) => TimeSpan().FlatMap(ifSuccess, ifFailure);

      public override IResult<TimeSpan> AsTimeSpan() => TimeSpan().Map(ts => ts.AsTimeSpan());

      public override IResult<TimeSpan> AsTimeSpan(Func<TimeSpan> defaultValue) => AsTimeSpan().Or(defaultValue);
   }

   public class JsonInteger : JsonItem
   {
      protected int integer;

      public JsonInteger(string name, int integer)
      {
         this.name = name;
         this.integer = integer;
      }

      public override void GenerateValue(JsonWriter writer) => writer.Write(integer);

      public int ToInteger() => integer;

      public override IResult<JsonInteger> Integer() => this.Success();

      public override IResult<T> Integer<T>(Func<JsonInteger, T> map) => tryTo(() => map(this));

      public override T Integer<T>(Func<JsonInteger, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<int> AsInteger() => integer.Success();

      public override IResult<int> AsInteger(Func<int> defaultValue) => integer.Success();

      public override IMaybe<JsonInteger> IfInteger() => this.Some();
   }

   public class JsonDouble : JsonItem
   {
      protected double @double;

      public JsonDouble(string name, double @double)
      {
         this.name = name;
         this.@double = @double;
      }

      public override void GenerateValue(JsonWriter writer) => writer.Write(@double);

      public double ToDouble() => @double;

      public override IResult<JsonDouble> Double() => this.Success();

      public override IResult<T> Double<T>(Func<JsonDouble, T> map) => tryTo(() => map(this));

      public override T Double<T>(Func<JsonDouble, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<double> AsDouble() => @double.Success();

      public override IResult<double> AsDouble(Func<double> defaultValue) => @double.Success();

      public override IMaybe<JsonDouble> IfDouble() => this.Some();
   }

   public class JsonBoolean : JsonItem
   {
      bool boolean;

      public JsonBoolean(string name, bool boolean)
      {
         this.name = name;
         this.boolean = boolean;
      }

      public override void GenerateValue(JsonWriter writer) => writer.Write(boolean);

      public bool ToBoolean() => boolean;

      public override IResult<JsonBoolean> Boolean() => this.Success();

      public override IResult<T> Boolean<T>(Func<JsonBoolean, T> map) => tryTo(() => map(this));

      public override T Boolean<T>(Func<JsonBoolean, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<bool> AsBoolean() => boolean.Success();

      public override IResult<bool> AsBoolean(Func<bool> defaultValue) => boolean.Success();

      public override IMaybe<JsonBoolean> IfBoolean() => this.Some();
   }

   public class JsonByteArray : JsonItem
   {
      byte[] byteArray;

      public JsonByteArray(string name, byte[] byteArray)
      {
         this.name = name;
         this.byteArray = byteArray;
      }

      public override void GenerateValue(JsonWriter writer) => writer.Write(byteArray);

      public byte[] ToByteArray() => byteArray;

      public override IResult<JsonByteArray> ByteArray() => this.Success();

      public override IResult<T> ByteArray<T>(Func<JsonByteArray, T> map) => tryTo(() => map(this));

      public override T ByteArray<T>(Func<JsonByteArray, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<byte[]> AsByteArray() => byteArray.Success();

      public override IResult<byte[]> AsByteArray(Func<byte[]> defaultValue) => byteArray.Success();

      public override IMaybe<JsonByteArray> IfByteArray() => this.Some();
   }

   public class JsonFileName : JsonItem
   {
      FileName fileName;

      public JsonFileName(string name, FileName fileName)
      {
         this.name = name;
         this.fileName = fileName;
      }

      public override void GenerateValue(JsonWriter writer) => writer.Write(fileName);

      public FileName ToFileName() => fileName;

      public override IResult<JsonFileName> FileName() => this.Success();

      public override IResult<T> FileName<T>(Func<JsonFileName, T> map) => tryTo(() => map(this));

      public override T FileName<T>(Func<JsonFileName, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<FileName> AsFileName() => fileName.Success();

      public override IResult<FileName> AsFileName(Func<FileName> defaultValue) => fileName.Success();

      public override IMaybe<JsonFileName> IfFileName() => this.Some();
   }

   public class JsonFolderName : JsonItem
   {
      FolderName folderName;

      public JsonFolderName(string name, FolderName folderName)
      {
         this.name = name;
         this.folderName = folderName;
      }

      public override void GenerateValue(JsonWriter writer) => writer.Write(folderName);

      public FolderName ToFolderName() => folderName;

      public override IResult<JsonFolderName> FolderName() => this.Success();

      public override IResult<T> FolderName<T>(Func<JsonFolderName, T> map) => tryTo(() => map(this));

      public override T FolderName<T>(Func<JsonFolderName, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<FolderName> AsFolderName() => folderName.Success();

      public override IResult<FolderName> AsFolderName(Func<FolderName> defaultValue) => folderName.Success();

      public override IMaybe<JsonFolderName> IfFolderName() => this.Some();
   }

   public class JsonEnum : JsonItem
   {
      Enum enumeration;

      public JsonEnum(string name, Enum enumeration)
      {
         this.name = name;
         this.enumeration = enumeration;
      }

      public override void GenerateValue(JsonWriter writer) => writer.Write(enumeration);

      public Enum ToEnum() => enumeration;

      public override IResult<JsonEnum> Enum() => this.Success();

      public override IResult<T> Enum<T>(Func<JsonEnum, T> map) => tryTo(() => map(this));

      public override T Enum<T>(Func<JsonEnum, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<Enum> AsEnum() => enumeration.Success();

      public override IResult<Enum> AsEnum(Func<Enum> defaultValue) => enumeration.Success();

      public override IMaybe<JsonEnum> IfEnum() => this.Some();
   }

   public class JsonGuid : JsonItem
   {
      protected Guid guid;

      public JsonGuid(string name, Guid guid)
      {
         this.name = name;
         this.guid = guid;
      }

      public override void GenerateValue(JsonWriter writer) => writer.Write(guid.ToString().ToUpper());

      public Guid ToGuid() => guid;

      public override IResult<JsonGuid> Guid() => this.Success();

      public override IResult<T> Guid<T>(Func<JsonGuid, T> map) => tryTo(() => map(this));

      public override T Guid<T>(Func<JsonGuid, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<Guid> AsGuid() => guid.Success();

      public override IResult<Guid> AsGuid(Func<Guid> defaultValue) => guid.Success();

      public override IMaybe<JsonGuid> IfGuid() => this.Some();
   }

   public class JsonTimeSpan : JsonItem
   {
      protected TimeSpan timeSpan;

      public JsonTimeSpan(string name, TimeSpan timeSpan)
      {
         this.name = name;
         this.timeSpan = timeSpan;
      }

      public override void GenerateValue(JsonWriter writer) => writer.Write(timeSpan.ToLongString(true));

      public override IResult<JsonTimeSpan> TimeSpan() => this.Success();

      public override IResult<T> TimeSpan<T>(Func<JsonTimeSpan, T> map) => tryTo(() => map(this));

      public override T TimeSpan<T>(Func<JsonTimeSpan, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<TimeSpan> AsTimeSpan() => timeSpan.Success();

      public override IResult<TimeSpan> AsTimeSpan(Func<TimeSpan> defaultValue) => timeSpan.Success();

      public override IMaybe<JsonTimeSpan> IfTimeSpan() => this.Some();

      public TimeSpan ToTimeSpan() => timeSpan;
   }

   public class JsonNull : JsonItem
   {
      public JsonNull(string name) => this.name = name;

      public override void GenerateValue(JsonWriter writer) => writer.Write("null");

      public override IResult<JsonNull> Null() => this.Success();

      public override IMaybe<JsonNull> IfNull() => this.Some();
   }

   public class JsonAny : JsonItem
   {
      protected object @object;

      public JsonAny(string name, object @object)
      {
         this.name = name;
         this.@object = @object;
      }

      public override void GenerateValue(JsonWriter writer) => writer.Write(@object);

      public object ToObject() => @object;

      public override IResult<JsonAny> Any() => this.Success();

      public override IResult<T> Any<T>(Func<JsonAny, T> map) => tryTo(() => map(this));

      public override T Any<T>(Func<JsonAny, T> ifSuccess, Func<Exception, T> ifFailure) => ifSuccess(this);

      public override IResult<object> AsAny() => @object.Success();

      public override IResult<object> AsAny(Func<object> defaultValue) => @object.Success();

      public override IMaybe<JsonAny> IfAny() => this.Some();
   }

   public class JsonNothing : JsonBase
   {
      public override void Generate(JsonWriter writer) { }

      public override bool IsContainer => false;
   }
}