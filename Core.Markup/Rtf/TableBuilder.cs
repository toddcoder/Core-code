using Core.DataStructures;
using Core.Monads;

namespace Core.Markup.Rtf;

public class TableBuilder
{
   protected MaybeQueue<TableBuilderItem> items;

   public TableBuilder()
   {
      items = new MaybeQueue<TableBuilderItem>();
   }

   public void Add(TableBuilderItem item) => items.Enqueue(item);

   public Maybe<TableBuilderItem> Peek() => items.Peek();

   public Maybe<TableBuilderItem> Next() => items.Dequeue();
}