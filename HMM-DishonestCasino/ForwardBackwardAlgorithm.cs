namespace DishonestCasino
{
    public class ForwardBackwardAlgorithm : IAlgorithm
    {
        int ObservationLengthSequence; // T - Length of observation sequence
        int StatesCount;               // N - Number of states in model
        int ObservationCount;          // M - Number of observation in model
        double[,] State;               // A - State transition probabilities
        double[,] Observation;         // B - Observation probability matrix
        double[] Initial;              // Pi - Initial state distribution
        int[] ObservationSequence;     // O - Observation sequence

        // additional variable for algorithm forward-backward to store new model
        double[,] NewState;
        double[,] NewObservation;
        double[] NewInitial;

        public ForwardBackwardAlgorithm(
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
            double probability = 0;
            states = new int[ObservationLengthSequence];
            double[,] alfa, beta, gamma;
            double[, ,] digamma;
            // First step to compute Alfa value
            double compare = ForwardAlgorithm(out alfa);
            do
            {
                // If computing take to much time (because of model complication)
                // there is a possibility to make a stop of algorithm by adding iteration limit
                probability = compare;
                gamma = BackwardAlgorithm(alfa, out beta, probability);
                ReestimateModel(alfa, beta, probability, gamma, out digamma);
                compare = ForwardAlgorithm(out alfa);
            }
            while (probability < compare);
            int mostState = 0;
            int mostNextState = 0;
            double mostHighestPosibility = 0;
            for (int index = 0; index < ObservationLengthSequence - 1; index++)
            {
                mostHighestPosibility = 0;
                for (int state = 0; state < StatesCount; state++)
                    for (int state2 = 0; state2 < StatesCount; state2++)
                        if (digamma[index, state, state2] > mostHighestPosibility)
                        {
                            mostHighestPosibility = digamma[index, state, state2];
                            mostState = state;
                            mostNextState = state2;
                        }
                states[index] = mostState;
                states[++index] = mostNextState;
            }
            return probability;
        }

        // input model fi (A, B, pi) -> output P (O | fi) and alfa - partial probability of observation sequence up to time t
        double ForwardAlgorithm(out double[,] alfa)
        {
            alfa = new double[ObservationLengthSequence, StatesCount];
            for (int state = 0; state < StatesCount; state++) // for i = 0, 1, ..., N -1
                alfa[0, state] = Initial[state] * Observation[state, ObservationSequence[0]];
            double sum = 0;
            for (int sequenceIndex = 1; sequenceIndex < ObservationLengthSequence; sequenceIndex++) // for t = 1, 2, ..., T-1
                for (int state = 0; state < StatesCount; state++) // for i = 0, 1, ..., N -1
                {
                    for (int state2 = 0; state2 < StatesCount; state2++) // for j = 0, 1, ..., N -1
                        sum += alfa[sequenceIndex - 1, state2] * State[state2, state];
                    alfa[sequenceIndex, state] = sum * Observation[state, ObservationSequence[sequenceIndex]];
                    sum = 0;
                }
            double probability = 0;
            for (int state = 0; state < StatesCount; state++) // for i = 0, 1, ..., N -1
                probability += alfa[ObservationLengthSequence - 1, state];
            return probability;
        }

        // input model fi (A, B, pi) and O observation sequence -> output Q - state sequence
        double[,] BackwardAlgorithm(double[,] alfa, out double[,] beta, double propability)
        {
            beta = new double[ObservationLengthSequence, StatesCount];
            for (int state = 0; state < StatesCount; state++)
                beta[ObservationLengthSequence - 1, state] = 1;
            for (int index = ObservationLengthSequence - 2; index >= 0; index--)
                for (int state = 0; state < StatesCount; state++)
                    for (int state2 = 0; state2 < StatesCount; state2++)
                        beta[index, state] += State[state, state2] *
                                              Observation[state2, ObservationSequence[index + 1]] *
                                              beta[index + 1, state2];
            double[,] gamma = new double[ObservationLengthSequence - 1, StatesCount];
            for (int index = 0; index < ObservationLengthSequence - 1; index++)
                for (int state = 0; state < StatesCount; state++)
                    gamma[index, state] = alfa[index, state] * beta[index, state] / propability;
            return gamma;
        }

        // input O, N, M -> output model fi (A, B, pi)
        void ReestimateModel(double[,] alfa, double[,] beta, double propability, double[,] gamma, out double[, ,] digamma)
        {
            digamma = new double[ObservationLengthSequence - 1, StatesCount, StatesCount];
            for (int index = 0; index < ObservationLengthSequence - 1; index++)
                for (int state = 0; state < StatesCount; state++)
                    for (int state2 = 0; state2 < StatesCount; state2++)
                        digamma[index, state, state2] = alfa[index, state] *
                                                        State[state, state2] * Observation[state2, ObservationSequence[index + 1]] *
                                                        beta[index + 1, state2] / propability;
            NewInitial = new double[StatesCount];
            for (int state = 0; state < StatesCount; state++)
                NewInitial[state] = gamma[0, state];
            double sumOfGammas;
            double sumOfDigammas;
            NewState = new double[StatesCount, StatesCount];
            for (int state = 0; state < StatesCount; state++)
                for (int state2 = 0; state2 < StatesCount; state2++)
                {
                    sumOfGammas = 0;
                    sumOfDigammas = 0;
                    for (int index = 0; index < ObservationLengthSequence - 1; index++)
                    {
                        sumOfGammas += gamma[index, state];
                        sumOfDigammas += digamma[index, state, state2];
                    }
                    NewState[state, state2] = sumOfDigammas / sumOfGammas;
                }
            double sumOfGammasObservation;
            NewObservation = new double[StatesCount, ObservationCount];
            for (int state = 0; state < StatesCount; state++)
                for (int observation = 0; observation < ObservationCount; observation++)
                {
                    sumOfGammas = 0;
                    sumOfGammasObservation = 0;
                    for (int index = 0; index < ObservationLengthSequence - 1; index++)
                    {
                        sumOfGammas += gamma[index, state];
                        if (ObservationSequence[index] == observation)
                            sumOfGammasObservation += gamma[index, state];
                    }
                    NewObservation[state, observation] = sumOfGammasObservation / sumOfGammas;
                }
        }

        public bool GetNewModel(out double[] inital, out double[,] state, out double[,] observation)
        {
            if (NewInitial != null && NewState != null && NewObservation != null)
            {
                inital = NewInitial;
                state = NewState;
                observation = NewObservation;
                return true;
            }
            else
            {
                inital = null;
                state = null;
                observation = null;
                return false;
            }
        }

    }
}
