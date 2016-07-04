
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

            return Ok(new mediaSource(tmp.Id, tmp.UId, tmp.Filename, tmp.FileExt));
        }

        [HttpPut]
        [Route("{id}/skip")]
        public IHttpActionResult Skip(Guid id)
        {
            new SourceManager().MarkAsSkipped(id);
            return Ok("skipped");
        }

        [HttpPut]
        [Route("{id}/delete")]
        public IHttpActionResult Delete(Guid id)
        {
            new SourceManager().MarkAsDeleted(id);
            return Ok("deleted");
        }
    }
}