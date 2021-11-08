using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgorithmForPresentPrediction
{
    public class Population
    {
        public List<Chromosome> Chromosomes { get; set; }

        public List<Gen> GenesForEvaluation { get; set; }

        public Population(int numberOfChromosome, IEnumerable<Gen> trainingData, IEnumerable<Gen> testingData)
        {
            Chromosomes = new List<Chromosome>();
            GenesForEvaluation = testingData.ToList();
            for (int i = 0; i < numberOfChromosome; i++)
                Chromosomes.Add(new Chromosome(trainingData));
        }

        public void BuildModels() =>
            Parallel.ForEach(Chromosomes, chromosome => chromosome.BuildModel());

        public Chromosome GetBestModelAfterGenerations(int numberOfGenerations)
        {
            for (int i = 0; i < numberOfGenerations; i++)
            {
                Console.WriteLine($"Start of generation {i + 1}. Population qty: {Chromosomes.Count}");
                Chromosomes = GetBestModelsInList(Chromosomes.Count / 5);
                MutatePopulation();
                Console.WriteLine($"End of generation {i + 1}. Population qty: {Chromosomes.Count}");
            }

            return GetBestModelInList();
        }

        private List<Chromosome> GetBestModelsInList(int numberOfBestChromosomes)
        {
            EvaluateModels();
            return Chromosomes.OrderByDescending(chromosome => chromosome.Metrics.MicroAccuracy)
                .ThenByDescending(chromosome => chromosome.Metrics.MacroAccuracy)
                .Take(numberOfBestChromosomes).ToList();
        }

        private void EvaluateModels()
        {
            MLContext mlContext = new MLContext();
            for (int j = 0; j < Chromosomes.Count; j++)
            {
                Chromosomes[j].EvaluateModel(mlContext.Data.LoadFromEnumerable(GenesForEvaluation));
                Console.WriteLine($"Model {j + 1} accuracy: {Chromosomes[j].Metrics.MicroAccuracy * 100}%");
            }
        }

        private void MutatePopulation()
        {
            int populationQty = Chromosomes.Count * 5;
            Random rand = new Random();
            while (Chromosomes.Count < populationQty)
            {
                var firstIndex = rand.Next(0, Chromosomes.Count);
                var secondIndex = rand.Next(0, Chromosomes.Count);
                Chromosomes.AddRange(CrossoverChromosomes(Chromosomes[firstIndex].GetTrainingGenes(), Chromosomes[secondIndex].GetTrainingGenes()));
            }

            for (int i = 0; i < Chromosomes.Count; i++)
            {
                Chromosomes[i].Mutate();
            }
        }

        private List<Chromosome> CrossoverChromosomes(List<Gen> Parent1Genes, List<Gen> Parent2Genes)
        {
            var child1TrainingGenes = new List<Gen>();
            var child1MutationGenes = new List<Gen>();
            var child2TrainingGenes = new List<Gen>();
            var child2MutationGenes = new List<Gen>();
            Random rand = new Random();
            int minLengthOfGenes = Parent1Genes.Count > Parent2Genes.Count ? Parent2Genes.Count : Parent1Genes.Count;
            int crossoverPoint = rand.Next(1, minLengthOfGenes - 1);

            for (int i = 0; i < minLengthOfGenes; i++)
            {
                if (i < crossoverPoint)
                {
                    child1TrainingGenes.Add(Parent2Genes[i]);
                    child1MutationGenes.Add(Parent1Genes[i]);
                    child2TrainingGenes.Add(Parent1Genes[i]);
                    child2MutationGenes.Add(Parent2Genes[i]);
                }
                else
                {
                    child1TrainingGenes.Add(Parent1Genes[i]);
                    child1MutationGenes.Add(Parent2Genes[i]);
                    child2TrainingGenes.Add(Parent2Genes[i]);
                    child2MutationGenes.Add(Parent1Genes[i]);
                }
            }

            Chromosome chromosomeChild1 = new Chromosome(child1TrainingGenes, child1MutationGenes);
            Chromosome chromosomeChild2 = new Chromosome(child2TrainingGenes, child2MutationGenes);

            return new List<Chromosome> { chromosomeChild1, chromosomeChild2 };
        }

        public void MutateModels() =>
            Parallel.ForEach(Chromosomes, chromosome => chromosome.Mutate());

        private Chromosome GetBestModelInList()
        {
            EvaluateModels();
            return Chromosomes.OrderByDescending(chromosome => chromosome.Metrics.MicroAccuracy).ThenByDescending(chromosome => chromosome.Metrics.MicroAccuracy).ToList().First();
        }
    }
}
