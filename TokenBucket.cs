using System;
using System.Threading.Tasks;

namespace ThirdPartyData.Throttle
{ 
	class TokenBucket
	{
		static readonly object SyncRoot = new object();

		public TokenBucket(int maxCapacity, int interval) {
			MaxCapacity = maxCapacity;
			Interval = TimeSpan.FromMilliseconds((long)interval * 1000).Ticks;
			CurrentTokens = 0;
			NextRefill = DateTime.UtcNow.Ticks + interval;
		}

		protected long MaxCapacity { get; set; }
		protected long Interval { get; set; }
		protected long CurrentTokens { get; set; }
		protected long NextRefill { get; set; }

		public async Task<bool> GetToken() {
			UpdateTime();

			if (ShouldThrottle()) {
				UpdateTime();
				Console.WriteLine("Waiting for Next interval");
				await Task.Delay((int)NextRefill - (int)DateTime.UtcNow.Ticks);
				return true;
			}

			return true;
		}

		void UpdateTime () {
			lock(SyncRoot) {
				var currentTime = DateTime.UtcNow.Ticks;
				if(NextRefill >= currentTime) {
					return;
				}

				Console.WriteLine("Interval Reached Resetting Time");
				NextRefill = currentTime + Interval;
				CurrentTokens = 0;
			}
		}

		bool ShouldThrottle() {
			lock (SyncRoot) {
				UpdateTime();
				if (CurrentTokens < MaxCapacity && CurrentTokens >= 0) {
					CurrentTokens++;
					Console.WriteLine($"Bucket Capacity at: {CurrentTokens} of {MaxCapacity}");
					return false;
				}

				Console.WriteLine("Bucket Full");
				return true;
			}
		}
	}
}
