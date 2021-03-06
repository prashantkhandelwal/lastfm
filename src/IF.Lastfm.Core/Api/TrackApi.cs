﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IF.Lastfm.Core.Api.Commands.TrackApi;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;

namespace IF.Lastfm.Core.Api
{
    public class TrackApi : ITrackApi
    {
        public ILastAuth Auth { get; private set; }

        public TrackApi(ILastAuth auth)
        {
            Auth = auth;
        }

        public async Task<LastResponse> ScrobbleAsync(Scrobble scrobble)
        {
            const string apiMethod = "track.scrobble";

            var methodParameters = new Dictionary<string, string>
            {
                {"artist", scrobble.Artist},
                {"album", scrobble.Album},
                {"track", scrobble.Track},
                {"albumArtist", scrobble.AlbumArtist},
                {"chosenByUser", Convert.ToInt32(scrobble.ChosenByUser).ToString()},
                {"timestamp", scrobble.TimePlayed.ToUnixTimestamp().ToString()},
                {"sk", Auth.UserSession.Token}
            };

            var apisig = Auth.GenerateMethodSignature(apiMethod, methodParameters);

            var postContent = LastFm.CreatePostBody(apiMethod,
                Auth.ApiKey,
                apisig,
                methodParameters);

            var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync(LastFm.ApiRoot, postContent);
            string json = await response.Content.ReadAsStringAsync();

            LastFmApiError error;
            if (LastFm.IsResponseValid(json, out error) && response.IsSuccessStatusCode)
            {
                return LastResponse.CreateSuccessResponse();
            }
            else
            {
                return LastResponse.CreateErrorResponse<LastResponse>(error);
            }
        }

        public async Task<PageResponse<LastShout>> GetShoutsForTrackAsync(string trackname, string artistname, bool autocorrect = false, int page = 0, int count = LastFm.DefaultPageLength)
        {
            var command = new GetTrackShoutsCommand(Auth, trackname, artistname)
                          {
                              Page = page,
                              Count = count,
                              Autocorrect = autocorrect
                          };
            return await command.ExecuteAsync();
        }

        public async Task<LastResponse<LastTrack>> GetInfoAsync(string trackname, string artistname, string username = "")
        {
            var command = new GetTrackInfoCommand(Auth)
                          {
                              TrackName = trackname,
                              ArtistName = artistname,
                              Username = username
                          };

            return await command.ExecuteAsync();
        }

        public async Task<LastResponse<LastTrack>> GetInfoByMbidAsync(string mbid)
        {
            var command = new GetTrackInfoCommand(Auth)
            {
                TrackMbid = mbid
            };

            return await command.ExecuteAsync();
        }

        public async Task<PageResponse<LastTrack>> GetSimilarTracksAsync(string trackname, string artistname, bool autocorrect = false, int limit = 100)
        {
            var command = new GetSimilarTracksCommand(Auth, trackname, artistname)
            {
                Autocorrect = autocorrect,
                Limit = limit
            };

            return await command.ExecuteAsync();
        }

        public async Task<LastResponse> LoveTrackAsync(string trackname, string artistname)
        {
            var command = new LoveTrackCommand(Auth, trackname, artistname);
            return await command.ExecuteAsync();
        }

        public async Task<LastResponse> UnloveTrackAsync(string trackname, string artistname)
        {
            var command = new UnloveTrackCommand(Auth, trackname, artistname);
            return await command.ExecuteAsync(); 
        }

        public async Task<PageResponse<LastTrack>> SearchForTrackAsync(string trackname, int page = 1, int itemsPerPage = LastFm.DefaultPageLength)
        {
            var command = new SearchTracksCommand(Auth, trackname)
            {
                Page = page,
                Count = itemsPerPage
            };

            return await command.ExecuteAsync();
        }
    }
}