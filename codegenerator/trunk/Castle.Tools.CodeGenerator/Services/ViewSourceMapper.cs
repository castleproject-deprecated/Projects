using System;
using System.Collections.Generic;
using System.IO;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public class ViewSourceMapper : IViewSourceMapper
  {
    #region Member Data
    private ILogger _logger;
    private ITreeCreationService _treeService;
    private INamingService _naming;
    private string viewDirectory = @"Views";
    private string componentsDirectory = @"Components";
    #endregion

    #region ViewSourceMapper()
    public ViewSourceMapper(ILogger logger, ITreeCreationService treeService, INamingService naming)
    {
      _logger = logger;
      _treeService = treeService;
      _naming = naming;
    }
    #endregion

    #region Methods
    public void AddViewSource(string path)
    {
      string viewName = Path.GetFileNameWithoutExtension(path);
      TreeNode node = null;
      foreach (string part in BreakPath(path))
      {
        string controllerName = part + "Controller";
        node = _treeService.FindNode(controllerName);
        if (node == null)
        {
          string viewComponentName = part + "Component";
          node = _treeService.FindNode(viewComponentName);
          if (node == null)
          {
            node = _treeService.FindNode(part);
            if (node == null)
            {
              continue;
            }
          }
        }
        _treeService.PushNode(node);
      }
      _treeService.PopToRoot();
      if (node == null)
      {
        _logger.LogInfo("Unable to map view: {0}", path);
        return;
      }

      node.AddChild(new ViewTreeNode(viewName));
    }
    #endregion

    #region Methods
    private string[] BreakPath(string path)
    {
      List<string> parts = new List<string>(Path.GetDirectoryName(path).Split('\\', '/'));
      while (parts.Count > 0)
      {
        if (parts[0] == viewDirectory)
        {
          parts.RemoveAt(0);
          return parts.ToArray();
        }
        parts.RemoveAt(0);
      }
      throw new ArgumentException("No Views directory in view path: " + path);
    }
    #endregion
  }
}
