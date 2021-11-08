using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithmForPresentPrediction
{
    public class Chromosome
    {
        private readonly List<Gen> _genesForTrain;
        private readonly List<Gen> _genesForMutation;
        private ITransformer _mlModel;
        private MLContext _mlContext;
        public MulticlassClassificationMetrics Metrics { get; set; }

        public Chromosome(IEnumerable<Gen> trainingData)
        {
            var trainingDatasetSize = trainingData.Count() / 2;
            _genesForTrain = GetRandomGenes(trainingData, trainingDatasetSize);
            _genesForMutation = trainingData.Except(_genesForTrain).ToList();
        }

        public Chromosome(IEnumerable<Gen> trainingGenes, IEnumerable<Gen> mutationGenes)
        {
            _genesForTrain = trainingGenes.ToList();
            _genesForMutation = mutationGenes.ToList();
        }

        private List<Gen> GetRandomGenes(IEnumerable<Gen> source, int genesCount) => source.OrderBy(arg => Guid.NewGuid()).Take(genesCount).ToList();

        public void BuildModel()
        {
            _mlContext = new MLContext();

            var trainingDataView = _mlContext.Data.LoadFromEnumerable(_genesForTrain.OrderBy(person => person.Present).Where(present => present != null));

            var dataProcessPipeline = _mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "Label", inputColumnName: nameof(Gen.Present))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "AgeFeaturized", inputColumnName: nameof(Gen.Age)))
                .Append(_mlContext.Transforms.Text.FeaturizeText("SexFeaturized", nameof(Gen.Sex)))
                .Append(_mlContext.Transforms.Text.FeaturizeText("RelationFeaturized", nameof(Gen.Relation)))
                .Append(_mlContext.Transforms.Text.FeaturizeText("OccasionFeaturized", nameof(Gen.Occasion)))
                .Append(_mlContext.Transforms.Text.FeaturizeText("InterestsFeaturized", nameof(Gen.Interests)))
                .Append(_mlContext.Transforms.Text.FeaturizeText("PriceLevelFeaturized", nameof(Gen.PriceLevel)))
                .Append(_mlContext.Transforms.Text.FeaturizeText("PsycoTypeFeaturized", nameof(Gen.PsycoType)))
                .Append(_mlContext.Transforms.Concatenate("Features", "AgeFeaturized", "SexFeaturized", "RelationFeaturized",
                                                         "OccasionFeaturized", "InterestsFeaturized", "PriceLevelFeaturized", "PsycoTypeFeaturized"))
                .AppendCacheCheckpoint(_mlContext);

            var trainer = _mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features")
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName: "PredictedLabel", inputColumnName: "Label"));

            var trainingPipeline = dataProcessPipeline.Append(trainer);
            _mlModel = trainingPipeline.Fit(trainingDataView);
            Console.WriteLine("Model built.");
        }

        public void EvaluateModel(IDataView testingDataset) =>
            Metrics = _mlContext.MulticlassClassification.Evaluate(_mlModel.Transform(testingDataset));

        public void SaveModel(string path) =>
            _mlContext.Model.Save(_mlModel, _mlContext.Data.LoadFromEnumerable(_genesForTrain).Schema, path);

        public void Mutate()
        {
            BuildModel();
            Random rand = new Random();
            var maxMutationQty = _genesForTrain.Count / 10 + 1;
            int mutationQty = rand.Next(0, maxMutationQty);
            for (int i = 0; i < mutationQty; i++)
            {
                int trainingIndex = rand.Next(0, _genesForTrain.Count);
                int mutationIndex = rand.Next(0, _genesForMutation.Count);
                var temp = _genesForTrain[trainingIndex];
                _genesForTrain[trainingIndex] = _genesForMutation[mutationIndex];
                _genesForMutation[mutationIndex] = temp;
            }
        }

        public List<Gen> GetTrainingGenes() => _genesForTrain;
    }
}
