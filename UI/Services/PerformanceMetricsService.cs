using System.Collections.Concurrent;
using System.Diagnostics;
using Entity.DTO.Admin;

namespace UI.Services
{
	public interface IPerformanceMetricsService
	{
		void RecordRequest(long elapsedMs);
		SystemPerformanceDto GetSnapshot();
	}

	public class PerformanceMetricsService : IPerformanceMetricsService
	{
		private readonly ConcurrentQueue<long> _latencyMs = new();
		private long _totalRequests;

		public void RecordRequest(long elapsedMs)
		{
			Interlocked.Increment(ref _totalRequests);
			_latencyMs.Enqueue(elapsedMs);
			while (_latencyMs.Count > 5000 && _latencyMs.TryDequeue(out _)) { }
		}

		public SystemPerformanceDto GetSnapshot()
		{
			var arr = _latencyMs.ToArray();
			Array.Sort(arr);
			double avg = arr.Length == 0 ? 0 : arr.Average();
			double p95 = Percentile(arr, 0.95);
			double p99 = Percentile(arr, 0.99);

			var proc = Process.GetCurrentProcess();
			return new SystemPerformanceDto
			{
				TotalRequests = _totalRequests,
				AverageResponseMs = avg,
				P95ResponseMs = p95,
				P99ResponseMs = p99,
				ManagedMemoryBytes = GC.GetTotalMemory(false),
				WorkingSetBytes = proc.WorkingSet64,
				LastUpdatedUtc = DateTime.UtcNow
			};
		}

		private static double Percentile(long[] sorted, double percentile)
		{
			if (sorted.Length == 0) return 0;
			double n = (sorted.Length - 1) * percentile + 1;
			if (n == 1d) return sorted[0];
			if (n == sorted.Length) return sorted[^1];
			var k = (int)n;
			double d = n - k;
			return sorted[k - 1] + d * (sorted[k] - sorted[k - 1]);
		}
	}
}


