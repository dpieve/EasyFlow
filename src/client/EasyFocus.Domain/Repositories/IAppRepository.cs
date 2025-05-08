using EasyFocus.Domain.Entities;

namespace EasyFocus.Domain.Repositories;

public interface IAppRepository
{
    public Task LoadData(List<Tag> defaultTags, AppSettings defaultSettings);
    public Task SaveData();
    public int GetNextSessionId();
    public int GetNextTagId();
    public AppSettings? GetSettings();
    public void SetSettings(AppSettings settings);

    public List<Session> GetSessions();
    public void AddSession(Session session);
    public void UpdateSession(int index, Session session);
    public bool DeleteSession(Session session);

    public List<Tag> GetTags();
    public void AddTag(Tag tag);
    public void UpdateTag(int index, Tag tag);
    public bool DeleteTag(Tag tag); 
}