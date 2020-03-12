using System;
using Microsoft.ML.Data;

namespace GitHubIssueClassification
{
    public class IssuePrediction
    {
        [ColumnName("PredictedLabel")]
        public String Area { get; set; }
    }
}
