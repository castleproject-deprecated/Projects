namespace Castle.MonoRail.Rest.Mime
{
    using System.Collections.Generic;

    public class MimeType
    {
        private List<string> _extensionSynonyms;
        private string _mimeString;
        private string _symbol;
        private List<string> _synonyms;

        public MimeType()
        {
        }

        public MimeType(string mimeString, string symbol, List<string> synonyms, List<string> extensionSynonyms)
        {
            _mimeString = mimeString;
            _symbol = symbol;
            _synonyms = synonyms;
            _extensionSynonyms = extensionSynonyms;
        }

        public string MimeString
        {
            get { return _mimeString; }
            set { _mimeString = value; }
        }

        public string Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        public List<string> Synonyms
        {
            get { return _synonyms; }
            set { _synonyms = value; }
        }

        public List<string> ExtensionSynonyms
        {
            get { return _extensionSynonyms; }
            set { _extensionSynonyms = value; }
        }
    }

    public class MimeTypes : List<MimeType>
    {
        public MimeTypes() : base(15)
        {
        }

        public void RegisterBuiltinTypes()
        {
            //copied from rails mime_types

            Register("*/*", "all");
            Register("text/plain", "text", null, new string[] {"txt"});
            Register("text/html", "html", new string[] {"application/xhtml+xml"}, new string[] {"xhtml"});
            Register("text/javascript", "js", new string[] {"application/javascript", "application/x-javascript"});
            Register("text/css", "css");
            Register("text/calendar", "ics");
            Register("text/csv", "csv");
            Register("application/xml", "xml", new string[] {"text/xml", "application/x-xml"});
            Register("application/rss+xml", "rss");
            Register("application/atom+xml", "atom");
            Register("application/x-yaml", "yaml", new string[] {"text/yaml"});
            Register("multipart/form-data", "multipart_form");
            Register("application/x-www-form-urlencoded", "url_encoded_form");

            //http://www.ietf.org/rfc/rfc4627.txt
            Register("application/json", "json", new string[] {"text/x-json"});
        }

        public void Register(string mimeString, string symbol)
        {
            Register(mimeString, symbol, null, null);
        }

        public void Register(string mimeString, string symbol, IEnumerable<string> synonyms)
        {
            Register(mimeString, symbol, synonyms, null);
        }

        public void Register(string mimeString, string symbol, IEnumerable<string> synonyms,
                             IEnumerable<string> extensionSynonyms)
        {
            List<string> synList = new List<string>(), extentionList = new List<string>();

            if (synonyms != null)
            {
                synList.AddRange(synonyms);
            }
            if (extensionSynonyms != null)
            {
                extentionList.AddRange(extensionSynonyms);
            }

            Add(new MimeType(mimeString, symbol, synList, extentionList));
        }
    }
}