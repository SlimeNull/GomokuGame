using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace FlaskSharp
{
    public class HttpHeaders : Dictionary<string, string>
    {
        public HttpHeaders()
        {
        }

        public HttpHeaders(IDictionary<string, string> dictionary) : base(dictionary)
        {
        }

        public HttpHeaders(IEqualityComparer<string>? comparer) : base(comparer)
        {
        }

        public HttpHeaders(int capacity) : base(capacity)
        {
        }

        public HttpHeaders(IDictionary<string, string> dictionary, IEqualityComparer<string>? comparer) : base(dictionary, comparer)
        {
        }

        public HttpHeaders(int capacity, IEqualityComparer<string>? comparer) : base(capacity, comparer)
        {
        }

        protected HttpHeaders(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

#if NET6_0_OR_GREATER

        public HttpHeaders(IEnumerable<KeyValuePair<string, string>> collection) : base(collection)
        {
        }

        public HttpHeaders(IEnumerable<KeyValuePair<string, string>> collection, IEqualityComparer<string>? comparer) : base(collection, comparer)
        {
        }

#endif

        public void AddValue(string key, string value)
        {
            if (TryGetValue(key, out string? headerStr))
            {
                StringBuilder sb = new StringBuilder(headerStr);
                if (!string.IsNullOrWhiteSpace(headerStr))
                    sb.Append(", ");
                sb.Append(value);

                this[key] = sb.ToString();
            }
            else
            {
                this[key] = value;
            }
        }

        public string Host
        {
            get => TryGetValue("Host", out string? value) ? value : string.Empty;
            set => this["Host"] = value;
        }

        public string Referer
        {
            get => TryGetValue("Referer", out string? value) ? value : string.Empty;
            set => this["Referer"] = value;
        }

        public string Authorization
        {
            get => TryGetValue("Authorization", out string? value) ? value : string.Empty;
            set => this["Authorization"] = value;
        }

        public string Cookie
        {
            get => TryGetValue("Cookie", out string? value) ? value : string.Empty;
            set => this["Cookie"] = value;
        }

        public string Location
        {
            get => TryGetValue("Location", out string? value) ? value : string.Empty;
            set => this["Location"] = value;
        }

        public string UserAgent
        {
            get => TryGetValue("User-Agent", out string? value) ? value : string.Empty;
            set => this["User-Agent"] = value;
        }

        public string ContentType
        {
            get => TryGetValue("Content-Type", out string? value) ? value : string.Empty;
            set => this["Content-Type"] = value;
        }

        public long ContentLength
        {
            get => TryGetValue("Content-Length", out string? valueStr) && long.TryParse(valueStr, out long value) ? value : 0;
            set => this["Content-Length"] = Convert.ToString(value);
        }

        public string Connection
        {
            get => TryGetValue("Connection", out string? value) ? value : string.Empty;
            set => this["Connection"] = value;
        }

        public bool ConnectionKeepAlive
        {
            get
            {
                if (TryGetValue("Connection", out string? value))
                    return value.IndexOf("Keep-Alive", StringComparison.OrdinalIgnoreCase) >= 0;
                else
                    return false;
            }
            set
            {
                this["Connection"] = "Keep-Alive";
            }
        }

        public bool ConnectionClose
        {
            get
            {
                if (TryGetValue("Connection", out string? value))
                    return value.IndexOf("Close", StringComparison.OrdinalIgnoreCase) >= 0;
                else
                    return false;
            }
            set
            {
                this["Connection"] = "Close";
            }
        }
    }
}