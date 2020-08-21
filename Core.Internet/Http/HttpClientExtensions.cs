using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Internet.Http
{
   public static class HttpClientExtensions
   {
      public static async Task<ICompletion<string>> StringAsync(this HttpClient httpClient, string url, CancellationToken token)
      {
         try
         {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadAsStringAsync()).Completed(token);
         }
         catch (Exception exception)
         {
            return interrupted<string>(exception);
         }
      }
   }
}