
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
        private readonly TagManager _tagManager = new TagManager();

        [HttpGet]
        [Route("{userId:int}/{collectionId:int}")]
        public IHttpActionResult Get(int userId, int collectionId)
        {
            var collId = collectionId > 0 ? collectionId : (int?) null;
            return Ok(_tagManager.GetTags(userId, collId).Select(kvp => new tag(kvp.Key.ToString(), kvp.Value)).ToList());
        }

        [HttpGet]
        [Route]
        public IHttpActionResult Get(string mediaId)
        {
            Guid uId;

            if (!Guid.TryParse(mediaId, out uId))
                return BadRequest("Invalid MediaId");

            return Ok(_tagManager.GetTags(uId).Select(kvp => new tag(kvp.Key.ToString(), kvp.Value)).ToList());
        }

        [HttpPost]
        [Route]
        public IHttpActionResult Post(tagCreation newTag)
        {
            if (string.IsNullOrEmpty(newTag.tag))
                return BadRequest("Tag cannot be empty");

            return Ok(_tagManager.Save(newTag.userId, newTag.collectionId, newTag.tag));
            // if the tag already exists - do i return a bad request or just the id of the existing one?
        }
    }
}