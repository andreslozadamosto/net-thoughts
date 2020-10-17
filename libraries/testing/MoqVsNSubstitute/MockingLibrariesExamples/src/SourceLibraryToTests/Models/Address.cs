using SourceLibraryToTests.interfaces;
using System.Threading.Tasks;

namespace SourceLibraryToTests.Models
{
    public class Address : IAddress
    {
        public string Street { get; set; }

        public async Task<string> Find()
        {
            await Task.Delay(1);

            return "ada";
        }
    }
}
