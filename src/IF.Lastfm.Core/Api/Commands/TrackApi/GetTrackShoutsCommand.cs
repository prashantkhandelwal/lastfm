﻿using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: InternalsVisibleTo("IF.Lastfm.Core.Tests")]
namespace IF.Lastfm.Core.Api.Commands.TrackApi
{
    internal class GetTrackShoutsCommand : GetAsyncCommandBase<PageResponse<Shout>>
    {
        public string TrackName { get; set; }
        public string ArtistName { get; set; }
        public bool Autocorrect { get; set; }

        public GetTrackShoutsCommand(IAuth auth, string trackname, string artistname) : base(auth)
        {
            Method = "track.getShouts";
            TrackName = trackname;
            ArtistName = artistname;
        }

        public override void SetParameters()
        {
            Parameters.Add("track", TrackName);
            Parameters.Add("artist", ArtistName);
            Parameters.Add("autocorrect", Convert.ToInt32(Autocorrect).ToString());

            AddPagingParameters();
            DisableCaching();
        }

        public async override Task<PageResponse<Shout>> HandleResponse(HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();

            LastFmApiError error;
            if (LastFm.IsResponseValid(json, out error) && response.IsSuccessStatusCode)
            {
                var jtoken = JsonConvert.DeserializeObject<JToken>(json).SelectToken("shouts");

                return Shout.ParsePageJToken(jtoken);
            }
            else
            {
                return LastResponse.CreateErrorResponse<PageResponse<Shout>>(error);
            }
        }
    }
}