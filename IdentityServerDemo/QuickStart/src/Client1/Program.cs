using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client1
{
    /// <summary>
    /// 使用密码保护api
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //获取实际的端点地址，也就是IdentityServer的地址
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            //请求令牌以访问api1
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "ro.client",
                ClientSecret = "secret",

                UserName = "alice",
                Password = "password",
                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }
            Console.WriteLine(tokenResponse.Json);


            //访问令牌发送到API
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var resonse = await apiClient.GetAsync("http://localhost:5001/identity");
            if (!resonse.IsSuccessStatusCode)
            {
                Console.WriteLine(resonse.StatusCode);
            }
            else
            {
                var content = await resonse.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
            Console.ReadLine();
        }
    }
}
