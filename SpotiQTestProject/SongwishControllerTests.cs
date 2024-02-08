using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using spotiq_backend;
using spotiq_backend.DataAccess;
using spotiq_backend.Domain.Entities;
using spotiq_backend.Controllers;
using Microsoft.AspNetCore.Mvc;



namespace spotiq_backend.DataAccess
{
    public class SongwishControllerTests
    {
        //private TestServer _server;
        //private HttpClient _client;

        //[SetUp]
        //public void Setup()
        //{
        //    _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        //    _client = _server.CreateClient();
        //}

        //public SpotiqContext? _spotiqContext;

        //[SetUp]
        //public void OneTimeSetup()
        //{
        //    EfMethods efMethods = new(_spotiqContext);
        //    efMethods.RebuildDatabase();
        //}

        //[Test]
        //public void TestSongwishController1()
        //{
        //    SongwishController songwishController = new(_spotiqContext);

        //    Songwish songwish = new()
        //    {
        //        Name = "foo",
        //        ArtistName = "bar",
        //        SpotifyId = "12345"
        //    };
        //    _spotiqContext.Songwish.Add(songwish);

        //    IActionResult result = (IActionResult)songwishController.Get(1);

        //    Assert.IsNotNull(result);

            //Assert.IsInstanceOf(typeof(Songwish), result.GetType());
        
    }
}