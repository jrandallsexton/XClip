
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
        public IHttpActionResult Post([FromBody]batch batch)
        {

            // get the mediaSource
            
            var batchItems = new List<XBatchItem>();
            foreach (var x in batch.items)
            {
                batchItems.Add(new XBatchItem()
                {
                    Tags = new List<int>(),
                    Duration = string.Empty,
                    Index = x.index,
                    Start = x.start,
                    Stop = x.stop
                });    
            }

            var source = new SourceManager().Get(batch.sourceId);

            var newBatch = new XBatch
            {
                Filename = source.Filename,
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