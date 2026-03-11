using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Models
{
    public class TelegramConfig
    {
        public string Token {get;set;} = string.Empty;
        public string ChatId {get;set;} = string.Empty;
    }
}