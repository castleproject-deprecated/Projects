// Copyright 2006 Gokhan Castle - http://altinoren.com/
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

namespace Castle.ActiveWriter.ToolWindow
{
    using System;
    using System.Collections;
    using Microsoft.VisualStudio.Modeling;
    using System.Windows.Forms;
    using Microsoft.VisualStudio.Modeling.Shell;
    using System.Runtime.InteropServices;

    [Guid(Constants.ActiveWriterClassDetailsToolWindowId)]
    internal class ActiveWriterClassDetailsToolWindow : ToolWindow
    {
        private ClassDetailsControl control;

        public ActiveWriterClassDetailsToolWindow(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override IWin32Window Window
        {
            get { return control; }
        }

        public override string WindowTitle
        {
            get { return "ActiveWriter Class Details"; }
        }

        protected override int BitmapResource
        {
            get { return 104; }
        }

        protected override int BitmapIndex
        {
            get { return 0; }
        }

        protected override void OnToolWindowCreate()
        {
            control = new ClassDetailsControl();
            control.Clear();
        }

        protected override void OnDocumentWindowChanged(ModelingDocView oldView, ModelingDocView newView)
        {
            base.OnDocumentWindowChanged(oldView, newView);

            ModelingDocData data1 = (oldView != null) ? oldView.DocData : null;
            if ((data1 != null) && data1 is ActiveWriterDocData)
            {
                oldView.SelectionChanged -= OnDocumentSelectionChanged;

                Model model = (Model)data1.RootElement;
                model.ModelPropertyAdded -= OnModelPropertyAdded;
                model.ModelPropertyDeleted -= OnModelPropertyDeleted;
                model.ModelPropertyChanged -= OnModelPropertyChanged;
            }

            ModelingDocData data2 = (newView != null) ? newView.DocData : null;
            if ((data2 != null) && data2 is ActiveWriterDocData)
            {
                newView.SelectionChanged += OnDocumentSelectionChanged;

                Model model = (Model)data2.RootElement;
                model.ModelPropertyAdded += OnModelPropertyAdded;
                model.ModelPropertyDeleted += OnModelPropertyDeleted;
                model.ModelPropertyChanged += OnModelPropertyChanged;
            }

            OnDocumentSelectionChanged(data2, EventArgs.Empty);
        }

        public void OnModelPropertyAdded(ElementAddedEventArgs e)
        {
            ModelProperty property = e.ModelElement as ModelProperty;
            if (property != null)
                control.Display(property.ModelClass);
        }

        public void OnModelPropertyDeleted(ElementDeletedEventArgs e)
        {
            control.Clear();
        }

        public void OnModelPropertyChanged(ElementPropertyChangedEventArgs e)
        {
            ModelProperty property = e.ModelElement as ModelProperty;
            if (property != null)
                control.Display(property.ModelClass);
        }

        public void OnDocumentSelectionChanged(object sender, EventArgs e)
        {
            if (sender != null)
            {
                ModelingDocView view = sender as ModelingDocView;
                if (view != null)
                {
                    ICollection selection = view.GetSelectedComponents();
                    if (selection.Count == 1)
                    {
                        IEnumerator enumerator = selection.GetEnumerator();
                        enumerator.MoveNext();
                        ClassShape shape = enumerator.Current as ClassShape;
                        if (shape != null)
                        {
                            ModelClass modelClass = shape.ModelElement as ModelClass;
                            if (modelClass != null)
                            {
                                control.Display(modelClass);
                                return;
                            }
                        }

                        ModelProperty property = enumerator.Current as ModelProperty;
                        if (property != null)
                        {
                            control.Display(property.ModelClass);
                            return;
                        }
                    }
                }

                control.Clear();
            }
        }
    }
}