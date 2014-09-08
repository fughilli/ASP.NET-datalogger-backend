using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace TestWebApi
{
    public class DataLogController : ApiController
    {
        public class datapoint
        {
            public int time;
            public int value;
        }

        public static List<datapoint> datapoints = InitDatapoints();

        private static List<datapoint> InitDatapoints()
        {
            List<datapoint> ret = new List<datapoint>();
            ret.Add(new datapoint { time = 0, value = 1 });
            ret.Add(new datapoint { time = 1, value = 2 });
            return ret;
        }

        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, datapoints);
        }

        public HttpResponseMessage Get([FromUri]int index)
        {
            // If the index is positive, return the zero-indexed element from datapoints
            if (index >= 0 && index < datapoints.Count)
            {
                return Request.CreateResponse(HttpStatusCode.Found, datapoints[index]);
            }
            else
            {
                // If the index is negative, and the negative index does not underflow
                if ((datapoints.Count + index) >= 0)
                {
                    // Return the element zero-indexed from the end of datapoints by (1-index)
                    return Request.CreateResponse(HttpStatusCode.Found, datapoints[datapoints.Count + index]);
                }
                else
                {
                    // The indexed element does not exist!
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "That datapoint does not exist");
                }
            }
        }

        public HttpResponseMessage Get([FromUri]int index, [FromUri]int count)
        {
            //// If the index is positive, return the zero-indexed element from datapoints
            //if (index >= 0 && index < datapoints.Count)
            //{
            //    return Request.CreateResponse(HttpStatusCode.Found, datapoints[index]);
            //}
            //else
            //{
            //    // If the index is negative, and the negative index does not underflow
            //    if ((datapoints.Count + index) >= 0)
            //    {
            //        // Return the element zero-indexed from the end of datapoints by (1-index)
            //        return Request.CreateResponse(HttpStatusCode.Found, datapoints[datapoints.Count + index]);
            //    }
            //    else
            //    {
            //        // The indexed element does not exist!
            //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "That datapoint does not exist");
            //    }
            //}
            if (index >= 0 && index < datapoints.Count)
            {
                return Request.CreateResponse(HttpStatusCode.Found, datapoints.GetRange(index, count));
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "That datapoint does not exist");
            }
        }

        public void Post([FromBody]datapoint d)
        {
            datapoints.Add(d);
        }
    }
}