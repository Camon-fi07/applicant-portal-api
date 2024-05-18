using System.Text.Json.Serialization;

namespace Common.Enums
{
    [JsonConverter((typeof(JsonStringEnumConverter)))]
    public enum GuideType
    {
        EducationLevels,
        DocumentTypes,
        Faculties,
        EducationPrograms
    }
}
