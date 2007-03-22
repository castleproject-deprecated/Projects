using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Castle.Tools.CodeGenerator
{
  public class DictionaryAdapterFactory : IDictionaryAdapterFactory
  {
    #region Methods
    public T GetAdapter<T>(IDictionary dictionary) where T : IDictionaryAdapter
    {
      AppDomain appDomain = Thread.GetDomain();
      string adapterAssemblyName = GetAdapterAssemblyName<T>();
      Assembly adapterAssembly = GetExistingAdapterAssembly(appDomain, adapterAssemblyName);

      if (adapterAssembly == null)
        adapterAssembly = CreateAdapterAssembly<T>(appDomain, adapterAssemblyName);

      return GetExistingAdapter<T>(adapterAssembly, dictionary);
    }

    private static Assembly CreateAdapterAssembly<T>(AppDomain appDomain, string adapterAssemblyName)
    {
      AssemblyName assemblyName = new AssemblyName(adapterAssemblyName);
      AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
      ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(adapterAssemblyName);

      TypeBuilder typeBuilder = CreateAdapterType<T>(moduleBuilder);
      FieldBuilder dictionaryField = CreateAdapterDictionaryField(typeBuilder);
      CreateAdapterConstructor(typeBuilder, dictionaryField);

      List<PropertyInfo> properties = new List<PropertyInfo>();
      RecursivelyDiscoverProperties(properties, typeof(T));
      properties.ForEach(delegate(PropertyInfo propertyInfo) {
        CreateAdapterProperty(typeBuilder, dictionaryField, propertyInfo);
      });

      typeBuilder.CreateType();

      return assemblyBuilder;
    }

    private static void CreateAdapterConstructor(TypeBuilder typeBuilder, FieldBuilder dictionaryField)
    {
      ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
        MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard, new Type[] { typeof(IDictionary) });
      constructorBuilder.DefineParameter(1, ParameterAttributes.None, "dictionary");

      ILGenerator ilGenerator = constructorBuilder.GetILGenerator();

      Type objType = Type.GetType("System.Object");
      ConstructorInfo objectConstructorInfo = objType.GetConstructor(new Type[0]);

      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Call, objectConstructorInfo);
      ilGenerator.Emit(OpCodes.Nop);
      ilGenerator.Emit(OpCodes.Nop);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldarg_1);
      ilGenerator.Emit(OpCodes.Stfld, dictionaryField);
      ilGenerator.Emit(OpCodes.Nop);
      ilGenerator.Emit(OpCodes.Ret);
    }

    private static FieldBuilder CreateAdapterDictionaryField(TypeBuilder typeBuilder)
    {
      return typeBuilder.DefineField("dictionary", typeof(IDictionary), FieldAttributes.Private);
    }

    private static void CreateAdapterProperty(TypeBuilder typeBuilder, FieldBuilder dictionaryField, PropertyInfo property)
    {
      PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(property.Name, property.Attributes, property.PropertyType, null);
      MethodAttributes propertyMethodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual;

      if (property.CanRead)
        CreateAdapterPropertyGetMethod(typeBuilder, dictionaryField, propertyBuilder, property, propertyMethodAttributes);

      if (property.CanWrite)
        CreateAdapterPropertySetMethod(typeBuilder, dictionaryField, propertyBuilder, property, propertyMethodAttributes);
    }

    private static void CreateAdapterPropertyGetMethod(TypeBuilder typeBuilder, FieldBuilder dictionaryField, PropertyBuilder propertyBuilder,
                                                       PropertyInfo property, MethodAttributes propertyMethodAttributes)
    {
      MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + property.Name, propertyMethodAttributes, property.PropertyType, null);

      ILGenerator getILGenerator = getMethodBuilder.GetILGenerator();
      Label label = getILGenerator.DefineLabel();

      getILGenerator.DeclareLocal(property.PropertyType);
      getILGenerator.Emit(OpCodes.Nop);
      getILGenerator.Emit(OpCodes.Ldarg_0);
      getILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
      getILGenerator.Emit(OpCodes.Ldstr, GetDictionaryFieldName(property));
      getILGenerator.Emit(OpCodes.Callvirt, typeof(IDictionary).GetMethod("get_Item", new Type[] { typeof(Object) }));

      if (property.PropertyType.IsValueType)
        getILGenerator.Emit(OpCodes.Unbox_Any, property.PropertyType);
      else
        getILGenerator.Emit(OpCodes.Castclass, property.PropertyType);

      getILGenerator.Emit(OpCodes.Stloc_0);
      getILGenerator.Emit(OpCodes.Br_S, label);
      getILGenerator.MarkLabel(label);
      getILGenerator.Emit(OpCodes.Ldloc_0);
      getILGenerator.Emit(OpCodes.Ret);

      propertyBuilder.SetGetMethod(getMethodBuilder);
    }

    private static void CreateAdapterPropertySetMethod(TypeBuilder typeBuilder, FieldBuilder dictionaryField, PropertyBuilder propertyBuilder,
                                                       PropertyInfo property, MethodAttributes propertyMethodAttributes)
    {
      MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_" + property.Name, propertyMethodAttributes, null, new Type[] { property.PropertyType });

      ILGenerator setILGenerator = setMethodBuilder.GetILGenerator();

      setILGenerator.Emit(OpCodes.Nop);
      setILGenerator.Emit(OpCodes.Ldarg_0);
      setILGenerator.Emit(OpCodes.Ldfld, dictionaryField);
      setILGenerator.Emit(OpCodes.Ldstr, GetDictionaryFieldName(property));
      setILGenerator.Emit(OpCodes.Ldarg_1);

      if (property.PropertyType.IsValueType)
        setILGenerator.Emit(OpCodes.Box, property.PropertyType);

      setILGenerator.Emit(OpCodes.Callvirt, typeof(IDictionary).GetMethod("set_Item", new Type[] { typeof(Object), typeof(Object) }));
      setILGenerator.Emit(OpCodes.Nop);
      setILGenerator.Emit(OpCodes.Ret);

      propertyBuilder.SetSetMethod(setMethodBuilder);
    }

    private static TypeBuilder CreateAdapterType<T>(ModuleBuilder moduleBuilder)
    {
      TypeBuilder typeBuilder = moduleBuilder.DefineType(GetAdapterFullTypeName<T>(), TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit);
      typeBuilder.AddInterfaceImplementation(typeof(T));

      return typeBuilder;
    }

    private static string GetAdapterAssemblyName<T>()
    {
      return typeof(T).Assembly.GetName().Name + "." + typeof(T).FullName + ".DictionaryAdapter";
    }

    private static string GetAdapterFullTypeName<T>()
    {
      return typeof(T).Namespace + "." + GetAdapterTypeName<T>();
    }

    private static string GetAdapterTypeName<T>()
    {
      return typeof(T).Name.Substring(1) + "DictionaryAdapter";
    }

    private static DictionaryAdapterKeyPrefixAttribute GetDictionaryAdapterAttribute(PropertyInfo property)
    {
      List<Type> interfaces = new List<Type>();

      interfaces.Add(property.DeclaringType);
      interfaces.AddRange(property.DeclaringType.GetInterfaces());

      foreach (Type type in interfaces)
      {
        object[] attributes = type.GetCustomAttributes(typeof(DictionaryAdapterKeyPrefixAttribute), false);
        if (attributes.Length > 0)
          return (DictionaryAdapterKeyPrefixAttribute)attributes[0];
      }

      return null;
    }

    private static string GetDictionaryFieldName(PropertyInfo property)
    {
      DictionaryAdapterKeyPrefixAttribute dictionaryAdapterKeyPrefixAttribute = GetDictionaryAdapterAttribute(property);
      string prefix = dictionaryAdapterKeyPrefixAttribute == null ? string.Empty : dictionaryAdapterKeyPrefixAttribute.KeyPrefix;
      return prefix + property.Name;
    }

    private static T GetExistingAdapter<T>(Assembly assembly, IDictionary dictionary)
    {
      string adapterFullTypeName = GetAdapterFullTypeName<T>();
      return (T)Activator.CreateInstance(assembly.GetType(adapterFullTypeName, true), dictionary);
    }

    private static Assembly GetExistingAdapterAssembly(AppDomain appDomain, string assemblyName)
    {
      return Array.Find(appDomain.GetAssemblies(), delegate(Assembly assembly) {
        return assembly.GetName().Name == assemblyName;
      });
    }

    private static void RecursivelyDiscoverProperties(List<PropertyInfo> properties, Type currentType)
    {
      properties.AddRange(currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance));

      Array.ForEach(currentType.GetInterfaces(), delegate(Type parentInterface) {
        RecursivelyDiscoverProperties(properties, parentInterface);
      });
    }
    #endregion
  }
}