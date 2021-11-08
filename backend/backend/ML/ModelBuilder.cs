using backend.Models;
using Microsoft.ML;
using System.Collections.Generic;
using System.Linq;

namespace backend.ML
{
    public class ModelBuilder
    {
        public static void Build(IEnumerable<Person> trainingDataset, string pathToModel)
        {
            var mlContext = new MLContext();
            var trainingDataView = mlContext.Data.LoadFromEnumerable(trainingDataset.OrderBy(person => person.Present).Where(present => present != null));

            var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "KeyColumn", inputColumnName: nameof(Person.Present))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "AgeFeaturized", inputColumnName: nameof(Person.Age)))
                .Append(mlContext.Transforms.Text.FeaturizeText("SexFeaturized", nameof(Person.Sex)))
                .Append(mlContext.Transforms.Text.FeaturizeText("RelationFeaturized", nameof(Person.Relation)))
                .Append(mlContext.Transforms.Text.FeaturizeText("OccasionFeaturized", nameof(Person.Occasion)))
                .Append(mlContext.Transforms.Text.FeaturizeText("InterestsFeaturized", nameof(Person.Interests)))
                .Append(mlContext.Transforms.Text.FeaturizeText("PriceLevelFeaturized", nameof(Person.PriceLevel)))
                .Append(mlContext.Transforms.Text.FeaturizeText("PsycoTypeFeaturized", nameof(Person.PsycoType)))
                .Append(mlContext.Transforms.Concatenate("Features", "AgeFeaturized", "SexFeaturized", "RelationFeaturized",
                                                         "OccasionFeaturized", "InterestsFeaturized", "PriceLevelFeaturized", "PsycoTypeFeaturized"))
                .AppendCacheCheckpoint(mlContext);

            var trainer = mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "KeyColumn", featureColumnName: "Features")
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName: nameof(Person.Present), inputColumnName: "KeyColumn"));

            var trainingPipeline = dataProcessPipeline.Append(trainer);
            ITransformer trainedModel = trainingPipeline.Fit(trainingDataView);

            mlContext.Model.Save(trainedModel, trainingDataView.Schema, pathToModel);
        }
    }
}
