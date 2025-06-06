﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Authentication;

namespace SevenStar.ApiService.Controllers;


public class TestController : ApiControllerBase
{
    private string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];
    private IStringLocalizer<TestController> _localizer;

    public TestController(IStringLocalizer<TestController> localizer)
    {
        _localizer = localizer;
    }

    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
    
    [HttpGet]
    public WeatherForecast[] Weatherforecast()
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
        return forecast;
    }
    
    [HttpGet]
    public void TestException()
    {
        throw new Exception("Test Exception");
    }

    [HttpGet]
    public IActionResult TestLocalization()
    {
        var message = _localizer["Greeting.Hello"];
        return Ok(new { message = message });
    }

    [HttpGet]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        //// ⛔ TODO: 這裡請替換為實際帳密驗證
        //if (request.Username == "admin" && request.Password == "123456")
        //{
        //    var token = _jwt.GenerateToken("user123", "Admin");

        //    return Ok(new { token });
        //}

        //return Unauthorized();

        return Unauthorized();
    }
}