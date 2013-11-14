﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using ChartTests.Charting;

namespace ChartTests
{
    /// <summary>
    /// Summary description for ChartHandler
    /// </summary>
    public class ChartHandler : IHttpHandler
    {
        private IEnumerable<KeyValuePair<int, int>> GenerateRandomPoints(int count, int seed, int maxValue)
        {
            var random = new Random(seed);
            for (int i = 1; i < count; i++)
            {
                int randomx = random.Next(0, maxValue);
                int randomy = random.Next(0, maxValue);
                yield return new KeyValuePair<int, int>(randomx, randomy);
            }
        }

        private int DataPointsCountParam
        {
            get
            {
                return int.Parse(HttpContext.Current.Request[CommonChartControl.DataPointsCountParamName]);
            }
        }

        private int SaltParam
        {
            get
            {
                return int.Parse(HttpContext.Current.Request[CommonChartControl.AddressSaltParamName]);
            }
        }

        private int MaxValueParam
        {
            get
            {
                return int.Parse(HttpContext.Current.Request[CommonChartControl.MaxValueParamName]);
            }
        }

        private Guid FactoryIdParam
        {
            get
            {
                return Guid.Parse(HttpContext.Current.Request[CommonChartControl.ChartTypeParamName]);
            }
        }

        private ChartParameters CreateParameters()
        {
            return new ChartParameters
                {
                    SeriaData = GenerateRandomPoints(DataPointsCountParam, SaltParam, MaxValueParam)
                };
        }
        private static readonly object Locker = new object();
        public void ProcessRequest(HttpContext context)
        {
            // As far i can see some of the charts are not thread safe, and sometimes we can get deadlock without a reason.
            // Since I have no will to spent time on this issue, this patch this solves the problem
            lock (Locker)
            {
                var currentFactory = FactoriesCollection.Instance.SingleOrDefault(f => f.Id == FactoryIdParam);
                if (currentFactory == null)
                    return;
                context.Response.ContentType = "image/png";
                using (var image = currentFactory.GenerateChart(CreateParameters()).CreateChartImage())
                {
                    var stream = new MemoryStream();
                    image.Save(stream, ImageFormat.Png);
                    byte[] imageBlob = stream.GetBuffer();
                    context.Response.OutputStream.Write(imageBlob, 0, imageBlob.Length);
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}