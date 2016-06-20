using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DishonestCasino;

namespace AlgorithmUnitTests
{
    [Subject("AlgorithmForwardBackwardUnitTests")]
    public class ForwardBackwardAlgorithmUnitTests
    {
        static double[,] StateMatrix;
        static double[,] ObservationMatrix;
        static double[] InitialState;
        static int[] ObservationSequence;
        static ForwardBackwardAlgorithm algorithm;

        static double probabilityOfSequence;
        static double probabilityOfSequenceSecond;

        static int[] foundedSequence;
        static double probabilityOfObservationWhenUsingExactModel;

        static List<int[]> allPossibleSequence;
        static double sumOfProbabilityOfAllPossibleSequence;

        Establish context = () =>
        {
            allPossibleSequence = new List<int[]> ();
            for (int i = 0; i < 4 * 4; i++)
            {
                int[] tab = new int[4];
                int temp = i;
                for (int j = 0; j < 4; j++)
                {
                    if (temp % 2 == 1)
                        tab[j] = 1;
                    temp = temp / 2;
                }
                allPossibleSequence.Add(tab);
            }
            sumOfProbabilityOfAllPossibleSequence = 0;
            StateMatrix = new double[,] { {0.7, 0.3 }, {0.4, 0.6 } };
            ObservationMatrix = new double[,] { { 0.1, 0.4, 0.5 }, { 0.7, 0.2, 0.1 } };
            InitialState = new double[] { 0.6, 0.4 };
            ObservationSequence = new int[] { 0, 1, 0, 2 };
            algorithm = new ForwardBackwardAlgorithm(StateMatrix, ObservationMatrix, InitialState, ObservationSequence);
        };

        Because of = () => 
        {
            probabilityOfSequence = algorithm.ProbabilityOfStateSequence(new int[] { 0, 0, 1, 1 });
            probabilityOfSequenceSecond = algorithm.ProbabilityOfStateSequence(new int[] { 1, 1, 1, 0 });

            probabilityOfObservationWhenUsingExactModel = algorithm.FindStateSequence(out foundedSequence);

            foreach (var sequence in allPossibleSequence)
                sumOfProbabilityOfAllPossibleSequence += algorithm.ProbabilityOfStateSequence(sequence);
        };

        It ProbabilityOfStateSequenceShouldBeEqualToReferenceResult = () =>
        {
            probabilityOfSequence.ShouldBeCloseTo(0.000212, 0.000001);
            probabilityOfSequenceSecond.ShouldBeCloseTo(0.002822, 0.000001);
        };

        It AlgorithmFoundTheMostProbablyStateSequenceFromObservation = () => 
                        foundedSequence.ShouldEqual(new int[] { 1, 1, 1, 0 });

        It FoundedSequenceProbabilityShouldBeEqualToSumOfAllPossibleProbability = () => 
            sumOfProbabilityOfAllPossibleSequence.ShouldBeCloseTo(probabilityOfObservationWhenUsingExactModel, 0.000001);
    }
}
