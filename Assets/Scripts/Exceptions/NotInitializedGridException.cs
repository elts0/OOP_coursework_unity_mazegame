using System;
public class NotInitializedGridException : Exception
{
    public NotInitializedGridException() : base("Cell Grid wasnt initialized...") { }
    public void ShowMessageWindow()
    {
        NativeWinAlert.Error("NotInitializedGridException", Message);
    }
}
