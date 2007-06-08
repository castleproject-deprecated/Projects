package
{
    import flexunit.framework.AssertionFailedError;
    import flexunit.framework.TestListener;
    import flexunit.framework.Test;
    import flexunit.framework.TestCase;
    import flexunit.framework.TestResult;
    import castle.flexbridge.common.AsyncTask;
    import flexunit.textui.ResultPrinter;

    public class TestReport implements TestListener
    {
        private var _testResult:TestResult;
        private var _totalTests:int;
        
        private var _currentTestIndex:int;
        private var _currentTestError:Error;
        
        private var _asyncTask:AsyncTask;
        
        [Bindable]
        public var report:String;
        
        public function runTests(testCase:Test):AsyncTask
        {
            _testResult = new TestResult();
            _testResult.addListener(this);
            _testResult.addListener(new ResultPrinter());
            _totalTests = testCase.countTestCases();
            
            _currentTestIndex = 0;
            _currentTestError = null;
            report = "";
            
            return AsyncTask.start(function(task:AsyncTask):void
            {
                _asyncTask = task;
    
                if (_totalTests == 0)
                    done();
                else
                    testCase.runWithResult(_testResult);
            });
        }
        
        public function addError(test:Test, error:Error):void
        {
            _currentTestError = error;
        }
        
        public function addFailure(test:Test, error:AssertionFailedError):void
        {
            _currentTestError = error;
        }
        
        public function startTest(test:Test):void
        {
            _currentTestError = null;
            _currentTestIndex += 1;
        }

        public function endTest(test:Test):void
        {
            report += _currentTestIndex + ") ";
            
            if (_currentTestError == null)
            {
                report += "[PASS] " + test.toString() + "\n\n";
            }
            else
            {
                if (_currentTestError is AssertionFailedError)
                    report += "[FAIL] " + test.toString() + "\n";
                else
                    report += "[ERROR] " + test.toString() + "\n";
                    
                report += _currentTestError.getStackTrace();
                report += "\n\n";
                
                _currentTestError = null;
            }
            
            if (_currentTestIndex == _totalTests)
                done();
        }
        
        private function done():void
        {
            report += "\nSummary: " + _totalTests + " total, "
                + _testResult.failureCount() + " failures, "
                + _testResult.errorCount() + "errors\n";
            
            if (_testResult.errorCount() != 0 || _testResult.failureCount() != 0)
                report += "FAILURES!\n";
            else
                report += "OK.\n";
            
            _asyncTask.done(this);
        }
    }
}