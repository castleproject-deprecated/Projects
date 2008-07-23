using System;
using System.Collections.Generic;
using Castle.Tools.CodeGenerator.Model.TreeNodes;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface ITreeCreationService
  {
    TreeNode Root { get; }
    TreeNode Peek { get; }
    void PushNode(TreeNode node);
    void PopNode();
    void PopToRoot();
    TreeNode FindNode(string name);
    void PushArea(string name);
  }
}
