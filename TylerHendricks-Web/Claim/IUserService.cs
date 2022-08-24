namespace TylerHendricks_Web.Claim
{
    public interface IUserService
    {
        void ClearSession();
        T GetSeesionvalue<T>(string Key);
        string GetUserId();
        void SetSeesionvalue<T>(string Key, T value);
    }
}