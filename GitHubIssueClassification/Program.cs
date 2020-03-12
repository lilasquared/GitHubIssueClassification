using System;
using System.IO;

namespace GitHubIssueClassification
{
    class Program
    {
        static void Main(String[] args)
        {
            Console.WriteLine("Hello World!");
            var config = new TrainerConfig
            {
                TrainDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "issues_train.tsv"),
                TestDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "issues_test.tsv"),
                ModelPath = Path.Combine(AppContext.BaseDirectory, "Models", "github_issues.zip"),
                Label = "Area",
            }.WithFeature("Title").WithFeature("Description");

            var trainer = new Trainer<GitHubIssue, IssuePrediction>(config);

            Console.WriteLine("Training...");
            var model = trainer.TrainOrLoad();
            Console.WriteLine("Training...done!");
            Console.WriteLine("Evaluating...");
            trainer.Evaluate();
            Console.WriteLine("Evaluating...done!");

            trainer.Save();

            var issue = new GitHubIssue()
            {
                Title = "WebSockets communication is slow in my machine",
                Description = "The WebSockets communication used under the covers by SignalR looks like is going slow in my development machine.."
            };

            var prediction = trainer.Predict(issue);

            Console.WriteLine($"The prediciton is...{prediction.Area}!");
            Console.ReadLine();
        }
    }
}
