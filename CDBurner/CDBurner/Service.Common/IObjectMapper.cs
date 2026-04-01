using CDBurner.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CDBurner.Service.Common
{
    public interface IObjectMapper
    {
        StudyModel MapStudy(Dictionary<string, JsonElement> dic);
    }
}
