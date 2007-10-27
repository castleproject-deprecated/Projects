// Copyright 2006-2007 Ken Egozi http://www.kenegozi.com/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace TestModel
{
	using System;

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
