using Microsoft.Extensions.Configuration;
using SearchRepo.API.Domain.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using System.Text.Json;
using SearchRepo.API.Domain.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace SearchRepo.API.Services
{
    public class SearchService : ISearchService
    {
        private readonly IDistributedCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private IConfiguration Configuration { get; set; }
        private string _githubApiAddress;
        private string _githubToken;

        public SearchService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IDistributedCache cache)
        {
            _httpClientFactory = httpClientFactory;
            Configuration = configuration;
            _cache = cache;
        }

        private string GithubApiAddress
        {
            get 
            {
                if (String.IsNullOrEmpty(_githubApiAddress))
                {
                    _githubApiAddress = Configuration.GetValue<string>("AppSettings:GithubApiAddress");
                }

                return _githubApiAddress;
            }
        }

        private string GithubToken
        {
            get
            {
                if (String.IsNullOrEmpty(_githubToken))
                {
                    _githubToken = Configuration.GetValue<string>("AppSettings:GithubToken");
                }

                return _githubToken;
            }
        }

        public async Task<IEnumerable<GalleryItem>> GetGithubReposAsync(string searchInput)
        {
            IList<GalleryItem> galleryItems = new List<GalleryItem>();
            var uri = new Uri(GithubApiAddress + searchInput);
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AppName", "1.0"));
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", GithubToken);

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string resJson = await response.Content.ReadAsStringAsync();
                JsonNode gitHubNode = JsonNode.Parse(resJson)!;
                // Write JSON from a JsonNode
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = gitHubNode!.ToJsonString(options);
                // Get a JSON object from a JsonNode.
                JsonNode items = gitHubNode!["items"]!;
                foreach (var item in items.AsArray())
                {
                    string repoName = item!["name"]!.ToString();
                    string avatarOwner = item["owner"]!["avatar_url"]!.ToString();
                    GalleryItem galleryItem = new GalleryItem();
                    galleryItem.RepositoryName = repoName;
                    galleryItem.AvatarOwner = avatarOwner;
                    galleryItem.IdentityId = Guid.NewGuid().ToString();
                    galleryItems.Add(galleryItem);
                }
            }
            return galleryItems;
        }
        public async Task<IEnumerable<BookmarkRepo>> GetBookmarkedRepositoriesAsync()
        {
            IEnumerable<BookmarkRepo> list = new List<BookmarkRepo>();
            var bookmarkRepositoryCache = await _cache.GetStringAsync("BookmarkRepositoryCache");
            if(bookmarkRepositoryCache != null)
            {
                list = JsonConvert.DeserializeObject<IEnumerable<BookmarkRepo>>(bookmarkRepositoryCache);
            }

            return list;
        }
        public async Task UpdateBookmarkRepositoryAsync(BookmarkRepo bookmarkRepo)
        {
            var bookmarkRepos = await _cache.GetStringAsync("BookmarkRepositoryCache");
            if(bookmarkRepos == null)
            {
                var repos = new List<BookmarkRepo>();
                repos.Add(bookmarkRepo);
                var reposString = JsonConvert.SerializeObject(repos);
                await _cache.SetStringAsync("BookmarkRepositoryCache", reposString);
            }
            else
            {
                var list = JsonConvert.DeserializeObject<IEnumerable<BookmarkRepo>>(bookmarkRepos);
                list = list.Append(bookmarkRepo);
                var reposString = JsonConvert.SerializeObject(list);
                await _cache.SetStringAsync("BookmarkRepositoryCache", reposString);
            }

        }
        /// <summary>
        /// Convert an object to a byte array
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// Convert a byte array to an Object
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        private Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}
