using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Core.Applications
{
	public class ParallelProcessor<T>
	{
		SlicedList<T>[] listSlices;
		int numberOfThreads;
		Action<T> action;
		ManualResetEvent[] manualResetEvents;

		public ParallelProcessor(int numberOfThreads, Action<T> action)
		{
			this.numberOfThreads = numberOfThreads;
			listSlices = new SlicedList<T>[this.numberOfThreads];
			this.action = action;
			manualResetEvents = new ManualResetEvent[this.numberOfThreads];

			for (var i = 0; i < this.numberOfThreads; i++)
			{
				listSlices[i] = new SlicedList<T>();
				manualResetEvents[i] = new ManualResetEvent(false);
				listSlices[i].indexes = new LinkedList<int>();
				listSlices[i].manualResetEvent = manualResetEvents[i];
			}
		}

		public void ForEach(IEnumerable<T> items)
		{
			prepareListSlices(items.ToArray());
			for (var i = 0; i < numberOfThreads; i++)
			{
				manualResetEvents[i].Reset();
				ThreadPool.QueueUserWorkItem(doWork, listSlices[i]);
			}

			WaitHandle.WaitAll(manualResetEvents);
		}

		void prepareListSlices(IEnumerable<T> items)
		{
			var array = items.ToArray();
			for (var i = 0; i < numberOfThreads; i++)
			{
				listSlices[i].items = array;
				listSlices[i].indexes.Clear();
			}

			for (var i = 0; i < array.Length; i++)
         {
            listSlices[i % numberOfThreads].indexes.AddLast(i);
         }
      }

		void doWork(object o)
		{
			var slicedList = (SlicedList<T>)o;

			foreach (var i in slicedList.indexes)
         {
            action(slicedList.items[i]);
         }

         slicedList.manualResetEvent.Set();
		}
	}

	public class SlicedList<T>
	{
		public T[] items;
		public LinkedList<int> indexes;
		public ManualResetEvent manualResetEvent;
	}
}