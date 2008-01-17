using System;
using System.CodeDom;
using Castle.Tools.CodeGenerator.Model.TreeNodes;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface IGenerator
  {
    void Generate(TreeNode node);
  }
}