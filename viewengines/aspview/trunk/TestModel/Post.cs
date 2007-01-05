using System;
using System.Collections.Generic;
using System.Text;

namespace TestModel
{
    public class Post
    {
        private DateTime _publishDate;

        public DateTime PublishDate
        {
            get { return _publishDate; }
            set { _publishDate = value; }
        }

        private string _content;

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

    }
}
