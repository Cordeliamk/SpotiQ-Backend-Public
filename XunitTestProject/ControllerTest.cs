using spotiq_backend.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using spotiq_backend.Domain.Entities;
using spotiq_backend.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FluentAssertions.Common;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace XunitTestProject
{
    public class ControllerTest
    {
        private async Task<SpotiqContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<SpotiqContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            SpotiqContext? databaseContext = new SpotiqContext(options);
            databaseContext.Database.EnsureCreated();

            if (await databaseContext.Songwish.CountAsync() < 0)
            {
                for (int i = 0; i < 11; i++)
                {
                    databaseContext.Songwish.Add(
                        new Songwish()
                        {
                            Name = "Smelly cat",
                            ArtistName = "Phoebe",
                            SpotifyId = "000001",
                            SpotifyHostId = 1
                        });
                }
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async void Test1()
        {
            // Arrange
            Songwish? song = new Songwish()
            {
                Name = "Dead cat",
                ArtistName = "Black Debbath",
                SpotifyId = "000002",
                SpotifyHostId = 2
            };
            SpotiqContext? dbContext = await GetDbContext();
            SongwishController? songWishController = new SongwishController(dbContext);

            //Act
            var postResult = songWishController.Create(song);
            var getResult = await songWishController.Get(song.Id);

            

            //Assert
            postResult.Should().NotBeNull();
            postResult.Should().BeOfType<Task<IActionResult>>();
            Assert.NotNull(getResult);

            //var result = getResult.; 
            //result.Should().BeEquivalentTo(song);
        }

        [Fact]
        public async void SongwishController_Get_ReturnsSongwish()
        {
            //Arrange
            int id = 1;
            SpotiqContext? dbContext = await GetDbContext();
            SongwishController? songWishController = new SongwishController(dbContext);

            //Act
            var result = songWishController.Get(id);
            //var actionResult = await songWishController.Get(id);

            //var okResult = actionResult as OkObjectResult;
            //var actualResult = okResult.Value as Configuration;
            

            //Assert
            result.Should().NotBeNull();
            //actualResult.Should().BeEquivalentTo()
        }

        [Fact]
        public async void PollTest()
        {
            //Arrange
            int id = 1;
            SpotiqContext? dbContext = await GetDbContext();
            PollController pollController = new PollController(dbContext);

            //Act
            var result = pollController.Create(id);

            //Assert
            result.Should().NotBeNull();

        }
    }
}