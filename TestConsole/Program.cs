// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using LibGomokuGame.Models;

internal class Program
{
    private static async Task Main(string[] args)
    {
        HttpClient hc = new HttpClient();
        while (true)
        {
            Console.WriteLine(await hc.GetStringAsync("http://localhost:5000/register"));
        }
    }
}

class TestClass
{
    public string Name { get; set; } = string.Empty;
}
