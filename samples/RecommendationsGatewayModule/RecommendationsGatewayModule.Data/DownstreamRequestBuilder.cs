using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using RecommendationsGatewayModule.Core;
using RecommendationsGatewayModule.Core.Configuration;

namespace RecommendationsGatewayModule.Data
{
    public class DownstreamRequestBuilder
    {
        private readonly DownstreamRequest _request = new DownstreamRequest();
        
        public virtual DownstreamRequest Build()
        {
            return _request;
        }

        public DownstreamRequestBuilder WithScenario(Scenario scenario)
        {
            _request.Scenario = scenario;
            return this;
        }

        public DownstreamRequestBuilder WithMethod(string method)
        {
            _request.Method = new HttpMethod(method);
            return this;
        }

        public DownstreamRequestBuilder WithContentString(string content, string contentType)
        {
            _request.Content = new System.Net.Http.StringContent(content, Encoding.UTF8, contentType);
            return this;
        }

        public DownstreamRequestBuilder WithContentJson(object json)
        {
            //construct content to send
            _request.Content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
            return this;
        }

        public DownstreamRequestBuilder WithHeaders(IDictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                _request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
            return this;
        }

        public DownstreamRequestBuilder WithUrl(string url)
        {
            _request.RequestUri = new Uri(url);
            return this;
        }
    }
}
