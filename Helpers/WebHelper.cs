using SouthernApi.Interfaces;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SouthernApi.Helpers
{
    public class WebHelper : IWebHelper
    { 
        public string CreateJobStatusQueryUrl(string jobId)
        {
            return @"https://pim-itemregistry-qa2.test.com/rest/V1.0/list/JobHistory/bySearch?query=JobHistory.Id = " + jobId + "&fields=JobHistory.Id,JobHistory.User,JobHistory.CreationTime,JobHistory.ModificationTime,JobHistory.ScheduledAt,JobHistory.ProblemLogIdentifier,JobHistory.CurrentStep,JobHistory.CurrentState,JobHistory.Progress,JobHistory.GroupKey1,JobHistory.GroupKey2,JobHistory.GroupKey3,JobHistory.JobGroup,JobHistory.JobType,JobHistory.ServerIdentifier,JobHistory.JobSeriesId,JobHistory.PostponedExecutions,JobHistory.JobLocale";
        }

        public StreamContent GetStreamForImage(string filePath)
        {
            try
            {
                FileStream fileStream = File.OpenRead(filePath);
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                return streamContent;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public string GetPostMediaManaerUrl(string fileId)
        {
            return "https://pim-itemregistry-qa2.test.com/rest/V1.0/media/" + fileId + "?categoryId=003000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            //return "https://pim-itemregistry-qa.test.com/rest/V1.0/media/" + fileId;

        }
    }
}
