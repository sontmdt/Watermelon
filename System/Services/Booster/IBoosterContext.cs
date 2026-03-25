public interface IBoosterContext : IService
{
    eActiveBooster ActiveBooster { get; }
    bool IsActive { get; }
    bool InCancelZone { get; set; }
    void Activate(eActiveBooster type);
    void Deactivate();
}
