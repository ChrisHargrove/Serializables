using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BatteryAcid.Serializables
{
    [Serializable]
    public class SerializableUri : ISerializationCallbackReceiver
    {
        private const string ExampleUri = "https://www.example.com";

        public SerializableUri()
            => uri = new Uri(ExampleUri);

        public SerializableUri(string uriString)
            => uri = new Uri(uriString);

        public SerializableUri(Uri uri)
            => this.uri = uri;

        public SerializableUri(Uri baseUri, Uri relativeUri)
            => uri = new Uri(baseUri, relativeUri);

        public SerializableUri(string uriString, UriKind uriKind)
            => uri = new Uri(uriString, uriKind);

        private Uri uri;

        [SerializeField, HideInInspector]
        private string uriString;

        public bool IsDefaultPort => uri.IsDefaultPort;
        public string Authority => uri.Authority;
        public string DnsSafeHost => uri.DnsSafeHost;
        public string Fragment => uri.Fragment;
        public string Host => uri.Host;
        public UriHostNameType HostNameType => uri.HostNameType;
        public string IdnHost => uri.IdnHost;
        public bool IsAbsoluteUri => uri.IsAbsoluteUri;
        public bool IsFile => uri.IsFile;
        public string[] Segments => uri.Segments;
        public bool IsUnc => uri.IsUnc;
        public string LocalPath => uri.LocalPath;
        public string OriginalString => uri.OriginalString;
        public string PathAndQuery => uri.PathAndQuery;
        public int Port => uri.Port;
        public string Query => uri.Query;
        public string Scheme => uri.Scheme;
        public string AbsoluteUri => uri.AbsoluteUri;
        public bool IsLoopback => uri.IsLoopback;
        public string AbsolutePath => uri.AbsolutePath;
        public string UserInfo => uri.UserInfo;
        public bool UserEscaped => uri.UserEscaped;

        public override bool Equals(object obj)
        {
            if (obj is SerializableUri serialized)
            {
                return serialized.uri == this.uri;
            }
            if (obj is Uri uri)
            {
                return this.uri == uri;
            }
            return base.Equals(obj);
        }

        public string GetComponents(UriComponents components, UriFormat format)
            => uri.GetComponents(components, format);

        public override int GetHashCode()
            => uri.GetHashCode();

        public string GetLeftPart(UriPartial part)
            => uri.GetLeftPart(part);

        public bool IsBaseOf(Uri uri)
            => this.uri.IsBaseOf(uri);

        public bool IsWellFormedOriginalString()
            => uri.IsWellFormedOriginalString();

        public Uri MakeRelativeUri(Uri uri)
            => this.uri.MakeRelativeUri(uri);

        public override string ToString()
            => uri.ToString();

        #region Operators

        public static implicit operator Uri(SerializableUri serialized)
            => serialized.uri;

        public static implicit operator SerializableUri(Uri uri)
            => new SerializableUri(uri);

        public static bool operator ==(SerializableUri a, SerializableUri b)
            => a.uri == b.uri;

        public static bool operator !=(SerializableUri a, SerializableUri b)
            => a.uri != b.uri;

        public static bool operator ==(SerializableUri a, Uri b)
            => a.uri == b;

        public static bool operator !=(SerializableUri a, Uri b)
            => a.uri != b;

        public static bool operator ==(Uri a, SerializableUri b)
            => a == b.uri;

        public static bool operator !=(Uri a, SerializableUri b)
            => a != b.uri;

        #endregion

        #region ISerializationCallbackReceiver

        public void OnAfterDeserialize()
        {
            uri = new Uri(uriString);
        }

        public void OnBeforeSerialize()
        {
            uriString = uri.ToString();
        }

        #endregion
    }
}
