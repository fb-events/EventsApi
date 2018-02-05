using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventsApi.Model;
using Flurl.Http;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Text;

namespace EventsApi.Controllers
{
    [Route("api/[controller]")]
    public class EventsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var groupId = "zygeiviai";

            var response = JsonConvert.DeserializeObject<Response>(await GetData(groupId));
            var calendar = ParseResponseToCalendar(response);

            var encodedCalendar = Encoding.UTF8.GetBytes(calendar);

            return File(encodedCalendar, "application/octet-stream", "kalendorius.ics");
        }

        private async Task<string> GetData(string groupId)
        {
            var since = DateTimeOffset.Now.AddDays(-7).ToUnixTimeSeconds();
            var until = DateTimeOffset.Now.AddDays(4 * 7).ToUnixTimeSeconds();

           return await (await $"https://graph.facebook.com/v2.11/{groupId}/events?access_token=155115031944644|<secret>&debug=all&format=json&method=get&pretty=0&suppress_http_code=1&&since={since}&until={until}".GetAsync()).Content.ReadAsStringAsync();
        }

        private string ParseResponseToCalendar(Response response)
        {
            var calendarEvents = new List<CalendarEvent>();

            foreach(var d in response.Data)
            {
                calendarEvents.Add(
                    new CalendarEvent
                    {
                        End = d.End_time == null ? new CalDateTime(ParseDateTime(d.Start_time).AddHours(1), "Europe/Vilnius") : new CalDateTime(ParseDateTime(d.End_time), "Europe/Vilnius"),
                        Start = new CalDateTime(ParseDateTime(d.Start_time), "Europe/Vilnius"),
                        Summary = d.Name,
                        Location = d.Place != null ? d.Place.Name : "",
                        Description = (d.Place != null && d.Place.Location != null) ? $"{d.Place.Location.Street}, {d.Place.Location.City}\n{d.Description}\n\nRenginio nuoroda https://www.facebook.com/events/{d.Id}" : $"{d.Description}\n\nRenginio nuoroda https://www.facebook.com/events/{d.Id}"
                    }
                );
            }

            var calendar = new Ical.Net.Calendar();
            
            calendar.AddProperty("X-WR-CALNAME", "VUŽK");
            calendar.AddProperty("X-WR-TIMEZONE", "Europe/Vilnius");
            calendar.AddProperty("METHOD", "PUBLISH");
            calendar.AddProperty("CALSCALE", "GREGORIAN");

            calendar.Events.AddRange(calendarEvents);

            var serializer = new CalendarSerializer();
            return serializer.SerializeToString(calendar);
        }

        private DateTime ParseDateTime(string dateTime)
        {
            var dt = DateTime.Parse(dateTime.Split("+").First(), CultureInfo.InvariantCulture);
            return dt;
        }
    }
}
