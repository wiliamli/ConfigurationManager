namespace Jwell.ConfigurationManager.Core.Schedule
{
    public interface ISchedulePolicy
    {
        int Fail();

        void Success();
    }
}
