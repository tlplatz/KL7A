using KL7A.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class BaseTests
    {

        public const string DEFAULT_PLAIN_TEXT = "The quick brown fox jumps over the lazy dog. 1234567890?????";
        public const string DEFAULT_MESSAGE_KEY = "AAAAA";
        public const string DEFAULT_CIPHER_TEXT =
@"11111 ALFA ALFA ALFA ALFA ALFA
TBEUX HBNWN FNUIO KNSRF GXLZK KNIUF GOCZB JSLQT MKJLQ HSWVP 
YUWPZ BLXZJ YXWXU XJOCJ MTAZP IMTNG YOVGA PQLXR VMFWE KHNEM 
WYTIJ IVGUF CJZJY MNDNV QNKMN EBDUP TEUOV KXJPA BMZIP OZOER 
ACPBP DYKVN 11111";

        [TestMethod]
        public void VerifyThatDefaultSettingsAndWiringReturnsExpectedCipherText()
        {
            Assert.AreEqual(KL7A.Configuration.Wiring.Current, KL7A.Configuration.Wiring.Default());
            KL7A.Configuration.Settings s = KL7A.Configuration.Settings.Default();

            KL7A.Message msg = new KL7A.Message(s);
            msg.MessageKey = DEFAULT_MESSAGE_KEY;
            string ct = msg.Encipher(DEFAULT_PLAIN_TEXT);
            Assert.AreEqual(DEFAULT_CIPHER_TEXT, ct);
        }


        [TestMethod]
        public void VerifyThatIntervalWiringWorks()
        {
            Wiring w = Wiring.Random(true);
        }
    }
}
