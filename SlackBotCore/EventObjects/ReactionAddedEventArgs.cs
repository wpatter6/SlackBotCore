using SlackBotCore.Objects;

namespace SlackBotCore.EventObjects
{
    public class ReactionAddedEventArgs
    {
        public string Reaction;
        public SlackUser User;
        public SlackMessage Message;
        public SlackFile File;

        public ReactionAddedEventArgs(string reaction, SlackUser user, SlackMessage message)
        {
            Reaction = reaction;
            User = user;
            Message = message;
        }

        public ReactionAddedEventArgs(string reaction, SlackFile file)
        {
            Reaction = reaction;
            File = file;
        }
    }
}
