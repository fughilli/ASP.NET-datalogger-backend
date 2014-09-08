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
            public double value;
        }

        public static List<datapoint> datapoints = InitDatapoints();

        private static List<datapoint> InitDatapoints()
        {
            List<datapoint> ret = new List<datapoint>();
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
                return Request.CreateResponse(HttpStatusCode.OK, datapoints[index]);
            }
            else
            {
                // If the index is negative, and the negative index does not underflow
                if ((datapoints.Count + index) >= 0)
                {
                    // Return the element zero-indexed from the end of datapoints by (1-index)
                    return Request.CreateResponse(HttpStatusCode.OK, datapoints[datapoints.Count + index]);
                }
            }
            // The indexed element does not exist!
            return Request.CreateErrorResponse(HttpStatusCode.RequestedRangeNotSatisfiable, "That datapoint does not exist");
        }

        public HttpResponseMessage Get([FromUri]int index, [FromUri]int count)
        {
            // If the index is positive, return the zero-indexed element from datapoints and count-1 elements after it
            if (index >= 0 && index < datapoints.Count)
            {
                if ((index + count) < datapoints.Count)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, datapoints.GetRange(index, count));
                }
                else
                {
                    // The specified range is invalid
                    return Request.CreateErrorResponse(HttpStatusCode.RequestedRangeNotSatisfiable, "The specified range is invalid");
                }
            }
            else
            {
                // If the index is negative, and the negative index does not underflow
                if ((datapoints.Count + index) >= 0)
                {
                    if ((index + count) < 0)
                    {
                        // Return the element zero-indexed from the end of datapoints by (1-index)
                        return Request.CreateResponse(HttpStatusCode.OK, datapoints.GetRange(datapoints.Count + index, count));
                    }
                    else
                    {
                        // The specified range is invalid
                        return Request.CreateErrorResponse(HttpStatusCode.RequestedRangeNotSatisfiable, "The specified range is invalid");
                    }
                }
            }
            // The indexed element does not exist!
            return Request.CreateErrorResponse(HttpStatusCode.RequestedRangeNotSatisfiable, "The start datapoint is out of range");
        }

        public void Post([FromBody]datapoint d)
        {
            datapoints.Add(d);
        }
    }
}