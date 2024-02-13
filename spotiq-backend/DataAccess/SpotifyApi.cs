using Azure.Core;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Net;
using spotiq_backend.Domain.Entities;

namespace spotiq_backend.DataAccess
{
    public class SpotifyApi
    {
        private readonly SpotifyHost _spotifyHost;
        public SpotifyApi(SpotifyHost spotifyHost)
        {
            _spotifyHost = spotifyHost;
        }

        //public async void AddToQueue()
        public async Task AddToQueue(string trackId, int retryAttempts)
        {
            string trackUri = "spotify:track:" + trackId;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _spotifyHost.AccessToken);
            var content = new StringContent(JsonConvert.SerializeObject(new { uri = trackUri }), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string url = $"https://api.spotify.com/v1/me/player/queue?device_id={_spotifyHost.DeviceId}&uri={trackUri}";
            Console.WriteLine(url);
            var response = await client
                .PostAsync(url, content);
            Console.WriteLine(response.StatusCode);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (retryAttempts < 5)
                {
                    retryAttempts++;
                    _spotifyHost.AccessToken = await RefreshToken(_spotifyHost.RefreshToken!);
                    Thread.Sleep(retryAttempts * 1000);
                    await AddToQueue(trackId, retryAttempts);
                }
                else 
                {
                    Console.WriteLine("Cannot reach device. update refresh token");
                }
            }
        }

        public async Task<string> RefreshToken(string refreshToken)
        {
            var url = "https://accounts.spotify.com/api/token";
            var _client = new HttpClient();
            var content = new Dictionary<string, string>();
            content.Add("grant_type", "refresh_token");
            content.Add("refresh_token", refreshToken);

            _client.DefaultRequestHeaders.Add("Authorization",
                "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_spotifyHost.ClientId}:{_spotifyHost.ClientSecret}")));
            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(content) };
            var response = await _client.SendAsync(req);
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return data["access_token"];
            //var access_token = data["access_token"];
        }
        
    }
}


/*
        public async Task<string?> RefreshToken2()
        {
            var content = "grant_type=refresh_token&";
            content += "refresh_token=" + f_refresh_token; //GetFromDb
            content += "&client_id=" + client_id;
            HttpContent httpContent = new StringContent(content);

            _client.DefaultRequestHeaders.Add("Authorization",
                "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{client_id}:{client_secret}")));
            _client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            var response = await _client.PostAsync("https://accounts.spotify.com/api/token", httpContent);
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.Content);
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return data["access_token"];
            //var access_token = data["access_token"];
        }*/

/*
        private async Task<string> RefreshAccessTokenAsync()
        {
            var tokenEndpoint = "https://accounts.spotify.com/api/token";
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{client_id}:{client_secret}"));
            var content = new StringContent($"grant_type=refresh_token&refresh_token={f_refresh_token}");
            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            request.Content = content;

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<dynamic>(json);
            var access_token = tokenResponse.access_token;
            Console.WriteLine(access_token);
            var expiresIn = TimeSpan.FromSeconds((int)tokenResponse.expires_in);
            var refreshInterval = expiresIn.Subtract(TimeSpan.FromMinutes(5));
            _tokenRefreshTimer = new Timer(async _ => await RefreshAccessTokenAsync(), null, refreshInterval, Timeout.InfiniteTimeSpan);
            return access_token;

        }*/