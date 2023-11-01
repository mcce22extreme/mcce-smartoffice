﻿using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Mcce22.SmartOffice.Client.Models;
using Newtonsoft.Json;

namespace Mcce22.SmartOffice.Client.Managers
{
    public abstract class ManagerBase<T> where T : IModel
    {
        protected HttpClient HttpClient { get; }

        protected string BaseUrl { get; }

        public ManagerBase(string baseUrl, HttpClient httpClient)
        {
            BaseUrl = baseUrl;
            HttpClient = httpClient;
        }

        public async Task<T[]> GetList()
        {
            var json = await HttpClient.GetStringAsync(BaseUrl);

            var entries = JsonConvert.DeserializeObject<T[]>(json);

            return entries;
        }

        //public virtual async Task<T> Save(T model)
        //{
        //    if (string.IsNullOrEmpty(model.Identifier))
        //    {
        //        var response = await HttpClient.PostAsJsonAsync(BaseUrl, model);

        //        await EnsureSuccessStatusCode(response);

        //        return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        //    }
        //    else
        //    {
        //        var response = await HttpClient.PutAsJsonAsync($"{BaseUrl}/{model.Identifier}", model);

        //        await EnsureSuccessStatusCode(response);

        //        return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        //    }
        //}

        public virtual async Task<T> Create(T model)
        {
            var response = await HttpClient.PostAsJsonAsync(BaseUrl, model);

            await EnsureSuccessStatusCode(response);

            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        public virtual async Task<T> Update(T model)
        {
            var response = await HttpClient.PutAsJsonAsync($"{BaseUrl}/{model.Identifier}", model);

            await EnsureSuccessStatusCode(response);

            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        public async Task Delete(string identifier)
        {
            await EnsureSuccessStatusCode(await HttpClient.DeleteAsync($"{BaseUrl}/{identifier}"));
        }

        protected async Task EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
