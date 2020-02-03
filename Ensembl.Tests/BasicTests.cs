using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ensembl.Tests
{
    [TestClass]
    public class BasicTests
    {
        [ClassInitialize]
        public static void Init(TestContext _)
        {
            EnsemblInitializer.Init();
        }

        [TestMethod]
        public void NsAreInsertedAppropriatelyAtTheBeginning()
        {
            var expected = "NNNNNNNNNN";

            var slice = new Slice(species: "homo sapiens", chromosomeName: "1");
            var seq = slice.GetSequenceString(1, 10);

            Assert.AreEqual(expected, seq);
        }

        [TestMethod]
        public void TransitionFromNsToRevComplSuccessful()
        {
            var expected = "NTAACCCTAACCCTAACCCT";

            var seq = new Slice(species: "homo sapiens", chromosomeName: "1").GetSequenceString(10000, 10019);

            Assert.AreEqual(expected, seq);
        }
    }
}
