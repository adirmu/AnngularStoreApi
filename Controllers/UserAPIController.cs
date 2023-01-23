using AnngularStoreApi.Data;
using JWT.Builder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Cors;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;

namespace AnngularStoreApi.Controllers {
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserapiController : ApiController {
        readonly MockDB db = MockDB.getInstance();
        private string activeUser;
        protected override void Initialize(HttpControllerContext controllerContext) {
            activeUser = null;
            var jwt = MockDB.getJwt();
            try {
                var data = jwt.Decode<Dictionary<string, object>>(controllerContext.Request.Headers.Authorization.Parameter);
                if (data.TryGetValue("user",out var val) & data.TryGetValue("exp", out var exp)) {
                    if (DateTime.Now.Ticks - (decimal)exp <= 0 ) { 
                        activeUser = val as string;
                    }
                }
            } catch (Exception) {
                /*invalid jwt*/
            }
            base.Initialize(controllerContext);
        }

        [HttpGet]
        public string Login(string username, string pass, bool register=false) {
            if (register) {
                return MockDB.jwtToken(Register(username,pass));
            }
            var users = db.Users.Where(x => x.Name == username);
            if (users.Count() == 1) {
                if(users.First().Password == pass) {
                    return MockDB.jwtToken(users.First());
                }
            }
            return null;
        }

        [HttpGet]
        public User Register(string username, string pass) {
            var users = db.Users.Where(x => x.Name == username);
            if (users.Count() != 0) {
                return null;
            }
            var addedUser = new User() {
                Id = Guid.NewGuid().ToString(),
                Name = username,
                Password = pass,
                Favorites = new List<Repository>()
            };
            db.Users.Add(addedUser);
            return addedUser;
        }

        [HttpGet]
        public string AddToFav(string name, string url, string avatar) {
            if (activeUser == null) 
                return null;

            var user = db.Users.FirstOrDefault(x => x.Id == activeUser);
            if (user == null)
                return null;

            if (!string.IsNullOrEmpty(name)) {
                user.Favorites.Add(new Repository() {
                    Name = name,
                    URL = url,
                    Avatar = avatar
                });
            }
            return JsonConvert.SerializeObject(new { favorites = user.Favorites, jwt = MockDB.jwtToken(user) });
        }

        [HttpGet]
        public async Task<string> Search(string search) {
            if (activeUser == null)
                return null;

            var apiUrl = "https://api.github.com/search/repositories?q=" + search;
            using (HttpClient client = new HttpClient()) {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //interview is the name i gave to the app on github for thier api
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("interview", "1"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("auth", "<enter auth code>");

                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode) {
                    var data = await response.Content.ReadAsStringAsync();
                    return data;
                }
            }
            return null;
        }
    }
}
