namespace WebApi.Hal.Web.Data
{
    public interface ICommand
    {
        void Execute(IBeerContext context);
    }
}