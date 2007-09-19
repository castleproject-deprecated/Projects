using System;
using Castle.MonoRail.Framework;
using Castle.Core;
using Castle.MonoRail.Framework.Services;

namespace Castle.MonoRail.Rest
{
    public class DefaultUrlProvider : IServiceEnabledComponent
    {
        private IControllerTree controllerTree;
        private IUrlTokenizer urlTokenizer;


        #region IServiceEnabledComponent Members

        public IUrlTokenizer Tokenizer
        {
            get { return urlTokenizer; }
            set { urlTokenizer = value; }
        }

        public void Service(IServiceProvider provider)
        {
            controllerTree = (IControllerTree)provider.GetService(typeof(IControllerTree));
            urlTokenizer = (IUrlTokenizer)provider.GetService(typeof(IUrlTokenizer));


            
            if (controllerTree != null && urlTokenizer != null)
            {
                controllerTree.ControllerAdded += this.ControllerAddedToTree;
            }
        }

        #endregion

        public void ControllerAddedToTree(object sender, ControllerAddedEventArgs args)
        {

            //If in a virtual directory I need to prepend a slash

            string url = args.Area + "/" + args.ControllerName + ".rails";
            urlTokenizer.AddDefaultRule(url, args.Area, args.ControllerName, "collection");
            urlTokenizer.AddDefaultRule("/" + url, args.Area, args.ControllerName, "collection");
        }
    }
}
