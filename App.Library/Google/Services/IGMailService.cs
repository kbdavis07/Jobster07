namespace App.Library.Google.Services;
public interface IGMailService 
{
    public Task<List<string>> GetMail();
}