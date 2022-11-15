using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using ELTracker.Managers;
using ELTracker.Models;
using ELTracker.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using RestSharp;
using Serilog;

namespace ELTracker.Services;

internal class TrackingService : DiscordClientService
{
    private readonly ILogger<TrackingService> _logger;
    private readonly RestClient _restClient;
    private readonly IRestSettings _restSettings;
    private readonly IServerManager _serverManager;
    private readonly IDonationManager _donationManager;


    public TrackingService(DiscordSocketClient client, ILogger<TrackingService> logger, RestClient restClient, IOptions<RestSettings> restSettings, IServiceProvider provider) : base(client, logger)
    {
        _logger = logger;
        _restClient = restClient;
        _restSettings = restSettings.Value;
        _serverManager = provider.CreateAsyncScope().ServiceProvider.GetRequiredService<IServerManager>();
        _donationManager = provider.CreateAsyncScope().ServiceProvider.GetRequiredService<IDonationManager>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Client.WaitForReadyAsync(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Starting cycle....");
            await FetchDonations(stoppingToken);
            await DonorInsertion(stoppingToken);
            await DiscordIdScan(stoppingToken);
            _logger.LogInformation("Cycle complete, next cycle will begin in 60 seconds.");
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }

    private async Task DiscordIdScan(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting scan for donor Discord ids...");
        if (stoppingToken.IsCancellationRequested)
        {
            _logger.LogWarning("Scanning for donor Discord ids has been cancelled");
            return;
        }

        var donors = await _donationManager.GetAllDonors();
        foreach (var donor in donors.Where(donor => donor.Id.Contains("ExtraLife")))
        {
            string[] discordTag = { };
            if (donor.Username.Contains('#')) discordTag = donor.Username.Split('#');
            if (discordTag.Length <= 0) continue;
            var user = Client.GetUser(discordTag[0].Trim(), discordTag[1].Trim());
            if (user == null) continue;
            var newRecord = (Donor)donor.Clone();
            newRecord.Id = user.Id.ToString();
            var replaceResult = await _donationManager.ReplaceDonorRecord(donor, newRecord);
            if (replaceResult.Succeeded) continue;
            _logger.LogError("Unable to replace/update donor record for {username}\r\n\tReason: {errorMsg}", donor.Username, replaceResult.ErrorMessage);
        }
        _logger.LogInformation("Scan of donor Discord ids complete");
    }

    private async Task DonorInsertion(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting insertion of donors from donations...");
        if (stoppingToken.IsCancellationRequested)
        {
            _logger.LogWarning("Insertion of new donors has been cancelled");
            return;
        }

        var donations = await _donationManager.GetAllDonations();
        foreach (var donation in donations)
        {
            if (donation.SentToDonors) continue;
            Donor donor;
            var regex = new Regex("([\\s]#)");
            if (regex.IsMatch(donation.DisplayName))
            {
                donor = new Donor
                {
                    Id = $"ExtraLife-{Guid.NewGuid()}-{DateTime.Now.Year}",
                    Username = regex.Replace(donation.DisplayName, "#"),
                    DonationAliases = new List<string>(),
                    DonationAmount = donation.Amount,
                    AssignedTier = ""
                };
            }
            else
            {
                donor = new Donor
                {
                    Id = $"ExtraLife-{Guid.NewGuid()}-{DateTime.Now.Year}",
                    Username = donation.DisplayName,
                    DonationAliases = new List<string>(),
                    DonationAmount = donation.Amount,
                    AssignedTier = ""
                };
            }

            var donorExists = await _donationManager.CheckDonorExists(donation.DisplayName);
            if (donorExists) continue;
            var result = await _donationManager.InsertDonor(donor);
            if (!result.Succeeded)
            {
                _logger.LogError("Unable to create donor record for {username}\r\n\tReason: {errorMsg}", donation.DisplayName, result.ErrorMessage);
                continue;
            }

            var updateResult = await _donationManager.DonationSentToDonors(donation, true);
            if (updateResult.Succeeded) continue;
            _logger.LogError("Failed to update donation record for {username}\r\n\tReason: {errorMsg}", donation.DisplayName, result.ErrorMessage);
        }
        _logger.LogInformation("Donor insertion complete");
    }

    private async Task FetchDonations(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Fetching donations...");
        if (stoppingToken.IsCancellationRequested)
        {
            _logger.LogWarning("Fetching of donations has been cancelled");
            return;
        }

        Log.Warning(_restSettings.RequestUri);
        var originRequest = new RestRequest(_restSettings.RequestUri);
        var originResponse = await _restClient.GetAsync(originRequest, stoppingToken);
        var requestPagination = new Dictionary<int, string>();
        if (originResponse.Headers != null)
        {
            foreach (var header in originResponse.Headers)
            {
                if (header.Name != "Num-Records") continue;
                var recordCount = Convert.ToInt32(header.Value);
                if (recordCount <= 100) continue;
                var pageEnumerable = SplitResponse(recordCount, 100).ToList();
                foreach (var enumerable in pageEnumerable)
                {
                    switch (enumerable)
                    {
                        case 100:
                            requestPagination.Add(enumerable, $"{_restSettings.RequestUri}?orderBy=modifiedDateUTC+DESC");
                            break;
                        case > 100:
                            requestPagination.Add(enumerable, $"{_restSettings.RequestUri}?orderBy=modifiedDateUTC+DESC&offset={enumerable+1}");
                            break;
                        default:
                            requestPagination.Add(enumerable, $"{_restSettings.RequestUri}?orderBy=modifiedDateUTC+DESC&offset={recordCount-enumerable+1}");
                            break;
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }

        if (requestPagination.Count > 0)
        {
            for (var index = 0; index < requestPagination.Count; index++)
            {
                var pageUri = requestPagination.ElementAt(index);
                var request = new RestRequest(pageUri.Value);
                var response = await _restClient.GetAsync(request, stoppingToken);
                if (response.Content != null)
                {
                    var donations = JsonConvert.DeserializeObject<List<Donation>>(response.Content);
                    if (donations == null) continue;
                    foreach (var donation in donations)
                    {
                        var donationExists = await _donationManager.CheckDonationExists(donation.DisplayName);
                        if (donationExists) continue;
                        var result = await _donationManager.InsertDonation(donation);
                        if (!result.Succeeded) _logger.LogError("Unable to create donation record for {username}\r\n\tReason: {errorMsg}", donation.DisplayName, result.ErrorMessage);
                    }
                }

                _logger.LogInformation("All donations from page {pageNum} have been processed", index);
            }
            _logger.LogInformation("Donation fetch complete");
        }
    }

    private static IEnumerable<int> SplitResponse(int input, int chunkSize)
    {
        if(chunkSize <= 0)
        {
            throw new ArgumentException("Chunk size must be greater than zero.", nameof(chunkSize));
        }

        if(input <= 0)
        {
            throw new ArgumentException("Input must be greater than zero.", nameof(input));
        }

        var outputList = new List<int>();
        for (var i = 1; i <= input / chunkSize; i++)
        {
            outputList.Add(i * chunkSize);
        }

        if (decimal.Divide(input, chunkSize) % 1 == 0) return outputList;
        Math.DivRem(input, chunkSize, out var remainder);
        outputList.Add(remainder);

        return outputList;
    }
}