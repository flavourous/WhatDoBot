using System.Threading.Tasks;

namespace WhatDoBot
{
    public interface IWhatDoNoobotHost
    {
        Task Start();
        void Stop();
    }
}