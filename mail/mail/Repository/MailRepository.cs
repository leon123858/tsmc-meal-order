using mail.Model;
using Microsoft.Extensions.Options;
using Npgsql;

namespace mail.Repository;

public class MailRepository
{
    private readonly string _connectionString;

    public MailRepository(IOptions<DatabaseSetting> databaseSettings)
    {
        var secretPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? databaseSettings.Value.Password;
        _connectionString =
            $"Host={databaseSettings.Value.Host};Username={databaseSettings.Value.UserName};Password={secretPassword};Database={databaseSettings.Value.DatabaseName}";
    }

    public void Create(Mail mail, string userEmail)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        InsertMailData(connection, mail.Id, userEmail, mail.Status);
        connection.Close();
    }

    public void UpdateMailStatus(Guid id, MailStatus status)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        UpdateMail(connection, id, status);
        connection.Close();
    }

    public Mail? GetMailData(Guid mailId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        var mail = GetMail(connection, mailId);
        connection.Close();
        return mail;
    }

    public List<Mail> GetMailData(string userEmail)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        var mailList = GetUserMailList(connection, userEmail);
        connection.Close();
        return mailList;
    }

    public Mail StopMailSend(Guid mailId, MailStatus originalStatus)
    {
        if (originalStatus != MailStatus.UNSEND && originalStatus != MailStatus.SENDING)
            throw new Exception($"Mail status is End status. Mail ID: {mailId.ToString()}");

        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            return UpdateMailTransaction(connection, transaction, mailId, originalStatus, MailStatus.STOPPED);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw new Exception(e.Message);
        }
    }

    private static Mail UpdateMailTransaction(NpgsqlConnection connection, NpgsqlTransaction transaction, Guid mailId,
        MailStatus originalStatus,
        MailStatus newStatus)
    {
        var mailBeforeUpdate = GetMail(connection, transaction, mailId);

        if (mailBeforeUpdate == null) throw new Exception($"Mail record not found. Mail ID: {mailId.ToString()}");
        if (mailBeforeUpdate.Status != originalStatus)
            throw new Exception($"Mail status is not {originalStatus.ToString()}. Mail ID: {mailId.ToString()}");

        UpdateMail(connection, transaction, mailId, newStatus);
        transaction.Commit();

        var mailAfterUpdate = GetMail(connection, mailId);
        if (mailAfterUpdate == null) throw new Exception($"Mail record not found. Mail ID: {mailId.ToString()}");
        return mailAfterUpdate;
    }

    private static Mail? ReadMailData(NpgsqlCommand cmd, Guid mailId)
    {
        Mail? mail = null;
        cmd.Parameters.AddWithValue("mailId", mailId);
        using var reader = cmd.ExecuteReader();
        if (reader.Read()) mail = new Mail(reader.GetGuid(0), (MailStatus)reader.GetInt32(2));

        return mail;
    }

    private static Mail? GetMail(NpgsqlConnection connection, Guid mailId)
    {
        using var cmd =
            new NpgsqlCommand("SELECT id, user_email, status FROM mail WHERE id=@mailId LIMIT 1", connection);
        return ReadMailData(cmd, mailId);
    }

    private static Mail? GetMail(NpgsqlConnection connection, NpgsqlTransaction transaction, Guid mailId)
    {
        using var cmd = new NpgsqlCommand("SELECT id, user_email, status FROM mail WHERE id=@mailId LIMIT 1",
            connection,
            transaction);
        return ReadMailData(cmd, mailId);
    }

    private static List<Mail> GetUserMailList(NpgsqlConnection connection, string userEmail)
    {
        var list = new List<Mail>();
        using var cmd = new NpgsqlCommand("SELECT id, status FROM mail WHERE user_email=@userEmail", connection);
        cmd.Parameters.AddWithValue("userEmail", userEmail);
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) list.Add(new Mail(reader.GetGuid(0), (MailStatus)reader.GetInt32(1)));

        return list;
    }

    private static void UpdateMail(NpgsqlConnection connection, Guid mailId, MailStatus status)
    {
        using var updateCommand =
            new NpgsqlCommand("UPDATE mail SET status = @status WHERE id = @id", connection);
        UpdateMailData(updateCommand, mailId, status);
    }

    private static void UpdateMail(NpgsqlConnection connection, NpgsqlTransaction transaction, Guid mailId,
        MailStatus status)
    {
        using var updateCommand =
            new NpgsqlCommand("UPDATE mail SET status = @status WHERE id = @id", connection, transaction);
        UpdateMailData(updateCommand, mailId, status);
    }

    private static void UpdateMailData(NpgsqlCommand cmd, Guid mailId, MailStatus status)
    {
        cmd.Parameters.AddWithValue("id", mailId);
        cmd.Parameters.AddWithValue("status", (int)status);
        var rowsAffected = cmd.ExecuteNonQuery();
        if (rowsAffected != 1) throw new Exception($"Error updating mail data {mailId.ToString()}");
    }

    private static void InsertMailData(NpgsqlConnection connection, Guid mailId, string userEmail, MailStatus status)
    {
        using var insertCommand =
            new NpgsqlCommand("INSERT INTO mail (id, user_email, status) VALUES (@id, @userEmail, @status)",
                connection);
        insertCommand.Parameters.AddWithValue("id", mailId);
        insertCommand.Parameters.AddWithValue("userEmail", userEmail);
        insertCommand.Parameters.AddWithValue("status", (int)status);

        var rowsAffected = insertCommand.ExecuteNonQuery();
        if (rowsAffected != 1) throw new Exception("Error inserting mail data");
    }
}