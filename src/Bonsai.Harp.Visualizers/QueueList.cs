using System;
using System.Collections;
using System.Collections.Generic;

namespace Bonsai.Harp.Visualizers
{
    internal class QueueList<T> : IReadOnlyList<T>
    {
        int head;
        int tail;
        int count;
        T[] buffer;

        public QueueList() : this(capacity: 4)
        {
        }

        public QueueList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            EnsureCapacity(capacity);
        }

        private void EnsureCapacity(int capacity)
        {
            var array = new T[capacity];
            if (count > 0)
            {
                if (head < tail)
                {
                    Array.Copy(buffer, head, array, 0, count);
                }
                else
                {
                    Array.Copy(buffer, head, array, 0, buffer.Length - head);
                    Array.Copy(buffer, 0, array, buffer.Length - head, tail);
                }
            }

            buffer = array;
            head = 0;
            tail = count;
        }

        private int GetIndexInternal(int index)
        {
            return (head + index) % buffer.Length;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return buffer[GetIndexInternal(index)];
            }
            set
            {
                if (index < 0 || index >= count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                buffer[GetIndexInternal(index)] = value;
            }
        }

        public int Count => count;

        public void Enqueue(T item)
        {
            if (count >= buffer.Length)
            {
                EnsureCapacity(buffer.Length * 2);
            }

            buffer[tail] = item;
            tail = (tail + 1) % buffer.Length;
            count++;
        }

        public bool TryDequeue(out T result)
        {
            if (count == 0)
            {
                result = default;
                return false;
            }

            result = buffer[head];
            buffer[head] = default;
            head = (head + 1) % buffer.Length;
            count--;
            return true;
        }

        public bool TryDequeueLast(out T result)
        {
            if (count == 0)
            {
                result = default;
                return false;
            }

            var index = tail - 1;
            if (index < 0) index += buffer.Length;
            result = buffer[index];
            buffer[index] = default;
            tail = index;
            count--;
            return true;
        }

        public void Clear()
        {
            if (head < tail)
            {
                Array.Clear(buffer, head, count);
            }
            else
            {
                Array.Clear(buffer, head, buffer.Length - head);
                Array.Clear(buffer, 0, tail);
            }

            head = 0;
            tail = 0;
            count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
            {
                yield return buffer[GetIndexInternal(i)];
            }
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
