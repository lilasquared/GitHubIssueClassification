using System;
using System.Collections.Generic;

namespace GitHubIssueClassification
{
    public class TrainerConfig
    {
        public String TrainDataPath { get; set; }
        public String TestDataPath { get; set; }
        public String ModelPath { get; set; }
        public IList<String> Features { get; set; }
        public String Label { get; set; }

        public TrainerConfig()
        {
            Features = new List<String>();
        }

        public TrainerConfig WithFeature(String feature)
        {
            Features.Add(feature);
            return this;
        }
    }
}
