using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Core.Dates;
using Core.Dates.DateIncrements;
using Core.Matching;
using Core.Strings;
using static System.Console;
using Timeout = System.Threading.Timeout;

namespace Core.Applications.Invokers
{
   public class BackgroundTask
   {
      protected Action<BackgroundTask> background;
      protected Action<BackgroundTask> foreground;
      protected Thread thread;
      protected TimeSpan sleepTime;
      protected Stopwatch stopwatch;
      protected Wait wait;
      protected int maxLength;
      protected ManualResetEvent shutDown;
      protected ManualResetEvent pause;
      protected Stack<Position> positions;
      protected char paddingChar;
      protected bool backgroundEnabled;

      public BackgroundTask(Action<BackgroundTask> background, Action<BackgroundTask> foreground, TimeSpan sleepTime)
      {
         this.background = background;
         this.foreground = foreground;
         this.sleepTime = sleepTime;
         thread = new Thread(executeBackground);
         stopwatch = new Stopwatch();
         wait = new Wait();
         shutDown = new ManualResetEvent(false);
         pause = new ManualResetEvent(true);
         positions = new Stack<Position>();
         paddingChar = ' ';
         maxLength = BufferWidth;
         backgroundEnabled = true;
      }

      public BackgroundTask(Action<BackgroundTask> background, Action<BackgroundTask> foreground)
         : this(background, foreground, 1.Millisecond()) { }

      public char PaddingChar
      {
         get => paddingChar;
         set => paddingChar = value;
      }

      public int MaxLength
      {
         get => maxLength;
         set => maxLength = value <= 0 ? BufferWidth : value;
      }

      public bool BackgroundEnabled
      {
         get => backgroundEnabled;
         set => backgroundEnabled = value;
      }

      protected void executeBackground()
      {
         while (true)
         {
            pause.WaitOne(Timeout.Infinite);
            if (shutDown.WaitOne(0))
            {
               break;
            }

            if (backgroundEnabled)
            {
               background(this);
            }
         }
      }

      public void Execute()
      {
         CursorVisible = false;

         thread.Start();
         try
         {
            foreground(this);
         }
         finally
         {
            Stop();

            CursorVisible = true;
         }
      }

      public void Sleep()
      {
         Thread.Yield();
         sleepTime.Sleep();
      }

      public void StartTiming() => stopwatch.Start();

      public string StopTiming()
      {
         stopwatch.Stop();
         return stopwatch.Elapsed.ToLongString(true);
      }

      public void ResetTiming() => stopwatch.Reset();

      public void WriteBackground(int left, int top, object obj, bool pad = false)
      {
         lock (this)
         {
            var x = left % BufferWidth;
            var y = top % BufferHeight;
            SetCursorPosition(x, y);
            var text = obj?.ToString() ?? "";
            if (pad)
            {
               text = text.PadRight(maxLength, paddingChar);
            }

            Console.Write(text);
            SetCursorPosition(x, y);
         }
      }

      public void Wait(int left, int top) => WriteBackground(left, top, wait.Next());

      public void ElapsedTime(int left, int top, bool longForm = true, bool includeMilliseconds = true)
      {
         string text;
         if (longForm)
         {
            text = stopwatch.Elapsed.ToLongString(includeMilliseconds);
         }
         else
         {
            text = stopwatch.Elapsed.ToString("g");
            if (!includeMilliseconds && text.Contains("."))
            {
               text = text.Substitute("'.' /d+ $; f", "");
            }
         }

         WriteBackground(left, top, text, true);
      }

      public void Write(object text)
      {
         var str = text?.ToString() ?? "";
         Console.Write(str.PadRight(maxLength, paddingChar));
      }

      public void Write(int left, int top, object text)
      {
         Pause();

         lock (this)
         {
            positions.Push(Position.Save());
            SetCursorPosition(left % BufferWidth, top % BufferHeight);

            Write(text);

            if (positions.Count > 0)
            {
               var position = positions.Pop();
               position.Retrieve();
            }
         }

         Resume();
      }

      public void WriteLine(object text = null)
      {
         Console.WriteLine();
         Console.WriteLine(text?.ToString() ?? "");
         positions.Clear();
      }

      public void Pause() => pause.Reset();

      public void Resume() => pause.Set();

      public void Stop()
      {
         shutDown.Set();
         pause.Set();
         thread.Join();
      }

      public (int, int) GetPosition() => (CursorLeft, CursorTop);

      public void SetPosition(Tuple<int, int> position)
      {
         CursorLeft = position.Item1;
         CursorTop = position.Item2;
      }

      public string Line(char ch) => ch.ToString().Repeat(maxLength);
   }
}