using CDBurner.Model;
using CDBurner.Service.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CDBurner.Service
{
    public class ObjectMapper : IObjectMapper
    {
        public StudyModel MapStudy(Dictionary<string, JsonElement> dic)
        {
            string Get(string tag)
            {
                if (!dic.ContainsKey(tag)) return null;

                var el = dic[tag];

                if (el.TryGetProperty("Value", out var val) && val.GetArrayLength() > 0)
                {
                    var first = val[0];

                    if (first.ValueKind == JsonValueKind.Object &&
                        first.TryGetProperty("Alphabetic", out var name))
                        return name.GetString();

                    return first.ToString();
                }

                return null;
            }

            return new StudyModel
            {
                Id = Get("0020000D"),
                Patient = Get("00100010"),
                PatientId = Get("00100020"),
                PatientSex = Get("00100040"),
                Modality = Get("00080061"),
                Description = Get("00081030"),
                Url = Get("00081190"),
                Physician = Get("00080090")
            };
        }
    }
}
