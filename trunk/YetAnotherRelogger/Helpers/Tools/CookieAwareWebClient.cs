using System;
using System.ComponentModel;
using System.Net;

namespace YetAnotherRelogger.Helpers.Tools
{
    public class CookieAwareWebClient : WebClient
    {
        private readonly CookieContainer _cookies = new CookieContainer();

        public CookieContainer Cookies
        {
            get { return _cookies; }
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest webRequest = base.GetWebRequest(address);
            if (webRequest != null && webRequest.GetType() == typeof (HttpWebRequest))
            {
                ((HttpWebRequest) webRequest).CookieContainer = _cookies;
            }
            return webRequest;
        }

        private void InitializeComponent()
        {
            var resources = new ComponentResourceManager(typeof (CookieAwareWebClient));
            Headers = ((WebHeaderCollection) (resources.GetObject("$this.Headers")));
        }
    }
}