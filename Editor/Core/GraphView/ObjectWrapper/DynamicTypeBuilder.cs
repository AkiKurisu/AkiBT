using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
namespace Kurisu.AkiBT.Editor
{
    // Code from Unity.Kinematica
    internal static class DynamicTypeBuilder
    {
        private const string kDynamicTypeBuilderAssemblyName = "AkiBTDynamicTypeBuilderAssembly";
        private static ModuleBuilder m_ModuleBuilder;

        private static ModuleBuilder CreateModuleBuilder()
        {
            if (m_ModuleBuilder != null)
                return m_ModuleBuilder;

            var appDomain = AppDomain.CurrentDomain;
            var assemblyName = new AssemblyName(kDynamicTypeBuilderAssemblyName);
            var assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            m_ModuleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            return m_ModuleBuilder;
        }

        private const string k_DynamicTypePrefix = "AkiBTDynamicTypeDerivingFrom";
        public static Type MakeDerivedType<T>(Type baseClass)
        {
            ModuleBuilder moduleBuilder = CreateModuleBuilder();
            string tAssemblyName = typeof(T).Assembly.GetName().Name;
            string typeName = $"{baseClass.Namespace}_{baseClass.Name}";

            string currentAssemblyName = typeof(DynamicTypeBuilder).Assembly.GetName().Name.Replace('.', '_');

            if (baseClass.IsGenericType)
            {
                //Get rid of the '`N' after the class name for the # of generic args
                //TODO: If there are >= 10 args (highly unlikely) this will break (:
                typeName = typeName[..^2];
                typeName += $"_{string.Join("_", baseClass.GetGenericArguments().Select(t => t.FullName))}";
            }

            string typeNameWithoutAssembly = typeName.Replace('.', '_');

            typeName = $"{k_DynamicTypePrefix}_{tAssemblyName}_{typeName}";
            typeName = typeName.Replace('.', '_');
            Type[] moduleBuilderTypes = moduleBuilder.GetTypes();


            var existingType = moduleBuilderTypes.SingleOrDefault(t => t.Name.EndsWith(typeNameWithoutAssembly) && !t.Name.StartsWith($"{k_DynamicTypePrefix}_{currentAssemblyName}"));
            if (existingType != null)
            {
                return existingType;
            }


            existingType = moduleBuilderTypes.SingleOrDefault(t => t.Name == typeName);
            if (existingType != null)
            {
                return existingType;
            }
            var baseConstructor = baseClass.GetConstructor(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance, null, new Type[0], null);
            var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public, baseClass);
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
            var ilGenerator = constructor.GetILGenerator();

            if (baseConstructor != null)
            {
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Call, baseConstructor);
            }

            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ret);
            return typeBuilder.CreateType();
        }
    }
}
