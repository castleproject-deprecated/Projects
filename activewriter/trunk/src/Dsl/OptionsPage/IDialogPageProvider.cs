namespace Castle.ActiveWriter
{
    using Microsoft.VisualStudio.Shell;

    public interface IDialogPageProvider
    {
        DialogPage GetDialogPage<T>();
    }
}