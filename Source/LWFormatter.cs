using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LWSerializer
{
    public abstract unsafe class ILWFormatter<T>
    {
        public abstract void Serialize(LwBinaryWriter writer, T value);
        public abstract void DeSerialize(LwBinaryReader reader, out T value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int GetSize<T>() where T : unmanaged
        {
            return Unsafe.SizeOf<T>();
        }
    }

    internal static class LWFormatterCache<T>
    {
        public static readonly ILWFormatter<T> Formatter;
        
        static LWFormatterCache()
        {
            Formatter = LWFormatterProvider.GetFormatter<T>();
        }
    }
}