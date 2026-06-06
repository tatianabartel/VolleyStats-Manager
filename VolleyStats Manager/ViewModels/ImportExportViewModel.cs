using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using System.Text.Json.Serialization;
using VolleyStats_Manager.Models.DTO;
using VolleyStats_Manager.Models.DTO.Export;
using VolleyStats_Manager.Models.Services;

namespace VolleyStats_Manager.ViewModels;

public partial class ImportExportViewModel(PlayerService playerService, MatchService matchService, PlayerStatsService statsService,
    AppSessionService session) : ObservableObject
{
    private readonly PlayerService _playerService = playerService;
    private readonly MatchService _matchService = matchService;
    private readonly PlayerStatsService _statsService = statsService;
    private readonly AppSessionService _session = session;
    private string? _selectedFilePath;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    [ObservableProperty]
    public partial string SelectedExportOption { get; set; } = "All";
    [ObservableProperty]
    public partial string SelectedFileName { get; set; } = "No file selected";
    [ObservableProperty]
    public partial bool HasSelectedFile { get; set; } = false;
    [ObservableProperty]
    public partial string ImportResultMessage { get; set; } = string.Empty;
    [ObservableProperty]
    public partial bool HasImportResult { get; set; } = false;
    public List<string> ExportOptions { get; } = ["Players", "Matches", "Statistics", "All"];

    [RelayCommand]
    private async Task Export()
    {
        if (!_session.HasTeamSelected) return;

        try
        {
            var teamId = _session.CurrentTeamId!.Value;
            var fileName = $"{SelectedExportOption}_{DateTime.Now:yyyyMMdd_HHmm}.json";

            object data = SelectedExportOption switch
            {
                "Players" => await BuildPlayersExport(teamId),
                "Matches" => await BuildMatchesExport(teamId),
                "Statistics" => await BuildStatsExport(teamId),
                _ => new FullExportDto
                {
                    Players = await BuildPlayersExport(teamId),
                    Matches = await BuildMatchesExport(teamId),
                    Statistics = await BuildStatsExport(teamId)
                }
            };

            var json = JsonSerializer.Serialize(data, JsonOptions);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            await FileSaver.Default.SaveAsync(fileName, new MemoryStream(bytes));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Export Error", ex.Message, "OK");
        }
    }

    private async Task<List<PlayerDTO>> BuildPlayersExport(int teamId)
        => await _playerService.GetPlayersByTeamAsync(teamId);

    private async Task<List<MatchDTO>> BuildMatchesExport(int teamId)
        => await _matchService.GetMatchesByTeamAsync(teamId);

    private async Task<List<PlayerStatsRecordDTO>> BuildStatsExport(int teamId)
    {
        var playerIds = (await _playerService.GetPlayersByTeamAsync(teamId))
            .Select(p => p.Id).ToList();
        return await _statsService.GetRawStatsByTeamAsync(playerIds);
    }

    [RelayCommand]
    private async Task PickFile()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".json" } },
                { DevicePlatform.macOS, new[] { "json" } }
            })
        });

        if (result == null) return;

        _selectedFilePath = result.FullPath;
        SelectedFileName = result.FileName;
        HasSelectedFile = true;
        HasImportResult = false;
    }

    [RelayCommand]
    private async Task Import()
    {
        if (_selectedFilePath == null || !_session.HasTeamSelected) return;

        if (!File.Exists(_selectedFilePath))
        {
            ImportResultMessage = "File not found.";
            HasImportResult = true;
            return;
        }

        string content;
        try
        {
            content = await File.ReadAllTextAsync(_selectedFilePath);
        }
        catch (Exception ex)
        {
            ImportResultMessage = $"Failed to read file: {ex.Message}";
            HasImportResult = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            ImportResultMessage = "File is empty.";
            HasImportResult = true;
            return;
        }

        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            int totalImported = 0;
            int totalSkipped = 0;

            if (root.ValueKind == JsonValueKind.Object)
            {
                bool hasPlayers = root.TryGetProperty("Players", out _) || root.TryGetProperty("players", out _);
                bool hasMatches = root.TryGetProperty("Matches", out _) || root.TryGetProperty("matches", out _);
                bool hasStats = root.TryGetProperty("Statistics", out _) || root.TryGetProperty("statistics", out _);

                if (!hasPlayers && !hasMatches && !hasStats)
                {
                    ImportResultMessage = "Unrecognized file format.";
                    HasImportResult = true;
                    return;
                }

                var full = JsonSerializer.Deserialize<FullExportDto>(content, JsonOptions);
                if (full != null)
                {
                    var (pi, ps) = await ImportPlayers(full.Players);
                    var (mi, ms) = await ImportMatches(full.Matches);
                    var (si, ss) = await ImportStatistics(full.Statistics);
                    totalImported = pi + mi + si;
                    totalSkipped = ps + ms + ss;
                }
            }
            else if (root.ValueKind == JsonValueKind.Array)
            {
                if (root.GetArrayLength() == 0)
                {
                    ImportResultMessage = "File contains an empty array.";
                    HasImportResult = true;
                    return;
                }

                var first = root[0];
                if (first.TryGetProperty("Opponent", out _) || first.TryGetProperty("opponent", out _))
                {
                    var matches = JsonSerializer.Deserialize<List<MatchDTO>>(content, JsonOptions);
                    if (matches != null)
                        (totalImported, totalSkipped) = await ImportMatches(matches);
                }
                else if (first.TryGetProperty("MatchOpponent", out _) || first.TryGetProperty("matchOpponent", out _))
                {
                    var stats = JsonSerializer.Deserialize<List<PlayerStatsRecordDTO>>(content, JsonOptions);
                    if (stats != null)
                        (totalImported, totalSkipped) = await ImportStatistics(stats);
                }
                else
                {
                    var players = JsonSerializer.Deserialize<List<PlayerDTO>>(content, JsonOptions);
                    if (players != null)
                        (totalImported, totalSkipped) = await ImportPlayers(players);
                }
            }
            else
            {
                ImportResultMessage = "Unrecognized file format.";
                HasImportResult = true;
                return;
            }

            if (totalImported == 0 && totalSkipped == 0)
                ImportResultMessage = "No records found in the file.";
            else if (totalImported == 0)
                ImportResultMessage = $"No new records imported. {totalSkipped} record(s) already exist in the system.";
            else if (totalSkipped > 0)
                ImportResultMessage = $"Imported {totalImported} record(s). {totalSkipped} skipped (already exist or invalid data).";
            else
                ImportResultMessage = $"Successfully imported {totalImported} record(s).";
            HasImportResult = true;
        }
        catch (JsonException)
        {
            ImportResultMessage = "The file could not be read. Make sure it was exported from this application and has not been modified manually.";
            HasImportResult = true;
        }
        catch (Exception ex)
        {
            ImportResultMessage = $"Import failed: {ex.Message}";
            HasImportResult = true;
        }
    }

    private async Task<(int imported, int skipped)> ImportPlayers(List<PlayerDTO> players)
    {
        if (players == null || players.Count == 0) return (0, 0);

        int imported = 0, skipped = 0;
        foreach (var dto in players)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            {
                skipped++;
                continue;
            }

            try
            {
                await _playerService.CreatePlayerAsync(
                    dto.FirstName,
                    dto.LastName,
                    dto.Number,
                    dto.Position,
                    _session.CurrentTeamId!.Value);
                imported++;
            }
            catch (InvalidOperationException)
            {
                skipped++;
            }
        }

        return (imported, skipped);
    }

    private async Task<(int imported, int skipped)> ImportMatches(List<MatchDTO> matches)
    {
        if (matches == null || matches.Count == 0) return (0, 0);

        int imported = 0, skipped = 0;
        foreach (var dto in matches)
        {
            if (string.IsNullOrWhiteSpace(dto.Opponent) ||
                dto.Sets == null || dto.Sets.Count == 0 ||
                dto.Sets.Any(s => s.OurPoints < 0 || s.OpponentPoints < 0))
            {
                skipped++;
                continue;
            }

            try
            {
                await _matchService.CreateMatchAsync(
                    dto.Opponent,
                    dto.MatchDate,
                    _session.CurrentTeamId!.Value,
                    dto.Sets);
                imported++;
            }
            catch (InvalidOperationException)
            {
                skipped++;
            }
        }

        return (imported, skipped);
    }

    private async Task<(int imported, int skipped)> ImportStatistics(List<PlayerStatsRecordDTO> stats)
    {
        if (stats == null || stats.Count == 0) return (0, 0);

        var teamId = _session.CurrentTeamId!.Value;
        var players = await _playerService.GetPlayersByTeamAsync(teamId);
        var matches = await _matchService.GetMatchesByTeamAsync(teamId);

        int imported = 0, skipped = 0;
        foreach (var dto in stats)
        {
            if (string.IsNullOrWhiteSpace(dto.PlayerFirstName) ||
                string.IsNullOrWhiteSpace(dto.PlayerLastName) ||
                string.IsNullOrWhiteSpace(dto.MatchOpponent))
            {
                skipped++;
                continue;
            }

            var player = players.FirstOrDefault(p =>
                p.FirstName == dto.PlayerFirstName && p.LastName == dto.PlayerLastName);
            var match = matches.FirstOrDefault(m =>
                m.Opponent == dto.MatchOpponent &&
                m.MatchDate.Date == dto.MatchDate.Date);

            if (player == null || match == null)
            {
                skipped++;
                continue;
            }

            try
            {
                await _statsService.CreateStatsAsync(
                    player.Id, match.Id,
                    dto.PointsScored, dto.Blocks, dto.Aces, dto.Errors, dto.Serves);
                imported++;
            }
            catch (InvalidOperationException)
            {
                skipped++;
            }
        }

        return (imported, skipped);
    }
}
