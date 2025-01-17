using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Performance
{
	/// <summary>
	///     A struct to represent value bins in histogram data.
	/// </summary>
	[Serializable]
	public struct ValueBin
	{
		/// <summary>
		///     The upper bound of the bin. It is assumed that the lower bound of the bin is the upper bound of the previous bin.
		/// </summary>
		public float v;

		/// <summary>
		///     The number of values that occurred in this bin.
		/// </summary>
		public int f;
	}

	/// <summary>
	///     Utility methods for working with ValueBin
	/// </summary>
	public static class ValueBinUtils
	{
		/// <summary>
		///     Add a given value into the appropriate bin from an array of bins.
		///     Note that values greater than the upper bound of the final bin will be dropped.
		/// </summary>
		/// <param name="bins">An array of bins. It is assumed that the bins are sorted into ascending order of value.</param>
		/// <param name="value">The value to store.</param>
		public static void AddValue (this ValueBin[] bins, float value)
		{
			for (var i = 0; i < bins.Length; ++i) {
				if (bins [i].v < value)
					continue;
				bins [i].f++;
				return;
			}
		}

		/// <summary>
		///     Calculate estimated percentiles from histogram data.
		/// </summary>
		/// <param name="bins">The bins holding the histogram data. It is assumed the bins are in ascending order of value.</param>
		/// <param name="percentile">The percentile to measure, between 0 and 1. 0.5 will give the estimated median.</param>
		/// <returns></returns>
		public static float EstimatedPercentile (this ValueBin[] bins, float percentile)
		{
			var totalSamples = 0;
			foreach (var bin in bins)
				totalSamples += bin.f;
			return bins.EstimatedPercentile (totalSamples, percentile);
		}

		/// <summary>
		///     Calculate estimated percentiles from histogram data.
		/// </summary>
		/// <param name="bins">The bins holding the histogram data. It is assumed the bins are in ascending order of value.</param>
		/// <param name="totalSamples">The total number of samples stored in all the bins.</param>
		/// <param name="percentile">The percentile to measure, between 0 and 1. 0.5 will give the estimated median.</param>
		/// <returns></returns>
		public static float EstimatedPercentile (this ValueBin[] bins, int totalSamples, float percentile)
		{
			var sampleIndex = Mathf.RoundToInt (percentile * totalSamples);
			if (sampleIndex >= totalSamples)
				sampleIndex = totalSamples - 1;
			var targetBin = 0;
			while (sampleIndex >= 0 && targetBin < bins.Length)
				sampleIndex -= bins [targetBin++].f;
			targetBin--; // we overshot by 1
			if (targetBin >= bins.Length)
				return bins [bins.Length - 1].v;
			return (bins [targetBin].v + ((targetBin > 0) ? bins [targetBin - 1].v : 0)) * 0.5f;
		}
	}

	/// <summary>
	///     Utility class for building ValueBin arrays.
	/// </summary>
	internal class ValueBinBuilder
	{
		private readonly List<ValueBin> _bins;

		/// <summary>
		///     Initialize a new ValueBinBuilder.
		/// </summary>
		public ValueBinBuilder ()
		{
			_bins = new List<ValueBin> ();
		}

		/// <summary>
		///     Get the absolute upper bound of the last bin added.
		/// </summary>
		public float CurrentHighestValue {
			get { return _bins.Count > 0 ? _bins [_bins.Count - 1].v : 0; }
		}

		/// <summary>
		///     The array of ValueBin structures produced.
		/// </summary>
		public ValueBin[] Result {
			get { return _bins.ToArray (); }
		}

		/// <summary>
		///     Add a bin of the given size to the list. The upper bound of the bin will be calculated based on the size and the
		///     upper bound of the previous bin.
		/// </summary>
		/// <param name="size">The size of the bin.</param>
		public void AddBin (float size)
		{
			_bins.Add (new ValueBin { v = CurrentHighestValue + size, f = 0 });
		}

		/// <summary>
		///     Add multiple bins, of equal size, to 'fill' until a given upper bound value is met. This is useful for quickly
		///     specifying e.g. "1ms bins from 10ms through to 50ms".
		/// </summary>
		/// <param name="size">The size of the bin.</param>
		/// <param name="limit">
		///     The upper bound to meet. Note that the final bin may have an upper bound that exceeds this value,
		///     if the space to be covered is not evenly divisible by size.
		/// </param>
		public void AddBinsUpTo (float size, float limit)
		{
			while (CurrentHighestValue < limit)
				AddBin (size);
		}

	}
}
