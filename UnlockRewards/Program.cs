using System.Text;
using Ekko;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

Console.Title = "Untitled - Notepad";
Console.ForegroundColor = ConsoleColor.White;
var token = new CancellationTokenSource();
var watcher = new LeagueClientWatcher();
watcher.OnLeagueClient += async (clientWatcher, client) =>
{
    Console.Clear();
    token.Cancel();
    var api = new LeagueApi(client.ClientAuthInfo.RemotingAuthToken,client.ClientAuthInfo.RemotingPort);
    var response2 = await api.SendAsync(HttpMethod.Get, "/lol-rewards/v1/grants");
    var grants = JsonConvert.DeserializeObject<Grant[]>(response2);

    Console.WriteLine("Rewards: ");
    for (var index = 0; index < grants.Length; index++)
    {
        var grant = grants[index];
        foreach (var reward in grant.rewardGroup.rewards)
        {
            Console.WriteLine($"- {reward.localizations.title}");
        }
    }
    Console.WriteLine("Proceed? [Y/n]");
    var r = Console.ReadLine();
    if (r is not null && r.ToUpper() == "N")
        return;
    for (var index = 0; index < grants.Length; index++)
    {
        var grant = grants[index];
        foreach (var reward in grant.rewardGroup.rewards)
        {
            
           await api.SendAsync(HttpMethod.Post, $"/lol-rewards/v1/grants/{grant.info.id}/select",
                new StringContent(
                    JsonConvert.SerializeObject(new RootObject
                    {
                        rewardGroupId=grant.info.rewardGroupId,
                        selections=new []
                        {
                            reward.id
                        }
                       
                    }),Encoding.UTF8,"application/json"));
        }
    }
    Console.WriteLine("Done!");
    Console.ReadLine();
    Environment.Exit(0);
};
Console.WriteLine("Waiting for league client!");
await watcher.Observe(token.Token);
await Task.Delay(-1);
public class RootObject
{
    public string rewardGroupId { get; set; }
    public string[] selections { get; set; }
}




public class GrantorDescription
    {
        public string appName { get; set; }
        public string entityId { get; set; }
    }

    public class Info
    {
        public DateTime dateCreated { get; set; }
        public List<object> grantElements { get; set; }
        public string granteeId { get; set; }
        public GrantorDescription grantorDescription { get; set; }
        public string id { get; set; }
        public MessageParameters messageParameters { get; set; }
        public string rewardGroupId { get; set; }
        public List<object> selectedIds { get; set; }
        public string status { get; set; }
        public bool viewed { get; set; }
    }


    public class MessageParameters
    {
    }

  

    public class RewardGroup
    {
        public bool active { get; set; }
        public string celebrationType { get; set; }
        public List<object> childRewardGroupIds { get; set; }
        public string id { get; set; }
        public Localizations localizations { get; set; }
        public Media media { get; set; }
        public string productId { get; set; }
        public string rewardStrategy { get; set; }
        public List<Reward> rewards { get; set; }
        public SelectionStrategyConfig selectionStrategyConfig { get; set; }
        public List<object> types { get; set; }
    }

    public class Grant
    {
        public Info info { get; set; }
        public RewardGroup rewardGroup { get; set; }
    }

  

public class Localizations
{
    public string description { get; set; }
    public string title { get; set; }
    public string details { get; set; }
}

public class Media
{
    public string canvasBackgroundImage { get; set; }
    public string canvasSize { get; set; }
    public string iconUrl { get; set; }
}

public class Reward
{
    public string fulfillmentSource { get; set; }
    public string id { get; set; }
    public string itemId { get; set; }
    public string itemType { get; set; }
    public Localizations localizations { get; set; }
    public Media media { get; set; }
    public int quantity { get; set; }
}

public class Group
{
    public bool active { get; set; }
    public string celebrationType { get; set; }
    public List<object> childRewardGroupIds { get; set; }
    public string id { get; set; }
    public Localizations localizations { get; set; }
    public Media media { get; set; }
    public string productId { get; set; }
    public string rewardStrategy { get; set; }
    public List<Reward> rewards { get; set; }
    public SelectionStrategyConfig selectionStrategyConfig { get; set; }
    public List<object> types { get; set; }
}

public class SelectionStrategyConfig
{
    public int maxSelectionsAllowed { get; set; }
    public int minSelectionsAllowed { get; set; }
}
