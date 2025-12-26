using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum.Group.Functions
{
    public enum SearchMyGroupsEnum
    {
        Success,
        NoGroupsFound,
        Failed
    }

    public static class SearchMyGroupsEnumMessage
    {
        public static string GetMessage(this SearchMyGroupsEnum status)
        {
            return status switch
            {
                SearchMyGroupsEnum.Success => "My groups search completed successfully.",
                SearchMyGroupsEnum.NoGroupsFound => "No groups found matching the search term.",
                SearchMyGroupsEnum.Failed => "Failed to search my groups.",
                _ => "Unknown error."
            };
        }
    }
}
