﻿
namespace FreeChartTools.Charting.OxyPlotCharting
{
    /// <summary>
    /// The MIT License (MIT)
    /// Official link - http://oxyplot.codeplex.com/
    /// </summary>
    public class OxyPlotChartFactory: BaseChartFactory
    {
        public override IChartAdapter DoGenerateChart(ChartParameters parameters)
        {
            return new OxyPlotAdapter(parameters);
        }

        public override string ChartTypeName
        {
            get { return "Oxy Plot"; }
        }
    }
}