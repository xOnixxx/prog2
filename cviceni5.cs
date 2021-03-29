using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ConsoleApp1
{
    class EventQueue : IEnumerable<EventQueue.Event>
    {
        public abstract class Event
        {
            public enum EventState
            {
                Created,
                Scheduled,
                Executed,
                Cancelled
            }

            public Event Prev;
            public Event Next;
            public ulong Time;
            public EventState State;

            public Event()
            {
                State = EventState.Created;
            }

            public abstract void Execute();
        }

        private Event head;
        private Event tail;

        private void Unlink(Event e)
        {
            var prev = e.Prev;
            var next = e.Next;

            e.Prev = null;
            e.Next = null;

            if (prev != null)
            {
                prev.Next = next;
            }
            else
            {
                head = next;
            }

            if (next != null)
            {
                next.Prev = prev;
            }
            else
            {
                tail = prev;
            }
        }

        private void Link(Event e, Event prev, Event next)
        {
            e.Prev = prev;
            e.Next = next;

            if (prev != null)
            {
                prev.Next = e;
            }
            else
            {
                head = e;
            }

            if (next != null)
            {
                next.Prev = e;
            }
            else
            {
                tail = e;
            }
        }

        private void LinkAfter(Event e, Event prev) => Link(e, prev, prev != null ? prev.Next : head);
        private void LinkBefore(Event e, Event next) => Link(e, next != null ? next.Prev : tail, next);

        public void AddEvent(ulong time, Event e)
        {
            if (e.State == Event.EventState.Scheduled)
            {
                throw new InvalidOperationException("Cannot schedule already scheduled event");
            }

            var prev = tail;
            while (prev != null && prev.Time > time)
            {
                prev = prev.Prev;
            }

            LinkAfter(e, prev);

            e.State = Event.EventState.Scheduled;
            e.Time = time;
        }

        private class EventEnumerator : IEnumerator<Event>
        {
            private EventQueue queue;
            private bool first;

            public  EventEnumerator(EventQueue eq)
            {
                queue = eq;
                first = true;
            }

            public Event Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            { }

            public bool MoveNext()
            {
                if (first)
                {
                    Current = queue.head;
                    first = false;
                }
                else
                {
                    Current = Current.Next;
                }

                return Current != null;
            }

            public void Reset()
            {
                first = true;
            }
        }

        public IEnumerator<Event> GetEnumerator() => new EventEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class VerboseEvent : EventQueue.Event
    {
        public override void Execute() => Console.WriteLine($"Verbose event fired at {Time} ms");
    }

    class Program
    {
        static void Main(string[] args)
        {
            EventQueue eq = new EventQueue();
            eq.AddEvent(5, new VerboseEvent());
            eq.AddEvent(7, new VerboseEvent());
            eq.AddEvent(3, new VerboseEvent());

            foreach (var e in eq)
            {
                e.Execute();
            }
        }
    }
}
