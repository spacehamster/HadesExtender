using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace HadesExtender
{
    internal static class PdbSymbolImporter
    {
        internal unsafe static void ImportSymbols(SymbolResolver resolver)
        {
            foreach (var field in GetFieldsWithAttribute<PdbSymbolAttribute>())
            {
                if (!field.IsStatic)
                {
                    Console.Error.WriteLine("{0}.{1} must be static.", field.DeclaringType, field.Name);
                    continue;
                }

                var symbolAttribute = GetCustomAttribute<PdbSymbolAttribute>(field);
                var symbolName = string.IsNullOrEmpty(symbolAttribute.SymbolName) ? 
                    field.Name : 
                    symbolAttribute.SymbolName;

                if(!resolver.TryResolve(symbolName, out var address))
                {
                    var errorMessage = string.Format("{0} in {1}.{2}",
                        symbolName, field.DeclaringType, field.Name);
                    throw new UnresolvedSymbolException(errorMessage);
                }

                if (field.FieldType == typeof(IntPtr))
                    field.SetValue(null, address);
                else if (field.FieldType == typeof(UIntPtr))
                    field.SetValue(null, new UIntPtr(address.ToPointer()));
                else if (field.FieldType.IsPointer)
                    CreateStaticSetter<IntPtr>(field).Invoke(address);
                else if (field.FieldType.IsSubclassOf(typeof(Delegate)))
                    field.SetValue(null, Marshal.GetDelegateForFunctionPointer(address, field.FieldType));
                else
                    Console.Error.WriteLine("{0}.{1} must be of IntPtr, UIntPtr or delegate type.", field.DeclaringType, field.Name);
            }
        }
        static IEnumerable<FieldInfo> GetFieldsWithAttribute<T>()
                where T : Attribute
        {
            const BindingFlags AllMemberFlags = 0
                | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static | BindingFlags.Instance;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes())
                    foreach (var field in type.GetFields(AllMemberFlags))
                    {
                        var attribute = GetCustomAttribute<T>(field);

                        if (attribute == null)
                            continue;

                        yield return field;
                    }
        }

        static T GetCustomAttribute<T>(FieldInfo field)
            where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(field, typeof(T));
        }

        // FieldInfo.SetValue doesn't appear to work for T* fields,
        // so we have to use this absolutely disgusting hack instead.
        static Action<T> CreateStaticSetter<T>(FieldInfo field)
        {
            var method = new DynamicMethod(string.Empty, null, new[] { typeof(T) }, true);
            var il = method.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stsfld, field);
            il.Emit(OpCodes.Ret);

            return (Action<T>)method.CreateDelegate(typeof(Action<T>));
        }
    }
}
