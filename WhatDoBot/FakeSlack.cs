using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SlackConnector;
using SlackConnector.EventHandlers;
using SlackConnector.Models;

namespace WhatDoBot
{
    public class FakeContactDetails : ContactDetails
    {
        public FakeContactDetails(string id, String name)
        {
            GetType().GetProperty("Id").SetValue(this, id);
            GetType().GetProperty("Name").SetValue(this, name);
        }
    }
    public class FakeSlack : ISlackConnector, ISlackConnection
    {

        public IReadOnlyDictionary<string, SlackChatHub> ConnectedHubs { get; } =  new Dictionary<String, SlackChatHub>();

        public IReadOnlyDictionary<string, SlackUser> UserCache { get; } = new Dictionary<String,SlackUser>();

        public bool IsConnected => true;

        public DateTime? ConnectedSince => DateTime.Now;

        public string SlackKey => "1234";

        public ContactDetails Team => new FakeContactDetails("1", "MockTeam");

        public ContactDetails Self => new FakeContactDetails("0","Mock");

        public event DisconnectEventHandler OnDisconnect;
        public event ReconnectEventHandler OnReconnecting;
        public event ReconnectEventHandler OnReconnect;
        public event MessageReceivedEventHandler OnMessageReceived = async delegate { };
        public event ReactionReceivedEventHandler OnReaction;
        public event ChatHubJoinedEventHandler OnChatHubJoined;
        public event UserJoinedEventHandler OnUserJoined;
        public event PongEventHandler OnPong;

        public Task ArchiveChannel(string channelName)
        {
            return Task.CompletedTask;
        }

        public Task Close()
        {
            return Task.CompletedTask;
        }

        public async Task<ISlackConnection> Connect(string slackKey)
        {
            await Task.Delay(200);
            mTm = new Timer(Tick, null, 1000, Timeout.Infinite);
            return this;
        }
        Timer mTm;
        void Tick(object state)
        {
            var h = OnMessageReceived.GetInvocationList().ToArray();
            var awt = h.Select(d =>
            {
                var dd = (MessageReceivedEventHandler)d;
                return dd?.Invoke(new SlackMessage
                {
                    User = new SlackUser
                    {
                        Id="TestUser",
                        Name = "Test",
                    },
                    Timestamp = DateTime.Now.Ticks,
                    Text = "A message",
                    MentionsBot=false,
                    ChatHub = new SlackChatHub
                    {
                        Name = "Hob",
                        Id="HHOD",
                        Members = new String[0],
                        Type= SlackChatHubType.Channel
                    },
                    MessageSubType = SlackMessageSubType.ChannelArchive,
                    RawData= "Some rasw data"
                }) ?? Task.CompletedTask;
            }).ToArray();

            Task.WaitAll(awt);
        }

        public Task<SlackChatHub> CreateChannel(string channelName)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            mTm.Change(Timeout.Infinite, Timeout.Infinite);    
        }

        public Task<IEnumerable<SlackChatHub>> GetChannels()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SlackUser>> GetUsers()
        {
            return Task.FromResult(new[] { new SlackUser { Id = "is" } }.AsEnumerable());
        }

        public Task IndicateTyping(SlackChatHub chatHub)
        {
            throw new NotImplementedException();
        }

        public Task<SlackChatHub> JoinChannel(string channelName)
        {
            throw new NotImplementedException();
        }

        public Task<SlackChatHub> JoinDirectMessageChannel(string user)
        {
            throw new NotImplementedException();
        }

        public Task Ping()
        {
            throw new NotImplementedException();
        }

        public Task Say(BotMessage message)
        {
            throw new NotImplementedException();
        }

        public Task<SlackPurpose> SetChannelPurpose(string channelName, string purpose)
        {
            throw new NotImplementedException();
        }

        public Task<SlackTopic> SetChannelTopic(string channelName, string topic)
        {
            throw new NotImplementedException();
        }

        public Task Upload(SlackChatHub chatHub, string filePath)
        {
            throw new NotImplementedException();
        }

        public Task Upload(SlackChatHub chatHub, Stream stream, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}