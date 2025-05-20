using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot.Types;

namespace BoardMarking
{
    internal class User
    {
        public string Name { get; set; } = "Невідомий";
        public string TelegramName { get; set; } = "Невідомий";
        public string TelegramUsername { get; set; } = "Невідомий";
        public StateType State { get; set; } = StateType.None;
        public long UserId { get; set; } = 0;
        public Message LastBotMessage { get; set; } = null;
        public Message LastUserMessage { get; set; } = null;

        public User(long userId = 0, string name = "Невідомий", string telegramName = "Невідомий", string telegramUsername = "Невідомий", StateType state = StateType.None) {
            UserId = userId;
            Name = name;
            TelegramName = telegramName;
            State = state;

            if (telegramUsername != null)
            {
                TelegramUsername = telegramUsername;
            }
        }

    }
}
