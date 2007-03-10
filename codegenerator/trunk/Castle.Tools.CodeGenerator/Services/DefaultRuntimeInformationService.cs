using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public class DefaultRuntimeInformationService : IRuntimeInformationService
  {
    #region Member Data
    private ReaderWriterLock _lock = new ReaderWriterLock();
    private Dictionary<Type, Dictionary<MethodSignature, MethodInformation>> _methods = new Dictionary<Type, Dictionary<MethodSignature, MethodInformation>>();
    #endregion

    #region IRuntimeInformationService Members
    public MethodInformation ResolveMethodInformation(MethodSignature signature )
    {
      MethodInformation returned;

      _lock.AcquireReaderLock(-1);

      if (_methods.ContainsKey(signature.Type))
      {
        if (_methods[signature.Type].ContainsKey(signature))
        {
          returned = _methods[signature.Type][signature];
          _lock.ReleaseLock();
          return returned;
        }
      }

      _lock.UpgradeToWriterLock(-1);

      if (!_methods.ContainsKey(signature.Type))
      {
        _methods[signature.Type] = new Dictionary<MethodSignature, MethodInformation>();
      }

      MethodInfo method = signature.Type.GetMethod(signature.Name, BindingFlags.Public | BindingFlags.Instance, null, signature.Types, new ParameterModifier[0]);
      if (method == null)
      {
        _lock.ReleaseLock();
        throw new MissingMethodException(String.Format("Missing method: '{0}' on '{1}'", signature.Name, signature.Type.FullName));
      }
      returned = new MethodInformation(method, GetParameters(method), method.GetCustomAttributes(true));
      _methods[signature.Type][signature] = returned;

      _lock.ReleaseLock();

      return returned;
    }

    public MethodInformation ResolveMethodInformation(Type type, string name, Type[] types)
    {
      return ResolveMethodInformation(new MethodSignature(type, name, types));
    }
    #endregion

    #region Methods
    protected virtual ParameterInformation[] GetParameters(MethodInfo method)
    {
      List<ParameterInformation> parameters = new List<ParameterInformation>();
      foreach (ParameterInfo parameterInfo in method.GetParameters())
      {
        parameters.Add(new ParameterInformation(parameterInfo, parameterInfo.GetCustomAttributes(true)));
      }
      return parameters.ToArray();
    }
    #endregion
  }
}
