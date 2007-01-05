using System;
using System.Collections.Generic;
using System.Text;

namespace TestModel
{
    public class Blog
    {
        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private Post[] _posts;

        public Post[] Posts
        {
            get { return _posts; }
            set { _posts = value; }
        }

    }
}
