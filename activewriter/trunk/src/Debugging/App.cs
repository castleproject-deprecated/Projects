using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Configuration;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;

namespace Debugging
{
    class Class1
    {
        public static void Main()
        {
            IConfigurationSource source = System.Configuration.ConfigurationManager.GetSection("activerecord") as IConfigurationSource;
            log4net.Config.XmlConfigurator.Configure();
            ActiveRecordStarter.Initialize(source, typeof(Blog), typeof(Post));

            Post.DeleteAll();
            Blog.DeleteAll();

            Blog blog = new Blog();
            blog.blog_name = "Test";
            blog.Create();
            Console.WriteLine("Created ID: {0}", blog.blog_id);

            Blog oldBlog = Blog.Find(blog.blog_id);

            Post post = new Post();
            post.post_category = ".Net";
            post.post_contents = "Content 123";
            post.post_created = DateTime.Now;

            post.Blog = oldBlog;
            post.Save();
            oldBlog.Posts.Add(post);

            oldBlog.Save();

            Console.WriteLine("Created Post ID: {0}", post.post_id);

            IList<Post> posts = oldBlog.Posts;
            Console.WriteLine("Post count: {0}", posts.Count);


            Console.ReadLine();
        }
    }
}
