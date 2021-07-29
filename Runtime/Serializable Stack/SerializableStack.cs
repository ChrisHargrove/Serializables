using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BatteryAcid.Serializables
{
    [Serializable]
    public abstract class SerializableStackBase
    { }

    [Serializable]
    public class SerializableStack<TValue> : SerializableStackBase, IReadOnlyCollection<TValue>, ICollection, ISerializationCallbackReceiver
    {
        [Serializable]
        private class ValueWrapper
        {
            public TValue Value;

            public ValueWrapper(TValue value)
            {
                Value = value;
            }
        }

        public SerializableStack()
            => stack = new Stack<TValue>();

        public SerializableStack(IEnumerable<TValue> enumerable)
            => stack = new Stack<TValue>(enumerable);

        public SerializableStack(int capacity)
            => stack = new Stack<TValue>(capacity);

        private Stack<TValue> stack;

        [SerializeField, HideInInspector]
        private List<ValueWrapper> values;

        public void Clear()
            => stack.Clear();

        public bool Contains(TValue value)
            => stack.Contains(value);

        public TValue Peek()
            => stack.Peek();

        public TValue Pop()
            => stack.Pop();

        public void Push(TValue value)
            => stack.Push(value);

        public TValue[] ToArray()
            => stack.ToArray();

        public void TrimExcess()
            => stack.TrimExcess();

        #region Operators

        public static implicit operator Stack<TValue>(SerializableStack<TValue> serialized)
            => serialized.stack;

        public static implicit operator SerializableStack<TValue>(Stack<TValue> stack)
            => new SerializableStack<TValue>(stack);

        #endregion

        #region IReadOnlyCollection

        public int Count => stack.Count;

        public bool IsSynchronized => ((ICollection)stack).IsSynchronized;

        public object SyncRoot => ((ICollection)stack).SyncRoot;

        public IEnumerator<TValue> GetEnumerator()
        {
            return ((IEnumerable<TValue>)stack).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)stack).GetEnumerator();
        }

        #endregion

        #region ICollection

        public void CopyTo(Array array, int index)
        {
            ((ICollection)stack).CopyTo(array, index);
        }

        #endregion

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize()
        {
            if (stack == null)
            {
                values = null;
            }
            else
            {
                values = new List<ValueWrapper>();
                Stack<TValue>.Enumerator enumerator = stack.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    values.Add(new ValueWrapper(enumerator.Current));
                }
            }
        }

        public void OnAfterDeserialize()
        {
            if (values != null)
            {
                if (stack == null) stack = new Stack<TValue>(values.Count);
                else stack.Clear();

                for (int i = values.Count - 1; i >= 0; i--)
                    stack.Push(values[i].Value);
            }
        }

        #endregion
    }
}
