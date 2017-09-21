using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackBotCore;
using System.Threading;
using System.Threading.Tasks;

namespace SlackBotTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task ConnectDisconnect()
        {
            var bot = GetBot();

            await bot.Connect();

            Thread.Sleep(3000);

            await bot.Disconnect();
        }

        private SlackBot GetBot()
        {
            return new SlackBot(_testToken);
        }

        private const string _testToken = "xoxb-244064150499-sPdob5PnERib9nmMjwwn3JOq";
    }
}
