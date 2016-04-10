
using System;
using System.Collections.Generic;
using System.Web.Http;

using XClip.Api.Models;
using XClip.DataObjects;

namespace XClip.Api.Controllers
{
    [RoutePrefix("api/batches")]
    public class BatchController : ApiController
    {

        [HttpPost]
        [Route]
        public IHttpActionResult Post(batch batch)
        {

            // get the mediaSource
            
            var batchItems = new List<XBatchItem>();
            foreach (var x in batch.items)
            {
                batchItems.Add(new XBatchItem()
                {
                    Id = Guid.NewGuid(),
                    Tags = new List<int>(),
                    Duration = string.Empty,
                    Index = x.index,
                    Start = x.start,
                    Stop = x.stop
                });    
            }

            var newBatch = new XBatch
            {
                Filename = "foo",
                Id = Guid.NewGuid(),
                InputDir = string.Empty,
                Items = batchItems,
                OutputDir = string.Empty,
                OutputMask = string.Empty,
                SrcId = batch.sourceId
            };

            var created = new BatchManager().Save(newBatch);

            if (created)
                return Ok(newBatch);

            return BadRequest("Batch creation failed");

        }
    }
}