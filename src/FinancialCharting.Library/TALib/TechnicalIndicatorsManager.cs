﻿#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

using FinancialCharting.Library.Enum;
using FinancialCharting.Library.Logging;
using FinancialCharting.Library.Models.Common;
using FinancialCharting.Library.Models.Indicator;
using FinancialCharting.Library.Models.MarketData.Common;
using FinancialCharting.Library.Models.MarketData.Custom;
using FinancialCharting.Library.Models.MarketData.Interfaces;
using FinancialCharting.Library.ProjectExceptions;
using FinancialCharting.Library.TALib.Interfaces;

using TicTacTec.TA.Library;

#endregion

namespace FinancialCharting.Library.TALib
{
	public class TechnicalIndicatorsManager
	{
		private readonly ILogWrapper _logger;
		private readonly ITALibProvider _taLibProvider;

		public TechnicalIndicatorsManager(ILogWrapper logger, ITALibProvider taLibProvider)
		{
			_logger = logger;
			_taLibProvider = taLibProvider;
		}

		public OperationResult<List<Indicator>> CalculateIndicator(List<IMarketData> data, BaseIndicatorParameters parameters)
		{
			if (data.First().GetType() != typeof (OhlcData) ||
				data.First().GetType() != typeof (OhlcvData) ||
				data.First().GetType() != typeof (OhlcvOpenInterestData) ||
				data.First().GetType() != typeof (YahooData) ||
				data.First().GetType() != typeof (BudapestseData) ||
				data.First().GetType() != typeof (SgxData))
			{
				throw new IndicatorException("Cannot calculate indicator for current datasource");
			}
			else
			{
				
			}

			try
			{
				switch (parameters.Type)
				{
					case IndicatorType.BBANDS:
						//var dates = data.Cast < DateClose(x => x.Date).ToList();
						//var closes = data.Cast < DateClose(x => x.Value).ToList();
						//return CalculateBbands(dates, closes, parameters);

					case IndicatorType.RSI:
						//	list.AddRange(CalculateRsi(data, parameters));
						//	break;

					case IndicatorType.SMA:
						//var dates = data.Select(x => x.Date).ToList();
						//var closes = data.Select(x => x.Close).ToList();
						//return CalculateSma(dates, closes, parameters);

					case IndicatorType.EMA:
						//return CalculateEma(data, parameters);

					case IndicatorType.WMA:
						//return CalculateWma(data, parameters);

					case IndicatorType.STOCH:
						//	list.AddRange(CalculateStoch(data, parameters));
						//	break;

					case IndicatorType.AROON:
						//	list.AddRange(CalculateAroon(data, parameters));
						//	break;

					case IndicatorType.AROONOSC:
						//	list.AddRange(CalculateAroonOsc(data, parameters));
						//	break;

					case IndicatorType.ADX:
						//	list.AddRange(CalculateAdx(data, parameters));
						//	break;

					case IndicatorType.MACD:
						//	list.AddRange(CalculateMacd(data, parameters));
						//	break;

					default:
						throw new ArgumentException("indicatorType");
				}
			}
			catch (TALibException ex)
			{
				return new OperationResult<List<Indicator>>(false, ex.Message);
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
				return new OperationResult<List<Indicator>>(false, ex.Message);
			}
		}

		private OperationResult<List<Indicator>> CalculateBbands(List<DateTime> dates, List<double> closes, BaseIndicatorParameters settings)
		{
			var list = new List<Indicator>();

			var parameters = (BBandsParameters)settings;
			var optInTimePeriod = parameters.Period;
			var optInNbDevUp = parameters.DeviationUp;
			var optInNbDevDn = parameters.DeviationDn;

			var calculation = _taLibProvider.CalculateBbands(closes, optInTimePeriod, optInNbDevUp, optInNbDevDn, Core.MAType.Sma);
			if (calculation.Success)
			{
				var result = (IndicatorTripleSeries)calculation.Data;
				var datetime = dates.GetRange(result.OutBegIdx, result.OutNBElement);
				var outReal1 = result.OutReal1.ToList().GetRange(0, result.OutNBElement);
				var outReal2 = result.OutReal2.ToList().GetRange(0, result.OutNBElement);
				var outReal3 = result.OutReal3.ToList().GetRange(0, result.OutNBElement);

				list.Add(new Indicator("Upper Band", "line", datetime, outReal1));
				list.Add(new Indicator("Middle Band", "line", datetime, outReal2));
				list.Add(new Indicator("Lower Band", "line", datetime, outReal3));

				return new OperationResult<List<Indicator>>(list);
			}
			else
			{
				return new OperationResult<List<Indicator>>(false, calculation.ErrorMessage);
			}
		}

