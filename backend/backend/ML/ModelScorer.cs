using backend.Models;
using Microsoft.ML;

namespace backend.ML
{
    public class ModelScorer
    {
        private MLContext _mlContext;
        private PredictionEngine<Person, PresentsPrediction> _predictionEngine;

        public ModelScorer(string modelPath)
        {
            _mlContext = new MLContext();
            var loadedModel = _mlContext.Model.Load(modelPath, out var modelSchema);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<Person, PresentsPrediction>(loadedModel);
        }

        public string[] PredictPresents(Person person)
        {
            return _predictionEngine.Predict(person).GetTopThreePresents();
        }
    }
}
