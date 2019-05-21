using SouthernApi.Model.Request;

namespace SouthernApi.Interfaces
{
    public interface IItemDataHelper
    {
        ScheduleJobRequest GenerateScheduleJobRequest(string folderLocation, string catalogId, string mappingId, string assignmentMode);
    }
}
