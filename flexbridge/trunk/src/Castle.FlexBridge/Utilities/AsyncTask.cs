// Copyright 2007 Castle Project - http://www.castleproject.org/
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Castle.FlexBridge.Utilities;

namespace Castle.FlexBridge.Gateway.RequestProcessors
{
    /// <summary>
    /// Processes a task asynchronously.
    /// </summary>
    public abstract class AsyncTask : EventAsyncResult
    {
        private IEnumerator<Step> steps;
        private Step currentStep;
        private Exception exception;
        private bool inMoveNext;
        private bool pendingResume;

        /// <summary>
        /// Creates an async result.
        /// </summary>
        /// <param name="callback">The async callback</param>
        /// <param name="asyncState">The async state</param>
        protected AsyncTask(AsyncCallback callback, object asyncState)
            : base(callback, asyncState)
        {
            currentStep = new Step("Initializing the task.");
        }

        /// <summary>
        /// Starts processing the task.
        /// </summary>
        public virtual void BeginTask()
        {
            OnBeginTask();

            steps = GetSteps().GetEnumerator();
            Resume();
        }

        /// <summary>
        /// Waits for the task to complete and performs any final processing.
        /// If an exception occurred while the task was being processed, it is rethrown here.
        /// </summary>
        public virtual void EndTask()
        {
            WaitForCompletion();

            OnEndTask(exception);

            if (exception != null)
                throw new AsyncTaskFailedException(String.Format(CultureInfo.CurrentCulture,
                    "An exception occurred while processing step '{0}'.", currentStep.Description));
        }

        /// <summary>
        /// Called when <see cref="BeginTask" /> is called, before any steps are executed.
        /// </summary>
        protected virtual void OnBeginTask()
        {
        }

        /// <summary>
        /// Called when <see cref="EndTask" /> is called, after all steps have executed.
        /// </summary>
        /// <param name="exception">The exception that was thrown during request processing, or null if none</param>
        protected virtual void OnEndTask(Exception exception)
        {
        }

        /// <summary>
        /// Gets the enumeration of steps to perform.
        /// The first step will be executed synchronously.  All subsequent steps must be
        /// triggered by a call to <see cref="Resume" />.
        /// </summary>
        /// <returns>The enumeration of steps</returns>
        protected abstract IEnumerable<Step> GetSteps();

        /// <summary>
        /// Processes the next step of the task.
        /// If no steps remain or if an error occurs, the task is signaled as completed.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="Resume" /> was
        /// called when the task was not running</exception>
        protected void Resume()
        {
            if (steps == null)
                throw new InvalidOperationException("Resume should only be called while the task is running.");

            if (inMoveNext)
            {
                pendingResume = true;
                return;
            }

            try
            {
                for (; ; )
                {
                    try
                    {
                        inMoveNext = true;
                        if (!steps.MoveNext())
                        {
                            currentStep = Step.Finished;
                            break;
                        }
                    }
                    finally
                    {
                        inMoveNext = false;
                    }

                    currentStep = steps.Current;
                    if (currentStep.IsFinished)
                        break;

                    Debug.WriteLine(currentStep.Description);

                    // If we received a synchronous resumption, then loop again
                    if (pendingResume)
                        pendingResume = false;
                    else
                        return;
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            steps = null;
            SignalCompletion(false);
        }

        /// <summary>
        /// Calls <see cref="Resume" />.
        /// Intended for use as an <see cref="AsyncCallback" />.
        /// </summary>
        /// <param name="result">Ignored</param>
        protected void OnAsyncResultCallbackResume(IAsyncResult result)
        {
            Resume();
        }

        /// <summary>
        /// Tracks execution steps for debugging purposes.
        /// </summary>
        protected struct Step
        {
            private string description;

            /// <summary>
            /// A step that indicates that processing is finished.
            /// </summary>
            public static readonly Step Finished = new Step(false);

            private Step(bool dummy)
            {
                this.description = null;
            }

            /// <summary>
            /// Creates a step.
            /// </summary>
            /// <param name="description">The description of the step for debugging purposes</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="description"/> is null</exception>
            public Step(string description)
            {
                if (description == null)
                    throw new ArgumentNullException("description");

                this.description = description;
            }

            /// <summary>
            /// Returns true if the step indicates that processing is finished.
            /// </summary>
            public bool IsFinished
            {
                get { return description == null; }
            }

            /// <summary>
            /// Gets the description of the step.
            /// </summary>
            public string Description
            {
                get { return description; }
            }
        }
    }
}
