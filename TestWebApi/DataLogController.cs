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
        private static readonly string MESSAGE_DATAPOINT_NOT_FOUND = "That datapoint does not exist.";
        private static readonly string MESSAGE_INVALID_RANGE = "The specified range is invalid.";
        private static readonly string MESSAGE_INVALID_START_INDEX = "The start datapoint is out of range.";
        private static readonly string MESSAGE_INVALID_JSON = "The provided JSON object is invalid.";

        public class datapoint
        {
            public int time;
            public float value;
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
                return Request.CreateResponse(
                    HttpStatusCode.OK,
                    datapoints[index]);
            }
            else
            {
                // If the index is negative, and the negative index does not underflow
                if ((datapoints.Count + index) >= 0)
                {
                    // Return the element zero-indexed from the end of datapoints by (1-index)
                    return Request.CreateResponse(
                        HttpStatusCode.OK,
                        datapoints[datapoints.Count + index]);
                }
            }
            // The indexed element does not exist!
            return Request.CreateErrorResponse(
                HttpStatusCode.RequestedRangeNotSatisfiable,
                MESSAGE_DATAPOINT_NOT_FOUND);
        }

        public HttpResponseMessage Get([FromUri]int index, [FromUri]int count, [FromUri]bool strict)
        {
            // strict means that the returned enumerable has to be of length count
            // if strict is disabled, only the total available elements after index
            // such that length <= count will be returned

            try
            {
                // If the index is positive, in range, and count is greater than 0, 
                // return the zero-indexed element and count-1 elements after it from datapoints
                if (index >= 0 && index < datapoints.Count && count >= 1)
                {
                    if (strict)
                    {
                        if ((index + count) < datapoints.Count)
                        {
                            return Request.CreateResponse(
                                HttpStatusCode.OK,
                                datapoints.GetRange(index, count));
                        }
                    }
                    else
                    {
                        // not strict, return what's available
                        return Request.CreateResponse(
                            HttpStatusCode.OK,
                            datapoints.GetRange(index, Math.Min(count, datapoints.Count - index)));
                    }
                }
                // If the index is negative 
                if (index < 0)
                {
                    if (strict)
                    {
                        // if the negative index does not underflow
                        if ((datapoints.Count + index) >= 0)
                        {
                            if ((index + count) < 0)
                            {
                                // Return the element zero-indexed from the end of datapoints by (1-index)
                                return Request.CreateResponse(
                                    HttpStatusCode.OK,
                                    datapoints.GetRange(datapoints.Count + index, count));
                            }
                        }
                    }
                    else
                    {
                        int startIndex = Math.Max(0, datapoints.Count + index);
                        int rangeCount = 1;
                        if (startIndex > 0)
                        { 
                            rangeCount = Math.Min(count, -index);
                        }
                        else
                        {
                            rangeCount = Math.Min(count, datapoints.Count);
                        }
                        // Not strict, return what's available
                        return Request.CreateResponse(
                                HttpStatusCode.OK,
                                datapoints.GetRange(startIndex, rangeCount));
                    }
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError,
                    e.Message);
            }
            // The indexed element does not exist!
            return Request.CreateErrorResponse(
                HttpStatusCode.RequestedRangeNotSatisfiable,
                MESSAGE_INVALID_START_INDEX);
        }

        public HttpResponseMessage Post([FromBody]datapoint d)
        {
            if (d != null && !float.IsNaN(d.value))
            {
                datapoints.Add(d);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            return Request.CreateErrorResponse(
                HttpStatusCode.BadRequest,
                MESSAGE_INVALID_JSON);
        }
    }
}