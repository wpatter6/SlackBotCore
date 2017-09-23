using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackBotCore;
using SlackBotCore.Objects;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace SlackBotTest
{
    [TestClass]
    public class SlackBotCoreTests
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

            Assert.IsNotNull(msg);
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

            attachment.ImageUrl = "https://pldh.net/media/dreamworld/143.png";

            var msg = await bot.Team.GetChannel(_testChannelId).SendMessageAsync("*ATTACHMENT!*", attachment);
            
            await bot.Disconnect();
            
            Assert.IsNotNull(msg);
        }

        [TestMethod]
        public async Task SendAndDeleteMessage()
        {
            var bot = GetBot();

            await bot.Connect();

            var msg = await bot.Team.GetChannel(_testChannelId).SendMessageAsync("*_DELETE ME!!_*");
            
            var result = await msg.DeleteAsync();
            
            await bot.Disconnect();

            Assert.IsNotNull(msg);
            Assert.IsTrue(result.Ok);
        }
        
        [TestMethod]
        public async Task SendAndUpdateMessage()
        {
            var bot = GetBot();

            await bot.Connect();

            var msg = await bot.Team.GetChannel(_testChannelId).SendMessageAsync("*_EDIT ME!!_*");
            
            var result = await msg.UpdateAsync("How nice, I have been edited.");

            await bot.Disconnect();

            Assert.IsNotNull(msg);
            Assert.IsNotNull(result);
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

            Assert.IsNotNull(msg);
        }

        [TestMethod]
        public async Task SendMessageAndAddReaction()
        {
            var bot = GetBot();

            await bot.Connect();

            var msg = await bot.SendMessageAsync(bot.Team.GetChannel(_testChannelId), "_TESTING THUMBS UP_");
            
            var result = await msg.AddReactionAsync("thumbsup");

            await bot.Disconnect();

            Assert.IsNotNull(msg);
            Assert.IsTrue(result.Ok);
        }

        [TestMethod]
        public async Task SendMessageAndAddRemoveReaction()
        {
            var bot = GetBot();

            await bot.Connect();

            var msg = await bot.SendMessageAsync(bot.Team.GetChannel(_testChannelId), "_TESTING THUMBS UP REMOVED :(_");

            var result1 = await msg.AddReactionAsync("thumbsup");

            var result2 = await msg.RemoveReactionAsync("thumbsup");

            await bot.Disconnect();

            Assert.IsNotNull(msg);
            Assert.IsTrue(result1.Ok);
            Assert.IsTrue(result2.Ok);
        }

        [TestMethod]
        public async Task SendMessageAndAddStar()
        {
            var bot = GetBot();

            await bot.Connect();

            var msg = await bot.SendMessageAsync(bot.Team.GetChannel(_testChannelId), "_TESTING STAR_");

            var result = await msg.AddStarAsync();

            await bot.Disconnect();

            Assert.IsNotNull(msg);
            Assert.IsTrue(result.Ok);
        }

        [TestMethod]
        public async Task SendMessageAndAddRemoveStar()
        {
            var bot = GetBot();

            await bot.Connect();

            var msg = await bot.SendMessageAsync(bot.Team.GetChannel(_testChannelId), "_TESTING STAR REMOVED :(_");

            var result1 = await msg.AddStarAsync();

            var result2 = await msg.RemoveStarAsync();

            await bot.Disconnect();

            Assert.IsNotNull(msg);
            Assert.IsTrue(result1.Ok);
            Assert.IsTrue(result2.Ok);
        }
        
        [TestMethod]
        public async Task SendEphemeral()
        {
            var bot = GetBot();

            await bot.Connect();

            var user = bot.Team.Users.FirstOrDefault(x => x.Name == "wpatter6");

            Assert.IsNotNull(user);

            var channel = bot.Team.GetChannel(_testChannelId);

            Assert.IsNotNull(channel);

            var result = await user.SendEphemeralAsync(channel, "_TEST EPHEMERAL_");

            await bot.Disconnect();

            Assert.IsTrue(result.Ok);
        }

        #region utilities
        private SlackBot GetBot()
        {
            return new SlackBot(_testToken);
        }
        private const string _testChannelId = "C702Y1WG4";
        private const string _testToken = "xoxb-244064150499-95ViGwxH2vbnw1K0pxD1lZLg";
        #endregion
    }
}
