using System.IO;
using System.Text;

namespace WebApi.Hal.Tests
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
