
using System;
using System.Web.Http;

using XClip.Api.Models;

namespace XClip.Api.Controllers
{
    [RoutePrefix("api/media")]
    public class MediaSourceController : ApiController
    {
        [HttpGet]
        [Route("{collectionId:int}/random")]
        public IHttpActionResult Get(int collectionId)
        {
            var tmp = new BatchManager().RandomSource(collectionId);

            if (tmp == null)
                return NotFound();

            return Ok(new mediaSource(tmp.Id, tmp.UId, tmp.FName, tmp.FExt));
        }

        [HttpPut]
        [Route("{mediaId:alpha}")]
        public IHttpActionResult Skip(string mediaId)
        {
            Guid uId;

            if (!Guid.TryParse(mediaId, out uId))
                return NotFound();

            new SourceManager().MarkAsSkipped(uId);

            return Ok("skipped");
        }
    }
}