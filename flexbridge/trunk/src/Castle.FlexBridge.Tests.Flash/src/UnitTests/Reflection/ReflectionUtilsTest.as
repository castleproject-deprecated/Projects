package UnitTests.Reflection
{
	import flexunit.framework.TestCase;
	import flexunit.framework.Assert;
	import Castle.FlexBridge.Flash.Reflection.*;
	import flash.utils.getQualifiedClassName;
	import UnitTests.Reflection.Resources.*;
	
	public class ReflectionUtilsTest extends TestCase
	{
		public function testEmptyClass():void
		{
			var info:ClassInfo = ReflectionUtils.getClassInfo(EmptyClass);

			Assert.assertEquals(getQualifiedClassName(EmptyClass), info.name);
			Assert.assertEquals(EmptyClass, info.type);
			Assert.assertEquals(false, info.isInterface);

			Assert.assertEquals(1, info.superclasses.length);
			Assert.assertEquals(Object, info.superclasses[0]);
						
			Assert.assertEquals(0, info.interfaces.length);
			
			Assert.assertEquals(0, info.constructor.parameters.length);
			
			Assert.assertEquals(0, info.constants.length);
			
			Assert.assertEquals(0, info.fields.length);
			
			Assert.assertEquals(0, info.methods.length);
			
			Assert.assertEquals(0, info.properties.length);
		}
		
		public function testEmptyInterface():void
		{
			var info:ClassInfo = ReflectionUtils.getClassInfo(EmptyInterface);

			Assert.assertEquals(getQualifiedClassName(EmptyInterface), info.name);
			Assert.assertEquals(EmptyInterface, info.type);
			Assert.assertEquals(true, info.isInterface);

			Assert.assertEquals(0, info.interfaces.length);
			
			Assert.assertEquals(0, info.superclasses.length);
			
			Assert.assertNull(info.constructor);
			
			Assert.assertEquals(0, info.constants.length);
			
			Assert.assertEquals(0, info.fields.length);
			
			Assert.assertEquals(0, info.methods.length);
			
			Assert.assertEquals(0, info.properties.length);
		}
		
		public function testKitchenSinkInterface():void
		{
			var info:ClassInfo = ReflectionUtils.getClassInfo(KitchenSinkInterface);

			Assert.assertEquals(getQualifiedClassName(KitchenSinkInterface), info.name);
			Assert.assertEquals(KitchenSinkInterface, info.type);
			Assert.assertEquals(true, info.isInterface);
			
			Assert.assertEquals(1, info.interfaces.length);
			Assert.assertEquals(EmptyInterface, info.interfaces[0]);
			
			Assert.assertEquals(0, info.superclasses.length);
			
			Assert.assertNull(info.constructor);

			Assert.assertEquals(0, info.constants.length);
			
			Assert.assertEquals(0, info.fields.length);
			
			Assert.assertEquals(2, info.methods.length);
			
			var simpleInstanceMethod:MethodInfo = MethodInfo(findByName(info.methods, "simpleInstanceMethod"));
			Assert.assertEquals(KitchenSinkInterface, simpleInstanceMethod.declaringType);
			Assert.assertEquals("simpleInstanceMethod", simpleInstanceMethod.name);
			Assert.assertEquals(Void, simpleInstanceMethod.returnType);
			Assert.assertEquals(0, simpleInstanceMethod.parameters.length);
			Assert.assertEquals(false, simpleInstanceMethod.isStatic);

			var complexInstanceMethod:MethodInfo = MethodInfo(findByName(info.methods, "complexInstanceMethod"));
			Assert.assertEquals(KitchenSinkInterface, complexInstanceMethod.declaringType);
			Assert.assertEquals("complexInstanceMethod", complexInstanceMethod.name);
			Assert.assertEquals(Object, complexInstanceMethod.returnType);
			Assert.assertEquals(3, complexInstanceMethod.parameters.length);
			Assert.assertEquals(false, complexInstanceMethod.isStatic);
			
			Assert.assertEquals(0, ParameterInfo(complexInstanceMethod.parameters[0]).index);
			Assert.assertEquals(int, ParameterInfo(complexInstanceMethod.parameters[0]).type);
			Assert.assertEquals(false, ParameterInfo(complexInstanceMethod.parameters[0]).isOptional);

			Assert.assertEquals(1, ParameterInfo(complexInstanceMethod.parameters[1]).index);
			Assert.assertEquals(Any, ParameterInfo(complexInstanceMethod.parameters[1]).type);
			Assert.assertEquals(false, ParameterInfo(complexInstanceMethod.parameters[1]).isOptional);

			Assert.assertEquals(2, ParameterInfo(complexInstanceMethod.parameters[2]).index);
			Assert.assertEquals(String, ParameterInfo(complexInstanceMethod.parameters[2]).type);
			Assert.assertEquals(true, ParameterInfo(complexInstanceMethod.parameters[2]).isOptional);
			
			Assert.assertEquals(3, info.properties.length);
			
			var readWriteInstanceProperty:PropertyInfo = PropertyInfo(findByName(info.properties, "readWriteInstanceProperty"));
			Assert.assertEquals(KitchenSinkInterface, readWriteInstanceProperty.declaringType);
			Assert.assertEquals("readWriteInstanceProperty", readWriteInstanceProperty.name);
			Assert.assertEquals(int, readWriteInstanceProperty.type);
			Assert.assertEquals(true, readWriteInstanceProperty.isReadable);
			Assert.assertEquals(true, readWriteInstanceProperty.isWriteable);
			Assert.assertEquals(false, readWriteInstanceProperty.isStatic);
			
			var readOnlyInstanceProperty:PropertyInfo = PropertyInfo(findByName(info.properties, "readOnlyInstanceProperty"));
			Assert.assertEquals(KitchenSinkInterface, readOnlyInstanceProperty.declaringType);
			Assert.assertEquals("readOnlyInstanceProperty", readOnlyInstanceProperty.name);
			Assert.assertEquals(int, readOnlyInstanceProperty.type);
			Assert.assertEquals(true, readOnlyInstanceProperty.isReadable);
			Assert.assertEquals(false, readOnlyInstanceProperty.isWriteable);
			Assert.assertEquals(false, readOnlyInstanceProperty.isStatic);

			var writeOnlyInstanceProperty:PropertyInfo = PropertyInfo(findByName(info.properties, "writeOnlyInstanceProperty"));
			Assert.assertEquals(KitchenSinkInterface, writeOnlyInstanceProperty.declaringType);
			Assert.assertEquals("writeOnlyInstanceProperty", writeOnlyInstanceProperty.name);
			Assert.assertEquals(int, writeOnlyInstanceProperty.type);
			Assert.assertEquals(false, writeOnlyInstanceProperty.isReadable);
			Assert.assertEquals(true, writeOnlyInstanceProperty.isWriteable);
			Assert.assertEquals(false, writeOnlyInstanceProperty.isStatic);
		}

		public function testKitchenSinkClass():void
		{
			var info:ClassInfo = ReflectionUtils.getClassInfo(KitchenSinkClass);

			Assert.assertEquals(getQualifiedClassName(KitchenSinkClass), info.name);
			Assert.assertEquals(KitchenSinkClass, info.type);
			Assert.assertEquals(false, info.isInterface);
			
			Assert.assertEquals(1, info.interfaces.length);
			Assert.assertEquals(EmptyInterface, info.interfaces[0]);
			
			Assert.assertEquals(2, info.superclasses.length);
			Assert.assertEquals(EmptyClass, info.superclasses[0]);
			Assert.assertEquals(Object, info.superclasses[1]);
			
			Assert.assertEquals(3, info.constructor.parameters.length);
			
			Assert.assertEquals(0, ParameterInfo(info.constructor.parameters[0]).index);
			Assert.assertEquals("Invalid constructor parameter type.  The robustDescribeType() hack for constructor parameters may be broken.",
				int, ParameterInfo(info.constructor.parameters[0]).type);
			Assert.assertEquals(false, ParameterInfo(info.constructor.parameters[0]).isOptional);

			Assert.assertEquals(1, ParameterInfo(info.constructor.parameters[1]).index);
			Assert.assertEquals("Invalid constructor parameter type.  The robustDescribeType() hack for constructor parameters may be broken.",
				Any, ParameterInfo(info.constructor.parameters[1]).type);
			Assert.assertEquals(false, ParameterInfo(info.constructor.parameters[1]).isOptional);

			Assert.assertEquals(2, ParameterInfo(info.constructor.parameters[2]).index);
			Assert.assertEquals("Invalid constructor parameter type.  The robustDescribeType() hack for constructor parameters may be broken.",
				String, ParameterInfo(info.constructor.parameters[2]).type);
			Assert.assertEquals(true, ParameterInfo(info.constructor.parameters[2]).isOptional);

			Assert.assertEquals(2, info.constants.length);
			
			var instanceConstant:ConstantInfo = ConstantInfo(findByName(info.constants, "instanceConstant"));
			Assert.assertEquals("instanceConstant", instanceConstant.name);
			Assert.assertEquals(int, instanceConstant.type);
			Assert.assertEquals(false, instanceConstant.isStatic);

			var staticConstant:ConstantInfo = ConstantInfo(findByName(info.constants, "staticConstant"));
			Assert.assertEquals("staticConstant", staticConstant.name);
			Assert.assertEquals(int, staticConstant.type);
			Assert.assertEquals(true, staticConstant.isStatic);
			
			Assert.assertEquals(2, info.fields.length);

			var instanceField:FieldInfo = FieldInfo(findByName(info.fields, "instanceField"));
			Assert.assertEquals("instanceField", instanceField.name);
			Assert.assertEquals(int, instanceField.type);
			Assert.assertEquals(false, instanceField.isStatic);

			var staticField:FieldInfo = FieldInfo(findByName(info.fields, "staticField"));
			Assert.assertEquals("staticField", staticField.name);
			Assert.assertEquals(int, staticField.type);
			Assert.assertEquals(true, staticField.isStatic);
			
			Assert.assertEquals(4, info.methods.length);
			
			var simpleInstanceMethod:MethodInfo = MethodInfo(findByName(info.methods, "simpleInstanceMethod"));
			Assert.assertEquals(KitchenSinkClass, simpleInstanceMethod.declaringType);
			Assert.assertEquals("simpleInstanceMethod", simpleInstanceMethod.name);
			Assert.assertEquals(Void, simpleInstanceMethod.returnType);
			Assert.assertEquals(0, simpleInstanceMethod.parameters.length);
			Assert.assertEquals(false, simpleInstanceMethod.isStatic);

			var complexInstanceMethod:MethodInfo = MethodInfo(findByName(info.methods, "complexInstanceMethod"));
			Assert.assertEquals(KitchenSinkClass, complexInstanceMethod.declaringType);
			Assert.assertEquals("complexInstanceMethod", complexInstanceMethod.name);
			Assert.assertEquals(Object, complexInstanceMethod.returnType);
			Assert.assertEquals(3, complexInstanceMethod.parameters.length);
			Assert.assertEquals(false, complexInstanceMethod.isStatic);
			
			Assert.assertEquals(0, ParameterInfo(complexInstanceMethod.parameters[0]).index);
			Assert.assertEquals(int, ParameterInfo(complexInstanceMethod.parameters[0]).type);
			Assert.assertEquals(false, ParameterInfo(complexInstanceMethod.parameters[0]).isOptional);

			Assert.assertEquals(1, ParameterInfo(complexInstanceMethod.parameters[1]).index);
			Assert.assertEquals(Any, ParameterInfo(complexInstanceMethod.parameters[1]).type);
			Assert.assertEquals(false, ParameterInfo(complexInstanceMethod.parameters[1]).isOptional);

			Assert.assertEquals(2, ParameterInfo(complexInstanceMethod.parameters[2]).index);
			Assert.assertEquals(String, ParameterInfo(complexInstanceMethod.parameters[2]).type);
			Assert.assertEquals(true, ParameterInfo(complexInstanceMethod.parameters[2]).isOptional);

			var simpleStaticMethod:MethodInfo = MethodInfo(findByName(info.methods, "simpleStaticMethod"));
			Assert.assertEquals(KitchenSinkClass, simpleStaticMethod.declaringType);
			Assert.assertEquals("simpleStaticMethod", simpleStaticMethod.name);
			Assert.assertEquals(Void, simpleStaticMethod.returnType);
			Assert.assertEquals(0, simpleStaticMethod.parameters.length);
			Assert.assertEquals(true, simpleStaticMethod.isStatic);

			var complexStaticMethod:MethodInfo = MethodInfo(findByName(info.methods, "complexStaticMethod"));
			Assert.assertEquals(KitchenSinkClass, complexStaticMethod.declaringType);
			Assert.assertEquals("complexStaticMethod", complexStaticMethod.name);
			Assert.assertEquals(Object, complexStaticMethod.returnType);
			Assert.assertEquals(3, complexStaticMethod.parameters.length);
			Assert.assertEquals(true, complexStaticMethod.isStatic);
			
			Assert.assertEquals(0, ParameterInfo(complexStaticMethod.parameters[0]).index);
			Assert.assertEquals(int, ParameterInfo(complexStaticMethod.parameters[0]).type);
			Assert.assertEquals(false, ParameterInfo(complexStaticMethod.parameters[0]).isOptional);

			Assert.assertEquals(1, ParameterInfo(complexStaticMethod.parameters[1]).index);
			Assert.assertEquals(Any, ParameterInfo(complexStaticMethod.parameters[1]).type);
			Assert.assertEquals(false, ParameterInfo(complexStaticMethod.parameters[1]).isOptional);

			Assert.assertEquals(2, ParameterInfo(complexStaticMethod.parameters[2]).index);
			Assert.assertEquals(String, ParameterInfo(complexStaticMethod.parameters[2]).type);
			Assert.assertEquals(true, ParameterInfo(complexStaticMethod.parameters[2]).isOptional);
			
			Assert.assertEquals(6, info.properties.length);
			
			var readWriteInstanceProperty:PropertyInfo = PropertyInfo(findByName(info.properties, "readWriteInstanceProperty"));
			Assert.assertEquals(KitchenSinkClass, readWriteInstanceProperty.declaringType);
			Assert.assertEquals("readWriteInstanceProperty", readWriteInstanceProperty.name);
			Assert.assertEquals(int, readWriteInstanceProperty.type);
			Assert.assertEquals(true, readWriteInstanceProperty.isReadable);
			Assert.assertEquals(true, readWriteInstanceProperty.isWriteable);
			Assert.assertEquals(false, readWriteInstanceProperty.isStatic);
			
			var readOnlyInstanceProperty:PropertyInfo = PropertyInfo(findByName(info.properties, "readOnlyInstanceProperty"));
			Assert.assertEquals(KitchenSinkClass, readOnlyInstanceProperty.declaringType);
			Assert.assertEquals("readOnlyInstanceProperty", readOnlyInstanceProperty.name);
			Assert.assertEquals(int, readOnlyInstanceProperty.type);
			Assert.assertEquals(true, readOnlyInstanceProperty.isReadable);
			Assert.assertEquals(false, readOnlyInstanceProperty.isWriteable);
			Assert.assertEquals(false, readOnlyInstanceProperty.isStatic);

			var writeOnlyInstanceProperty:PropertyInfo = PropertyInfo(findByName(info.properties, "writeOnlyInstanceProperty"));
			Assert.assertEquals(KitchenSinkClass, writeOnlyInstanceProperty.declaringType);
			Assert.assertEquals("writeOnlyInstanceProperty", writeOnlyInstanceProperty.name);
			Assert.assertEquals(int, writeOnlyInstanceProperty.type);
			Assert.assertEquals(false, writeOnlyInstanceProperty.isReadable);
			Assert.assertEquals(true, writeOnlyInstanceProperty.isWriteable);
			Assert.assertEquals(false, writeOnlyInstanceProperty.isStatic);
			
			var readWriteStaticProperty:PropertyInfo = PropertyInfo(findByName(info.properties, "readWriteStaticProperty"));
			Assert.assertEquals(KitchenSinkClass, readWriteStaticProperty.declaringType);
			Assert.assertEquals("readWriteStaticProperty", readWriteStaticProperty.name);
			Assert.assertEquals(int, readWriteStaticProperty.type);
			Assert.assertEquals(true, readWriteStaticProperty.isReadable);
			Assert.assertEquals(true, readWriteStaticProperty.isWriteable);
			Assert.assertEquals(true, readWriteStaticProperty.isStatic);
			
			var readOnlyStaticProperty:PropertyInfo = PropertyInfo(findByName(info.properties, "readOnlyStaticProperty"));
			Assert.assertEquals(KitchenSinkClass, readOnlyStaticProperty.declaringType);
			Assert.assertEquals("readOnlyStaticProperty", readOnlyStaticProperty.name);
			Assert.assertEquals(int, readOnlyStaticProperty.type);
			Assert.assertEquals(true, readOnlyStaticProperty.isReadable);
			Assert.assertEquals(false, readOnlyStaticProperty.isWriteable);
			Assert.assertEquals(true, readOnlyStaticProperty.isStatic);

			var writeOnlyStaticProperty:PropertyInfo = PropertyInfo(findByName(info.properties, "writeOnlyStaticProperty"));
			Assert.assertEquals(KitchenSinkClass, writeOnlyStaticProperty.declaringType);
			Assert.assertEquals("writeOnlyStaticProperty", writeOnlyStaticProperty.name);
			Assert.assertEquals(int, writeOnlyStaticProperty.type);
			Assert.assertEquals(false, writeOnlyStaticProperty.isReadable);
			Assert.assertEquals(true, writeOnlyStaticProperty.isWriteable);
			Assert.assertEquals(true, writeOnlyStaticProperty.isStatic);			
		}
		
		private function findByName(array:Array, name:String):Object
		{
			for each (var element:Object in array)
				if (element.name == name)
					return element;
			
			Assert.fail("Did not find element with name " + name);
			return undefined;
		}
	}
}