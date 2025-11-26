using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Enum.Notification.Types
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Preposition
    {
        Join,
        To,
        On,
        In
    }
}
