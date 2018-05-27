using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;
using common.Model;
using common.Model.RuleEngine;

namespace CoreEngine.RulesChecker
{
    public class DistanceFinder: iRuleChecker
    {
        public DistanceFinder(DistanceValidatorRequest Request)
        {
            this.Request = Request;
        }

        public DistanceFinder(TransactionResponse transDetails)
        {

            Request = new DistanceValidatorRequest()
            {
                CurrentLatitute = double.Parse(transDetails.Latitude),
                CurrentLongitute = double.Parse(transDetails.Longitude)
            };

            if (transDetails.ImmediateLastRes != null)
            {
                Request.LastTransactionDateTime = transDetails.ImmediateLastRes.TransactionDT;
                Request.OriginaLatitute = double.Parse(transDetails.ImmediateLastRes.Latitude ?? "0");
                Request.OriginaLongitute = double.Parse(transDetails.ImmediateLastRes.Longitude ?? "0");
            }
        }
        public DistanceValidatorRequest Request = new DistanceValidatorRequest();

        public decimal Validate()
        {
            if (Request != null)
            {
                //First Transaction
                if (Request.OriginaLatitute.Equals(0) && Request.OriginaLongitute.Equals(0))
                {
                    return 0;
                }
                GeoCoordinate distanceFrom = new GeoCoordinate()
                {
                    Latitude = Request.OriginaLatitute,
                    Longitude = Request.OriginaLongitute,
                };
                GeoCoordinate distanceTo = new GeoCoordinate()
                {
                    Latitude = Request.CurrentLatitute,
                    Longitude = Request.CurrentLongitute
                };
                var timeDiff = (DateTime.Now - Request.LastTransactionDateTime).TotalMinutes;

                double distance = distanceFrom.GetDistanceTo(distanceTo);
                var perMinute = Request.AverageSpeed;

                var avgTravelledDistanceInMinute = (distance / timeDiff);

                if (avgTravelledDistanceInMinute < perMinute)
                    return 0;

                var avgExtraTravelled = avgTravelledDistanceInMinute/ perMinute;

                //To Allow Card to Be used with Additional Boundary Limit
                if (avgExtraTravelled <= Request.AllowedDifferenceLimit)
                    return 0;

                if (avgExtraTravelled < 1)
                    return 0;
                if (avgExtraTravelled < 1.1)
                    return 0.1M;
                if (avgExtraTravelled < 1.2)
                    return 0.2M;
                if (avgExtraTravelled < 1.3)
                    return 0.3M;
                if (avgExtraTravelled < 1.4)
                    return 0.4M;
                if (avgExtraTravelled < 1.5)
                    return 0.5M;
                if (avgExtraTravelled < 1.6)
                    return 0.6M;
                if (avgExtraTravelled < 1.7)
                    return 0.7M;
                if (avgExtraTravelled < 1.8)
                    return 0.8M;
                if (avgExtraTravelled < 1.9)
                    return 0.9M;

                return 1;
            }

            return 1;
        }

    }
}
