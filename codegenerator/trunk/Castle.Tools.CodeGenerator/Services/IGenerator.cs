using System;
using System.CodeDom;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public interface IGenerator
  {
    void Generate(TreeNode node);
  }
}