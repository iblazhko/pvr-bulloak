﻿namespace BullOak.Common.WebApi
{
    using System.Web.Http;
    using System.Net.Http;

    public static class HttpResponseAcceptedExtension
    {
        public static IHttpActionResult Accepted(this ApiController controller, object value = null)
        {
            return new HttpResponseAcceptedResult(controller.Request, value);
        }
    }
}
