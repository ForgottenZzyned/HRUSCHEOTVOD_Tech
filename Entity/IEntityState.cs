public interface IEntityState
{
    void Enter(Entity entity);
    void Update();
    void Exit();
}
