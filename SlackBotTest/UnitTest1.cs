using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackBotCore;
using SlackBotCore.Objects;
using System.Drawing;
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

            Thread.Sleep(1000);

            await bot.Disconnect();
        }

        [TestMethod]
        public async Task SendMessage()
        {
            var bot = GetBot();

            await bot.Connect();

            var msg = await bot.Team.GetChannel(_testChannelId).SendMessageAsync("_TEST_ *MESSAGE!* <http://www.google.com|LINK!>");

            await bot.Disconnect();
        }
        [TestMethod]
        public async Task SendMessageAttachment()
        {
            var bot = GetBot();

            await bot.Connect();

            var attachment = new SlackAttachment()
            {
                Color = Color.Blue
            };
            attachment.Fields.Add(new SlackAttachmentField("Field 1", "LALALALALALALALALALALALALALA"));
            attachment.Fields.Add(new SlackAttachmentField("Field 2", "EFGH", true));
            attachment.Fields.Add(new SlackAttachmentField("Field 3", "IJKL", true));

            var msg = await bot.Team.GetChannel(_testChannelId).SendMessageAsync("*ATTACHMENT!*", attachment);

            await bot.Disconnect();
        }

        [TestMethod]
        public async Task SendAndDeleteMessage()
        {
            var bot = GetBot();

            await bot.Connect();

            var msg = await bot.Team.GetChannel(_testChannelId).SendMessageAsync("*_DELETE ME!!_*");

            await msg.DeleteAsync();

            await bot.Disconnect();
        }
        
        [TestMethod]
        public async Task SendAndUpdateMessage()
        {
            var bot = GetBot();

            await bot.Connect();

            var msg = await bot.Team.GetChannel(_testChannelId).SendMessageAsync("*_EDIT ME!!_*");

            await msg.UpdateAsync("How nice, I have been edited.");

            await bot.Disconnect();
        }

        [TestMethod]
        public async Task SendAndReceiveMessage()
        {
            var bot = GetBot();
            bot.MessageReceived += async (sender, e) =>
            {
                if(e.Content == "_TESTING_")
                {
                    Assert.IsTrue(e.Channel.Id == _testChannelId);
                    Assert.IsTrue(e.User.Id == bot.BotUser.Id);
                    await bot.Team.GetChannel(_testChannelId).SendMessageAsync("_TESTING SUCCESS_");
                }
            };

            await bot.Connect();

            var msg = await bot.SendMessageAsync(bot.Team.GetChannel(_testChannelId), "_TESTING_");

            await bot.Disconnect();
        }

        private SlackBot GetBot()
        {
            return new SlackBot(_testToken);
        }

        private const string _testChannelId = "C702Y1WG4";
        private const string _testToken = "xoxb-244064150499-kHwmGC42AS1e7pZHRnW07SUM";
    }
}
