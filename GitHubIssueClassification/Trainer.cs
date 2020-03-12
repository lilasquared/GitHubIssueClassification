using System;
using System.IO;
using System.Linq;
using Microsoft.ML;

namespace GitHubIssueClassification
{
    public class Trainer<TModel, TPrediction>
        where TModel : class 
        where TPrediction : class, new()
    {
        private MLContext _context;
        private TrainerConfig _config;
        private IDataView _trainingData;
        private ITransformer _model;

        public Trainer(TrainerConfig config)
        {
            _context = new MLContext(seed: 0); ;
            _config = config;
        }

        public ITransformer TrainOrLoad()
        {
            if (File.Exists(_config.ModelPath))
            {
                _model = _context.Model.Load(_config.ModelPath, out var modelInputSchema);
            }
            else
            {
                _trainingData = _context.Data.LoadFromTextFile<TModel>(_config.TrainDataPath, hasHeader: true);

                _model = ProcessData(_context)
                    .Append(_context.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                    .Append(_context.Transforms.Conversion.MapKeyToValue("PredictedLabel"))
                    .Fit(_trainingData);
            }

            return _model;
        }

        public void Evaluate()
        {
            var data = _context.Data.LoadFromTextFile<TModel>(_config.TestDataPath, hasHeader: true);
            var metrics = _context.MulticlassClassification.Evaluate(_model.Transform(data));

            Console.WriteLine($"*************************************************************************************************************");
            Console.WriteLine($"*       Metrics for Multi-class Classification model - Test Data     ");
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"*       MicroAccuracy:    {metrics.MicroAccuracy:0.###}");
            Console.WriteLine($"*       MacroAccuracy:    {metrics.MacroAccuracy:0.###}");
            Console.WriteLine($"*       LogLoss:          {metrics.LogLoss:#.###}");
            Console.WriteLine($"*       LogLossReduction: {metrics.LogLossReduction:#.###}");
            Console.WriteLine($"*************************************************************************************************************");
        }

        public void Save()
        {
            if (!File.Exists(_config.ModelPath))
            {
                _context.Model.Save(_model, _trainingData.Schema, _config.ModelPath);
            }
        }

        public TPrediction Predict(TModel example)
        {
            if (_model != null)
            {
                var engine = _context.Model.CreatePredictionEngine<TModel, TPrediction>(_model);
                return engine.Predict(example);
            }

            throw new InvalidOperationException("no model trained or loaded!");
        }

        private IEstimator<ITransformer> ProcessData(MLContext context)
        {
            IEstimator<ITransformer> pipeline = context.Transforms.Conversion.MapValueToKey(inputColumnName: _config.Label, outputColumnName: "Label");

            foreach (var feature in _config.Features)
            {
                pipeline = pipeline.Append(context.Transforms.Text.FeaturizeText(inputColumnName: feature, outputColumnName: $"{feature}_Feature"));
            }

            return pipeline
                .Append(context.Transforms.Concatenate("Features", _config.Features.Select(x => $"{x}_Feature").ToArray()))
                .AppendCacheCheckpoint(context);
        }
    }
}
