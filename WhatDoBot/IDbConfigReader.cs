using Noobot.Core.Configuration;
using System.Threading.Tasks;

namespace WhatDoBot
{
    public interface IDbConfigReader : IConfigReader
    {
        Task SetBotKey(string botkey);
    }
}