using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ensembl.Tests
{
    [TestClass]
    public class TestsAgainstRestEndpoint
    {
        const string speciesDbName = "homo_sapiens_core_99_38";

        private HttpClient client;

        [TestInitialize]
        public void ClientSetup()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("text/plain"));
        }

        [DataRow(1, 10)]            // trivial
        [DataRow(10_000, 10_005)]   // transition N region to ori=-1
        [DataRow(535988, 536000)]   // transition ori=-1 to ori=1
        [DataRow(1, 1_000_000)]     // friggin' huge
        [DataTestMethod]
        public async Task CompareAgainstRestEndpoint(int start, int end)
        {
            var expected = await client.GetStringAsync($"http://rest.ensembl.org/sequence/region/homo_sapiens/1:{start}..{end}:1?content-type=text/plain");

            var seq = new Slice(species: speciesDbName, chromosomeName: "1").GetSequenceString(start, end);

            Assert.AreEqual(expected, seq);
        }
    }
}