﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoolNameGenerator.GA;
using CoolNameGenerator.GA.Chromosomes;
using CoolNameGenerator.GA.Crossovers;
using CoolNameGenerator.GA.Fitnesses;
using CoolNameGenerator.GA.Mutations;
using CoolNameGenerator.GA.Selections;
using CoolNameGenerator.GA.Terminations;
using CoolNameGenerator.Helper;
using CoolNameGenerator.Helper.Threading;

namespace CoolNameGenerator.GeneticWordProcessing
{
    public class WordGaController : GeneticControllerBase
    {
        public static volatile HashSet<string> EnglishWords;
        public static volatile HashSet<string> EnglishNames;
        public static volatile HashSet<string> FinglishWords;
        public static volatile HashSet<string> FinglishNames;

        public Func<IChromosome> ChromosomeFactory;
        public Action<IList<IChromosome>> DrawAllAction;
        public Action<IChromosome> DrawAction;

        public WordGaController(Func<IChromosome> chromosomeFactory, Action<IList<IChromosome>> drawAllAction)
        {
            ChromosomeFactory = chromosomeFactory;
            DrawAllAction = drawAllAction;
        }

        public WordGaController()
        {
            ChromosomeFactory = () => new WordChromosome();
        }

        public override void ConfigGa(GeneticAlgorithm ga)
        {
            base.ConfigGa(ga);
            ga.TaskExecutor = new SmartThreadPoolTaskExecutor()
            {
                MinThreads = 25,
                MaxThreads = 50
            };
        }

        public override IChromosome CreateChromosome()
        {
            return ChromosomeFactory();
        }

        public override IFitness CreateFitness()
        {
            var fitness = new WordFitness();

            fitness.EvaluateFunc = word =>
            {
                var scores = new List<int>()
                {
                    fitness.EvaluateLength(word.Length),
                    fitness.EvaluateMatchingEnglishWords(word, EnglishWords, EnglishNames, FinglishWords, FinglishNames),
                    fitness.EvaluateDuplicatChar(word)
                };

                return scores.Sum().EvaluateScoreByIntVal();
            };

            return fitness;
        }

        public override void Draw(IChromosome bestChromosome)
        {
            DrawAction?.Invoke(bestChromosome);
        }

        public void DrawAll(IList<IChromosome> chromosomes)
        {
            DrawAllAction?.Invoke(chromosomes);
        }

        public async Task LoadWordFiles()
        {
            EnglishWords = await FileExtensions.ReadWordFileAsync("EnglishWords");
            EnglishNames = await FileExtensions.ReadWordFileAsync("EnglishNames");
            FinglishWords = await FileExtensions.ReadWordFileAsync("FinglishWords");
            FinglishNames = await FileExtensions.ReadWordFileAsync("FinglishNames");
        }


        public override ITermination CreateTermination()
        {
            return new OrTermination(new TimeEvolvingTermination(TimeSpan.FromHours(2)),
                new FitnessThresholdTermination(0.998));
        }


        public override ICrossover CreateCrossover()
        {
            return new UniformCrossover();
            //return new TwoPointCrossover();
        }

        public ICrossover[] CreateCrossovers()
        {
            return new ICrossover[]
            {
                new UniformCrossover(),
                new TwoPointCrossover(),
                new ThreeParentCrossover(),
                new OnePointCrossover()
            };
        }


        public override IMutation CreateMutation()
        {
            return new UniformMutation();
            //return new ReverseSequenceMutation();
        }

        public IMutation[] CreateMutations()
        {
            return new IMutation[]
            {
                new UniformMutation(),
                new ReverseSequenceMutation()
            };
        }

        public override ISelection CreateSelection()
        {
            //return new RouletteWheelSelection();
            //return new StochasticUniversalSamplingSelection();
            // new TournamentSelection();
            return new EliteSelection(50);
        }
    }
}