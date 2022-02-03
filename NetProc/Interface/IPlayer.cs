namespace NetProc
{
    public interface IPlayer
    {
        int ExtraBalls { get; set; }
        double GameTime { get; set; }
        string Name { get; set; }
        long Score { get; set; }
    }
}