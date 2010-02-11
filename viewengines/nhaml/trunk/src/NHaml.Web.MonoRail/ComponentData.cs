using System;
using System.Collections.Generic;
using System.IO;

namespace NHaml.Web.MonoRail
{
    public class ComponentData
    {
        public NHamlMonoRailView View { get; set; }
        public string Name { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
        public Dictionary<string, Action<TextWriter>> Sections{ get; set;}

        public ComponentData(NHamlMonoRailView view, string name, IDictionary<string, object> parameters):this(view,name, parameters, null)
        {
        }
        public ComponentData(NHamlMonoRailView view, string name, IDictionary<string, object> parameters, Action<TextWriter> action)
        {
            Body = action;
            View = view;
            Name = name;
            Parameters = parameters;
            Sections = new Dictionary<string, Action<TextWriter>>();
        }

        public Action<TextWriter> Body { get; set; }

        public ComponentData AddSection(string sectionName, Action<TextWriter> action)
        {
            Sections.Add(sectionName,action);
            return this;
        }

        public void Render()
        {
            View.RenderComponent(this);
        }
    }
}