		private OperationResult<List<Indicator>> CalculateRsi(List<DateTime> dates, List<double> closes, BaseIndicatorParameters settings)
		{
			var list = new List<Indicator>();

			var parameters = (RsiParameters)settings;
			var optInTimePeriod = parameters.Period;

			var calculation = _taLibProvider.CalculateRsi(closes, optInTimePeriod);
			if (calculation.Success)
			{
				var result = (IndicatorSingleSeries)calculation.Data;
				var datetime = dates.GetRange(result.OutBegIdx, result.OutNBElement);
				var outReal1 = result.OutReal1.ToList().GetRange(0, result.OutNBElement);

				list.Add(new Indicator("RSI", "line", datetime, outReal1));

				return new OperationResult<List<Indicator>>(list);
			}
			else
			{
				return new OperationResult<List<Indicator>>(false, calculation.ErrorMessage);
			}
		}

		private OperationResult<List<Indicator>> CalculateStoch(List<DateTime> dates,
																List<double> highs,
																List<double> lows,
																List<double> closes,
																BaseIndicatorParameters settings)
		{
			var list = new List<Indicator>();
			var parameters = (StochParameters)settings;

			var optInFastKPeriod = parameters.FastKPeriod;
			var optInSlowKPeriod = parameters.SlowKPeriod;
			var optInSlowDPeriod = parameters.SlowDPeriod;

			var calculation = _taLibProvider.CalculateStoch(highs,
															lows,
															closes,
															optInFastKPeriod,
															optInSlowKPeriod,
															Core.MAType.Sma,
															optInSlowDPeriod,
															Core.MAType.Sma);
			if (calculation.Success)
			{
				var result = (IndicatorDoubleSeries)calculation.Data;
				var datetime = dates.GetRange(result.OutBegIdx, result.OutNBElement);
				var outReal1 = result.OutReal1.ToList().GetRange(0, result.OutNBElement);
				var outReal2 = result.OutReal2.ToList().GetRange(0, result.OutNBElement);

				list.Add(new Indicator("Stoch %K", "line", datetime, outReal1));
				list.Add(new Indicator("Stoch %D", "line", datetime, outReal2));

				return new OperationResult<List<Indicator>>(list);
			}
			else
			{
				return new OperationResult<List<Indicator>>(false, calculation.ErrorMessage);
			}
		}

		private OperationResult<List<Indicator>> CalculateAroonOsc(List<DateTime> dates,
																   List<double> highs,
																   List<double> lows,
																   BaseIndicatorParameters settings)
		{
			var list = new List<Indicator>();

			var parameters = (AroonOscParameters)settings;
			var optInTimePeriod = parameters.Period;

			var calculation = _taLibProvider.CalculateAroonOsc(highs, lows, optInTimePeriod);
			if (calculation.Success)
			{
				var result = (IndicatorSingleSeries)calculation.Data;
				var datetime = dates.GetRange(result.OutBegIdx, result.OutNBElement);
				var outReal1 = result.OutReal1.ToList().GetRange(0, result.OutNBElement);

				list.Add(new Indicator("AroonOsc", "line", datetime, outReal1));

				return new OperationResult<List<Indicator>>(list);
			}
			else
			{
				return new OperationResult<List<Indicator>>(false, calculation.ErrorMessage);
			}
		}

		private OperationResult<List<Indicator>> CalculateAroon(List<DateTime> dates,
																List<double> highs,
																List<double> lows,
																BaseIndicatorParameters settings)
		{
			var list = new List<Indicator>();

			var parameters = (AroonParameters)settings;
			var optInTimePeriod = parameters.Period;

			var calculation = _taLibProvider.CalculateAroon(highs, lows, optInTimePeriod);
			if (calculation.Success)
			{
				var result = (IndicatorDoubleSeries)calculation.Data;
				var datetime = dates.GetRange(result.OutBegIdx, result.OutNBElement);
				var outReal1 = result.OutReal1.ToList().GetRange(0, result.OutNBElement);
				var outReal2 = result.OutReal2.ToList().GetRange(0, result.OutNBElement);

				list.Add(new Indicator("Aroon Up", "line", datetime, outReal1));
				list.Add(new Indicator("Aroon Down", "line", datetime, outReal2));

				return new OperationResult<List<Indicator>>(list);
			}
			else
			{
				return new OperationResult<List<Indicator>>(false, calculation.ErrorMessage);
			}
		}

		private OperationResult<List<Indicator>> CalculateSma(List<DateTime> dates, List<double> closes, BaseIndicatorParameters settings)
		{
			var list = new List<Indicator>();

			var parameters = (SmaParameters)settings;
			var optInTimePeriod = parameters.Period;

			var calculation = _taLibProvider.CalculateSma(closes, optInTimePeriod);
			if (calculation.Success)
			{
				var result = (IndicatorSingleSeries)calculation.Data;
				var datetime = dates.GetRange(result.OutBegIdx, result.OutNBElement);
				var outReal1 = result.OutReal1.ToList().GetRange(0, result.OutNBElement);

				list.Add(new Indicator("SMA", "line", datetime, outReal1));

				return new OperationResult<List<Indicator>>(list);
			}
			else
			{
				return new OperationResult<List<Indicator>>(false, calculation.ErrorMessage);
			}
		}

