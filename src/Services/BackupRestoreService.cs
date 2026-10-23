using CommunityToolkit.Maui.Storage;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Data;
using PersonalFinance.Services.Interfaces;

namespace PersonalFinance.Services;

public class BackupRestoreService : IBackupRestoreService
{
    private readonly IDbContextFactory<FinanceDbContext> _dbContextFactory;
    private readonly string _dbPath;

    public BackupRestoreService(
        IDbContextFactory<FinanceDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "finance.db");
    }

    public async Task BackupAsync()
    {
        try
        {
            if (!File.Exists(_dbPath))
            {
                await Shell.Current.DisplayAlert(
                    "Backup",
                    "No local database found.",
                    "OK");
                return;
            }

            string backupFileName =
                $"finance-backup-{DateTime.Now:yyyyMMdd-HHmmss}.db";

            string tempBackupPath = Path.Combine(
                FileSystem.CacheDirectory,
                $"backup-{Guid.NewGuid():N}.db");

            await CreateBackupSnapshotAsync(tempBackupPath);

            await using Stream sourceStream =
                File.OpenRead(tempBackupPath);

            var result =
                await FileSaver.Default.SaveAsync(
                    backupFileName,
                    sourceStream);

            TryDeleteFile(tempBackupPath);

            if (!result.IsSuccessful)
            {
                await Shell.Current.DisplayAlert(
                    "Backup failed",
                    result.Exception?.Message ?? "Backup was canceled.",
                    "OK");

                return;
            }

            await Shell.Current.DisplayAlert(
                "Backup complete",
                "Backup saved successfully.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Backup failed",
                ex.Message,
                "OK");
        }
    }

    public async Task RestoreAsync()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(
                new PickOptions
                {
                    PickerTitle = "Select backup file (.db)",
                    FileTypes = new FilePickerFileType(
                        new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.Android, [ "application/octet-stream", "application/x-sqlite3", "application/vnd.sqlite3" ] },
                            { DevicePlatform.WinUI, [ ".db", ".sqlite", ".sqlite3" ] },
                            { DevicePlatform.iOS, [ "public.data" ] },
                            { DevicePlatform.MacCatalyst, [ "public.data" ] }
                        })
                });

            if (result == null)
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Restore",
                "This will overwrite local data. Continue?",
                "Restore",
                "Cancel");

            if (!confirm)
                return;

            string tempRestorePath = Path.Combine(
                FileSystem.CacheDirectory,
                $"restore-{Guid.NewGuid():N}.db");

            await using (Stream source = await result.OpenReadAsync())
            await using (FileStream target = File.Create(tempRestorePath))
            {
                await source.CopyToAsync(target);
            }

            SqliteConnection.ClearAllPools();

            TryDeleteFile(_dbPath);
            TryDeleteFile(GetWalPath(_dbPath));
            TryDeleteFile(GetShmPath(_dbPath));

            File.Copy(tempRestorePath, _dbPath, overwrite: true);

            TryDeleteFile(tempRestorePath);

            ValidateSqliteFile(_dbPath);

            SqliteConnection.ClearAllPools();

            await using var context =
                await _dbContextFactory.CreateDbContextAsync();

            await context.Database.EnsureCreatedAsync();

            // IMPORTANT: repopulate defaults when missing
            DatabaseSeeder.Seed(context);

            await Shell.Current.DisplayAlert(
                "Restore complete",
                "Backup restored successfully.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Restore failed",
                ex.Message,
                "OK");
        }
    }

    private static void ValidateSqliteFile(string databasePath)
    {
        using var connection =
            new SqliteConnection($"Data Source={databasePath}");

        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = "PRAGMA schema_version;";
        _ = command.ExecuteScalar();
    }

    private async Task CreateBackupSnapshotAsync(string snapshotPath)
    {
        TryDeleteFile(snapshotPath);

        SqliteConnection.ClearAllPools();

        await using var source =
            new SqliteConnection($"Data Source={_dbPath};Mode=ReadOnly");

        await using var destination =
            new SqliteConnection($"Data Source={snapshotPath}");

        await source.OpenAsync();
        await destination.OpenAsync();

        source.BackupDatabase(destination);
    }

    private static void TryDeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static string GetWalPath(string databasePath)
        => databasePath + "-wal";

    private static string GetShmPath(string databasePath)
        => databasePath + "-shm";
}