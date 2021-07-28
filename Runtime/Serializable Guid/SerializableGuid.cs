using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BatteryAcid.Serializables
{
    /// <summary>
    /// Wrapper Class that allows Unity to Serialize Guid's in the inspector
    /// <para>
    /// Allows for implicit conversion to and from a Guid.
    /// </para>
    /// </summary>
    [Serializable]
    public class SerializableGuid : ISerializationCallbackReceiver, IComparable, IComparable<Guid>, IEquatable<Guid>, IFormattable
    {
        public SerializableGuid()
            => guid = Guid.Empty;

        public SerializableGuid(Guid guid)
            => this.guid = guid;

        public SerializableGuid(byte[] bytes) : this(new Guid(bytes)) { }
        public SerializableGuid(string guidString) : this(new Guid(guidString)) { }
        public SerializableGuid(int a, short b, short c, byte[] d) : this(new Guid(a, b, c, d)) { }
        public SerializableGuid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k) : this(new Guid(a, b, c, d, e, f, g, h, i, j, k)) { }
        public SerializableGuid(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k) : this(new Guid(a, b, c, d, e, f, g, h, i, j, k)) { }

        private Guid guid;

        [SerializeField]
        private string guidString;

        #region Operators

        public static implicit operator Guid(SerializableGuid serializedGuid) => serializedGuid.guid;
        public static implicit operator SerializableGuid(Guid guid) => new SerializableGuid(guid);

        public static bool operator ==(SerializableGuid a, Guid b) => a.guid == b;
        public static bool operator !=(SerializableGuid a, Guid b) => a.guid != b;

        public static bool operator ==(Guid a, SerializableGuid b) => a == b.guid;
        public static bool operator !=(Guid a, SerializableGuid b) => a != b.guid;

        #endregion

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize()
        {
            guidString = guid.ToString();
        }

        public void OnAfterDeserialize()
        {
            guid = Guid.Parse(guidString);
        }

        #endregion

        #region IComparable

        public int CompareTo(object obj)
        {
            return ((IComparable)guid).CompareTo(obj);
        }

        public int CompareTo(Guid other)
        {
            return ((IComparable<Guid>)guid).CompareTo(other);
        }

        #endregion

        #region IEquatable

        public bool Equals(Guid other)
        {
            return ((IEquatable<Guid>)guid).Equals(other);
        }

        #endregion

        #region IFormattable

        public string ToString(string format)
        {
            return guid.ToString(format);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ((IFormattable)guid).ToString(format, formatProvider);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is SerializableGuid serialized)
            {
                return this.guid == serialized.guid;
            }
            if (obj is Guid guid)
            {
                return this.guid == guid;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }

        public override string ToString()
        {
            return guid.ToString();
        }

        public byte[] ToByteArray()
        {
            return guid.ToByteArray();
        }
    }
}
