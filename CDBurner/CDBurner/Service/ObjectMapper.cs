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

            string dateStr = Get("00080020");
            string timeStr = Get("00080030");

            DateTime date = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dateStr))
            {
                try
                {
                    int year = int.Parse(dateStr.Substring(0, 4));
                    int month = int.Parse(dateStr.Substring(4, 2));
                    int day = int.Parse(dateStr.Substring(6, 2));

                    int hour = 0, minute = 0, second = 0;

                    if (!string.IsNullOrEmpty(timeStr))
                    {
                        if (timeStr.Length >= 6)
                        {
                            hour = int.Parse(timeStr.Substring(0, 2));
                            minute = int.Parse(timeStr.Substring(2, 2));
                            second = int.Parse(timeStr.Substring(4, 2));
                        }
                    }

                    date = new DateTime(year, month, day, hour, minute, second);
                }
                catch
                {
                    date = DateTime.MinValue;
                }
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
                Date = date
            };
        }
    }
}
