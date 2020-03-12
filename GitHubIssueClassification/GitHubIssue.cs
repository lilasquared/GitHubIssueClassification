using System;
using Microsoft.ML.Data;

namespace GitHubIssueClassification
{
    public class GitHubIssue
    {
        [LoadColumn(0)]
        public String ID { get; set; }

        [LoadColumn(1)]
        public String Area { get; set; }

        [LoadColumn(2)]
        public String Title { get; set; }

        [LoadColumn(3)]
        public String Description { get; set; }
    }
}
