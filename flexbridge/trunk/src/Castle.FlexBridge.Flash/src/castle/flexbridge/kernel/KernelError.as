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

package castle.flexbridge.kernel
{
	/**
	 * Error thrown by the inversion of control container to indicate problems
	 * such as when a component cannot be resolved.
	 */
	public class KernelError extends Error
	{
		/**
		 * Creates a kernel error.
		 * @param message The message text.
		 */
		public function KernelError(message:String = "")
		{
			super(message);
		}
	}
}