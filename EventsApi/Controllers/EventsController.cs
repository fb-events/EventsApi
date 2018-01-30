﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventsApi.Controllers
{
    [Route("api/[controller]")]
    public class EventsController : Controller
    {
        [HttpGet("{groupId}")]
        public async Task<IActionResult> Get(string groupId)
        {
            return Ok(await (await $"https://graph.facebook.com/v2.11/{groupId}/events?access_token=EAACEdEose0cBAPTMu24Va7DQ31NYO0mOcoCD5wBjhSoyZASpt6860WNcagnvpIV9RVE2ywC2wZBQI5ZC0VqYgmbniYVObNZBJZBybEnB5I2O6jxmKYOEndGTxreCZAqaeun7YaX6oSL8fZBan55GpsPklC2JMrDtoIpWuCSj2Q2JnHLrEUgKQjAlJYwqOQxZCvMZD&debug=all&format=json&method=get&pretty=0&suppress_http_code=1".GetAsync()).Content.ReadAsStringAsync());
        }
    }
}
