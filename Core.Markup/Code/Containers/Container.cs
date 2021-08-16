using System.Collections;
using System.Collections.Generic;
using Core.Enumerables;
using Core.Markup.Code.Blocks;
using Core.Monads;

namespace Core.Markup.Code.Containers
{
   public abstract class Container : IEnumerable<Block>
   {
      protected List<Block> blocks;

      public Container()
      {
         blocks = new List<Block>();
      }

      public void Add(Block block) => blocks.Add(block);

      public Block CurrentBlock
      {
         get
         {
            if (blocks.Count == 0)
            {
               var paragraph = new Paragraph();
               blocks.Add(paragraph);

               return paragraph;
            }
            else
            {
               return blocks[blocks.Count - 1];
            }
         }
      }

      public IEnumerator<Block> GetEnumerator() => blocks.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public Maybe<Block> LastBlock => blocks.LastOrNone();
   }
}