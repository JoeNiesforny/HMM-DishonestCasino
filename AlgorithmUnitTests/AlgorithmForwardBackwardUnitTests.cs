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
    public class AlgorithmForwardBackwardUnitTests
    {
        static double[,] StateMatrix;
        static double[,] ObservationMatrix;
        static double[] InitialState;
        static int[] ObservationSequence;
        static AlgorithmForwardBackward algorithm;

        static double probabilityOfSequence;
        static double probabilityOfSequenceSecond;

        static int[] foundedSequence;
        static double probabilityOfOccurenceFoundedSequence;

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
            algorithm = new AlgorithmForwardBackward(StateMatrix, ObservationMatrix, InitialState, ObservationSequence);
        };

        Because of = () => 
        {
            probabilityOfSequence = Math.Round(algorithm.ProbabilityOfStateSequence(new int[] { 0, 0, 1, 1 }), 6);
            probabilityOfSequenceSecond = Math.Round(algorithm.ProbabilityOfStateSequence(new int[] { 1, 1, 1, 0 }), 6);

            probabilityOfOccurenceFoundedSequence = algorithm.FindStateSequence(out foundedSequence);

            foreach (var sequence in allPossibleSequence)
                sumOfProbabilityOfAllPossibleSequence += algorithm.ProbabilityOfStateSequence(sequence);
        };

        It ProbabilityOfStateSequenceShouldBeEqualToReferenceResult = () =>
        {
            probabilityOfSequence.ShouldEqual(0.000212);
            probabilityOfSequenceSecond.ShouldEqual(0.002822);
        };

        It AlgorithmFoundTheMostProbablyStateSequenceFromObservation = () => 
                        foundedSequence.ShouldEqual(new int[] { 1, 1, 1, 0 });

        It FoundedSequenceProbabilityShouldBeEqualToSumOfAllPossibleProbability = () => 
            double.Equals(sumOfProbabilityOfAllPossibleSequence, probabilityOfOccurenceFoundedSequence);
    }
}
