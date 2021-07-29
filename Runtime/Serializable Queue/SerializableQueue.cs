using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BatteryAcid.Serializables
{
    [Serializable]
    public abstract class SerializableQueueBase
    { }

    [Serializable]
    public class SerializableQueue<TValue> : SerializableQueueBase, IReadOnlyCollection<TValue>, ICollection, ISerializationCallbackReceiver
    {
        [Serializable]
        public class ValueWrapper
        {
            public TValue Value;

            public ValueWrapper(TValue value)
            {
                Value = value;
            }
        }

        public SerializableQueue()
            => queue = new Queue<TValue>();

        public SerializableQueue(IEnumerable<TValue> enumerable)
            => queue = new Queue<TValue>(enumerable);

        public SerializableQueue(int capacity)
            => queue = new Queue<TValue>(capacity);

        private Queue<TValue> queue;

        [SerializeField, HideInInspector]
        private List<ValueWrapper> values;

        public int Count => queue.Count;

        public void Clear()
            => queue.Clear();

        public bool Contains(TValue value)
            => queue.Contains(value);

        public TValue Peek()
            => queue.Peek();

        public TValue Dequeue()
            => queue.Dequeue();

        public void Enqueue(TValue value)
            => queue.Enqueue(value);

        public TValue[] ToArray()
            => queue.ToArray();

        public void TrimExcess()
            => queue.TrimExcess();

        #region Operators

        public static implicit operator Queue<TValue>(SerializableQueue<TValue> serialized)
            => serialized.queue;

        public static implicit operator SerializableQueue<TValue>(Queue<TValue> queue)
            => new SerializableQueue<TValue>(queue);

        #endregion

        #region ICollection
        public bool IsSynchronized => false;

        public object SyncRoot => queue;

        public void CopyTo(Array array, int index)
            => queue.CopyTo((TValue[])array, index);

        #endregion

        #region IEnumerable

        public IEnumerator<TValue> GetEnumerator()
            => queue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => queue.GetEnumerator();

        #endregion

        #region ISerializationCallbackReceiver

        public void OnAfterDeserialize()
        {
            if (values != null)
            {
                if (queue == null) queue = new Queue<TValue>(values.Count);
                else queue.Clear();

                for (int i = 0; i < values.Count; i++)
                    queue.Enqueue(values[i].Value);
            }
        }

        public void OnBeforeSerialize()
        {
            if (queue == null)
            {
                values = null;
            }
            else
            {
                values = new List<ValueWrapper>();
                Queue<TValue>.Enumerator enumerator = queue.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    values.Add(new ValueWrapper(enumerator.Current));
                }
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is SerializableQueue<TValue> serialized)
            {
                return this.queue == serialized.queue;
            }
            if (obj is Queue<TValue> queue)
            {
                return this.queue == queue;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return queue.GetHashCode();
        }

        public override string ToString()
        {
            return queue.ToString();
        }
    }
}
