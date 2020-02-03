using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ensembl.Tests
{
    [TestClass]
    public class BasicTests
    {
        const string speciesDbName = "homo_sapiens_core_99_38";

        [TestMethod]
        public void NsAreInsertedAppropriatelyAtTheBeginning()
        {
            var expected = "NNNNNNNNNN";

            var slice = new Slice(speciesDbName: speciesDbName, chromosomeName: "1");
            var seq = slice.GetSequenceString(1, 10);

            Assert.AreEqual(expected, seq);
        }

        [TestMethod]
        public void TransitionFromNsToRevComplSuccessful()
        {
            var expected = "NTAACCCTAACCCTAACCCT";

            var seq = new Slice(speciesDbName: speciesDbName, chromosomeName: "1").GetSequenceString(10000, 10019);

            Assert.AreEqual(expected, seq);
        }
    }
}
