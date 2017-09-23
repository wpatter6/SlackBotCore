using SlackBotCore.Objects;

namespace SlackBotCore.EventObjects
{
    public class ReactionAddedEventArgs
    {
        public string Reaction;
        public SlackUser User;
        public SlackMessage Message;
        public SlackFile File;
        public SlackFileComment FileComment;

        public ReactionAddedEventArgs(string reaction, SlackUser user, SlackMessage message)
        {
            Reaction = reaction;
            User = user;
            Message = message;
        }

        public ReactionAddedEventArgs(string reaction, SlackUser user, SlackFile file)
        {
            User = user;
            Reaction = reaction;
            File = file;
        }
        public ReactionAddedEventArgs(string reaction, SlackUser user, SlackFileComment fileComment)
        {
            Reaction = reaction;
            User = user;
            FileComment = fileComment;
        }
    }
}
