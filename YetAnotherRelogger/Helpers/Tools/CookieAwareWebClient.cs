using System;
using System.ComponentModel;
using System.Net;

namespace YetAnotherRelogger.Helpers.Tools
{
    public class CookieAwareWebClient : WebClient
    {
        public CookieContainer Cookies { get; } = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = base.GetWebRequest(address);
            if (webRequest != null && webRequest.GetType() == typeof (HttpWebRequest))
            {
                ((HttpWebRequest) webRequest).CookieContainer = Cookies;
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