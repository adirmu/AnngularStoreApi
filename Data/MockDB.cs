using JWT.Algorithms;
using JWT.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace AnngularStoreApi.Data {
    class MockDB {
        static RS256Algorithm alg;
        static MockDB instance;
        public List<User> Users { get; set; }

        internal static MockDB getInstance() {
            if (instance == null) {
                instance = new MockDB() {
                    Users = new List<User>() { new User() {
                        Name = "admin",
                        Password = "admin",
                        Id = "0",
                        Favorites = new List<Repository>()
                    } }
                };
                GenAlg();
            }
            return instance;
        }
        /// <summary>
        /// generates the rsa algorithem to be used with jwt
        /// </summary>
        private static void GenAlg() {
            var rsa_public = new RSACryptoServiceProvider(1024);
            var rsa_private = new RSACryptoServiceProvider(1024);
            try {
                using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/Data/rsa-private.xml")) {
                    rsa_public.FromXmlString(reader.ReadToEnd());
                }
                using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/Data/rsa-private.xml")) {
                    rsa_private.FromXmlString(reader.ReadToEnd());
                }

                alg = new RS256Algorithm(rsa_public, rsa_private);
            }
            finally {
                rsa_public.PersistKeyInCsp = true;
                rsa_private.PersistKeyInCsp = true;
            }
        }

        internal static JwtBuilder getJwt() {
            return JwtBuilder.Create().WithAlgorithm(alg);
        }

        internal static string jwtToken(User user) {
            var jwt = MockDB.getJwt();
            jwt.AddClaims(
                new Dictionary<string, object>{
                    { "exp", DateTime.Now.AddMinutes(20).Ticks },
                    { "username", user.Name},
                    { "user", user.Id },
                    { "favorites", user.Favorites },
                }
            );
            return jwt.Encode();
        }
    }

    public class User {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public List<Repository> Favorites { get; set; }
    }

    public class Repository {
        public string Name { get; set; }
        public string URL { get; set; }
        public string Avatar { get; set; }
    }
}