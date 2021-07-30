using System.Net.Http;
using Newtonsoft.Json;


public static class HttpClientHelper
{
	public static async Task PostByJsonContentTypeAsync(string url, object reqeustBody, Action<string> successFunction,Action<HttpRequestException> errorFunction, int timeout = 15)
	{
		var json = JsonConvert.SerializeObject(reqeustBody);

		using (var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(timeout) })
		using (var request = new HttpRequestMessage(HttpMethod.Post, url))
		using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
		{
			request.Content = stringContent;
			try
			{
				using (var httpResponseMessage = await client.SendAsync(request))
				{
					HttpResponseMessage response = await client.GetAsync(url);
					response.EnsureSuccessStatusCode();

					var responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
					successFunction(responseBody);
				}
			}
			catch (HttpRequestException e)
			{
				errorFunction(e);
			}
		}
	}
}

public class Program{
	public static async Task Main(string[] args)
	{
		//SuccessExecute
		await Execute("https://apiurl.com", new { value = "Yourvalue" }); //Result : {"value": "Yourvalue","id": 101}
		//ErrorExecute
		await Execute("https://apiurl.com/erro404", new { value = "Yourvalue" }); //Result : Error 404
	}

	public static async Task Execute(string url, object reqeustBody)
	{
		await HttpClientHelper.PostByJsonContentTypeAsync(url, reqeustBody
			, successFunction: responsebody =>
			{
				//Your Success Logic
				Console.WriteLine("Success");
				Console.WriteLine(responsebody);
			}, errorFunction: httpRequestException =>
			{
				//Your Error Solution Logic
				Console.WriteLine("Error");
				Console.WriteLine(httpRequestException.Message);
			}
		);
	}
}