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
        public async Task ConnectDisconnectWithToken()
        {
            var bot = GetBot();

            await bot.Connect();

            Thread.Sleep(3000);

            await bot.Disconnect();
        }

        [TestMethod]
        public async Task SendMessage()
        {
            var bot = GetBot();

            await bot.Connect();

            var msg = await bot.SendMessageAsync(bot.Team.GetChannel("C702Y1WG4"), "_TEST_ *MESSAGE!* <http://www.google.com|LINK!>");

            await bot.Disconnect();
        }
        private SlackBot GetBot()
        {
            return new SlackBot(_testToken);
        }

        private const string _testToken = "xoxb-244064150499-2FH8zNlUnPwt2AhQy9iDVYEq";
    }
}
