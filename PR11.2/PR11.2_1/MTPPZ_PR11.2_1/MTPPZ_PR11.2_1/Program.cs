using System;

namespace Practical11_2_Adapter
{

    public interface IDistanceTracker
    {
        double GetDistanceKm();
    }

    public class ExternalTracker
    {
        private readonly double _distanceInMiles;

        public ExternalTracker(double distanceInMiles)
        {
            _distanceInMiles = distanceInMiles;
        }

        public double GetDistanceMiles()
        {
            return _distanceInMiles;
        }
    }

    public class DistanceAdapter : IDistanceTracker
    {
        private readonly ExternalTracker _externalTracker;
        private const double MilesToKmRatio = 1.60934;

        public DistanceAdapter(ExternalTracker externalTracker)
        {
            _externalTracker = externalTracker ?? throw new ArgumentNullException(nameof(externalTracker));
        }

        public double GetDistanceKm()
        {
            double miles = _externalTracker.GetDistanceMiles();
            return miles * MilesToKmRatio;
        }
    }

    public class FitnessDashboard
    {
        private readonly IDistanceTracker _tracker;

        public FitnessDashboard(IDistanceTracker tracker)
        {
            _tracker = tracker;
        }

        public void DisplayDailyActivity()
        {
            double km = _tracker.GetDistanceKm();
            Console.WriteLine($"[Фітнес-додаток] Пройдена дистанція за сьогодні: {km:F2} км.");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;


            ExternalTracker thirdPartyApi = new ExternalTracker(5.2);
            Console.WriteLine($"[ExternalTracker API] Сирі дані з датчика: {thirdPartyApi.GetDistanceMiles():F2} миль");


            IDistanceTracker adapter = new DistanceAdapter(thirdPartyApi);
            var dashboard = new FitnessDashboard(adapter);

            dashboard.DisplayDailyActivity();
        }
    }
}