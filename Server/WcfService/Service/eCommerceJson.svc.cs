﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.ServiceModel.Web;
using System.Runtime.Serialization.Json;

using NLog;
using LSOmni.Common.Util;
using LSRetail.Omni.Domain.DataModel.Base;

namespace LSOmni.Service
{
    public class eCommerceJson  : LSOmniBase, IeCommerceJson
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public string PingGet()
        {
            return base.Ping();
        }

        public string VersionGet()
        {
            return base.Version();
        }

        protected override void HandleExceptions(Exception ex, string errMsg)
        {
            if (ex.GetType() == typeof(LSOmniServiceException))
            {
                LSOmniServiceException lEx = (LSOmniServiceException)ex;
                logger.Log(LogLevel.Error, lEx.Message, lEx);

                string json;
                using (MemoryStream ms = new MemoryStream())
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(WebServiceFault));
                    ser.WriteObject(ms, new WebServiceFault()
                    {
                        FaultCode = (int)lEx.StatusCode,
                        FaultMessage = errMsg + " - " + lEx.GetMessage()
                    });
                    json = Encoding.UTF8.GetString(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
                }
                throw new WebFaultException<string>(json, HttpStatusCode.RequestedRangeNotSatisfiable);
            }
            else
            {
                logger.Log(LogLevel.Error, ex, errMsg);

                string json;
                using (MemoryStream ms = new MemoryStream())
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(WebServiceFault));
                    ser.WriteObject(ms, new WebServiceFault()
                    {
                        FaultCode = (int)StatusCode.Error,
                        FaultMessage = (errMsg + " - " + ex.Message + ((ex.InnerException != null) ? " - " + ex.InnerException.Message : string.Empty)).Replace("\"", "'")
                    });
                    json = Encoding.UTF8.GetString(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
                }
                throw new WebFaultException<string>(json, HttpStatusCode.RequestedRangeNotSatisfiable);
            }
        }
    }
}
