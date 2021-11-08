using GeneticAlgorithmForPresentPrediction.DB;
using System;

namespace GeneticAlgorithmForPresentPrediction
{
    class Program
    {
        static void Main(string[] args)
        {
            var pathToModel = @"C:\Users\Vitaliy_Datsyshyn\Documents\VIka's diploma\repo\dev\backend\backend\model.zip";
            var dbContext = new PersonDbContext();
            var testingDataSet = dbContext.GetTestingDataSet();
            var population = new Population(20, dbContext.GetTrainingDataSet(testingDataSet), testingDataSet);
            population.BuildModels();
            var bestChromosome = population.GetBestModelAfterGenerations(3);
            bestChromosome.SaveModel(pathToModel);
        }
    }
}
