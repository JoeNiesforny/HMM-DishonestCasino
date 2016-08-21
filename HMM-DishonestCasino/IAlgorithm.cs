namespace DishonestCasino
{
    public interface IAlgorithm
    {
        double ProbabilityOfStateSequence(int[] stateSequence);

        double FindStateSequence(out int[] states);
    }
}
