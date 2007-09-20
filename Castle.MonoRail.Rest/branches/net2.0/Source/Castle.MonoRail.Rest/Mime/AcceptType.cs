namespace Castle.MonoRail.Rest.Mime
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

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

            for(int i = 0; i < splitHeaders.Length; i++)
            {
                string[] parms = splitHeaders[i].Split(';');
                AcceptType at = new AcceptType();
                at.Name = parms[0];
                at.Order = i;

                at.Q = parms.Length == 2 ? Convert.ToSingle(parms[1].Substring(2)) : 1;
                acceptTypes.Add(at);
            }

            AcceptType appXml = acceptTypes.Find(delegate(AcceptType at) { return at.Name == "application/xml"; });
            if (appXml != null)
            {
                Regex regEx = new Regex(@"\+xml$");

                int appXmlIndex;
                int idx = appXmlIndex = acceptTypes.IndexOf(appXml);

                while(idx < acceptTypes.Count)
                {
                    AcceptType at = acceptTypes[idx];
                    if (at.Q < appXml.Q)
                    {
                        break;
                    }

                    if (regEx.IsMatch(at.Name))
                    {
                        acceptTypes.Remove(at);
                        acceptTypes.Insert(appXmlIndex, at);
                        appXmlIndex++;
                    }
                    idx++;
                }
            }

            List<MimeType> returnTypes = new List<MimeType>();
            acceptTypes.Sort(new Comparison<AcceptType>(descendingAcceptTypes));
            foreach(AcceptType type in acceptTypes)
            {
//                returnTypes.AddRange(mimes.Where(m => m.MimeString == type.Name || m.Synonyms.Contains(type.Name)));                
                returnTypes.AddRange(mimes.FindAll(delegate(MimeType m)
                                                   {
                                                       return
                                                           m.MimeString == type.Name ||
                                                           m.Synonyms.Contains(type.Name);
                                                   }));
            }

            //return returnTypes.Distinct().ToArray();
			return returnTypes.ToArray();
        }

        private static int descendingAcceptTypes(AcceptType x, AcceptType y)
        {
        	return Comparer<float>.Default.Compare(x.Q, y.Q);
//
//            if (x == null)
//            {
//                return (y == null ? 0 : -1);
//            }
//            else
//            {
//                if (y == null)
//                {
//                    return 1;
//                }
//                return x.Q.CompareTo(y.Q);
//            }
        }
    }
}