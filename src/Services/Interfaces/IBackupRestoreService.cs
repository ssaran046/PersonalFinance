namespace PersonalFinance.Services.Interfaces;

public interface IBackupRestoreService
{
    Task BackupAsync();

    Task RestoreAsync();
}