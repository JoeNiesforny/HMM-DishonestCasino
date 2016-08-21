namespace DishonestCasino
{
    // dynamic programming way
    public class ViterbiAlgorithm : IAlgorithm
    {
        int ObservationLengthSequence; // T - Length of observation sequence
        int StatesCount;               // N - Number of states in model
        int ObservationCount;          // M - Number of observation in model
        double[,] State;               // A - State transition probabilities
        double[,] Observation;         // B - Observation probability matrix
        double[] Initial;              // Pi - Initial state distribution
        int[] ObservationSequence;     // O - Observation sequence

        public ViterbiAlgorithm(
            double[,] state,
            double[,] observation,
            double[] initial,
            int[] observationSequence)
        {
            State = state;
            StatesCount = state.GetLength(1);
            Observation = observation;
            ObservationCount = observation.GetLength(1);
            Initial = initial;
            ObservationSequence = observationSequence;
            ObservationLengthSequence = observationSequence.Length;
        }

        public double ProbabilityOfStateSequence(int[] stateSequence)
        {
            int length = stateSequence.Length;
            double probability = Initial[stateSequence[0]] *
                Observation[stateSequence[0], ObservationSequence[0]];
            for (int index = 1; index < length; index++)
                probability *= State[stateSequence[index - 1], stateSequence[index]] *
                    Observation[stateSequence[index], ObservationSequence[index]];
            return probability;
        }

        public double FindStateSequence(out int[] states)
        {
            return FindBestPath(out states);
        }

        double FindBestPath(out int[] states)
        {
            double[,] gamma = CountGamma(out states);
            
            double compareValue = 0;
            for (int state = 0; state < StatesCount; state++)
            {
                if (gamma[ObservationLengthSequence - 1, state] > compareValue)
                {
                    compareValue = gamma[ObservationLengthSequence - 1, state];
                    states[ObservationLengthSequence - 1] = state;
                }
            }

            return gamma[ObservationLengthSequence - 1, states[ObservationLengthSequence - 1]];
        }

        double[,] CountGamma(out int[] stateSequence)
        {
            double[,] gamma = new double[ObservationLengthSequence, StatesCount];
            
            for (int state = 0; state < StatesCount; state++)
                gamma[0, state] = Initial[state] * Observation[state, ObservationSequence[0]];
            
            var arrayOfGamma = new double[StatesCount];
            stateSequence = new int[ObservationLengthSequence];
            for (int index = 1; index < ObservationLengthSequence; index++)
                for (int state = 0; state < StatesCount; state++)
                {
                    var biggest = 0.0;
                    for (int state2 = 0; state2 < StatesCount; state2++)
                    {
                        arrayOfGamma[state2] = gamma[index - 1, state2] *
                            State[state2, state] *
                            Observation[state, ObservationSequence[index]];
                        if (biggest < arrayOfGamma[state2])
                        {
                            biggest = arrayOfGamma[state2];
                            stateSequence[index-1] = state2; 
                        }
                    }
                    gamma[index, state] = biggest;
                }
            return gamma;
        }
    }
}
