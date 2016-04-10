
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

using XClip.Api.Models;

namespace XClip.Api.Controllers
{
    [RoutePrefix("api/tags")]
    public class TagController : ApiController
    {
        [HttpGet]
        [Route]
        public IHttpActionResult Get()
        {
            return Ok(new TagManager().GetTags().Select(kvp => new tag(kvp.Key.ToString(), kvp.Value)).ToList());
        }

        [HttpGet]
        [Route]
        public IHttpActionResult Get(string mediaId)
        {
            Guid uId;

            if (!Guid.TryParse(mediaId, out uId))
                return BadRequest("Invalid MediaId");

            return Ok(new TagManager().GetTags(uId).Select(kvp => new tag(kvp.Key.ToString(), kvp.Value)).ToList());
        }
    }
}