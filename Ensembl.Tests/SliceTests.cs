using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Ensembl.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ensembl.Tests
{
    [TestClass]
    public class SliceTests
    {
        const string speciesDbName = "homo_sapiens_core_99_38";

        [ClassInitialize]
        public static void Init(TestContext _)
        {
            EnsemblConfig.ShortConnectionString = "Server=useastdb.ensembl.org;User ID=anonymous";
        }

        [TestMethod]
        public void JustLikeTheBioEnsemblExample()
        {
            var expected = "NTAACCCTAACCCTAACCCT";

            var slice = new Slice(species: "Human", chromosomeName: "1");
            var seq = slice[10000..10019];

            Assert.AreEqual(expected, seq);
        }

        [TestMethod]
        public void SingleLocationIndexer()
        {
            var expected = "T";

            var slice = new Slice(species: "Human", chromosomeName: "1");
            var seq = slice[10001];

            Assert.AreEqual(expected, seq);
        }

        [DataRow(1, 10)]            // trivial
        [DataRow(10_000, 10_005)]   // transition N region to ori=-1
        [DataRow(535988, 536000)]   // transition ori=-1 to ori=1
        [DataRow(1, 1_000_000)]     // friggin' huge
        [DataTestMethod]
        public async Task RangeIndexerAgainstRestEndpoint(int start, int end)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("text/plain"));

            var expected = await client.GetStringAsync($"http://rest.ensembl.org/sequence/region/homo_sapiens/1:{start}..{end}:1?content-type=text/plain");

            var seq = new Slice(species: "Human", chromosomeName: "1")[start..end];

            Assert.AreEqual(expected, seq);
        }
    }
}
