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
using System.Text;
using System.Threading;

namespace Castle.FlexBridge.Utilities
{
    /// <summary>
    /// An event-based implementation of AsyncResult.
    /// The callback is invoked and the waithandle is signaled automatically when
    /// <see cref="SignalCompletion" />.
    /// </summary>
    public class EventAsyncResult : IAsyncResult
    {
        private AsyncCallback callback;
        private object asyncState;

        private bool isCompleted;
        private EventWaitHandle asyncWaitHandle;
        private bool completedSynchronously;

        /// <summary>
        /// Creates an async result.
        /// </summary>
        /// <param name="callback">The async callback</param>
        /// <param name="asyncState">The async state</param>
        public EventAsyncResult(AsyncCallback callback, object asyncState)
        {
            this.callback = callback;
            this.asyncState = asyncState;
        }

        ///<summary>
        ///Gets an indication whether the asynchronous operation has completed.
        ///</summary>
        ///<returns>
        ///true if the operation is complete; otherwise, false.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public bool IsCompleted
        {
            get { return isCompleted; }
        }

        ///<summary>
        ///Gets a <see cref="T:System.Threading.WaitHandle"></see> that is used to wait for an asynchronous operation to complete.
        ///</summary>
        ///<returns>
        ///A <see cref="T:System.Threading.WaitHandle"></see> that is used to wait for an asynchronous operation to complete.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                lock (this)
                {
                    if (asyncWaitHandle == null)
                        asyncWaitHandle = new EventWaitHandle(isCompleted, EventResetMode.ManualReset);

                    return asyncWaitHandle;
                }
            }
        }

        ///<summary>
        ///Gets a user-defined object that qualifies or contains information about an asynchronous operation.
        ///</summary>
        ///<returns>
        ///A user-defined object that qualifies or contains information about an asynchronous operation.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public object AsyncState
        {
            get { return asyncState; }
        }

        ///<summary>
        ///Gets an indication of whether the asynchronous operation completed synchronously.
        ///</summary>
        ///<returns>
        ///true if the asynchronous operation completed synchronously; otherwise, false.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public bool CompletedSynchronously
        {
            get { return completedSynchronously; }
        }

        /// <summary>
        /// Sets the state of the <see cref="IAsyncResult" /> to completed and signals
        /// the callback and waithandle.
        /// </summary>
        /// <param name="completedSynchronously">True if the operation completed synchronously</param>
        /// <exception cref="InvalidOperationException">Thrown if the async result has already signaled completion</exception>
        public void SignalCompletion(bool completedSynchronously)
        {
            lock (this)
            {
                if (isCompleted)
                    throw new InvalidOperationException("The async result has already signaled completion.");

                this.isCompleted = true;
                this.completedSynchronously = completedSynchronously;

                if (asyncWaitHandle != null)
                    asyncWaitHandle.Set();
            }

            if (callback != null)
                callback(this);
        }

        /// <summary>
        /// Waits for completion.
        /// </summary>
        public void WaitForCompletion()
        {
            if (!isCompleted)
                AsyncWaitHandle.WaitOne();
        }
    }
}
