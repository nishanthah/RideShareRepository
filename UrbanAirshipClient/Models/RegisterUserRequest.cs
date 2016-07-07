using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Sample Request
 
    POST /api/named_users/associate HTTP/1.1
    Host: go.urbanairship.com
    Content-Type: application/json
    Accept: application/vnd.urbanairship+json; version=3;
    Authorization: Basic N1JhUG1GRTBRUC1MX3BES3NUeUxDQTpKRWY2S2RzSlJZLUc3YTZRT2cxOUpn
    Cache-Control: no-cache
    Postman-Token: d92d00de-70f4-db45-fdfc-608d387d365d

    {
       "channel_id": "df6a6b50-9843-0304-d5a5-743f246a4946",
       "device_type": "android",
       "named_user_id": "user1"
    }
*/

namespace UrbanAirshipClient.Models
{
    public class RegisterUserRequest
    {
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("device_type")]
        public string DeviceType { get; set; }

        [JsonProperty("named_user_id")]
        public string NamedUserId { get; set; }
    }
}
