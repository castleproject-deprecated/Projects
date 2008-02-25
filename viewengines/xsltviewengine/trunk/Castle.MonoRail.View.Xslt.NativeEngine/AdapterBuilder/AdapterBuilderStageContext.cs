using System.Xml.XPath;
using System.Collections;
using System;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using System.Collections.Generic;
using System.Reflection;

namespace Castle.MonoRail.View.Xslt.NativeEngine.AdapterBuilder
{
	/// <summary>
	/// Class holding context information required by the
	/// various stage responsible for building the adapter.
	/// </summary>
	public class AdapterBuilderStageContext
	{
		/// <summary>
		/// Initializes a new instance of the AdapterBuilderContext class.
		/// </summary>
		/// <param name="originalObject"></param>
		/// <param name="classEmitter"></param>
		/// <param name="adaptedObjectRef"></param>
		public AdapterBuilderStageContext(Type originalObject, ClassEmitter classEmitter, FieldReference adaptedObjectRef, MethodInfo originalMethod)
		{
			OriginalObject = originalObject;
			ClassEmitter = classEmitter;
			AdaptedObjectRef = adaptedObjectRef;
			OriginalMethod = originalMethod;
		}

		private Type _OriginalObject;
		public Type OriginalObject
		{
			get { return _OriginalObject; }
			private set { _OriginalObject = value; }
		}

		private ClassEmitter _ClassEmitter;
		public ClassEmitter ClassEmitter
		{
			get { return _ClassEmitter; }
			private set { _ClassEmitter = value; }
		}

		private FieldReference _AdaptedObjectRef;
		public FieldReference AdaptedObjectRef
		{
			get { return _AdaptedObjectRef; }
			private set { _AdaptedObjectRef = value; }
		}

		private MethodInfo _OriginalMethod;
		public MethodInfo OriginalMethod
		{
			get { return _OriginalMethod; }
			private set { _OriginalMethod = value; }
		}

		private MethodEmitter _NewMethodEmitter;
		public MethodEmitter NewMethodEmitter
		{
			get { return _NewMethodEmitter; }
			set { _NewMethodEmitter = value; }
		}

		private IDictionary<string, object> _StageCustomData = new Dictionary<string, object>();
		public object this[string key]
		{
			get
			{
				return _StageCustomData[key];
			}
			set
			{
				_StageCustomData[key] = value;
			}
		}

	}
}
