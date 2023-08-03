
public static class Utility
{   //seed 랜덤 값을 만드는데 기준이 되는 초기값을 말한다
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempIndex = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempIndex;
        }
        return array;
    }
}
