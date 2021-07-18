namespace AppLib
{
    public interface Time
    {
        long ToUnixTimeMilliseconds();
    }

    public delegate long UnixTimeMilliseconds();
}
