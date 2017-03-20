using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Ponto.WS
{
    public class WSOpen
    {
        public static async Task<T> GetWebAddress<T>(string[] palavras, string tipo)
        {
            string link = "https://maps.googleapis.com/maps/api/geocode/json?" + tipo + "=";
            string keyGoogle = "AIzaSyD362LLqBTMgpRmycbxTzgcBJRg8RgrvZw";
            string pesquisa = palavras[0];

            for (int i = 1; i < palavras.Length; i++)
            {
                pesquisa = pesquisa + "+" + palavras[i];
            }

            link = link + pesquisa + "&key=" + keyGoogle;

            HttpClient client = new HttpClient();
            var result = await client.GetAsync(link);
            var content = await result.Content.ReadAsStringAsync();

            var web = JsonConvert.DeserializeObject<T>(content);
            return web;
        }
    }
}
