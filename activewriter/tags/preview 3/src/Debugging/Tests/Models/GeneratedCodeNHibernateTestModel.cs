﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Debugging.Tests {
    using System;
    using System.Collections.Generic;
    using System.Collections;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Altinoren.ActiveWriter.CustomTool.ActiveWriterTemplatedCodeGenerator", "1.0.0.0")]
    [System.Diagnostics.DebuggerDisplay("blog_name = {1}")]
    public partial class NHBlog {
        
        private string _blog_name;
        
        private int _blog_id;
        
        private IList<NHPost> _posts;
        
        public string blog_name {
            get {
                return this._blog_name;
            }
            set {
                this._blog_name = value;
            }
        }
        
        public int blog_id {
            get {
                return this._blog_id;
            }
            set {
                this._blog_id = value;
            }
        }
        
        public IList<NHPost> Posts {
            get {
                return this._posts;
            }
            set {
                this._posts = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Altinoren.ActiveWriter.CustomTool.ActiveWriterTemplatedCodeGenerator", "1.0.0.0")]
    public partial class NHPost {
        
        private string _post_title;
        
        private string _post_contents;
        
        private string _post_category;
        
        private System.Nullable<System.DateTime> _post_created;
        
        private System.Nullable<bool> _post_published;
        
        private int _post_id;
        
        private NHBlog _nHBlog;
        
        public string post_title {
            get {
                return this._post_title;
            }
            set {
                this._post_title = value;
            }
        }
        
        public string post_contents {
            get {
                return this._post_contents;
            }
            set {
                this._post_contents = value;
            }
        }
        
        public string post_category {
            get {
                return this._post_category;
            }
            set {
                this._post_category = value;
            }
        }
        
        public System.Nullable<System.DateTime> post_created {
            get {
                return this._post_created;
            }
            set {
                this._post_created = value;
            }
        }
        
        public System.Nullable<bool> post_published {
            get {
                return this._post_published;
            }
            set {
                this._post_published = value;
            }
        }
        
        public int post_id {
            get {
                return this._post_id;
            }
            set {
                this._post_id = value;
            }
        }
        
        public NHBlog NHBlog {
            get {
                return this._nHBlog;
            }
            set {
                this._nHBlog = value;
            }
        }
    }
}