using common.Model.RuleEngine;
using System;

namespace common.Model.RuleEngine
{
    public class DistanceValidatorRequest : IRuleEngine
    {
        public DistanceValidatorRequest()
        {
            AverageSpeed = 60000;
            AllowedDifferenceLimit = 1.5;
        }
        public double OriginaLatitute { get; set; }
        public double OriginaLongitute { get; set; }

        public double CurrentLatitute { get; set; }
        public double CurrentLongitute { get; set; }

        public DateTime LastTransactionDateTime { get; set; }
        /// <summary>
        /// Speed in Meters Per Hour
        /// </summary>
        public double AverageSpeed { get; set; }

        public double AllowedDifferenceLimit { get; set; }

    }
}