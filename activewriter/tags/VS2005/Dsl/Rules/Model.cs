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

namespace Altinoren.ActiveWriter
{
    using Microsoft.VisualStudio.Modeling;
    
    public delegate void ModelPropertyAddedHandler(ElementAddedEventArgs e);
    public delegate void ModelPropertyDeletedHandler(ElementDeletedEventArgs e);
    public delegate void ModelPropertyChangedHandler(ElementPropertyChangedEventArgs e);
    
    public partial class Model : ModelElement
    {
        public event ModelPropertyAddedHandler ModelPropertyAdded;
        public event ModelPropertyDeletedHandler ModelPropertyDeleted;
        public event ModelPropertyChangedHandler ModelPropertyChanged;
        
        public void OnModelPropertyAdded(ElementAddedEventArgs e)
        {
            if (ModelPropertyAdded != null)
                ModelPropertyAdded(e);
        }

        public void OnModelPropertyDeleted(ElementDeletedEventArgs e)
        {
            if (ModelPropertyDeleted != null)
                ModelPropertyDeleted(e);
        }
        
        public void OnModelPropertyChanged(ElementPropertyChangedEventArgs e)
        {
            if (ModelPropertyChanged != null)
                ModelPropertyChanged(e);
        }
    }
}
