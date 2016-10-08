namespace UniversalDbUpdater.Common
{
    public class DateTimeFacade : IDateTimeFacade
    {
        public System.DateTime Now => System.DateTime.Now;
    }

    public interface IDateTimeFacade
    {
        System.DateTime Now { get; }
    }
}