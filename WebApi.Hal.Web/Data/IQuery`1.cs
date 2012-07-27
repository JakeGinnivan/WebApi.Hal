using System.Collections.Generic;

namespace WebApi.Hal.Web.Data
{
    public interface IQuery<out TResult>
    {
        IEnumerable<TResult> Execute(IBeerContext context);
    }
}