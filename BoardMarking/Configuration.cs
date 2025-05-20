using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardMarking
{
    class TextMessages
    {
        public string ShowOnCheckingList { get; set;}
        public string CreateEvent { get; set;}
        public string ChooseOption { get; set;}
        public string EnterFormattedDate { get; set;}
        public string CommandOnlyForAdmins { get; set;}
        public string YouAlreadyRegistered { get; set;}
        public string YourAlreadyMarcked { get; set;}
        public string TodayNoEvent { get; set;}
        public string TodayEventEndOrDidNotStart { get; set;}
        public string EventCreated { get; set;}
        public string ChooseYourStatus { get; set;}
        public string SendYourFullName { get; set;}
        public string ChooseYourInstatute { get; set;}
        public string EnterYourGroup { get; set;}
        public string CreateGoogleSheet { get; set;}
        public string YourStatusSaved { get; set; }
        public string YourStatusSavedSendCommand { get; set; }
        public string ChooseHowToProve { get; set; }
        public string CheckTelegramLocationAccess { get; set; }
        public string YouMarcked { get; set; }
        public string ShareLocation { get; set; }
        public string ChangeTypeOfProve { get; set; }
        public string PleaseSendYourLocation { get; set; }
        public string YourAreNotHere { get; set; }
        public string AllChecked { get; set; }
    }
    internal class Configuration
    {
        public string BotToken { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string User {  get; set; }
        public string Password { get; set; }
        public string Charset { get; set; }
        public int TimeEventStart { get; set; }
        public int TimeEventEnd { get; set; }
        public TextMessages txtmsgs { get; set; }
    }
}
