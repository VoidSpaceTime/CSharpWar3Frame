namespace War3Frame;

public static class HandleHelper
{
    public static void HandleAdd(JHandle handle)
    {
        War3.AddHandleReference(handle.Handle);
    }

    public static void HandleRemove(JHandle handle)
    {
        War3.SubHandleReference(handle.Handle);
    }
}