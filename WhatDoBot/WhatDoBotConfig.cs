﻿using Noobot.Core.Configuration;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhatDoBot
{
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
                    Description = "Ask me what stuff I've logged for you.",
                    ValidHandles = StartsWithHandle.For(whc),
                    EvaluatorFunc = Validate(WhatHappened)
                },
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
                    ctx.Add(new UserModel { UserId = msg.UserId, Confirmed = false, Name = msg.Username, Email = msg.UserEmail });
                    ctx.SaveChanges();
                    ModelContext.OnChanged();
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
            ModelContext.OnChanged();
            yield return msg.ReplyDirectlyToUser(gotit[rd.Next(0,gotit.Length-1)]);
        }

        const string whc = "what happened ";
        IEnumerable<ResponseMessage> WhatHappened(IncomingMessage msg, IValidHandle handle)
        {
            var user = ctx.Users.Single(x => x.UserId == msg.UserId);
            var utxt = new String(msg.RawText.Substring(whc.Length).TakeWhile(c => c != '?').ToArray());
            int? wk = null;
            switch (utxt)
            {
                case "last week": wk = -1; break;
                case "this week": wk = 0; break;
                default:
                    if (int.TryParse(utxt, out int wki))
                        wk = wki;
                    break;
            }
            if (!wk.HasValue)
            {
                yield return msg.ReplyDirectlyToUser($"Sorry, that {whc}didn't make sense to me!");
            }
            else
            {
                var wkv = ctx.WhatHappened(wk.Value, user);
                if (wkv.data.Count() == 0)
                {
                    yield return msg.ReplyDirectlyToUser("Nothing!");
                }
                else
                {
                    StringBuilder Response = new StringBuilder();
                    Response.AppendLine($"▓▒░▓ *Week of {wkv.start.ToShortDateString()}* ▓░▒▓");
                    var wd = wkv.data.GroupBy(x => (int)x.When.DayOfWeek).ToDictionary(x => x.Key, x => x as IEnumerable<LogModel>);
                    foreach (var x in new[] { 1,2,3,4,5,6,0 })
                    {
                        Response.Append($"*{(DayOfWeek)x}*");
                        if(wd.ContainsKey(x))
                        {
                            Response.AppendLine();
                            foreach(var it in wd[x])
                            {
                                Response.AppendLine($"{it.When.ToShortTimeString()}: {it.Data}");
                            }
                        }
                        else
                        {
                            Response.AppendLine(" _nothing_");
                        }
                    }
                    yield return msg.ReplyDirectlyToUser(Response.ToString());
                }
            }
        }
    }
}
