/*using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Rui.IO.Serialization
{
    /// <summary> StringAccess를 수집하여 사용되는 string 데이터를 데이터화 시키거나,
    /// 데이터화 된 string 데이터들을 StringAccess를 통해 접근할 수 있도록 등록하는 기능제공</summary>
    public unsafe struct StringAccessSerializer : IDisposable
    {
        //[NativeDisableUnsafePtrRestriction] <-- 잡에서 사용하고싶으면 이거푸셈
        private UnsafeList<StringAccess>* _accessList;

        public StringAccessSerializer(Allocator allocator)
        {
            _accessList = UnsafeList<StringAccess>.Create(100, allocator);
        }
        
        public void Set(in StringAccess access)
        {
            _accessList->Add(access);
        }

        /*public void Write(NativeBinaryWriter writer)
        {
            HashSet<StringAccess> hashSet = new();
            var headerPos = writer.Position;
            writer.WritePadding(4);
            int count = 0;
            var list = *_accessList;
            for (int i = 0; i < list.Length; i++)
            {
                var access = list.Ptr[i];
                if ( hashSet.Add(access))
                {
                    var str =access.GetString();
                    if (!string.IsNullOrEmpty(str))
                    {
                        count++;
                        writer.Write(str);
                    }
                }
            }
            writer.Write(count,headerPos);
        }#1#

        /*public void Read(NativeBinaryReader reader)
        {
            var list = *_accessList;
            list.Clear();
            int size = reader.Read<int>();
            for (int i = 0; i < size; i++)
            {
                var str = reader.ReadString();
                var access = new StringAccess(str);
                list.Add(access);
            }
            _accessList = &list;
        }#1#

        public static void ReadToPrepare(NativeBinaryReader_Legacy reader)
        {
            int size = reader.Read<int>();
            for (int i = 0; i < size; i++)
                StringAccess.Prepare(reader.ReadString());
        }
        
        public void Dispose()
        {
            UnsafeList<StringAccess>.Destroy(_accessList);
        }
    }
}*/