// Copyright 2006 Gokhan Altinoren - http://altinoren.com/
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

namespace Debugging.Tests
{
    using System;
    using System.Configuration;
    using NUnit.Framework;
    using Castle.ActiveRecord;
    using Castle.ActiveRecord.Framework;

    [TestFixture]
    public class GeneratedCodeRuntimeTest
    {
        #region Setup and Helpers

        protected IConfigurationSource GetConfigSource()
        {
            return ConfigurationManager.GetSection("activerecord") as IConfigurationSource;
        }

        protected void Recreate()
        {
            ActiveRecordStarter.CreateSchema();
        }

        [SetUp]
        public void Init()
        {
            ActiveRecordStarter.ResetInitializationFlag();
        }

        [TearDown]
        public void Drop()
        {
            try
            {
                ActiveRecordStarter.DropSchema();
            }
            catch (Exception)
            {

            }
        }

        #endregion

        [Test]
        public void DoesGeneratedCodeWork()
        {
            ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Blog), typeof(Post));
            Recreate();

            Blog[] blogs = Blog.FindAll();

            Assert.IsNotNull(blogs);
            Assert.AreEqual(0, blogs.Length);

            Blog blog = new Blog();
            blog.Name = "hammett's blog";
            blog.Author = "hamilton verissimo";
            blog.Save();

            blogs = Blog.FindAll();
            Assert.IsNotNull(blogs);
            Assert.AreEqual(1, blogs.Length);

            Blog retrieved = blogs[0];
            Assert.IsNotNull(retrieved);

            Assert.AreEqual(blog.Name, retrieved.Name);
            Assert.AreEqual(blog.Author, retrieved.Author);

            Post post = new Post();
            post.Category = "Castle";
            post.Content = "The content.";
            post.Title = "The title.";
            post.Blog = retrieved;
            post.Save();
            retrieved.Posts.Add(post);
            retrieved.Save();

            Blog retrievedHasMany = Blog.FindAll()[0];
            Assert.AreEqual(retrievedHasMany.Posts.Count, 1);
            Assert.AreEqual(retrievedHasMany.Posts[0].Id, post.Id);
        }
    }
}
