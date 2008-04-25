namespace Altinoren.ActiveWriter
{
    using Microsoft.VisualStudio.Shell;

    public interface IDialogPageProvider
    {
        DialogPage GetDialogPage<T>();
    }
}