// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveWriter.ServerExplorerSupport
{
    using EnvDTE;
    
    internal class OutputWindowHelper
    {
        private _DTE _dte;
        private OutputWindowPane _pane;

        public OutputWindowHelper(_DTE dte)
        {
            _dte = dte;
            _pane = GetOutputWindow("Output");
        }

        private OutputWindowPane GetOutputWindow(string windowName)
        {
            if (_dte == null || string.IsNullOrEmpty(windowName))
            {
                return null;
            }

            Window window = _dte.Windows.Item(Constants.vsext_wk_OutputWindow);
            OutputWindow output = (OutputWindow) window.Object;

            foreach (OutputWindowPane pane in output.OutputWindowPanes)
            {
                if (pane.Name == windowName)
                    return pane;
            }

            return output.OutputWindowPanes.Add(windowName);
        }

        public void Write(string message)
        {
            if (_pane != null)
            {
                _pane.Activate();
                _pane.OutputString(message);
                _pane.OutputString("\n");
            }
        }

        public void Log(string message)
        {
            Write(string.Format("ActiveWriter: {0}", message));
        }
    }
}