using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IF.Lastfm.Core.Api.Commands.UserApi
{
    internal class GetUserShoutsCommand : GetAsyncCommandBase<PageResponse<Shout>>
    {
        public string Username { get; set; }

        public GetUserShoutsCommand(IAuth auth, string username) : base(auth)
        {
            Method = "user.getShouts";
            Username = username;
        }

        public override Uri BuildRequestUrl()
        {
            var parameters = new Dictionary<string, string>
                             {
                                 {"user", Uri.EscapeDataString(Username)},
                             };

            base.AddPagingParameters(parameters);
            base.DisableCaching(parameters);

            var uristring = LastFm.FormatApiUrl(Method, Auth.ApiKey, parameters);
            return new Uri(uristring, UriKind.Absolute);
        }

        public async override Task<PageResponse<Shout>> HandleResponse(HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();

            LastFmApiError error;
            if (LastFm.IsResponseValid(json, out error) && response.IsSuccessStatusCode)
            {
                JToken jtoken = JsonConvert.DeserializeObject<JToken>(json).SelectToken("shouts");

                return Shout.ParsePageJToken(jtoken);
            }
            else
            {
                return PageResponse<Shout>.CreateErrorResponse(error);
            }
        }
    }
}