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

namespace Castle.ActiveWriter.Rules
{
    using Microsoft.VisualStudio.Modeling;

    [RuleOn(typeof(ModelProperty), FireTime = TimeToFire.TopLevelCommit)]
    public sealed class ModelPropertyDeletingRule : DeletingRule
    {
        public override void ElementDeleting(ElementDeletingEventArgs e)
        {
            base.ElementDeleting(e);

            ModelProperty property = e.ModelElement as ModelProperty;
            // TODO: Add nested support
            if (property != null && property.ModelClass != null)
            {
                Transaction transaction = property.Store.TransactionManager.CurrentTransaction.TopLevelTransaction;

                if (transaction != null && !transaction.IsSerializing && !transaction.Context.ContextInfo.ContainsKey(property.Name))
                    // We're adding the model element to the context so property deleted event wil use it to fire the ondeleted event
                    // on the Model class
                    transaction.Context.ContextInfo.Add(property.Name, property.ModelClass.Model);
            }
        }
    }
}
