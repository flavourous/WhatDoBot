using Noobot.Core.Configuration;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Toolbox.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatDoBot
{
    public class WhatDoBotConfig : ConfigurationBase
    {
        public WhatDoBotConfig()
        {
            UseMiddleware<WhatDoBotLogMiddleware>();
        }
    }

    public class DbConfigReader : IConfigReader
    {
        readonly ModelContext ctx;
        public DbConfigReader(ModelContext ctx)
        {
            this.ctx = ctx;
        }

        public void SetBotKey(String botkey)
        {
            var exist = ctx.Config.Where(x => x.Id == "botkey").FirstOrDefault();
            if (exist != null)
            {
                exist.Value = botkey;
                ctx.Update(exist);
            }
            else ctx.Config.Add(new ConfigModel { Id = "botkey", Value = botkey });
            ctx.SaveChanges();
        }

        public string SlackApiKey => ctx.Config.Where(x => x.Id == "botkey").Select(x => x.Value).FirstOrDefault();
        public bool HelpEnabled => true;
        public bool StatsEnabled => true;
        public bool AboutEnabled => true;

        public T GetConfigEntry<T>(string entryName)
        {
            var rec = ctx.Config.SingleOrDefault(x => x.Id == entryName);
            return (T)Convert.ChangeType(rec, typeof(T));
        }
    }

    public class WhatDoBotLogMiddleware : MiddlewareBase
    {
        readonly ModelContext ctx;
        public WhatDoBotLogMiddleware(IMiddleware next, ModelContext ctx) : base(next)
        {
            this.ctx = ctx;
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    MessageShouldTargetBot = true,
                    ShouldContinueProcessing = false,
                    VisibleInHelp = true,
                    Description = "If you just tell me stuff, I'll log it for you.",
                    ValidHandles = new[]{ new AlwaysMatchHandle() },
                    EvaluatorFunc = Validate(Log)
                }
            };
        }

        Func<IncomingMessage, IValidHandle, IEnumerable<ResponseMessage>> Validate(Func<IncomingMessage, IValidHandle, IEnumerable<ResponseMessage>> handler)
        {
            return (msg, vh) =>
            {
                var exists = ctx.Users.SingleOrDefault(x => x.UserId == msg.UserId);
                if (exists == null)
                {
                    ctx.Add(new UserModel { UserId = msg.UserId, Confirmed = false });
                    ctx.SaveChanges();
                    return new[] { msg.ReplyDirectlyToUser("I've not seen you before, waiting for you to get validated") };
                }
                else if (!exists.Confirmed)
                {
                    return new[] { msg.ReplyDirectlyToUser("Sorry, still waiting validation of you.") };
                }
                else
                {
                    return handler(msg, vh);
                }
            };
        }
        String[] gotit = new[] { "Got it", "Ok", "Alright", "Mmm-hmm" };
        Random rd = new Random((int)DateTime.Now.ToFileTimeUtc());
        IEnumerable<ResponseMessage> Log(IncomingMessage msg, IValidHandle handle)
        {
            var user = ctx.Users.Single(x => x.UserId == msg.UserId);
            ctx.Add(new LogModel { User = user, When = DateTime.Now, Data = msg.RawText });
            ctx.SaveChanges();
            yield return msg.ReplyDirectlyToUser(gotit[rd.Next(0,gotit.Length-1)]);
        }
    }
}
