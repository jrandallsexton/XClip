
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
        [Route("{userId:int}/{collectionId:int}")]
        public IHttpActionResult Get(int userId, int collectionId)
        {
            var collId = collectionId > 0 ? collectionId : (int?) null;
            return Ok(new TagManager().GetTags(userId, collId).Select(kvp => new tag(kvp.Key.ToString(), kvp.Value)).ToList());
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

        [HttpPost]
        [Route]
        public IHttpActionResult Post(string newTag)
        {
            if (string.IsNullOrEmpty(newTag))
                return BadRequest("Tag cannot be empty");

            return Ok("Created");
            // if the tag already exists - do i return a bad request or just the id of the existing one?
        }
    }
}