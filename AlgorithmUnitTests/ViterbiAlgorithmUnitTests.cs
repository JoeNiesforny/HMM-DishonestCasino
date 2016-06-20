using HMM_DishonestCasino;
using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmUnitTests
{
    [Subject("ViterbiAlgorithmUnitTests")]
    public class ViterbiAlgorithmUnitTests
    {
        static double[,] StateMatrix;
        static double[,] ObservationMatrix;
        static double[] InitialState;
        static int[] ObservationSequence;
        static ViterbiAlgorithm algorithm;

        static double probabilityOfSequence;
        static double probabilityOfSequenceSecond;

        static int[] foundedSequence;
        static double probabilityOfOccurenceFoundedSequence;
        

        Establish context = () =>
        {
            StateMatrix = new double[,] { { 0.7, 0.3 }, { 0.4, 0.6 } };
            ObservationMatrix = new double[,] { { 0.1, 0.4, 0.5 }, { 0.7, 0.2, 0.1 } };
            InitialState = new double[] { 0.6, 0.4 };
            ObservationSequence = new int[] { 0, 1, 0, 2 };
            algorithm = new ViterbiAlgorithm(StateMatrix, ObservationMatrix, InitialState, ObservationSequence);
        };

        Because of = () =>
        {
            probabilityOfSequence = algorithm.ProbabilityOfStateSequence(new int[] { 0, 0, 1, 1 });
            probabilityOfSequenceSecond = algorithm.ProbabilityOfStateSequence(new int[] { 1, 1, 1, 0 });

            probabilityOfOccurenceFoundedSequence = algorithm.FindStateSequence(out foundedSequence);
        };

        It ProbabilityOfStateSequenceShouldBeEqualToReferenceResult = () =>
        {
            probabilityOfSequence.ShouldBeCloseTo(0.000212, 0.000001);
            probabilityOfSequenceSecond.ShouldBeCloseTo(0.002822, 0.000001);
        };

        It AlgorithmFoundTheMostProbablyStateSequenceFromObservation = () =>
                        foundedSequence.ShouldEqual(new int[] { 1, 1, 1, 0 });

        It FoundedSequenceProbabilityShouldBeEqualToValueOfProbabilityCountedByKnowSequenceOfStates = () =>
            probabilityOfSequenceSecond.ShouldBeCloseTo(probabilityOfOccurenceFoundedSequence, 0.000001);
    }
}
