using System;
using System.Collections.Generic;

namespace LWSerializer
{
    public class StringAccessMap : ILwSerializable
    {
        internal static Dictionary<int, string> _strings = new Dictionary<int, string>();
        //Dictionary Tokens<int, token<int[]>>,  <token<int[]>, int>
        //List AllString
        
        internal int Allocate(string str)
        {
            int idx = str.GetHashCode();
            _strings.TryAdd(idx, str);
            return idx;
        }

        internal string Get(int hash)
        {
            _strings.TryGetValue(hash, out var str);
            return str;
        }
        
        void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
        {
            writer.Write(_strings);
        }

        void ILwSerializable.OnNativeRead(LwBinaryReader reader)
        {
            
        }
    }
    
    /// <summary> Hash를 사용해 문자열을 불러오거나 저장합니다. 직렬화시에도 Hash는 유지되나, Hash에 해당하는 문자열을 따로 저장하고, 불러올때 StringAccess.Prepare 호출을 해줘야합니다</summary>
    public struct StringAccess : IEquatable<StringAccess>
    {
        public static StringAccess Null = new StringAccess(0);
        private int _hashCode;

        public StringAccess(string str, StringAccessMap map)
        {
            _hashCode = map.Allocate(str);
        }

        private StringAccess(int hashcode)
        {
            _hashCode = hashcode;
        }

        public void SetString(string str, StringAccessMap map)
        {
            _hashCode = map.Allocate(str);
        }

        public string GetString(StringAccessMap map)
        {
            return map.Get(_hashCode);
        }

        public static bool operator ==(StringAccess l, StringAccess r)
        {
            return l.Equals(r);
        }

        public static bool operator !=(StringAccess l, StringAccess r)
        {
            return !(l == r);
        }

        #region interface
        public bool Equals(StringAccess other)
        {
            return _hashCode == other._hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is StringAccess other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
        #endregion
    }
}