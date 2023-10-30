namespace Kurisu.AkiBT
{
    public interface IIterable
    {
        NodeBehavior GetChildAt(int index);
        int GetChildCount();
    }
}
