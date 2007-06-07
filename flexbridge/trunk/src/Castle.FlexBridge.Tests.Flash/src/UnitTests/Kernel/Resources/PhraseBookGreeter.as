package UnitTests.Kernel.Resources
{
	public class PhraseBookGreeter implements IGreeter
	{
		private var _phraseBook:IPhraseBook;
		
		public function PhraseBookGreeter(phraseBook:IPhraseBook)
		{
			_phraseBook = phraseBook;
		}
		
		public function greet(name:String):String
		{
			return _phraseBook.hello + ", " + name + "!";
		}
	}
}