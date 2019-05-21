using SouthernApi.Interfaces;
using SouthernApi.Model.Request;
using System;
using System.Collections.Generic;

namespace SouthernApi.Helpers
{
    public class ItemDataHelper : IItemDataHelper
    {
        public ScheduleJobRequest GenerateScheduleJobRequest(string folderLocation, string catalogId, string mappingId, string assignmentMode)
        {
            try
            {
                return new ScheduleJobRequest
                {
                    Files = new List<Location> { new Location { Id = folderLocation } },
                    Mapping = new Mapping { Id = mappingId },
                    EntitySpecificData = new List<EntitySpecificData>
                    {
                        new EntitySpecificData { EntityIdentifier = "Article", Properties = new Properties { Parent = catalogId, GroupAssignmentMode = assignmentMode },
                    } }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
