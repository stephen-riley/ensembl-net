using System.Linq;
using Ensembl.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ensembl.Tests
{
    [TestClass]
    public class BasicTests
    {
        const string speciesDbName = "homo_sapiens_core_99_38";

        [ClassInitialize]
        public static void Init(TestContext _)
        {
            EnsemblConfig.ShortConnectionString = "Server=useastdb.ensembl.org;User ID=anonymous";
        }

        [TestMethod]
        public void NsAreInsertedAppropriatelyAtTheBeginning()
        {
            var expected = "NNNNNNNNNN";

            var slice = new Slice(species: speciesDbName, chromosomeName: "1");
            var seq = slice.GetSequenceString(1, 10);

            Assert.AreEqual(expected, seq);
        }

        [TestMethod]
        public void TransitionFromNsToRevComplSuccessful()
        {
            var expected = "NTAACCCTAACCCTAACCCT";

            var seq = new Slice(species: speciesDbName, chromosomeName: "1").GetSequenceString(10000, 10019);

            Assert.AreEqual(expected, seq);
        }

        [TestMethod]
        public void CanListAllAvailableSpecies()
        {
            var speciesDbNames = SpeciesCache.GetInstalledEnsemblDatabases();

            var homoSapiens = speciesDbNames.Where(name => name == speciesDbName).FirstOrDefault();

            Assert.IsNotNull(homoSapiens);
        }

        [DataRow("Human")]
        [DataRow("homo_sapiens")]
        [DataRow("Homo sapiens")]
        [DataRow("homo_sapiens_core_99_38")]
        [DataTestMethod]
        public void CanGetSpeciesDbNameFromCommonName(string name)
        {
            var result = SpeciesCache.GetDbNameForCommonName(name);

            Assert.AreEqual(speciesDbName, result);
        }

        [DataRow("homo_sapiens_core_99_38", true)]
        [DataRow("bob", false)]
        [DataTestMethod]
        public void CanTryCommonNameAsSpeciesDbName(string name, bool expected)
        {
            var result = SpeciesCache.TryCommonNameAsSpeciesDbName(name);

            Assert.AreEqual(expected, result);
        }
    }
}