		private OperationResult<List<Indicator>> CalculateEma(List<DateTime> dates, List<double> closes, BaseIndicatorParameters settings)
		{
			var list = new List<Indicator>();

			var parameters = (EmaParameters)settings;
			var optInTimePeriod = parameters.Period;

			var calculation = _taLibProvider.CalculateEma(closes, optInTimePeriod);
			if (calculation.Success)
			{
				var result = (IndicatorSingleSeries)calculation.Data;
				var datetime = dates.GetRange(result.OutBegIdx, result.OutNBElement);
				var outReal1 = result.OutReal1.ToList().GetRange(0, result.OutNBElement);

				list.Add(new Indicator("EMA", "line", datetime, outReal1));

				return new OperationResult<List<Indicator>>(list);
			}
			else
			{
				return new OperationResult<List<Indicator>>(false, calculation.ErrorMessage);
			}
		}

		private OperationResult<List<Indicator>> CalculateWma(List<DateTime> dates, List<double> closes, BaseIndicatorParameters settings)
		{
			var list = new List<Indicator>();

			var parameters = (WmaParameters)settings;
			var optInTimePeriod = parameters.Period;

			var calculation = _taLibProvider.CalculateWma(closes, optInTimePeriod);
			if (calculation.Success)
			{
				var result = (IndicatorSingleSeries)calculation.Data;
				var datetime = dates.GetRange(result.OutBegIdx, result.OutNBElement).Select(x => x.Date).ToList();
				var outReal1 = result.OutReal1.ToList().GetRange(0, result.OutNBElement);

				list.Add(new Indicator("WMA", "line", datetime, outReal1));

				return new OperationResult<List<Indicator>>(list);
			}
			else
			{
				return new OperationResult<List<Indicator>>(false, calculation.ErrorMessage);
			}
		}

		private OperationResult<List<Indicator>> CalculateMacd(List<DateTime> dates, List<double> closes, BaseIndicatorParameters settings)
		{
			var list = new List<Indicator>();

			var parameters = (MacdParameters)settings;
			var optInFastPeriod = parameters.FastPeriod;
			var optInSlowPeriod = parameters.SlowPeriod;
			var optInSignalPeriod = parameters.SignalPeriod;

			var calculation = _taLibProvider.CalculateMacd(closes, optInFastPeriod, optInSlowPeriod, optInSignalPeriod);
			if (calculation.Success)
			{
				var result = (IndicatorTripleSeries)calculation.Data;
				var datetime = dates.GetRange(result.OutBegIdx, result.OutNBElement);
				var outReal1 = result.OutReal1.ToList().GetRange(0, result.OutNBElement);
				var outReal2 = result.OutReal2.ToList().GetRange(0, result.OutNBElement);
				var outReal3 = result.OutReal3.ToList().GetRange(0, result.OutNBElement);

				list.Add(new Indicator("MACD", "line", datetime, outReal1));
				list.Add(new Indicator("MACD Histogram", "column", datetime, outReal2));
				list.Add(new Indicator("MACD Signal", "line", datetime, outReal3));

				return new OperationResult<List<Indicator>>(list);
			}
			else
			{
				return new OperationResult<List<Indicator>>(false, calculation.ErrorMessage);
			}
		}

		private OperationResult<List<Indicator>> CalculateAdx(List<DateTime> dates,
															  List<double> highs,
															  List<double> lows,
															  List<double> closes,
															  BaseIndicatorParameters settings)
		{
			var list = new List<Indicator>();

			var parameters = (AdxParameters)settings;
			var optInTimePeriod = parameters.Period;

			var calculation = _taLibProvider.CalculateAdx(highs, lows, closes, optInTimePeriod);
			if (calculation.Success)
			{
				var result = (IndicatorTripleSeries)calculation.Data;
				var datetime = dates.GetRange(result.OutBegIdx, result.OutNBElement);
				var outReal1 = result.OutReal1.ToList().GetRange(0, result.OutNBElement);
				var outReal2 = result.OutReal2.ToList().GetRange(0, result.OutNBElement);
				var outReal3 = result.OutReal3.ToList().GetRange(0, result.OutNBElement);

				list.Add(new Indicator("ADX", "line", datetime, outReal1));
				list.Add(new Indicator("DI+", "line", datetime, outReal2));
				list.Add(new Indicator("DI-", "line", datetime, outReal3));

				return new OperationResult<List<Indicator>>(list);
			}
			else
			{
				return new OperationResult<List<Indicator>>(false, calculation.ErrorMessage);
			}
		}
	}
}
