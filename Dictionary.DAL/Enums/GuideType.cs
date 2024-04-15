using System.Text.Json.Serialization;

namespace Dictionary.DAL.Enums
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
