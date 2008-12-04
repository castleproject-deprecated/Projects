using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeGeneration.Adapters
{
    public class CommentAdapter : CodeCommentStatementCollection
    {
        public CommentAdapter(params string[] comments)
        {
            if (comments != null)
            {
                Array.ForEach(comments,
                              comment =>
                                  {
                                      if (!String.IsNullOrEmpty(comment))
                                      {
                                          Add(new CodeCommentStatement(comment, true));
                                      }
                                  });
            }

            if (Count > 0)
            {
                Insert(0, new CodeCommentStatement("<summary>", true));
                Add(new CodeCommentStatement("</summary>", true));
            }
        }
    }
}
