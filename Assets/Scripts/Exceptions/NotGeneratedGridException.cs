using System;

public class NotGeneratedGridException : Exception
{
    public NotGeneratedGridException() : base("Cell Grid wasn't generated...") { }

    public void ShowMessageWindow()
    {
        NativeWinAlert.Error("NotGeneratedGridException", Message);
    }
}
