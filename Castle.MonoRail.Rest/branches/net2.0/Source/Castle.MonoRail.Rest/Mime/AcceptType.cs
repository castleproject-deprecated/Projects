using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Castle.MonoRail.Rest.Mime
{
    public class AcceptType
    {
        private string _name;
        private int _order;
        private float _q;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }

        public float Q
        {
            get { return _q; }
            set { _q = value; }
        }

        public static MimeType[] Parse(string acceptHeader, MimeTypes mimes)
        {
            
            string[] splitHeaders = acceptHeader.Split(',');
            List<AcceptType> acceptTypes = new List<AcceptType>(splitHeaders.Length);

            for (int i = 0; i < splitHeaders.Length; i++)
            {
                string[] parms = splitHeaders[i].Split(';');
                AcceptType at = new AcceptType();
                at.Name = parms[0];
                at.Order = i;

                at.Q = parms.Length == 2 ? Convert.ToSingle(parms[1].Substring(2)) : 1;
                acceptTypes.Add(at);
            }

            AcceptType appXml = acceptTypes.Find( delegate( AcceptType at) { return at.Name == "application/xml"; } );
            if (appXml != null)
            {
                Regex regEx = new Regex(@"\+xml$");

                int appXmlIndex;
                int idx = appXmlIndex = acceptTypes.IndexOf(appXml);

                while (idx < acceptTypes.Count)
                {
                    AcceptType at = acceptTypes[idx];
                    if (at.Q < appXml.Q)
                    {
                        break;
                    }
                    
                    if(regEx.IsMatch(at.Name)) {
                        acceptTypes.Remove(at);
                        acceptTypes.Insert(appXmlIndex,at);
                        appXmlIndex++;
                    }
                    idx++;
                }                
            }
                                                                                                        
            List<MimeType> returnTypes = new List<MimeType>();
            foreach (AcceptType type in acceptTypes.OrderByDescending(at => at.Q))
            {
                returnTypes.AddRange(mimes.Where(m => m.MimeString == type.Name || m.Synonyms.Contains(type.Name)));                
            }


            return returnTypes.Distinct().ToArray();
        }
    }
}
