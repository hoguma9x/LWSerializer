using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using LWSerializer.Formatters;

#if UNITY_5_3_OR_NEWER || UNITY_2017_1_OR_NEWER
using LWSerializer.Formatters.Unity;
using Unity.Collections;
#endif

namespace LWSerializer
{
    public static class LWFormatterProvider
    {
        private static readonly Dictionary<Type, bool> _unmanagedType = new Dictionary<Type, bool>();
      
        public static ILWFormatter<T> GetFormatter<T>()
        {
            var type = typeof(T);
            
            //ILwSerializable Type
            if (typeof(ILwSerializable).IsAssignableFrom(type))
                return (ILWFormatter<T>)Activator.CreateInstance(typeof(ILwSerializableFormatter<>).MakeGenericType(typeof(T)));
            
            //LwNativeMemory Type
            
            //string Type
            if (type == typeof(string))
                return (ILWFormatter<T>)Activator.CreateInstance(typeof(StringFormatter));
            
            if (type.IsArray)
            {
                return CreateArrayFormatter<T>();
            }
            else if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();
                
                // Dictionary Type
                if (genericTypeDefinition == typeof(Dictionary<,>))
                    return CreateDictionaryFormatter<T>(type);
                // List Type
                if (genericTypeDefinition == typeof(List<>))
                    return CreateListFormatter<T>(type);
                //Unity Collections Type
#if UNITY_5_3_OR_NEWER || UNITY_2017_1_OR_NEWER
                var args = type.GetGenericArguments();
                if(genericTypeDefinition == typeof(Unity.Collections.NativeArray<>))
                    return (ILWFormatter<T>)Activator.CreateInstance(typeof(NativeArrayFormatter<>).MakeGenericType(args[0]));
                if(genericTypeDefinition == typeof(NativeReference<>))
                    return (ILWFormatter<T>)Activator.CreateInstance(typeof(NativeReferenceFormatter<>).MakeGenericType(args[0]));
#endif
            }
            
            // unmanaged Type
            if (IsUnmanagedType<T>()) 
                return (ILWFormatter<T>)Activator.CreateInstance(typeof(UnmangedFormatter<>).MakeGenericType(typeof(T)));
            
            throw new NotImplementedException($"{type.Name} formatter, not supported ");
        }

        #region Create Generic Formatters
        private static ILWFormatter<T> CreateDictionaryFormatter<T>(Type type)
        {
            var args = type.GetGenericArguments();
            Type formatterType;
            bool _0Unmanaged = IsUnmanagedType(args[0]);
            bool _1Unmanaged = IsUnmanagedType(args[1]);
            
            if (_0Unmanaged && _1Unmanaged)
            {
                formatterType = typeof(DictionaryFormatter_UnmanagedKV<,>).MakeGenericType(args);
            }
            else if (_0Unmanaged && (!_1Unmanaged))
            {
                formatterType = typeof(DictionaryFormatter_UnmanagedK<,>).MakeGenericType(args);
            }
            else if ((!_0Unmanaged) && _1Unmanaged)
            {
                formatterType = typeof(DictionaryFormatter_UnmanagedV<,>).MakeGenericType(args);
            }
            else if ((!_0Unmanaged) && (!_1Unmanaged))
            {
                formatterType = typeof(DictionaryFormatter<,>).MakeGenericType(args);
            }
            else
            {
                throw new NotImplementedException($"{type.Name} formatter, not supported ");
            }
            
            return (ILWFormatter<T>)Activator.CreateInstance(formatterType);
        }

        private static ILWFormatter<T> CreateListFormatter<T>(Type type)
        {
            Type formatterType;
            var args = type.GetGenericArguments();
            
            if (IsUnmanagedType(args[0]))
                formatterType = typeof(ListFormatter_Unmanaged<>).MakeGenericType(args[0]);
            else
                formatterType = typeof(ListFormatter<>).MakeGenericType(args[0]);
            return (ILWFormatter<T>)Activator.CreateInstance(formatterType);
        }

        private static FORMATTER CreateGenericFormatter<TYPE, FORMATTER>() 
            where FORMATTER : ILWFormatter<TYPE>
            where TYPE : unmanaged
        {
            Type formatterType;
            formatterType = typeof(FORMATTER).MakeGenericType(typeof (TYPE));
            return (FORMATTER)Activator.CreateInstance(formatterType);
        }
        
        private static ILWFormatter<T> CreateArrayFormatter<T>()
        {
            Type elementType = typeof(T).GetElementType();
            Type formatterType;
            if (IsUnmanagedType(elementType))
                formatterType = typeof(ArrayFormatter_Unamanaged<>).MakeGenericType(elementType);
            else
                formatterType = typeof(ArrayFormatter<>).MakeGenericType(elementType);
            return (ILWFormatter<T>)Activator.CreateInstance(formatterType);
        }
        #endregion
        
        #region CheckUnmanagedType
        public static bool IsUnmanagedType<T>()
        {
            return !RuntimeHelpers.IsReferenceOrContainsReferences<T>();
        }

        public static bool IsUnmanagedType(Type type)
        {
            bool checkUnmanagedType(Type t)
            {
                if (!t.IsValueType) return false;
                if (t.IsPrimitive || t.IsEnum) return true;
                if (t.IsPointer) return true;

                // 4. 구조체라면 내부 필드를 재귀적으로 검사
                // BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic 중요
                foreach (var field in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    if (!checkUnmanagedType(field.FieldType)) 
                        return false;
                return true;
            }

            if (!_unmanagedType.TryGetValue(type, out var value))
                _unmanagedType.Add(type, value = checkUnmanagedType(type));
            return value;
        }
        #endregion
    }
}